using HealthCareApp.RepositoryServices;
using HealthCareApp.ViewModel.Appointment;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Stripe.Checkout;
using System.Security.Claims;

namespace HealthCareApp.Controllers
{
    public class AppointmentController : Controller
    {
        IGenericRepoServices<Appointment> appointmentService;
        IGenericRepoServices<AvailabilitySlots> slotService;
        IGenericRepoServices<Patient> patientService;

        public AppointmentController(IGenericRepoServices<Appointment> _appointmentService, IGenericRepoServices<AvailabilitySlots> _slotService, IGenericRepoServices<Patient> _patientService)
        {
            appointmentService = _appointmentService;
            slotService = _slotService;
            patientService = _patientService;
        }


        public ActionResult PatientUpcomingAppointments()
        {
            var patientId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            IEnumerable<Appointment> patientAppointments = appointmentService.FindAll(app => app.PatientId == patientId && !app.IsDeleted, app => app.AvailableSlot, app => app.AvailableSlot.Availability, app => app.AvailableSlot.Availability.Doctor, app => app.AvailableSlot.Availability.Doctor.Specialization, app => app.AvailableSlot.Availability.Clinic);
            //// check date and mark appointments as completed and payment status as paid
            foreach (var app in patientAppointments)
            {
                if (DateOnly.FromDateTime(DateTime.Now) > app.AvailableSlot.Availability.Date || (DateOnly.FromDateTime(DateTime.Now) == app.AvailableSlot.Availability.Date && TimeOnly.FromDateTime(DateTime.Now) > app.AvailableSlot.EndTime))
                {
                    app.Status = Status.Completed;
                    app.PaymentStatus = PaymentStatus.Paid;

                    appointmentService.Update(app);
                }
            }

            var upcomingAppVM = appointmentService.FindAllWithSelect
            (
                app => app.PatientId == patientId && app.Status == Status.Upcoming,
                app => new PatientUpcomingAppointmentsVM
                {
                    Id = app.Id,
                    Status = app.Status,
                    Day = app.AvailableSlot.Availability.dayOfWeek,
                    Date = app.AvailableSlot.Availability.Date,
                    StartTime = app.AvailableSlot.StartTime,
                    EndTime = app.AvailableSlot.EndTime,
                    DoctorTitle = app.AvailableSlot.Availability.Doctor.Title.ToString(),
                    DoctorName = $"{app.AvailableSlot.Availability.Doctor.FirstName} {app.AvailableSlot.Availability.Doctor.LastName}",
                    Specialization = app.AvailableSlot.Availability.Doctor.Specialization.Name,
                    Clinic = $"{app.AvailableSlot.Availability.Clinic.ClinicAddress} {app.AvailableSlot.Availability.Clinic.Region.RegionNameEn} {app.AvailableSlot.Availability.Clinic.Region.City.CityNameEn}",
                    PaymentStatus = app.PaymentStatus,
                    Mode = app.AvailableSlot.Availability.type,
                    Fees = app.AvailableSlot.Availability.Doctor.Fees
                }
            );

            return View(upcomingAppVM);
        }

