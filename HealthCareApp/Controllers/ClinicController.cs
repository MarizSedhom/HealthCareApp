using HealthCareApp.Models;
using HealthCareApp.RepositoryServices;
using HealthCareApp.ViewModel.Clinic;
using HealthCareApp.ViewModel.Doctor;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;

namespace HealthCareApp.Controllers
{
    public class ClinicController : Controller
    {
        private readonly IGenericRepoServices<Clinic> ClinicRepo;
        private readonly IGenericRepoServices<City> CityRepository;

        public ClinicController(IGenericRepoServices<Clinic> ClinicRepo , IGenericRepoServices<City>CityRepository)
        {
            this.ClinicRepo = ClinicRepo;
            this.CityRepository = CityRepository;
        }

        [HttpGet]
        public IActionResult GetDoctorClinics(string? doctorId=null)
        {
            if (doctorId == null)
                doctorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = ClinicRepo.FindAllWithSelect(s => true,c=>new ClinicInfoVM()
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



        public IActionResult DetailsByID(int id)
        {
            ClinicInfoVM clinic = ClinicRepo.FindWithSelect(c => c.Id == id, c => new ClinicInfoVM()
            {
                    ClinicAddress = c.ClinicAddress,
                    ClinicCity = c.Region.City.CityNameEn,
                    ClinicPhoneNumber = c.ClinicPhoneNumber,
                    ClinicRegion = c.Region.RegionNameEn,
                    Id = c.Id,
                    Name = c.Name,
                
            });
            clinic.PaginationInfo = new PaginationInfo();

            return View(clinic);
        }

        public IActionResult DetailsByName(string name, int page = 1, int pageSize = 5)
        {
            int skip = (page - 1) * pageSize;

            var subSpecializations = ClinicRepo.FindAllWithSelect(
                spec => spec.Name.ToLower().Contains(name.ToLower()),
                c=>new ClinicInfoVM()
                {
                    ClinicAddress = c.ClinicAddress,
                    ClinicCity = c.Region.City.CityNameEn,
                    ClinicPhoneNumber = c.ClinicPhoneNumber,
                    ClinicRegion = c.Region.RegionNameEn,
                    Id = c.Id,
                    Name = c.Name,
                }
            );


            return PartialView("_DetailsByName", subSpecializations);
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
            return View(clinic);
        }

        [HttpPost]
        public IActionResult Create(ClinicCreateVM clinicVM, int page)
        {
            if (ModelState.IsValid)
            {
                Clinic NewClinic = new Clinic()
                {
                    ClinicPhoneNumber = clinicVM.ClinicPhoneNumber,
                    ClinicAddress = clinicVM.ClinicAddress,
                    Name = clinicVM.Name,
                    RegionId = clinicVM.SelectedRegionId,
                    DoctorId = clinicVM.doctorId
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

        [HttpGet]
        public IActionResult Edit(int id, int page)
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
                regions=c.Region.City.Regions.Select(r=>new Item<int, string>() { Id = r.Id , Name=r.RegionNameEn }),
            });
            clinic.regions = clinic.regions.OrderBy(r => r.Name);
            clinic.cities = CityRepository.FindAllWithSelect(null, c => new Item<int, string>
            {
                Id = c.Id,
                Name = c.CityNameEn
            }).OrderBy(r => r.Name);

            
            ViewBag.CurrentPage = page;
            return View(clinic);
        }

        [HttpPost]
        public IActionResult Edit(ClinicCreateVM clinicVM, int page)
        {
            if (ModelState.IsValid)
            {
                Clinic oldClinic = ClinicRepo.GetById(clinicVM.Id.Value);
                oldClinic.ClinicPhoneNumber = clinicVM.ClinicPhoneNumber;
                oldClinic.Name = clinicVM.Name;
                oldClinic.RegionId = clinicVM.SelectedRegionId;
                 oldClinic.ClinicAddress = clinicVM.ClinicAddress;
                ClinicRepo.Update(oldClinic);
                //ClinicRepo.UpdateNoTracking(clinicVM);
                // ClinicRepo.Save();
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

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id, int page)
        {
            var deletedClin = ClinicRepo.GetById(id);
            string doctorId = deletedClin.DoctorId;
            ClinicRepo.SoftDelete(deletedClin);
            //ClinicRepo.Save();
            return RedirectToAction(nameof(GetDoctorClinics), new { page, doctorId = doctorId });
        }
    }
}