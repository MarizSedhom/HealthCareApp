using HealthCareApp.Models;
using HealthCareApp.RepositoryServices;
using HealthCareApp.ViewModel.Patient;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace HealthCareApp.Controllers
{
    public class PatientController : Controller
    {
        private readonly IGenericRepoServices<Patient> PatientRepo;
        private readonly IGenericRepoServices<MedicalRecord> MedicalRepo;
        private readonly UserManager<ApplicationUser> _userManager;

        public PatientController(IGenericRepoServices<Patient> PatientRepo, IGenericRepoServices<MedicalRecord> MedicalRepo, UserManager<ApplicationUser> userManager)
        {
            this.PatientRepo = PatientRepo;
            this.MedicalRepo = MedicalRepo;
            _userManager = userManager;
        }

        public IActionResult Index(int page = 1, int pageSize = 6)
        {
            int skip = (page - 1) * pageSize;
            if (skip < 0)
            {
                skip = 0; // Prevent negative skip values
            }
            var result = PatientRepo.FindAllForSearch(p => true, skip, pageSize, ["MedicalRecords"]);

            List<PatientVM> vm = new List<PatientVM>();
            foreach (var item in result)
            {
                PatientVM patient = new PatientVM()
                {
                    Id = item.Id,
                    FullName = item.FirstName + " " + item.LastName,

                    Age = DateTime.Today.Year - item.DateOfBirth.Year -
                    (item.DateOfBirth > new DateOnly(DateTime.Today.Year, item.DateOfBirth.Month, item.DateOfBirth.Day) ? 1 : 0),

                    EmergencyContact = item.EmergencyContact,
                    MedicalHistory = item.MedicalHistory,
                };
                vm.Add(patient);
            }

            var totalCount = PatientRepo.Count();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            string doctorId = "24D0C3DD-0CA8-44A3-A7CF-C054B75CDA8B";
            //= User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            ViewBag.DoctorId = doctorId;

            return View(vm);
        }

        public IActionResult DetailsByName(string name, int page = 1, int pageSize = 6)
        {
            int skip = (page - 1) * pageSize;

            var nameParts = name.Trim().Split(" ", StringSplitOptions.RemoveEmptyEntries);
            string firstPart = nameParts.Length > 0 ? nameParts[0].ToLower() : "";
            string secondPart = nameParts.Length > 1 ? nameParts[1].ToLower() : "";

            Expression<Func<Patient, bool>> criteria;

            if (string.IsNullOrEmpty(secondPart))
            {
                criteria = p =>
                    p.FirstName.ToLower().Contains(firstPart) ||
                    p.LastName.ToLower().Contains(firstPart);
            }
            else
            {
                criteria = p =>
                    (p.FirstName.ToLower().Contains(firstPart) && p.LastName.ToLower().Contains(secondPart)) ||
                    (p.FirstName.ToLower().Contains(secondPart) && p.LastName.ToLower().Contains(firstPart));
            }

            var patients = PatientRepo.FindAllForSearch(
                criteria,
                skip, pageSize, null,
                p => p.FirstName,
            OrderBy.Ascending);

            List<PatientVM> vm = new List<PatientVM>();
            foreach (var item in patients)
            {
                PatientVM patientVM = new PatientVM()
                {
                    FullName = item.FirstName + " " + item.LastName,

                    Age = DateTime.Today.Year - item.DateOfBirth.Year -
                    (item.DateOfBirth > new DateOnly(DateTime.Today.Year, item.DateOfBirth.Month, item.DateOfBirth.Day) ? 1 : 0),

                    EmergencyContact = item.EmergencyContact,
                    MedicalHistory = item.MedicalHistory,
                };
                vm.Add(patientVM);
            }

            var totalCount = PatientRepo.Count(criteria);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            return PartialView("_DetailsByName", vm);
        }
        public ActionResult DisplayPatientInfoForDoctor(string patientId,string doctorId=null)
        {
            var patientInfo = PatientRepo.FindWithSelect(p => p.Id == patientId,
                p => new PatientInfoForDoctorVM
                {
                    PatientFullName = $"{p.FirstName} {p.LastName}",
                    Age = DateTime.Now.Year - p.DateOfBirth.Year,
                    MedicalHistory = p.MedicalHistory
                });
            ViewBag.doctorId = doctorId;

            return View(patientInfo);
        }

        public IActionResult SearchForMedicalRecords(string patientId, string doctorId)
        {
            var medicalRecords = MedicalRepo.FindAll(mr => mr.PatientId == patientId && mr.DoctorId == doctorId);

            if (medicalRecords != null && medicalRecords.Any())
            {
                // Tell JS to redirect if there are medical records
                return Json(new { redirectTo = Url.Action("Index", "Clinic", new { patientId, doctorId }) });
            }
            else
            {
                // Return a partial view (No records found)
                return PartialView("_NoMedicalRecords");
            }
        }

        public IActionResult GetAllPatients()
        {
            var allPatients = PatientRepo.FindAllWithSelect
            (
                p => true,
                p => new PatientsInfoVM
                {
                    Id = p.Id,
                    PatientName = p.FirstName + " " + p.LastName,
                    Email = p.Email,
                    PhoneNumber = p.PhoneNumber,
                    EmergencyContact = p.EmergencyContact,
                    MedicalHistory = p.MedicalHistory,
                    Age = DateTime.Today.Year - p.DateOfBirth.Year
                }
            );

            return View(allPatients);
        }

        public IActionResult ManagePatientStat(string patientId)
        {
            var patientStat = PatientRepo.FindWithSelectIgnoreFilter
            (
                p => p.Id == patientId,
                p => new PatientStatVM
                {
                    PatientId = patientId,
                    UpcomingAppointments = p.Appointments.Where(app => app.PatientId == p.Id && app.Status == Status.Upcoming).Count(),
                    CompletedAppointments = p.Appointments.Where(app => app.PatientId == p.Id && app.Status == Status.Completed).Count(),
                    CanceledAppointments = p.Appointments.Where(app => app.PatientId == p.Id && app.Status == Status.Cancelled).Count(),
                    RescheduledAppointments = p.Appointments.Where(app => app.PatientId == p.Id && app.Status == Status.Rescheduled).Count(),
                    ApprovedReviews = p.Reviews.Where(r => r.PatientId == p.Id && r.IsApproved && !r.IsDeleted).Count(),
                    RejectedReviews = p.Reviews.Where(r => r.PatientId == p.Id && !r.IsApproved && r.IsDeleted).Count(),
                    PendingReviews = p.Reviews.Where(r => r.PatientId == p.Id && !r.IsApproved && !r.IsDeleted).Count(),
                }
            );
            return View(patientStat);
        }

        public IActionResult EditPatientInfo(string patientId)
        {
            var patientInfo = PatientRepo.FindWithSelect(p => p.Id == patientId,
                p => new EditPatientInfoVM
                {
                    patientId = p.Id,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    Email = p.Email,
                    DateOfBirth = p.DateOfBirth,
                    EmergencyContact = p.EmergencyContact,
                    MedicalHistory = p.MedicalHistory,
                    Gender = p.gender
                }
            );
            return View(patientInfo);
        }

        [HttpPost]
        public IActionResult EditPatientInfo(string patientId, EditPatientInfoVM editPatientVM)
        {
            if (ModelState.IsValid)
            {
                Patient patient = PatientRepo.GetById(patientId);

                patient.Email = editPatientVM.Email;
                patient.FirstName = editPatientVM.FirstName;
                patient.LastName = editPatientVM.LastName;
                patient.DateOfBirth = editPatientVM.DateOfBirth;
                patient.EmergencyContact = editPatientVM.EmergencyContact;
                patient.gender = editPatientVM.Gender;
                patient.MedicalHistory = editPatientVM.MedicalHistory;

                PatientRepo.Update(patient);
                return RedirectToAction("GetAllPatients");
            }
            else
            {
                return View(editPatientVM);
            }
        }

        public IActionResult DeactivatePatientAccount(string patientId)
        {
            var patient = PatientRepo.GetById(patientId);   ////////////////////////////////// notification
            if (patient != null)
            {
                patient.IsDeleted = true;
                PatientRepo.Update(patient);
                return RedirectToAction("GetAllPatients");
            }
            else
            {
                return NotFound();
            }
        }

        public IActionResult PatientHistory(string patientId)
        {
            return RedirectToAction("AppointmentsHistory", "Appointment", new { patientId });
        }


        //action for profile patient
        public async Task<IActionResult> GetPatientProfile()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return NotFound();
            }

            var patient = PatientRepo.Find(p=>p.Id==userId, p=>p.Appointments);

            if (patient == null)
            {
                return NotFound();
            }

            // Calculate age from date of birth
            int age = DateTime.Now.Year - patient.DateOfBirth.Year;
            if (DateTime.Now.DayOfYear < patient.DateOfBirth.DayOfYear)
            {
                age--;
            }

            // Create view model
            var viewModel = new PatientsInfoVM
            {
                Id = patient.Id,
                Age = age,
                PatientName = $"{patient.FirstName} {patient.LastName}",
                Email = patient.Email,
                PhoneNumber = patient.PhoneNumber,
                EmergencyContact = patient.EmergencyContact,
                MedicalHistory = patient.MedicalHistory
            };

            return View(viewModel);
        }

        public IActionResult EditPatientProfile()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return NotFound();
            }

            //var patient = PatientRepo.Find(p => p.Id == userId, p => p.Appointments);

            var patientInfo = PatientRepo.FindWithSelect(p => p.Id == userId,
                p => new EditPatientInfoVM
                {
                    patientId = p.Id,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    Email = p.Email,
                    DateOfBirth = p.DateOfBirth,
                    EmergencyContact = p.EmergencyContact,
                    MedicalHistory = p.MedicalHistory,
                    Gender = p.gender
                }
            );
            return View(patientInfo);
        }

        [HttpPost]
        public IActionResult EditPatientProfile(string patientId, EditPatientInfoVM editPatientVM)
        {
            if (ModelState.IsValid)
            {
                Patient patient = PatientRepo.GetById(patientId);

                patient.Email = editPatientVM.Email;
                patient.FirstName = editPatientVM.FirstName;
                patient.LastName = editPatientVM.LastName;
                patient.DateOfBirth = editPatientVM.DateOfBirth;
                patient.EmergencyContact = editPatientVM.EmergencyContact;
                patient.gender = editPatientVM.Gender;
                patient.MedicalHistory = editPatientVM.MedicalHistory;

                PatientRepo.Update(patient);
                return RedirectToAction("GetPatientProfile");
            }
            else
            {
                return View(editPatientVM);
            }
        }
    }
}