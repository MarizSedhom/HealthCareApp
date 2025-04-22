using HealthCareApp.RepositoryServices;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HealthCareApp.Controllers
{
    public class PatientController : Controller
    {
        private readonly IGenericRepoServices<Patient> PatientRepo;
        private readonly IGenericRepoServices<MedicalRecord> MedicalRepo;
        private readonly IGenericRepoServices<Appointment> AppointmentRepo;

        public PatientController(IGenericRepoServices<Patient> PatientRepo, IGenericRepoServices<MedicalRecord> MedicalRepo, IGenericRepoServices<Appointment> AppointmentRepo)
        {
            this.PatientRepo = PatientRepo;
            this.MedicalRepo = MedicalRepo;
            this.AppointmentRepo = AppointmentRepo;
        }

        public IActionResult DetailsByName(string name, int page = 1, int pageSize = 6)
        {
            string doctorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "d1";
            int skip = (page - 1) * pageSize;

            var nameParts = name.Trim().Split(" ", StringSplitOptions.RemoveEmptyEntries);
            string firstPart = nameParts.Length > 0 ? nameParts[0].ToLower() : "";
            string secondPart = nameParts.Length > 1 ? nameParts[1].ToLower() : "";

            // Get all appointments related to this doctor
            var appointments = AppointmentRepo
            .FindAllForSearch(
                ap => ap.AvailableSlot != null &&
              ap.AvailableSlot.Availability != null &&
              ap.Patient != null &&
              ap.AvailableSlot.Availability.DoctorId == doctorId &&
              (
                  (ap.Patient.FirstName.ToLower().Contains(firstPart) || ap.Patient.LastName.ToLower().Contains(firstPart)) ||
                  (!string.IsNullOrEmpty(secondPart) &&
                   ((ap.Patient.FirstName.ToLower().Contains(firstPart) && ap.Patient.LastName.ToLower().Contains(secondPart)) ||
                    (ap.Patient.FirstName.ToLower().Contains(secondPart) && ap.Patient.LastName.ToLower().Contains(firstPart))))
              ),
            skip,
            pageSize,
            ["Patient", "AvailableSlot", "AvailableSlot.Availability"]
            );


            var patients = appointments
                .Select(ap => ap.Patient)
                .Where(p => p != null)
                .Distinct()
                .ToList();

            var patientVMs = patients.Select(p => new PatientVM
            {
                Id = p.Id,
                FullName = $"{p.FirstName ?? ""} {p.LastName ?? ""}".Trim(),
                Age = p.DateOfBirth != default
                    ? DateTime.Today.Year - p.DateOfBirth.Year -
                      (p.DateOfBirth > DateOnly.FromDateTime(DateTime.Today.AddYears(-(DateTime.Today.Year - p.DateOfBirth.Year))) ? 1 : 0)
                    : 0,
                EmergencyContact = p.EmergencyContact ?? "N/A",
                MedicalHistory = p.MedicalHistory ?? "N/A",
                IsFirst = !p.Appointments.Any(a => a.AvailableSlot.Availability.DoctorId == doctorId)
            }).ToList();

            var totalCount = AppointmentRepo.
                FindAll(ap => ap.AvailableSlot.Availability.DoctorId == doctorId &&
                             (
                                (ap.Patient.FirstName.ToLower().Contains(firstPart) || ap.Patient.LastName.ToLower().Contains(firstPart)) ||
                                (!string.IsNullOrEmpty(secondPart) &&
                                 ((ap.Patient.FirstName.ToLower().Contains(firstPart) && ap.Patient.LastName.ToLower().Contains(secondPart)) ||
                                  (ap.Patient.FirstName.ToLower().Contains(secondPart) && ap.Patient.LastName.ToLower().Contains(firstPart))))
                             ),ap => ap.Patient, ap => ap.AvailableSlot, ap => ap.AvailableSlot.Availability)
                .Select(ap => ap.Patient.Id)
                .Distinct()
                .Count();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            ViewBag.DoctorId = doctorId;

            return PartialView("_DetailsByName", patientVMs);
        }

        public IActionResult GetDoctorPatients(int page = 1, int pageSize = 6)
        {
            string doctorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "d1";
            int skip = (page - 1) * pageSize;

            var appointments = AppointmentRepo
                .FindAllForSearch(ap => ap.AvailableSlot.Availability.DoctorId == doctorId, skip, pageSize, ["Patient", "AvailableSlot", "AvailableSlot.Availability"]);

            var patients = appointments
                .Select(ap => ap.Patient)
                .Where(p => p != null)
                .Distinct()
                .ToList();

            var patientVMs = patients.Select(p => new PatientVM
            {
                Id = p.Id,
                FullName = $"{p.FirstName ?? ""} {p.LastName ?? ""}".Trim(),
                Age = p.DateOfBirth != default
                    ? DateTime.Today.Year - p.DateOfBirth.Year -
                      (p.DateOfBirth > DateOnly.FromDateTime(DateTime.Today.AddYears(-(DateTime.Today.Year - p.DateOfBirth.Year))) ? 1 : 0)
                    : 0,
                EmergencyContact = p.EmergencyContact ?? "N/A",
                MedicalHistory = p.MedicalHistory ?? "N/A",

                // Check if this patient has only ONE appointment with this doctor
                IsFirst = !p.Appointments.Any(a => a.AvailableSlot.Availability.DoctorId == doctorId)
                }).ToList();

                var totalCount = AppointmentRepo
                .FindAll(ap => ap.AvailableSlot.Availability.DoctorId == doctorId, ap => ap.Patient)
                .Distinct()
                .Count();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            ViewBag.DoctorId = doctorId;

            return View(nameof(Index), patientVMs);
        }

    }
}

