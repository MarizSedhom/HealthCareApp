using HealthCareApp.Models;
using HealthCareApp.RepositoryServices;
using HealthCareApp.ViewModel.Appointment;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Stripe.Checkout;
using System.Diagnostics;

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

     
        public ActionResult Index(string patientId = "2") //
        {
            IEnumerable<Appointment> appointments = appointmentService.FindAll(app => app.PatientId == patientId, app => app.Patient, app => app.AvailableSlot, app => app.AvailableSlot.Availability, app => app.AvailableSlot.Availability.Doctor);

            // check date and mark appointments as completed and payment status as paid
            foreach(var app in appointments)
            {
                if (DateOnly.FromDateTime(DateTime.Now) > app.AvailableSlot.Availability.Date || (DateOnly.FromDateTime(DateTime.Now) == app.AvailableSlot.Availability.Date && TimeOnly.FromDateTime(DateTime.Now) > app.AvailableSlot.EndTime))
                {
                    app.Status = Status.Completed;
                    app.paymentStatus = PaymentStatus.Paid;

                    appointmentService.Update(app);
                }
            }

            return View(appointments);
        }

        
        public ActionResult Details(int id)
        {
            Appointment appointment = appointmentService.Find(app => app.Id == id, app => app.Patient, app => app.AvailableSlot, app => app.AvailableSlot.Availability, app => app.AvailableSlot.Availability.Doctor);

            return View(appointment);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Checkout(Appointment appointment)
        {
            TempData["Appointment"] = JsonConvert.SerializeObject(appointment);

            var domain = "http://localhost:5113/";
            var options = new SessionCreateOptions
            {
                SuccessUrl = domain + "Appointment/SaveAppointmentWithVisa",
                CancelUrl = domain + "Appointment/", // home
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                //CustomerEmail = appointment.Patient.Email
            };

            var sessionListItem = new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)(appointment.Amount * 100),
                    Currency = "egp",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = "Appointment With Doctor"
                    }
                },
                Quantity = 1,
            };
            options.LineItems.Add(sessionListItem);

            var service = new SessionService();
            Session session = service.Create(options);

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }


        public ActionResult ReserveAppointment(int slotId = 5, string patientId = "2") // passed from heba's part
        {
            AvailabilitySlots slot = slotService.Find(slot => slot.Id == slotId, slot => slot.Availability, slot => slot.Availability.Doctor);

            ViewBag.slot = slot;
            ViewBag.patientId = patientId;

            return View();
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

                appointment.paymentStatus = PaymentStatus.Paid;

                appointmentService.Add(appointment);
                return RedirectToAction(nameof(Index));
            }
            else
                return View(appointment);
        }

        [HttpPost]
        public ActionResult SaveAppointmentWithCash(Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                // Mark the selected slot as booked
                AvailabilitySlots reservedSlot = slotService.Find(slot => slot.Id == appointment.SlotId);
                reservedSlot.IsBooked = true;
                slotService.Update(reservedSlot);

                appointmentService.Add(appointment);
                return RedirectToAction(nameof(Index));
            }
            else
                return View(appointment);
        }


        public ActionResult Edit(int id)
        {
            Appointment appointment = appointmentService.Find(app => app.Id == id, app => app.Patient, app => app.AvailableSlot, app => app.AvailableSlot.Availability, app => app.AvailableSlot.Availability.Doctor);

            return View(appointment);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                appointmentService.Update(appointment);

                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(appointment);
            }
        }


        public ActionResult Delete(int id)
        {
            Appointment appointToBeDeleted = appointmentService.Find(app => app.Id == id, app => app.Patient, app => app.AvailableSlot, app => app.AvailableSlot.Availability, app => app.AvailableSlot.Availability.Doctor);

            return View(appointToBeDeleted);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Appointment appointment)
        {
            try
            {
                Appointment appoint = appointmentService.GetById(id);

                // Mark the reserved slot as free
                AvailabilitySlots slot = slotService.GetById(appoint.SlotId);
                slot.IsBooked = false;
                slotService.Update(slot);

                appointmentService.HardDelete(appoint);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(appointment);
            }
        }


        // doctor 
        public ActionResult DisplayUpcomingAppoinments(string doctorId = "hggvftgf55555555")
        {
            var upcomingAppointments = appointmentService.FindAllWithSelect(app => app.AvailableSlot.Availability.DoctorId == doctorId && app.Status == Status.Pending
            ,app => new UpcomingAppointmentsVM
            {
                Day = app.AvailableSlot.Availability.dayOfWeek,
                Date = app.AvailableSlot.Availability.Date,
                Time = app.AvailableSlot.StartTime,
                PatientId = app.PatientId,
                PatientName = $"{app.Patient.FirstName} {app.Patient.LastName}",
                PatientPhone = app.Patient.PhoneNumber,
                Mode = app.AvailableSlot.Availability.type,
                paymentStatus = app.paymentStatus,
                paymentMethod = app.paymentMethod

            },app => app.Patient, app => app.AvailableSlot, app => app.AvailableSlot.Availability, app => app.AvailableSlot.Availability.Doctor);

            return View(upcomingAppointments);
        }


        //admin  -- [soft delete]
        public ActionResult DisplayAllDoctorsAppoinments()
        {
            IEnumerable<Appointment> doctorsAppointments = appointmentService.FindAll(app => app.Id > 0, app => app.AvailableSlot, app => app.AvailableSlot.Availability, app => app.Patient, app => app.AvailableSlot.Availability.Doctor, app => app.AvailableSlot.Availability.Clinic);
            var patients = appointmentService.FindAll(app => app.Id > 0).Select(app => app.PatientName);
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
                doctorsAppointments = appointmentService.FindAll(app => app.Id > 0 && app.PatientName == patientName, app => app.AvailableSlot, app => app.AvailableSlot.Availability, app => app.Patient, app => app.AvailableSlot.Availability.Doctor, app => app.AvailableSlot.Availability.Clinic);
            }

            var patients = appointmentService.FindAll(app => app.Id > 0).Select(app => app.PatientName);
            ViewBag.patientsList = new SelectList(patients, patientName);

            return View(doctorsAppointments);
        }
    }
}
