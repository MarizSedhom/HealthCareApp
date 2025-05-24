using HealthCare.BLL.Interface.Repository;
using HealthCare.DAL.Models;
using HealthCareApp.ViewModel.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareApp.Controllers
{

    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private IGenericRepo<Patient> patientRepo;
        private IGenericRepo<HealthCare.DAL.Models.Doctor> doctorRepo;
        private IGenericRepo<Appointment> appointmentRepo;
        private IGenericRepo<Review> reviewRepo;
        private IGenericRepo<Clinic> clinicRepo;

        public AdminController(IGenericRepo<Patient> _patientRepo,IGenericRepo<HealthCare.DAL.Models.Doctor> _doctorRepo,
            IGenericRepo<Appointment> _appointmentRepo,IGenericRepo<Review> _reviewRepo,IGenericRepo<Clinic> _clinicRepo)
        {
            patientRepo = _patientRepo;
            doctorRepo = _doctorRepo;
            appointmentRepo = _appointmentRepo;
            reviewRepo = _reviewRepo;
            clinicRepo = _clinicRepo;
        }

        // GET: AdminController
        public ActionResult DisplayDashboard()
        {
            var allPatients = patientRepo.GetAll().Where(p => p.DateOfBirth != null).ToList();
            var malePatients = patientRepo.FindAll(p => p.gender == Gender.Male && p.DateOfBirth != null).ToList();
            var femalePatients = patientRepo.FindAll(p => p.gender == Gender.Female && p.DateOfBirth != null).ToList();

            var allDoctors = doctorRepo.GetAll().Where(d => d.DateOfBirth != null).ToList();
            var maleDoctors = doctorRepo.FindAll(d => d.gender == Gender.Male && d.DateOfBirth != null).ToList();
            var femaleDoctors = doctorRepo.FindAll(d => d.gender == Gender.Female && d.DateOfBirth != null).ToList();

            var dashboard = new DashboardVM
            {
                TotalPatients = allPatients.Count,
                TotalPannedPatients = patientRepo.FindAllWithSelectIgnoreFilter(p => p.IsDeleted, p => p.Id).Count(),
                TotalMalePatients = malePatients.Count,
                TotalFemalePatients = femalePatients.Count,
                AvgPatientsAge = allPatients.Any() ? (int)allPatients.Average(p => DateTime.Now.Year - p.DateOfBirth.Year) : 0,
                AvgMalePatientsAge = malePatients.Any() ? (int)malePatients.Average(p => DateTime.Now.Year - p.DateOfBirth.Year) : 0,
                AvgFemalePatientsAge = femalePatients.Any() ? (int)femalePatients.Average(p => DateTime.Now.Year - p.DateOfBirth.Year) : 0,

                TotalMaleDoctors = maleDoctors.Count,
                TotalFemaleDoctors = femaleDoctors.Count,
                AvgDoctorsAge = allDoctors.Any() ? (int)allDoctors.Average(d => DateTime.Now.Year - d.DateOfBirth.Year) : 0,
                AvgMaleDoctorsAge = maleDoctors.Any() ? (int)maleDoctors.Average(d => DateTime.Now.Year - d.DateOfBirth.Year) : 0,
                AvgFemaleDoctorsAge = femaleDoctors.Any() ? (int)femaleDoctors.Average(d => DateTime.Now.Year - d.DateOfBirth.Year) : 0,

                TotalDoctors = doctorRepo.GetAll().Count(),
                TotalApprovedDoctors = doctorRepo.FindAll(d => !d.IsDeleted).Count(),
                TotalRejectedDoctors = doctorRepo.FindAllWithSelectIgnoreFilter(d => d.IsDeleted, d => d.Id).Count(),
                TotalProfessors = doctorRepo.FindAll(d => d.Title == Title.Professor).Count(),
                TotalLecturers = doctorRepo.FindAll(d => d.Title == Title.Lecturer).Count(),
                TotalConsultants = doctorRepo.FindAll(d => d.Title == Title.Consultant).Count(),
                TotalSpecialists = doctorRepo.FindAll(d => d.Title == Title.Specialist).Count(),

                MaxAppointmentsNumThisMonth = appointmentRepo.FindAll(a => a.AvailableSlot.Availability.Date.Month == DateTime.Now.Month && a.AvailableSlot.Availability.Date.Year == DateTime.Now.Year, a => a.AvailableSlot, a => a.AvailableSlot.Availability).Count(),
                TotalUpcomingAppointments = appointmentRepo.FindAll(app => app.Status == Status.Upcoming).Count(),
                TotalRescheduledAppointments = appointmentRepo.FindAll(app => app.Status == Status.Rescheduled).Count(),
                TotalCancelledAppointments = appointmentRepo.FindAll(app => app.Status == Status.Cancelled).Count(),
                TotalCompletedAppointments = appointmentRepo.FindAll(app => app.Status == Status.Completed).Count(),
                TotalPayments = (int)(appointmentRepo.FindAll(app => app.Status == Status.Completed).Count() * appointmentRepo.FindAll(app => app.Status == Status.Completed).Sum(a=> a.Amount)),

                TotalPendingReviews = reviewRepo.FindAll(r => !r.IsApproved && !r.IsDeleted).Count(),
                TotalApprovedReviews = reviewRepo.FindAll(r => r.IsApproved && !r.IsDeleted).Count(),
                TotalRejectedReviews = reviewRepo.FindAll(r => !r.IsApproved && r.IsDeleted).Count()
            };

            return View(dashboard);
        }


    }
}
