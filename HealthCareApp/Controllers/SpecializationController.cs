using HealthCareApp.Models;
using HealthCareApp.RepositoryServices;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareApp.Controllers
{
    public class SpecializationController : Controller
    {
        private readonly IGenericRepoServices<Specialization> SpecializationRepo;

        public SpecializationController(IGenericRepoServices<Specialization> SpecializationRepo)
        {
            this.SpecializationRepo = SpecializationRepo;
        }
        
        public IActionResult Index()
        {
            return View(SpecializationRepo.GetAll());
        }
        
        public IActionResult DetailsByID(int id)
        {
            return View(SpecializationRepo.GetById(id));
        }

        public IActionResult DetailsByName(string name)
        {
            var specializations = SpecializationRepo.FindAll(spec => spec.Name.ToLower().Contains(name.ToLower()));
            return PartialView("_DetailsByName", specializations);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Specialization specialization)
        {
            if (ModelState.IsValid)
            {
                SpecializationRepo.Add(specialization);
                SpecializationRepo.Save();
                return RedirectToAction(nameof(Index));
            }
            else
                return View(specialization);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            return View(SpecializationRepo.GetById(id));
        }

        [HttpPost]
        public IActionResult Edit(Specialization specialization)
        {
            if (ModelState.IsValid)
            {
                SpecializationRepo.Update(specialization);
                SpecializationRepo.Save();
                return RedirectToAction(nameof(Index));
            }
            else
                return View(specialization);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            return View(SpecializationRepo.GetById(id));
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var deletedSpec = SpecializationRepo.GetById(id);
            SpecializationRepo.Delete(deletedSpec);
            SpecializationRepo.Save();
            return RedirectToAction(nameof(Index));
        }
    }
}
