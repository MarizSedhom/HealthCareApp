using HealthCareApp.RepositoryServices;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Security.Claims;

namespace HealthCareApp.Controllers
{
    public class PatientController : Controller
    {
        private readonly IGenericRepoServices<Patient> PatientRepo;
        private readonly IGenericRepoServices<MedicalRecord> MedicalRepo;

        public PatientController(IGenericRepoServices<Patient> PatientRepo, IGenericRepoServices<MedicalRecord> MedicalRepo)
        {
            this.PatientRepo = PatientRepo;
            this.MedicalRepo = MedicalRepo;
        }

        public IActionResult Index(int page = 1, int pageSize = 6)
        {
            int skip = (page - 1) * pageSize;
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
        public ActionResult DisplayPatientInfoForDoctor(string patientId)
        {
            var patientInfo = PatientRepo.FindWithSelect(p => p.Id == patientId,
                p => new PatientVM
                {
                    FullName = $"{p.FirstName} {p.LastName}",
                    Age = DateTime.Now.Year - p.DateOfBirth.Year,
                    MedicalHistory = p.MedicalHistory
                });

            return View(patientInfo);
        }

        public ActionResult SearchForMedicalRecords(string patientId, string doctorId)
        {
            var medicalRecords = MedicalRepo.FindAll(mr => mr.PatientId == patientId && mr.DoctorId == doctorId);

            if (medicalRecords != null && medicalRecords.Any())
            {
                return RedirectToAction("Eman enter your controller and action here");
            }
            else
            {
                return PartialView("_NoMedicalRecords");
            }
        }

    }
}
