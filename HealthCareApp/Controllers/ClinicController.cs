using HealthCareApp.RepositoryServices;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareApp.Controllers
{
    public class ClinicController : Controller
    {
        private readonly IGenericRepoServices<Clinic> ClinicRepo;

        public ClinicController(IGenericRepoServices<Clinic> ClinicRepo)
        {
            this.ClinicRepo = ClinicRepo;
        }

        public IActionResult Index(int page = 1, int pageSize = 5)
        {
            int skip = (page - 1) * pageSize;
            var result = ClinicRepo.FindAllForSearch(s => true, skip, pageSize);
            var totalCount = ClinicRepo.Count();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            return View(result);
        }

        public IActionResult DetailsByID(int id, int page)
        {
            ViewBag.CurrentPage = page;
            return View(ClinicRepo.Find(cl => cl.Id == id));
        }

        public IActionResult DetailsByName(string name, int page = 1, int pageSize = 5)
        {
            int skip = (page - 1) * pageSize;

            var subSpecializations = ClinicRepo.FindAllForSearch(
                spec => spec.Name.ToLower().Contains(name.ToLower()),
                skip, pageSize, null, s => s.Name, OrderBy.Ascending
            );

            var totalCount = ClinicRepo.Count(
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
            return View();
        }

        [HttpPost]
        public IActionResult Create(Clinic clinic, int page)
        {
            if (ModelState.IsValid)
            {
                ClinicRepo.Add(clinic);
                ClinicRepo.Save();
                return RedirectToAction(nameof(Index), new {page});
            }
            else
            {
                return View(clinic);
            }
        }

        [HttpGet]
        public IActionResult Edit(int id, int page)
        {
            ViewBag.CurrentPage = page;
            return View(ClinicRepo.Find(cl => cl.Id == id));
        }

        [HttpPost]
        public IActionResult Edit(Clinic clinic, int page)
        {
            if (ModelState.IsValid)
            {
                ClinicRepo.UpdateNoTracking(clinic);
                ClinicRepo.Save();
                return RedirectToAction(nameof(Index), new {page});
            }
            else
            {
                return View(clinic);
            }
        }

        [HttpGet]
        public IActionResult Delete(int id, int page)
        {
            ViewBag.CurrentPage = page;
            return View(ClinicRepo.Find(cl => cl.Id == id));
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id, int page)
        {
            var deletedClin = ClinicRepo.GetById(id);
            ClinicRepo.SoftDelete(deletedClin);
            ClinicRepo.Save();
            return RedirectToAction(nameof(Index), new { page });
        }
    }
}
