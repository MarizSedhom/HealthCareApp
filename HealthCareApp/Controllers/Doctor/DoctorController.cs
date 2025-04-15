using HealthCareApp.RepositoryServices;
using HealthCareApp.ViewModel.Doctor;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareApp.Controllers.Doctor
{
    public class DoctorController : Controller
    {
        private IGenericRepoServices<Models.Doctor> DoctorRepository { get; }

        public DoctorController(IGenericRepoServices<Models.Doctor> DectorRepository)
        {
            this.DoctorRepository = DectorRepository;
        }


        public IActionResult Index()
        {
            IEnumerable<DoctorIdxVM> doctors = DoctorRepository.FindAllWithSelect(null, d => new DoctorIdxVM()
            { 
                DoctorId = d.Id,
                FirstName = d.FirstName,
                LastName = d.LastName,
                ExperienceYears = d.ExperienceYears,
                Specialization = d.Specialization.Name,
                Title = d.Title,

            });
            return View(doctors);
        }
        //public IActionResult DoctorDetails(string DoctorId)
        //{

        //}
        public IActionResult UpdateDoctorProfile(string DoctorId = "1")
        {
            DrUpdateProfileVM profileVM = DoctorRepository.FindWithSelect(d=>d.Id==DoctorId,d => new DrUpdateProfileVM()
            {
                Clinics = d.Clinics.Select(c => $"{c.Name} ({c.ClinicRegion})"),
                DateOfBirth = d.DateOfBirth,
                Description = d.Description,
                ExperienceYears = d.ExperienceYears,
                Fees = d.Fees,
                FirstName = d.FirstName,
                LastName = d.LastName,
                gender = d.gender,
                ProfilePicture = d.ProfilePicture,
                Specialization = d.Specialization.Name,
                Title = d.Title,
                SubSpecializations = d.SubSpecializations.Select(s => s.Name),
                WaitingTimeInMinutes = d.WaitingTimeInMinutes
            } );

            return View(profileVM);
        }
    }
}
