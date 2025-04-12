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
            var result = ClinicRepo.FindAll(s => true, skip, pageSize);
            var totalCount = ClinicRepo.Count();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            return View(result);
        }

        public IActionResult DetailsByID(int id)
        {
            return View(ClinicRepo.Find(cl => cl.Id == id));
        }

        public IActionResult DetailsByName(string name, int page = 1, int pageSize = 5)
        {
            int skip = (page - 1) * pageSize;

            var subSpecializations = ClinicRepo.FindAll(
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
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Clinic clinic)
        {
            if (ModelState.IsValid)
            {
                ClinicRepo.Add(clinic);
                ClinicRepo.Save();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(clinic);
            }
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            return View(ClinicRepo.Find(cl => cl.Id == id));
        }

        [HttpPost]
        public IActionResult Edit(Clinic clinic)
        {
            if (ModelState.IsValid)
            {
                ClinicRepo.Update(clinic);
                ClinicRepo.Save();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(clinic);
            }
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            return View(ClinicRepo.Find(cl => cl.Id == id));
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var deletedClin = ClinicRepo.GetByIdNoTracking(cl => cl.Id == id);
            ClinicRepo.Delete(deletedClin);
            ClinicRepo.Save();
            return RedirectToAction(nameof(Index));
        }
    }
}
