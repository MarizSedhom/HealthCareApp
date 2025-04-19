using HealthCareApp.RepositoryServices;
using Microsoft.AspNetCore.Mvc;
namespace HealthCareApp.Controllers
{
    public class SubSpecializationController : Controller
    {
        private readonly IGenericRepoServices<Specialization> SpecializationRepo;
        private readonly IGenericRepoServices<SubSpecialization> SubSpecializationRepo;
        public SubSpecializationController(IGenericRepoServices<Specialization> SpecializationRepo, IGenericRepoServices<SubSpecialization> SubSpecializationRepo, IGenericRepoServices<SubSpecialization> SupSpecializationRepo)
        {
            this.SpecializationRepo = SpecializationRepo;
            this.SubSpecializationRepo = SubSpecializationRepo;
        }

        public IActionResult Index(int page = 1, int pageSize = 5)
        {
            int skip = (page - 1) * pageSize;
            if (skip < 0)
            {
                skip = 0; // Prevent negative skip values
            }
            var result = SubSpecializationRepo.FindAllForSearch(s => true, skip, pageSize, ["Specialization"]);
            var totalCount = SubSpecializationRepo.Count();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            return View(result);
        }

        public IActionResult DetailsByID(int id, int page)
        {
            var result = SubSpecializationRepo.Find(sspec => sspec.Id == id, sspec => sspec.Specialization);
            ViewBag.CurrentPage = page;
            return View(result);
        }

        public IActionResult DetailsByName(string name, int page = 1, int pageSize = 5)
        {
            int skip = (page - 1) * pageSize;

            var subSpecializations = SubSpecializationRepo.FindAllForSearch(
                spec => spec.Name.ToLower().Contains(name.ToLower()),
                skip, pageSize, ["Specialization"], s => s.Name, OrderBy.Ascending
            );

            var totalCount = SubSpecializationRepo.Count(
                spec => spec.Name.ToLower().Contains(name.ToLower())
            );

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            return PartialView("_DetailsByName", subSpecializations);
        }

        [HttpGet]
        public IActionResult Create(int page)
        {
            ViewBag.CurrentPage = page;
            SubSpecialization subSpecialization = new SubSpecialization();
            ViewBag.Specializations = SpecializationRepo.GetAllNoTracking();
            return View(subSpecialization);
        }

        [HttpPost]
        public IActionResult Create(SubSpecialization subSpecialization, int page)
        {
            if (ModelState.IsValid)
            {
                SubSpecializationRepo.Add(subSpecialization);
                SubSpecializationRepo.Save();
                return RedirectToAction(nameof(Index), new { page });
            }
            else
            {
                ViewBag.Specializations = SpecializationRepo.GetAllNoTracking();
                return View(subSpecialization);
            }
        }

        [HttpGet]
        public IActionResult Edit(int id, int page)
        {
            ViewBag.CurrentPage = page;
            var editedObj = SubSpecializationRepo.Find(sspec => sspec.Id == id, sspec => sspec.Specialization);
            ViewBag.Specializations = SpecializationRepo.GetAllNoTracking();
            ViewBag.CurrentPage = page;
            return View(editedObj);
        }

        [HttpPost]
        public IActionResult Edit(SubSpecialization subSpecialization, int page)
        {
            var existingSubSpecialization = SubSpecializationRepo.Find(
                sspec => sspec.Id == subSpecialization.Id, sspec => sspec.Specialization);

            existingSubSpecialization.Name = subSpecialization.Name;
            existingSubSpecialization.SpecializationId = subSpecialization.SpecializationId;

            if (ModelState.IsValid)
            {
                SubSpecializationRepo.UpdateNoTracking(existingSubSpecialization);
                SubSpecializationRepo.Save();
                return RedirectToAction(nameof(Index), new { page });
            }
            else
            {
                ViewBag.Specializations = SpecializationRepo.GetAllNoTracking();
                return View(subSpecialization);
            }
        }

        [HttpGet]
        public IActionResult Delete(int id, int page)
        {
            ViewBag.CurrentPage = page;
            var editedObj = SubSpecializationRepo.Find(sspec => sspec.Id == id, sspec => sspec.Specialization);
            return View(editedObj);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id, int page)
        {
            var deletedSSpec = SubSpecializationRepo.GetById(id);
            SubSpecializationRepo.SoftDelete(deletedSSpec);
            SubSpecializationRepo.Save();
            return RedirectToAction(nameof(Index), new { page });
        }
    }
}