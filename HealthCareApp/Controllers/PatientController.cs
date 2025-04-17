using HealthCareApp.RepositoryServices;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace HealthCareApp.Controllers
{
    public class PatientController : Controller
    {
        private readonly IGenericRepoServices<Patient> PatientRepo;

        public PatientController(IGenericRepoServices<Patient> PatientRepo)
        {
            this.PatientRepo = PatientRepo;
        }
        public ActionResult DisplayPatientInfoForDoctor(string patientId)
        {
            var patientInfo = PatientRepo.FindWithSelect(p => p.Id == patientId,
                p => new PatientInfoForDoctorVM
                {
                    PatientFullName = $"{p.FirstName} {p.LastName}",
                    Age = DateTime.Now.Year - p.DateOfBirth.Year,
                    MedicalHistory = p.MedicalHistory
                });

            return View(patientInfo);
        }

        public IActionResult Index(int page = 1, int pageSize = 5)
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

            return View(vm);
        }

        //public IActionResult DetailsByID(string id, int page = 1)
        //{
        //    var result = PatientRepo.GetByIdNoTracking(p => p.Id == id);
        //    PatientVM patientVM = new PatientVM()
        //    {
        //        Id = result.Id,
        //        FullName = result.FirstName + " " + result.LastName,

        //        Age = DateTime.Today.Year - result.DateOfBirth.Year -
        //            (result.DateOfBirth > new DateOnly(DateTime.Today.Year, result.DateOfBirth.Month, result.DateOfBirth.Day) ? 1 : 0),

        //        EmergencyContact = result.EmergencyContact,
        //        MedicalHistory = result.MedicalHistory,
        //    };
        //    ViewBag.CurrentPage = page;
        //    return View(patientVM);
        //}

        public IActionResult DetailsByName(string name, int page = 1, int pageSize = 5)
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

        //[HttpGet]
        //public IActionResult Edit(string id, int page = 1)
        //{
        //    var result = PatientRepo.GetByIdNoTracking(p => p.Id == id);
        //    PatientVM patientVM = new PatientVM()
        //    {
        //        Id = result.Id,
        //        FullName = result.FirstName + " " + result.LastName,

        //        Age = DateTime.Today.Year - result.DateOfBirth.Year - 
        //        (result.DateOfBirth > new DateOnly(DateTime.Today.Year, result.DateOfBirth.Month, result.DateOfBirth.Day) ? 1 : 0),

        //        EmergencyContact = result.EmergencyContact,
        //        MedicalHistory = result.MedicalHistory,
        //    };
        //    ViewBag.CurrentPage = page;
        //    return View(patientVM);
        //}

        //[HttpPost]
        //public IActionResult Edit(PatientVM patientVM, int page = 1)
        //{
        //    var result = PatientRepo.GetById(patientVM.Id);
        //    result.FirstName = patientVM.FullName.Split(" ")[0];
        //    result.LastName = patientVM.FullName.Split(" ")[1];
        //    result.DateOfBirth = patientVM.DateOfBirth;
        //    result.EmergencyContact = patientVM.EmergencyContact;
        //    result.MedicalHistory = patientVM.MedicalHistory;

        //    if (ModelState.IsValid)
        //    {
        //        PatientRepo.Save();
        //        return RedirectToAction(nameof(Index), new { page });
        //    }
        //    else
        //        return View(patientVM);
        //}

        //[HttpGet]
        //public IActionResult Delete(string id)
        //{
        //    var result = PatientRepo.GetByIdNoTracking(p => p.Id == id);
        //    PatientVM patientVM = new PatientVM()
        //    {
        //        ID = result.Id,
        //        FullName = result.FirstName + " " + result.LastName,
        //        DateOfBirth = result.DateOfBirth,
        //        EmergencyContact = result.EmergencyContact,
        //        MedicalHistory = result.MedicalHistory,
        //    };
        //    return View(patientVM);
        //}

        //[HttpPost, ActionName("Delete")]
        //public IActionResult DeleteConfirmed(string id)
        //{
        //    var deletedPatient = PatientRepo.GetById(id);
        //    PatientRepo.Delete(deletedPatient);
        //    PatientRepo.Save();
        //    return RedirectToAction(nameof(Index));
        //}
    }
}
