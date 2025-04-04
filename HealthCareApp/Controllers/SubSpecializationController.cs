using HealthCareApp.Models;
using HealthCareApp.RepositoryServices;
using HealthCareApp.Service_Layer;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareApp.Controllers
{
    public class SubSpecializationController : Controller
    {
        private readonly IGenericService<SubSpecialization,SubSpecializationVM> genericService;
        private readonly IGenericRepoServices<Specialization> genericRepoServices;

        public SubSpecializationController(IGenericService<SubSpecialization, SubSpecializationVM> genericService, IGenericRepoServices<Specialization> genericRepoServices)
        {
            this.genericService = genericService;
            this.genericRepoServices = genericRepoServices;
        }

        public IActionResult Index()
        {
            return View(genericService.GetAll());
        }

        public IActionResult DetailsByID(int id)
        {
            return View(genericService.Find(sspec => sspec.Id == id, ["Specialization"]));
        }

        public IActionResult DetailsByName(string name)
        {
            var subSpecializations = genericService.FindAll(sspec => sspec.Name.ToLower().Contains(name.ToLower()));
            return Json(subSpecializations);
        }

        [HttpGet]
        public IActionResult Create()
        {
            SubSpecializationVM vm = new SubSpecializationVM();
            vm.Specializations = genericRepoServices.GetAll();
            return View(vm);
        }

        [HttpPost]
        public IActionResult Create(SubSpecializationVM subSpecializationVM)
        {
            if (ModelState.IsValid)
            {
                genericService.Add(subSpecializationVM);
                genericService.Save();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                subSpecializationVM.Specializations = genericRepoServices.GetAll();
                return View(subSpecializationVM);
            }
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            return View(genericService.GetById(id));
        }

        [HttpPost]
        public IActionResult Edit(SubSpecializationVM subSpecializationVM)
        {
            if (ModelState.IsValid)
            {
                genericService.Update(subSpecializationVM);
                genericService.Save();
                return RedirectToAction(nameof(Index));
            }
            else
                return View(subSpecializationVM);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            return View(genericService.GetById(id));
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var deletedSpec = genericService.GetById(id);
            genericService.Delete(deletedSpec);
            genericService.Save();
            return RedirectToAction(nameof(Index));
        }
    }
}
