using AspNetCoreGeneratedDocument;
using HealthCareApp.Models;
using HealthCareApp.RepositoryServices;
using HealthCareApp.Service;
using HealthCareApp.ViewModel.Appointment;
using HealthCareApp.ViewModel.Clinic;
using HealthCareApp.ViewModel.Doctor;
using HealthCareApp.ViewModel.Review;

using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;

namespace HealthCareApp.Controllers.Doctor
{
    public class DoctorController : Controller
    {
        private readonly IGenericRepoServices<Specialization> specializationRepository;
        private readonly IFileService fileService;

        private IDoctorRepository DoctorRepository { get; }
        private IGenericRepoServices<Models.Review> ReviewRepository { get; }
        public IAvailabilityRepository AvailabilityRepository { get; }
        public IGenericRepoServices<Appointment> ApopointmentRepository { get; }
        private IGenericRepoServices<SubSpecialization> SubSpecializationRepository { get; }
        public DoctorController(IDoctorRepository DectorRepository, IGenericRepoServices<Specialization> SpecializationRepository,
            IGenericRepoServices<SubSpecialization> SubSpecializationRepository, IFileService fileService, 
            IGenericRepoServices<Models.Review> ReviewRepository, IAvailabilityRepository AvailabilityRepository , IGenericRepoServices<Appointment>apopointmentRepository)
        {
            this.DoctorRepository = DectorRepository;
            specializationRepository = SpecializationRepository;
            this.SubSpecializationRepository = SubSpecializationRepository;
            this.fileService = fileService;
            this.ReviewRepository = ReviewRepository;
            this.AvailabilityRepository = AvailabilityRepository;
            ApopointmentRepository = apopointmentRepository;
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

        //[HttpGet]
        //public IActionResult UpdateDoctorAdmin(string doctorId= "236a0bce-bf14-40ad-a62e-60e8f5e93997")
        //{
        //    Models.Doctor doctor = DoctorRepository.Find(d => d.Id == doctorId, d => d.Specialization, d => d.SubSpecializations);
        //    AdminUpdateDrVM doctorVM = new AdminUpdateDrVM(doctor);
        //    Specialization specs = specializationRepository.Find(s => s.Id == doctor.SpecializationId, s => s.SubSpecialization);
        //    doctorVM.Specializations = specializationRepository.FindAllWithSelect(null, s => new Item<int, string>() { Id = s.Id, Name = s.Name });
        //    doctorVM.SubSpecializationsList= specs.SubSpecialization.Select(s => new Item<int, string>
        //    {
        //        Id = s.Id,
        //        Name = s.Name,
        //    });
        //    doctorVM.CurrentPicturePath=FilePaths.DrImgPathRelative+doctorVM.ImgName;
        //    if (doctorVM.verificationFileName != null)
        //    {
        //        doctorVM.CurrrentverificationPath = FilePaths.DrVerificationRelative + doctorVM.verificationFileName;

        //    }
        //    return View(doctorVM);

        //}
        private AdminUpdateDrVM UpdateDoctorAdmin(string doctorId = "236a0bce-bf14-40ad-a62e-60e8f5e93997")
        {
            Models.Doctor doctor = DoctorRepository.Find(d => d.Id == doctorId, d => d.Specialization, d => d.SubSpecializations);
            AdminUpdateDrVM doctorVM = new AdminUpdateDrVM(doctor);
            Specialization specs = specializationRepository.Find(s => s.Id == doctor.SpecializationId, s => s.SubSpecialization);
            doctorVM.Specializations = specializationRepository.FindAllWithSelect(null, s => new Item<int, string>() { Id = s.Id, Name = s.Name });
            doctorVM.SubSpecializationsList = specs.SubSpecialization.Select(s => new Item<int, string>
            {
                Id = s.Id,
                Name = s.Name,
            });
            doctorVM.CurrentPicturePath = FilePaths.DrImgPathRelative + doctorVM.ImgName;
            if (doctorVM.verificationFileName != null)
            {
                doctorVM.CurrrentverificationPath = FilePaths.DrVerificationRelative + doctorVM.verificationFileName;

            }
            return doctorVM;

        }

        [HttpPost]
        public async Task<IActionResult> UpdateDoctorAdmin(AdminUpdateDrVM doctorVM)
        {
            if (ModelState.IsValid)
            {
                Models.Doctor doctor = DoctorRepository.Find(d => d.Id == doctorVM.doctorId, d => d.SubSpecializations);
                doctor.FirstName = doctorVM.FirstName;
                doctor.LastName = doctorVM.LastName;
                // doctor.PhoneNumber = profileVM.PhoneNumber;
                doctor.gender = doctorVM.gender;
                doctor.Fees = doctorVM.Fees;
                doctor.Description = doctorVM.Description;
                doctor.Title = doctorVM.Title;
                doctor.Description = doctorVM.Description;
                doctor.ExperienceYears = doctorVM.ExperienceYears;
                doctor.WaitingTimeInMinutes = doctorVM.WaitingTimeInMinutes;
                doctor.SpecializationId = doctorVM.SelectedSpecialization;
                doctor.DateOfBirth = doctorVM.DateOfBirth;
                doctor.SubSpecializations.Clear();

                IEnumerable<SubSpecialization> SubSpecializations = SubSpecializationRepository.FindAll(s => doctorVM.SelectedSubSpecializations.Contains(s.Id));
                foreach (var sub in SubSpecializations)
                {
                    sub.Doctors.Add(doctor);
                }

                if (doctorVM.ProfilePicture != null)
                {
                    string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", FilePaths.DrImagesPath);
                    if (!Directory.Exists(fullPath))
                    {
                        Directory.CreateDirectory(fullPath);
                    }
                    fileService.DeleteFile(doctor.ProfilePicture, FilePaths.DrImagesPath);
                    string imageName = await fileService.uploadFileAsync(doctorVM.ProfilePicture, FilePaths.DrImagesPath);
                    doctor.ProfilePicture = imageName;
                }
                if (doctorVM.verificationFileFromDr != null)
                {
                    string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", FilePaths.DrVerificationPath);
                    if (!Directory.Exists(fullPath))
                    {
                        Directory.CreateDirectory(fullPath);
                    }
                    fileService.DeleteFile(doctor.verificationFileName, FilePaths.DrVerificationPath);
                    string verificationFileName = await fileService.uploadFileAsync(doctorVM.verificationFileFromDr, FilePaths.DrVerificationPath);
                    doctor.verificationFileName = verificationFileName;
                }

                DoctorRepository.Update(doctor);
                return RedirectToAction(nameof(UpdateDoctorAdmin), new { doctorId = doctorVM.doctorId });
            }

            return RedirectToAction(nameof(UpdateDoctorAdmin), new { doctorId = doctorVM.doctorId });

            //return View(GetDrUpdateProfileVm(profileVM.DrId));
        }

        [HttpGet]
        // DoctorController.cs
        public IActionResult ViewDoctorDetails(string doctorId)
        {
            if (string.IsNullOrEmpty(doctorId))
            {
                return NotFound();
            }
            var doctor = DoctorRepository.Find(d => d.Id == doctorId, d => d.Specialization, d => d.SubSpecializations, d => d.Clinics);

            if (doctor == null)
            {
                return NotFound();
            }
            var reviews = ReviewRepository.FindAll(r => r.DoctorId == doctorId && !r.IsDeleted, r => r.Patient, r => r.Doctor).ToList();
            var approvedReviews = reviews.Where(r => r.IsApproved).ToList();

            // Get doctor availabilities
            IEnumerable<AvailabilityWithSlotVM> drAvailabilities = AvailabilityRepository.FindAllWithSelect(
                v => v.DoctorId == doctorId,
                v => new AvailabilityWithSlotVM()
                {
                    AvailabilityDate = v.Date,
                    AvailabilityId = v.Id,
                    AvailabilityType = v.type,
                    ClinicName = $"{v.Clinic.Region.City.CityNameEn} ({v.Clinic.Region.RegionNameEn})",
                    Slots = v.AvailableSlots.Select(s => new Slot()
                    {
                        EndTime = s.EndTime,
                        IsBooked = s.IsBooked,
                        SlotId = s.Id,
                        StartTime = s.StartTime
                    })
                }).OrderBy(v => v.AvailabilityDate);

            var viewModel = new DrProfileForPatientVM
            {
                Doctor = doctor,
                Reviews = approvedReviews.Select(r => new ReviewVM
                {
                    Id = r.Id,
                    Rating = r.Rating,
                    ReviewText = r.ReviewText,
                    ReviewDate = r.ReviewDate,
                    PatientName = $"{r.Patient.FirstName} {r.Patient.LastName}",
                    Age = DateOnly.FromDateTime(DateTime.Now).Year - r.Patient.DateOfBirth.Year
                }).ToList(),
                AverageRating = approvedReviews.Any() ? approvedReviews.Average(r => r.Rating) : 0.0,
                ReviewsCount = approvedReviews.Count(),
                Availabilities = drAvailabilities.ToList()
            };
            return View(viewModel);
        }


        [HttpGet]
        public IActionResult GetDoctorDetail(string DoctorId = null)
        {
            if(DoctorId == null)
            {
                DoctorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            }

            DrUpdateProfileVM profileVM = GetDrUpdateProfileVm(DoctorId);
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
                    string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", FilePaths.DrImagesPath);
                    if (!Directory.Exists(fullPath))
                    {
                        Directory.CreateDirectory(fullPath);
                    }
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
                profileVM.CurrentPicturePath = FilePaths.DrImgPathRelative + profileVM.ImgName;
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
        
        //doctor mangment
        
        [HttpGet]
        public IActionResult GetDoctorManagment(string doctorId  )        
        {
            Models.Doctor doctor = DoctorRepository.GetDrWithClinicAvailabilities(doctorId);
            DrManagementVM drManagementVM = new DrManagementVM();
            drManagementVM.doctorId = doctorId;
            drManagementVM.DoctorVM = UpdateDoctorAdmin(doctorId);
            drManagementVM.clinicInfoVMs = doctor.Clinics.Select(c => new ClinicInfoVM()
            {
                ClinicAddress = c.ClinicAddress,
                ClinicCity = c.Region.City.CityNameEn,
                ClinicPhoneNumber = c.ClinicPhoneNumber,
                ClinicRegion = c.Region.RegionNameEn,
                Id = c.Id,
                Name = c.Name,
                doctorId = doctorId
            });
            drManagementVM.Availabilities = doctor.availabilities.Select(v=>new GetAvailabilityForDrVM
            {
                AvailableSlotsCnt = v.AvailableSlots.Count(v => !v.IsBooked),
                AppointmentCnt = v.AvailableSlots.Count(v => v.IsBooked),
                ClinicName = $"{v.Clinic.Region.City.CityNameEn} ({v.Clinic.Region.RegionNameEn})",
                Date = v.Date,
                dayOfWeek = v.dayOfWeek,
                DoctorId = doctorId,
                Duration = v.Duration,
                StartTime = v.StartTime,
                EndTime = v.EndTime,
                TimeRange = $"{v.StartTime} - {v.EndTime}",
                Id = v.Id,
                type = v.type,
                
            }).OrderBy(v => v.Date);


          //var slots = doctor.availabilities.SelectMany(s => s.AvailableSlots).Where(s => s.IsBooked).Select(s => s.Appointment)//.Select(
            drManagementVM.upcomingAppointmentsVM= ApopointmentRepository.FindAllWithSelect( null,app => new UpcomingAppointmentsVM
            {
                Status = app.Status,
                Day = app.AvailableSlot.Availability.dayOfWeek,
                Date = app.AvailableSlot.Availability.Date,
                Time = app.AvailableSlot.StartTime,
                PatientId = app.PatientId,
                PatientName = $"{app.Patient.FirstName} {app.Patient.LastName}",
                PatientPhone = app.Patient.PhoneNumber,
                Mode = app.AvailableSlot.Availability.type,
                paymentStatus = app.PaymentStatus,
                paymentMethod = app.PaymentMethod,


            });
            return View(drManagementVM);
        }

        
    }
}
