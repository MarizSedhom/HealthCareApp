using HealthCareApp.Models;
using HealthCareApp.RepositoryServices;
using HealthCareApp.Service;
using HealthCareApp.ViewModel.Doctor;
using Microsoft.AspNetCore.Mvc;
using System.Numerics;
using System.Security.Claims;

namespace HealthCareApp.Controllers.Doctor
{
    public class DoctorController : Controller
    {
        private readonly IFileService fileService;

        private IGenericRepoServices<Models.Doctor> DoctorRepository { get; }
        public DoctorController(IGenericRepoServices<Models.Doctor> DectorRepository,IFileService fileService)
        {
            this.DoctorRepository = DectorRepository;
            this.fileService = fileService;
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
            DrUpdateProfileVM profileVM = GetDrUpdateProfileVm(DoctorId);
            return View(profileVM);
        }
        private DrUpdateProfileVM GetDrUpdateProfileVm(string DrId)
        {
            DrUpdateProfileVM profileVM = DoctorRepository.FindWithSelect(d => d.Id == DrId, d => new DrUpdateProfileVM()
            {
                Clinics = d.Clinics.Select(c => $"{c.Name} ({c.ClinicRegion})"),
                DateOfBirth = d.DateOfBirth,
                Description = d.Description,
                ExperienceYears = d.ExperienceYears,
                ImgName = d.ProfilePicture,
                Fees = d.Fees,
                FirstName = d.FirstName,
                LastName = d.LastName,
                gender = d.gender,
                Specialization = d.Specialization.Name,
                Title = d.Title,
                SubSpecializations = d.SubSpecializations.Select(s => s.Name),
                WaitingTimeInMinutes = d.WaitingTimeInMinutes,
                DrId = d.Id,
                PhoneNumber = d.PhoneNumber
            });
            profileVM.CurrentPicturePath = FilePaths.DrPathRelative + profileVM.ImgName;
            return profileVM;
        }


        [HttpPost]
        public async Task< IActionResult> UpdateDoctorProfile(DrUpdateProfileVM profileVM)
        {

            if (ModelState.IsValid) {

                Models.Doctor doctor = DoctorRepository.GetById(profileVM.DrId);
                doctor.FirstName = profileVM.FirstName;
                doctor.LastName = profileVM.LastName;
                doctor.PhoneNumber = profileVM.PhoneNumber;
                doctor.gender = profileVM.gender;
                doctor.Fees = profileVM.Fees;
                doctor.Description = profileVM.Description;
                doctor.Title = profileVM.Title;
                doctor.Description = profileVM.Description;
                doctor.ExperienceYears = profileVM.ExperienceYears;
                doctor.WaitingTimeInMinutes = profileVM.WaitingTimeInMinutes;
                if (profileVM.ProfilePicture != null)
                {
                    fileService.DeleteFile(doctor.ProfilePicture, FilePaths.DrPath);
                    string imageName = await fileService.uploadFileAsync(profileVM.ProfilePicture, FilePaths.DrPath);
                    doctor.ProfilePicture = imageName;
                }
                DoctorRepository.Update(doctor);
                return View(GetDrUpdateProfileVm(doctor.Id));
            }

            return View(GetDrUpdateProfileVm(profileVM.DrId));

        }        
        public IActionResult AfterDrRegisteration(string id)
        {

            //string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;


            return View();
        }


    }
}
