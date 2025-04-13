using HealthCareApp.Models;
using HealthCareApp.RepositoryServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

     
        public ActionResult Index(string patientId = "dgtdeytd53dhe") // passed from layout user  
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

       
        public ActionResult Create(int slotId = 8, string patientId = "dgtdeytd53dhe") // passed from heba's part
        {
            AvailabilitySlots slot = slotService.Find(slot => slot.Id == slotId, slot => slot.Availability, slot => slot.Availability.Doctor);

            ViewBag.slot = slot;
            ViewBag.patientId = patientId;

            return View();
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Appointment appointment)    
        { 
            if(ModelState.IsValid)
            {
                // Mark the selected slot as booked
                AvailabilitySlots reservedSlot = slotService.Find(slot => slot.Id == appointment.SlotId);
                reservedSlot.IsBooked = true;
                slotService.Update(reservedSlot);

                if (appointment.paymentMethod == PaymentMethod.Visa)
                    appointment.paymentStatus = PaymentStatus.Paid;

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


        public ActionResult DisplayUpcomingAppoinments(string doctorId = "hggvftgf55555555")
        {
            IEnumerable<Appointment> upcomingAppointments = appointmentService.FindAll(app => app.AvailableSlot.Availability.DoctorId == doctorId && app.Status == Status.Pending, app => app.Patient, app => app.AvailableSlot, app => app.AvailableSlot.Availability, app => app.AvailableSlot.Availability.Doctor);
            return View(upcomingAppointments);
        }


        //admin  -- [soft delete]
        public ActionResult DisplayAllDoctorsAppoinments()
        {
            IEnumerable<Appointment> doctorsAppointments = appointmentService.GetAll();
            return View(doctorsAppointments);
        }
    }
}