        public ActionResult ReserveAppointment(int slotId) // passed from heba's part
        {
            var patientId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var patient = patientService.GetById(patientId);

            var selectedSlot = slotService.FindWithSelect
            (
                 slot => slot.Id == slotId
                , slot => new ReserveAppointmentVM
                {
                    SlotId = slotId,
                    Day = slot.Availability.dayOfWeek,
                    Date = slot.Availability.Date,
                    StartTime = slot.StartTime,
                    EndTime = slot.EndTime,
                    DoctorId = slot.Availability.Doctor.Id,
                    DoctorImg = slot.Availability.Doctor.ProfilePicture,
                    DoctorTitle = slot.Availability.Doctor.Title.ToString(),
                    DoctorName = $"{slot.Availability.Doctor.FirstName} {slot.Availability.Doctor.LastName}",
                    WaitingTime = slot.Availability.Doctor.WaitingTimeInMinutes,
                    PatientName = $"{patient.FirstName} {patient.LastName}",
                    PatientPhone = patient.PhoneNumber,
                    Mode = slot.Availability.type,
                    Specialization = slot.Availability.Doctor.Specialization.Name,
                    DoctorDescription = slot.Availability.Doctor.Description,
                    Clinic = $"{slot.Availability.Clinic.ClinicAddress} {slot.Availability.Clinic.Region.RegionNameEn} {slot.Availability.Clinic.Region.City.CityNameEn}",
                    Fees = slot.Availability.Doctor.Fees
                }
            );
            return View(selectedSlot);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Checkout(ReserveAppointmentVM appointmentVM)
        {
            var patientId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var patientEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            Appointment appointment = new Appointment
            {
                PatientId = patientId,
                SlotId = appointmentVM.SlotId,
                PaymentMethod = appointmentVM.PaymentMethod,
                Amount = appointmentVM.Fees,
            };

            TempData["Appointment"] = JsonConvert.SerializeObject(appointment);

            var domain = "http://localhost:5113/";
            var options = new SessionCreateOptions
            {
                SuccessUrl = domain + "Appointment/SaveAppointmentWithVisa",
                CancelUrl = domain + $"Appointment/ReserveAppointment/{appointmentVM.SlotId}", // home
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                CustomerEmail = patientEmail
            };

            var sessionListItem = new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)(appointmentVM.Fees * 100),
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = $"Appointment With {appointmentVM.DoctorTitle} {appointmentVM.DoctorName}",
                        Description = $"at {appointmentVM.Clinic} on {appointmentVM.Date} from {appointmentVM.StartTime} to {appointmentVM.EndTime}",
                    },
                    Currency = "egp"
                },
                Quantity = 1,
            };
            options.LineItems.Add(sessionListItem);

            var service = new SessionService();
            Session session = service.Create(options);

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }


        public ActionResult SaveAppointmentWithVisa()
        {
            Appointment appointment = null;

            if (TempData["Appointment"] is string appointmentJson)
            {
                appointment = JsonConvert.DeserializeObject<Appointment>(appointmentJson);
            }
            if (appointment != null && ModelState.IsValid)
            {
                // Mark the selected slot as booked
                AvailabilitySlots reservedSlot = slotService.Find(slot => slot.Id == appointment.SlotId);
                reservedSlot.IsBooked = true;
                slotService.Update(reservedSlot);

                appointment.PaymentStatus = PaymentStatus.Paid;

                appointmentService.Add(appointment);
                return RedirectToAction(nameof(PatientUpcomingAppointments));
            }
            else
                return View(appointment);
        }

        [HttpPost]
        public ActionResult SaveAppointmentWithCash(ReserveAppointmentVM appointmentVM)
        {
            if (ModelState.IsValid)
            {
                var patientId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                Appointment appointment = new Appointment
                {
                    PatientId = patientId,
                    SlotId = appointmentVM.SlotId,
                    PaymentMethod = appointmentVM.PaymentMethod,
                    Amount = appointmentVM.Fees,
                };

                // Mark the selected slot as booked
                AvailabilitySlots reservedSlot = slotService.Find(slot => slot.Id == appointment.SlotId);
                reservedSlot.IsBooked = true;
                slotService.Update(reservedSlot);

                appointmentService.Add(appointment);
                return RedirectToAction(nameof(PatientUpcomingAppointments));
            }
            else
                return RedirectToAction(nameof(ReserveAppointment), appointmentVM.SlotId);
        }


        public ActionResult CancelAppointment(int id)
        {
            var appointToBeCancelled = appointmentService.FindWithSelect
            (
                app => app.Id == id,
                app => new PatientUpcomingAppointmentsVM
                {
                    Status = app.Status,
                    Day = app.AvailableSlot.Availability.dayOfWeek,
                    Date = app.AvailableSlot.Availability.Date,
                    StartTime = app.AvailableSlot.StartTime,
                    EndTime = app.AvailableSlot.EndTime,
                    DoctorTitle = app.AvailableSlot.Availability.Doctor.Title.ToString(),
                    DoctorName = $"{app.AvailableSlot.Availability.Doctor.FirstName} {app.AvailableSlot.Availability.Doctor.LastName}",
                    Specialization = app.AvailableSlot.Availability.Doctor.Specialization.Name,
                    Clinic = app.AvailableSlot.Availability.Clinic.ClinicAddress,
                    PaymentStatus = app.PaymentStatus,
                    PaymentMethod = app.PaymentMethod,
                    Fees = app.AvailableSlot.Availability.Doctor.Fees,
                    Mode = app.AvailableSlot.Availability.type
                }
            );

            return View(appointToBeCancelled);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelAppointment(int id, Appointment appointment)
        {
            try
            {
                Appointment canceledAppointment = appointmentService.GetById(id);

                // Mark the reserved slot as free
                AvailabilitySlots slot = slotService.GetById(canceledAppointment.SlotId);
                slot.IsBooked = false;
                slotService.Update(slot);

                canceledAppointment.Status = Status.Cancelled;
                if(canceledAppointment.PaymentMethod == PaymentMethod.Visa)
                {
                    canceledAppointment.PaymentStatus = PaymentStatus.Refunded; ///// ???? + notification object
                }
                appointmentService.SoftDelete(canceledAppointment);

                return RedirectToAction(nameof(PatientUpcomingAppointments));
            }
            catch
            {
                return View();
            }
        }


        // doctor 
        public ActionResult DisplayUpcomingAppoinments()
        {
            string doctorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var upcomingAppointments = appointmentService.FindAllWithSelect(app => app.AvailableSlot.Availability.DoctorId == doctorId && app.Status == Status.Upcoming
            , app => new UpcomingAppointmentsVM
            {
                Status = app.Status,
                Day = app.AvailableSlot.Availability.dayOfWeek,
                Date = app.AvailableSlot.Availability.Date,
                Time = app.AvailableSlot.StartTime,
                PatientId = app.PatientId,
                PatientName = $"{app.Patient.FirstName} {app.Patient.LastName}",
                PatientPhone = app.Patient.PhoneNumber,
                Mode = app.AvailableSlot.Availability.type,
                paymentStatus = app.PaymentStatus,
                paymentMethod = app.PaymentMethod

            });

            return View(upcomingAppointments);
        }

        public ActionResult AppointmentsHistory(string patientId)
        {
            if (patientId == null)
            {
                patientId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            }

            IEnumerable<Appointment> patientAppointments = appointmentService.FindAll(app => app.PatientId == patientId && !app.IsDeleted, app => app.AvailableSlot, app => app.AvailableSlot.Availability, app => app.AvailableSlot.Availability.Doctor, app => app.AvailableSlot.Availability.Doctor.Specialization, app => app.AvailableSlot.Availability.Clinic);
            foreach (var app in patientAppointments)
            {
                if (DateOnly.FromDateTime(DateTime.Now) > app.AvailableSlot.Availability.Date || (DateOnly.FromDateTime(DateTime.Now) == app.AvailableSlot.Availability.Date && TimeOnly.FromDateTime(DateTime.Now) > app.AvailableSlot.EndTime))
                {
                    app.Status = Status.Completed;
                    app.PaymentStatus = PaymentStatus.Paid;

                    appointmentService.Update(app);
                }
            }

            var appointmentsHistory = appointmentService.FindAllWithSelectIgnoreFilter
            (   app => app.PatientId == patientId,
                app => new AppointmentsHistoryVM
                {
                    Status = app.Status,
                    Date = $"{app.AvailableSlot.Availability.Date:dddd, MMMM dd, yyyy}",
                    StartTime = app.AvailableSlot.StartTime,
                    DoctorId = app.AvailableSlot.Availability.DoctorId,
                    DoctorName = $"{app.AvailableSlot.Availability.Doctor.Title} {app.AvailableSlot.Availability.Doctor.FirstName} {app.AvailableSlot.Availability.Doctor.LastName}",
                    Mode = app.AvailableSlot.Availability.type,
                    Clinic = $"{app.AvailableSlot.Availability.Clinic.ClinicAddress} {app.AvailableSlot.Availability.Clinic.Region.RegionNameEn} {app.AvailableSlot.Availability.Clinic.Region.City.CityNameEn}",
                    PaymentStatus = app.PaymentStatus
                }
            );
            return View(appointmentsHistory);
        }

        //admin 
        public ActionResult DisplayAllDoctorsAppoinments()
        {
            IEnumerable<Appointment> doctorsAppointments = appointmentService.FindAll(app => app.Id > 0, app => app.AvailableSlot, app => app.AvailableSlot.Availability, app => app.Patient, app => app.AvailableSlot.Availability.Doctor, app => app.AvailableSlot.Availability.Clinic);
            var patients = appointmentService.FindAll(app => app.Id > 0, app => app.Patient).Select(app => $"{app.Patient.FirstName} {app.Patient.LastName}");
            ViewBag.patientsList = new SelectList(patients);

            return View(doctorsAppointments);
        }


        [HttpPost]
        public ActionResult DisplayAllDoctorsAppoinments(string patientName)
        {
            IEnumerable<Appointment> doctorsAppointments;

            if (patientName == "all")
            {
                doctorsAppointments = appointmentService.FindAll(app => app.Id > 0, app => app.AvailableSlot, app => app.AvailableSlot.Availability, app => app.Patient, app => app.AvailableSlot.Availability.Doctor, app => app.AvailableSlot.Availability.Clinic);
            }
            else
            {
                var nameParts = patientName.Split(' ');
                var firstName = nameParts[0];
                var lastName = nameParts.Length > 1 ? nameParts[1] : string.Empty;
                doctorsAppointments = appointmentService.FindAll(app => app.Id > 0 && app.Patient.FirstName == firstName && app.Patient.LastName == lastName, app => app.AvailableSlot, app => app.AvailableSlot.Availability, app => app.Patient, app => app.AvailableSlot.Availability.Doctor, app => app.AvailableSlot.Availability.Clinic);
            }

            var patients = appointmentService.FindAll(app => app.Id > 0, app => app.Patient).Select(app => $"{app.Patient.FirstName} {app.Patient.LastName}");
            ViewBag.patientsList = new SelectList(patients, patientName);

            return View(doctorsAppointments);
        }
    }
}
