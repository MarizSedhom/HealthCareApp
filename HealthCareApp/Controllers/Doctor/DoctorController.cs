using HealthCareApp.Models;
using HealthCareApp.RepositoryServices;
using HealthCareApp.Service;
using HealthCareApp.ViewModel.Doctor;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;

namespace HealthCareApp.Controllers.Doctor
{
    public class DoctorController : Controller
    {
        private readonly IFileService fileService;

        private IGenericRepoServices<Models.Doctor> DoctorRepository { get; }
        private IGenericRepoServices<SubSpecialization> SubSpecializationRepository { get; }
        public DoctorController(IGenericRepoServices<Models.Doctor> DectorRepository ,IGenericRepoServices<SubSpecialization> SubSpecializationRepository, IFileService fileService)
        {
            this.DoctorRepository = DectorRepository;
            this.SubSpecializationRepository = SubSpecializationRepository;
            this.fileService = fileService;
        }
        [HttpGet]
        public IActionResult ViewApprovedDoctors()
        {
            IEnumerable<DoctorIdxVM> doctors = DoctorRepository.FindAllWithSelect(null, d => new DoctorIdxVM()
            { 
                DoctorId = d.Id,
                FirstName = d.FirstName,
                LastName = d.LastName,
                ExperienceYears = d.ExperienceYears,
                Specialization = d.Specialization.Name,
                Title = d.Title,
                verificationStatus = d.verificationStatus

            });
            return View(doctors);
        }

        [HttpGet]
        public IActionResult ViewDoctorDetails()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetDoctorDetail(string DoctorId = "1")
        {
            DrUpdateProfileVM profileVM = GetDrUpdateProfileVm(DoctorId);
            return View(profileVM);
        }

        [HttpGet]
        public IActionResult UpdateDoctorProfile()
        {
            string DoctorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            DrUpdateProfileVM profileVM = GetDrUpdateProfileVm(DoctorId);
            return View(profileVM);
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
                    fileService.DeleteFile(doctor.ProfilePicture, FilePaths.DrImagesPath);
                    string imageName = await fileService.uploadFileAsync(profileVM.ProfilePicture, FilePaths.DrImagesPath);
                    doctor.ProfilePicture = imageName;
                }
                DoctorRepository.Update(doctor);
                return RedirectToAction( nameof(GetDoctorDetail),new { DoctorId = profileVM.DrId});
            }

            return View(GetDrUpdateProfileVm(profileVM.DrId));

        }        
        public IActionResult AfterDrRegisteration()
        {
            string DoctorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //string DoctorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            AfterDrRegisterationVM AfterDrRegisteration = DoctorRepository.FindWithSelect(d => d.Id == DoctorId,
                d => new AfterDrRegisterationVM()
                {
                    Specialization = d.Specialization.Name,
                    SubSpecialization = d.Specialization.SubSpecialization.Select(s=>new Item<int, string>() { Id = s.Id , Name = s.Name}),
                     DoctorId = d.Id,
                    
                }
                );    

            return View(AfterDrRegisteration);
        }
        [HttpPost]
        public async Task< IActionResult> AfterDrRegisteration(string DoctorId, AfterDrRegisterationVM afterRegisteration)
        {

            //# string DoctorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Models.Doctor doctor = DoctorRepository.Find(d => d.Id == DoctorId);
            if (ModelState.IsValid) {


                doctor.Fees= afterRegisteration.Fees;
                doctor.WaitingTimeInMinutes= afterRegisteration.WaitingTimeInMinutes;

                IEnumerable<SubSpecialization> SubSpecializations = SubSpecializationRepository.FindAll(s => afterRegisteration.SelectedSubSpecializations.Contains(s.Id));
                foreach (var sub in SubSpecializations)
                {
                    sub.Doctors.Add(doctor);
                }
                if (afterRegisteration.ProfilePicture != null) 
                {                   
                    doctor.ProfilePicture= await fileService.uploadFileAsync(afterRegisteration.ProfilePicture,FilePaths.DrImagesPath);
                   //afterRegisteration.PictureReativeUrl = FilePaths.DrPathRelative + doctor.ProfilePicture;
                }
                if (afterRegisteration.doctorVerificationFile != null)
                {
                    doctor.verificationFileName = await fileService.uploadFileAsync(afterRegisteration.doctorVerificationFile, FilePaths.DrVerificationPath);
                }
                DoctorRepository.SaveChanges();

                //# need to redirect
            }
            afterRegisteration.SubSpecialization = SubSpecializationRepository.FindAllWithSelect(s => s.SpecializationId == doctor.SpecializationId, s => new Item<int, string>() { Id = s.Id, Name = s.Name });
            return View(afterRegisteration);
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
            if (profileVM.ImgName != null)
                profileVM.CurrentPicturePath = FilePaths.DrPathRelative + profileVM.ImgName;
            else
                profileVM.CurrentPicturePath = null;

            return profileVM;
        }

        public IActionResult GetAllDoctorsInfo()
        {
            var allDoctors = DoctorRepository.FindAllWithSelect
            (
                null,
                d => new DoctorInfoVM
                {
                    DoctorId = d.Id,
                    Title = d.Title,
                    FirstName = d.FirstName,
                    LastName = d.LastName,
                    Specialization = d.Specialization.Name,
                    Rate = d.Reviews.Count(r => r.IsApproved && !r.IsDeleted) > 0 ? d.Reviews.Where(r => r.IsApproved && !r.IsDeleted).Average(r => r.Rating) : 0.0,
                    Description = d.Description,
                    Fees = d.Fees,
                    ExperienceYears = d.ExperienceYears,
                    WaitingTimeInMinutes = d.WaitingTimeInMinutes,
                    ProfilePicture = d.ProfilePicture,
                    SubSpecializations = d.SubSpecializations.Select(s => s.Name).ToList(),
                    Clinics = d.Clinics.Select(c => new ClinicInfoVM
                    {
                        Id = c.Id,
                        Name = c.Name,
                        ClinicRegion = c.ClinicRegion,
                        ClinicAddress = c.ClinicAddress,
                        ClinicPhoneNumber = c.ClinicPhoneNumber,
                        ClinicCity = c.ClinicCity,
                    }).ToList()
                }
            );

            return View(allDoctors);
        }
    }
}
