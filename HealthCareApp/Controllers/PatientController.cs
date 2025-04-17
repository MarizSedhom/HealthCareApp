using HealthCareApp.Models;
using HealthCareApp.RepositoryServices;
using HealthCareApp.ViewModel.Patient;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareApp.Controllers
{
    public class PatientController : Controller
    {
        IGenericRepoServices<Patient> patientService;
        public PatientController(IGenericRepoServices<Patient> _patientService)
        {
            patientService = _patientService;
        }

        public ActionResult DisplayPatientInfoForDoctor(string patientId)
        {
            var patientInfo = patientService.FindWithSelect(p => p.Id == patientId,
                p => new PatientInfoForDoctorVM
                {
                    PatientFullName = $"{p.FirstName} {p.LastName}",
                    Age = DateTime.Now.Year - p.DateOfBirth.Year,
                    MedicalHistory = p.MedicalHistory
                });

            return View(patientInfo);
        }
    }
}
