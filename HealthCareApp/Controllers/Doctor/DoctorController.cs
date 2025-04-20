using HealthCareApp.Models;
using HealthCareApp.RepositoryServices;
using HealthCareApp.Service;
using HealthCareApp.ViewModel.Clinic;
using HealthCareApp.ViewModel.Doctor;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;

namespace HealthCareApp.Controllers.Doctor
{
    public class DoctorController : Controller
    {
        private readonly IGenericRepoServices<Specialization> specializationRepository;
        private readonly IFileService fileService;

        private IGenericRepoServices<Models.Doctor> DoctorRepository { get; }
        private IGenericRepoServices<SubSpecialization> SubSpecializationRepository { get; }
        public DoctorController(IGenericRepoServices<Models.Doctor> DectorRepository, IGenericRepoServices<Specialization> SpecializationRepository, IGenericRepoServices<SubSpecialization> SubSpecializationRepository, IFileService fileService)
        {
            this.DoctorRepository = DectorRepository;
            specializationRepository = SpecializationRepository;
            this.SubSpecializationRepository = SubSpecializationRepository;
            this.fileService = fileService;
        }

        //doctor pending page
        [HttpGet]
        public IActionResult ViewPendingDoctorForAdmin()
        {
            IEnumerable<Models.Doctor> doctors = DoctorRepository.FindAll(d => d.verificationStatus == VerificationStatus.Pinding,d=>d.Specialization).OrderBy(d=>d.CreatedAt);
            IEnumerable<DoctorIdxPendingVM> doctorsVM = doctors.Select(d => new DoctorIdxPendingVM()
            {
                doctorId = d.Id,
                DrName = $"{d.FirstName} {d.LastName}",
                Specialization = d.Specialization.Name,
                Title = d.Title.Value,
                ExperienceYears = d.ExperienceYears,
                CreatedAt = d.CreatedAt,
                verificationFileName = d.verificationFileName
            }).ToList();
            return View(doctorsVM);
        }

        //doctor pending page
        [HttpGet]
        public IActionResult ApproveDoctor(string doctorId,VerificationStatus isApproved)
        {
            Models.Doctor doctor = DoctorRepository.GetById(doctorId);
            doctor.verificationStatus = isApproved;
            DoctorRepository.SaveChanges();
            return RedirectToAction(nameof(ViewPendingDoctorForAdmin));
        }

        [HttpGet]
        public IActionResult ViewApprovedDoctorsAdmin()
        {
            IEnumerable<Models.Doctor> doctors = DoctorRepository.FindAll(d => d.verificationStatus == VerificationStatus.Accepted,d=>d.Specialization);
            IEnumerable<DoctorIdxVM> doctorsVm = doctors.Select(  d => new DoctorIdxVM()
            {
                DoctorId = d.Id,
                drName = $"{d.FirstName} {d.LastName}",
                ExperienceYears = d.ExperienceYears,
                Specialization = d.Specialization.Name,
                Title = d.Title.ToString(),
                //verificationStatus = d.verificationStatus
            });
            return View(doctorsVm);
        }

        [HttpGet]
        public IActionResult ViewDoctorDetailsForAdmin()
        {

            return View();
        }

        [HttpGet]
        public IActionResult UpdateDoctorAdmin(string doctorId= "96537cdd-bddf-4f55-b6ef-ab07e2d49f11")
        {
            Models.Doctor doctor = DoctorRepository.Find(d => d.Id == doctorId, d => d.Specialization, d => d.SubSpecializations);
            AdminUpdateDrVM doctorVM = new AdminUpdateDrVM(doctor);
            Specialization specs = specializationRepository.Find(s => s.Id == doctor.SpecializationId, s => s.SubSpecialization);
            doctorVM.Specializations = specializationRepository.FindAllWithSelect(null, s => new Item<int, string>() { Id = s.Id, Name = s.Name });
            doctorVM.SubSpecializationsList= specs.SubSpecialization.Select(s => new Item<int, string>
            {
                Id = s.Id,
                Name = s.Name,
            });
            return View(doctorVM);

        }

        //[HttpPost]
        //public IActionResult UpdateDoctorByAdmin(string doctorId,Object o)
        //{
        //    return View();

        //}


        //for doctor to read thier profile
        [HttpGet]
        public IActionResult GetDoctorDetail(string doctorId = null)  
        {
            if(doctorId == null)
                doctorId= User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            DrUpdateProfileVM profileVM = GetDrUpdateProfileVm(doctorId);
            return View(profileVM);
        }

        //for doctor to change thier profile
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
               // doctor.PhoneNumber = profileVM.PhoneNumber;
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
        public async Task< IActionResult> AfterDrRegisteration( AfterDrRegisterationVM afterRegisteration, string? DoctorId = null)
        {
            if (DoctorId == null) {
                 DoctorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            }

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
                Clinics = d.Clinics.Select(c => $"{c.Region.City.CityNameEn} ({c.Region.RegionNameEn})"),
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
                DrId = d.Id
            });
            if (profileVM.ImgName != null)
                profileVM.CurrentPicturePath = FilePaths.DrPathRelative + profileVM.ImgName;
            else
                profileVM.CurrentPicturePath = null; // #default image

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
                    Title = d.Title.ToString(),
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
                        ClinicRegion = c.Region.RegionNameEn,
                        ClinicAddress = c.ClinicAddress,
                        ClinicPhoneNumber = c.ClinicPhoneNumber,
                        ClinicCity = c.Region.City.CityNameEn,
                    }).ToList()
                }
            );

            return View(allDoctors);
        }
    }
}
