using HealthCare.BLL.Interface.Repository;
using HealthCare.DAL.Models;
using HealthCareApp.ViewModel.Clinic;
using HealthCareApp.ViewModel.Doctor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;

namespace HealthCareApp.Controllers
{
    public class ClinicController : Controller
    {
        private readonly IGenericRepo<Clinic> ClinicRepo;
        private readonly IGenericRepo<City> CityRepository;
        private readonly IConfiguration Configuration;

        public ClinicController(IGenericRepo<Clinic> ClinicRepo , IGenericRepo<City>CityRepository, IConfiguration configuration)
        {
            this.ClinicRepo = ClinicRepo;
            this.CityRepository = CityRepository;
            Configuration = configuration;
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetDoctorClinics( string? doctorId=null)
        {
            if (doctorId == null)
                doctorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = ClinicRepo.FindAllWithSelect(s => s.DoctorId==doctorId,c=>new ClinicInfoVM()
            {
                ClinicAddress = c.ClinicAddress,
                ClinicCity = c.Region.City.CityNameEn,
                ClinicPhoneNumber = c.ClinicPhoneNumber,
                ClinicRegion = c.Region.RegionNameEn,
                Id = c.Id,
                Name = c.Name,
                doctorId=c.DoctorId
            });
            ViewBag.doctorId = doctorId;

            return View(result);
        }

        [Authorize(Roles = "Admin,Doctor")]
        [HttpGet]
        public IActionResult ManageClinics(string doctorId = null)
        {
            if (doctorId == null)
                doctorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = ClinicRepo.FindAllWithSelect(s => true, c => new ClinicInfoVM()
            {
                ClinicAddress = c.ClinicAddress,
                ClinicCity = c.Region.City.CityNameEn,
                ClinicPhoneNumber = c.ClinicPhoneNumber,
                ClinicRegion = c.Region.RegionNameEn,
                Id = c.Id,
                Name = c.Name,
            });

            return View(result);

        }

        public IActionResult DetailsByID(int id, string returnUrl)
        {
            ClinicInfoVM clinic = ClinicRepo.FindWithSelect(c => c.Id == id, c => new ClinicInfoVM()
            {
                    ClinicAddress = c.ClinicAddress,
                    ClinicCity = c.Region.City.CityNameEn,
                    ClinicPhoneNumber = c.ClinicPhoneNumber,
                    ClinicRegion = c.Region.RegionNameEn,
                    Id = c.Id,
                    Name = c.Name,
                    ClinicLocation = c.ClinicLocation,

            });
            ViewBag.ReturnUrl = returnUrl;
            clinic.PaginationInfo = new PaginationInfo();

            return View(clinic);
        }

        public IActionResult DetailsByName(string name)
        {
            IEnumerable<ClinicInfoVM> clinicInfoVMs ;
            if (name != null)
            {
                clinicInfoVMs = ClinicRepo.FindAllWithSelect(
                    spec => spec.Name.ToLower().Contains(name.ToLower()),
                    c => new ClinicInfoVM()
                    {
                        ClinicAddress = c.ClinicAddress,
                        ClinicCity = c.Region.City.CityNameEn,
                        ClinicPhoneNumber = c.ClinicPhoneNumber,
                        ClinicRegion = c.Region.RegionNameEn,
                        Id = c.Id,
                        Name = c.Name,
                    }
                );
            }
            else
                clinicInfoVMs= new List<ClinicInfoVM>();

            return View("GetDoctorClinics", clinicInfoVMs);
        }

        /// Ajax Call
        public IActionResult GetRegions(int CityId)
        {
            City city = CityRepository.Find(c => c.Id == CityId, c => c.Regions);
            var regions = city.Regions.Select(c => new Item<int,string>
            {
                Id = c.Id,
                Name = c.RegionNameEn
            }).OrderBy(r=>r.Name). ToList();
            return Json(regions);
        }

        [Authorize(Roles = "Admin,Doctor")]
        [HttpGet]
        public IActionResult Create(int page , string?doctorId=null)
        {
            if (doctorId == null)
                doctorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            ClinicCreateVM clinic = new ClinicCreateVM();
            clinic.cities = CityRepository.FindAllWithSelect(null, c => new Item<int, string>
            {
                Id = c.Id,
                Name = c.CityNameEn
            }).OrderBy(r => r.Name);
            clinic.regions = new List<Item<int, string>>();
            clinic.doctorId = doctorId;

            clinic.PaginationInfo = new PaginationInfo();
            clinic.PaginationInfo.CurrentPage = page;
            ViewBag.CurrentPage = page;

            clinic.ClinicLocation = new Location();
            clinic.ClinicLocation.X = 0;
            clinic.ClinicLocation.Y = 0;
            clinic.ClinicLocation.KeyId = Configuration["GoogleMaps:ApiKey"];
            return View(clinic);
        }

        [Authorize(Roles = "Admin,Doctor")]
        [HttpPost]
        public IActionResult Create(ClinicCreateVM clinicVM,  int page)
        {
            if (ModelState.IsValid)
            {
                Clinic NewClinic = new Clinic()
                {
                    ClinicPhoneNumber = clinicVM.ClinicPhoneNumber,
                    ClinicAddress = clinicVM.ClinicAddress,
                    Name = clinicVM.Name,
                    RegionId = clinicVM.SelectedRegionId,
                    DoctorId = clinicVM.doctorId,
                    ClinicLocation = new Location()
                    {
                        X = clinicVM.ClinicLocation.X,
                        Y = clinicVM.ClinicLocation.Y,
                        KeyId = Configuration["GoogleMaps:ApiKey"]
                    }
                };
                ClinicRepo.Add(NewClinic);
                return RedirectToAction(nameof(GetDoctorClinics), new { page , doctorId=clinicVM.doctorId });
            }
            else
            {
                clinicVM.cities = CityRepository.FindAllWithSelect(null, c => new Item<int, string>
                {
                    Id = c.Id,
                    Name = c.CityNameEn
                }).OrderBy(r => r.Name);
                if(clinicVM.SelectedCityId > 0)
                {
                    var regions = CityRepository.FindWithSelect(c => c.Id == clinicVM.SelectedCityId, c => c.Regions);
                    clinicVM.regions = regions.Select(r => new Item<int, string>() { Id = r.Id, Name = r.RegionNameEn });
                }
                return View(clinicVM);
            }
        }

        [Authorize(Roles = "Admin,Doctor")]
        [HttpGet]
        public IActionResult Edit(int id, int page, string doctorId=null,string returnUrl = null)
        {

            ClinicCreateVM clinic = new ClinicCreateVM();
            clinic = ClinicRepo.FindWithSelect(c => c.Id == id, c => new ClinicCreateVM()
            {
                Id = c.Id,
                Name = c.Name,
                doctorId = c.DoctorId,
                ClinicPhoneNumber = c.ClinicPhoneNumber,
                ClinicAddress = c.ClinicAddress,
                SelectedCityId=c.Region.City.Id,
                SelectedRegionId=c.RegionId,
                ClinicLocation=c.ClinicLocation,
                regions=c.Region.City.Regions.Select(r=>new Item<int, string>() { Id = r.Id , Name=r.RegionNameEn }),
            });
            clinic.regions = clinic.regions.OrderBy(r => r.Name);
            clinic.cities = CityRepository.FindAllWithSelect(null, c => new Item<int, string>
            {
                Id = c.Id,
                Name = c.CityNameEn
            }).OrderBy(r => r.Name);

            ViewBag.ReturnUrl = returnUrl;
            ViewBag.CurrentPage = page;
            return View(clinic);
        }

        [Authorize(Roles = "Admin,Doctor")]
        [HttpPost]
        public IActionResult Edit(ClinicCreateVM clinicVM, int page , string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                Clinic oldClinic = ClinicRepo.GetById(clinicVM.Id.Value);
                oldClinic.ClinicPhoneNumber = clinicVM.ClinicPhoneNumber;
                oldClinic.Name = clinicVM.Name;
                oldClinic.RegionId = clinicVM.SelectedRegionId;
                 oldClinic.ClinicAddress = clinicVM.ClinicAddress;
                oldClinic.ClinicLocation = clinicVM.ClinicLocation;
                ClinicRepo.Update(oldClinic);
                //ClinicRepo.UpdateNoTracking(clinicVM);
                // ClinicRepo.Save();
                //if (returnUrl != null)
                //    return RedirectToAction(returnUrl);
                return RedirectToAction(nameof(GetDoctorClinics), new { page, doctorId = clinicVM.doctorId });
            }
            else
            {
                clinicVM.cities = CityRepository.FindAllWithSelect(null, c => new Item<int, string>
                {
                    Id = c.Id,
                    Name = c.CityNameEn
                });
                if (clinicVM.SelectedCityId > 0)
                {
                    var regions = CityRepository.FindWithSelect(c => c.Id == clinicVM.SelectedCityId, c => c.Regions);
                    clinicVM.regions = regions.Select(r => new Item<int, string>() { Id = r.Id, Name = r.RegionNameEn });
                }
                return View(clinicVM);
            }
        }

        [Authorize(Roles = "Admin,Doctor")]
        [HttpGet]
        public IActionResult Delete(int id, int page)
        {
            ViewBag.CurrentPage = page;
            ClinicInfoVM clinic = ClinicRepo.FindWithSelect(c => c.Id == id, c => new ClinicInfoVM()
            {
                ClinicAddress = c.ClinicAddress,
                ClinicCity = c.Region.City.CityNameEn,
                ClinicPhoneNumber = c.ClinicPhoneNumber,
                ClinicRegion = c.Region.RegionNameEn,
                doctorId = c.DoctorId,
                Id = c.Id,
                Name = c.Name,

            });
            return View(clinic);
        }

        [Authorize(Roles = "Admin,Doctor")]
        public IActionResult DeleteConfirmed(int id, int? page=null)
        {
            var deletedClin = ClinicRepo.GetById(id);
            string doctorId = deletedClin.DoctorId;
            ClinicRepo.SoftDelete(deletedClin);
            //ClinicRepo.Save();
            return RedirectToAction(nameof(GetDoctorClinics), new { page, doctorId = doctorId });
        }
    }
}