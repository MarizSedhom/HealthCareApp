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

        public IActionResult Index(int page = 1, int pageSize = 5)
        {
            int skip = (page - 1) * pageSize;
            var result = SpecializationRepo.FindAllForSearch(s => true, skip, pageSize);
            var totalCount = SpecializationRepo.Count();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            // Check if the request is an AJAX request
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                var resultItems = result.Select(r => new {
                    id = r.Id,
                    name = r.Name
                }).ToList();

                return Json(new
                {
                    items = resultItems,
                    currentPage = page,
                    totalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                });
            }

            return View(result); // Return the full view if not an AJAX request
        }

        public IActionResult DetailsByID(int id, int page = 1)
        {
            ViewBag.CurrentPage = page;
            return View(SpecializationRepo.GetByIdNoTracking(s => s.Id == id));
        }

        public IActionResult DetailsByName(string name, int page = 1, int pageSize = 5)
        {
            int skip = (page - 1) * pageSize;

            var specializations = SpecializationRepo.FindAllForSearch(
                spec => spec.Name.ToLower().Contains(name.ToLower()),
                skip, pageSize, null, s => s.Name, OrderBy.Ascending);

            var totalCount = SpecializationRepo.Count(
                spec => spec.Name.ToLower().Contains(name.ToLower())
            );

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);

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
        public IActionResult Edit(int id, int page = 1)
        {
            ViewBag.CurrentPage = page;
            return View(SpecializationRepo.GetByIdNoTracking(s => s.Id == id));
        }

        [HttpPost]
        public IActionResult Edit(Specialization specialization, int page = 1)
        {
            if (ModelState.IsValid)
            {
                SpecializationRepo.UpdateNoTracking(specialization);
                SpecializationRepo.Save();
                return RedirectToAction(nameof(Index), new {page});
            }
            else
                return View(specialization);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            return View(SpecializationRepo.GetByIdNoTracking(s => s.Id == id));
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