//public IActionResult Index(int page = 1, int pageSize = 6)
//{
//    int skip = (page - 1) * pageSize;
//    var result = PatientRepo.FindAllForSearch(p => true, skip, pageSize, ["MedicalRecords"]);

//    List<PatientVM> vm = new List<PatientVM>();
//    foreach (var item in result)
//    {
//        PatientVM patient = new PatientVM()
//        {
//            Id = item.Id,
//            FullName = item.FirstName + " " + item.LastName,

//            Age = DateTime.Today.Year - item.DateOfBirth.Year -
//            (item.DateOfBirth > new DateOnly(DateTime.Today.Year, item.DateOfBirth.Month, item.DateOfBirth.Day) ? 1 : 0),

//            EmergencyContact = item.EmergencyContact,
//            MedicalHistory = item.MedicalHistory,
//        };
//        vm.Add(patient);
//    }

//    var totalCount = PatientRepo.Count();

//    ViewBag.CurrentPage = page;
//    ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
//    string doctorId = "d1";
//    //= User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
//    ViewBag.DoctorId = doctorId;

//    return View(vm);
//}

//public IActionResult DetailsByName(string name, int page = 1, int pageSize = 6)
//{
//    int skip = (page - 1) * pageSize;

//    var nameParts = name.Trim().Split(" ", StringSplitOptions.RemoveEmptyEntries);
//    string firstPart = nameParts.Length > 0 ? nameParts[0].ToLower() : "";
//    string secondPart = nameParts.Length > 1 ? nameParts[1].ToLower() : "";

//    Expression<Func<Patient, bool>> criteria;

//    if (string.IsNullOrEmpty(secondPart))
//    {
//        criteria = p =>
//            p.FirstName.ToLower().Contains(firstPart) ||
//            p.LastName.ToLower().Contains(firstPart);
//    }
//    else
//    {
//        criteria = p =>
//            (p.FirstName.ToLower().Contains(firstPart) && p.LastName.ToLower().Contains(secondPart)) ||
//            (p.FirstName.ToLower().Contains(secondPart) && p.LastName.ToLower().Contains(firstPart));
//    }

//    var patients = PatientRepo.FindAllForSearch(
//        criteria,
//        skip, pageSize, null,
//        p => p.FirstName,
//        OrderBy.Ascending);

//    List<PatientVM> vm = new List<PatientVM>();
//    foreach (var item in patients)
//    {
//        PatientVM patientVM = new PatientVM()
//        {
//            FullName = item.FirstName + " " + item.LastName,

//            Age = DateTime.Today.Year - item.DateOfBirth.Year -
//            (item.DateOfBirth > new DateOnly(DateTime.Today.Year, item.DateOfBirth.Month, item.DateOfBirth.Day) ? 1 : 0),

//            EmergencyContact = item.EmergencyContact,
//            MedicalHistory = item.MedicalHistory,
//        };
//        vm.Add(patientVM);
//    }

//    var totalCount = PatientRepo.Count(criteria);

//    ViewBag.CurrentPage = page;
//    ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);

//    return PartialView("_DetailsByName", vm);
//}

//public IActionResult SearchForMedicalRecords(string patientId, string doctorId)
//{
//    var medicalRecords = MedicalRepo.FindAll(mr => mr.PatientId == patientId && mr.DoctorId == doctorId);

//    if (medicalRecords != null && medicalRecords.Any())
//    {
//        // Tell JS to redirect if there are medical records
//        return PartialView("_MedicalRecordsDisplayButton");
//    }
//    else
//    {
//        // Return a partial view (No records found)
//        return PartialView("_MedicalRecordsAddButton");
//    }
//}
