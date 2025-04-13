using HealthCareApp.Models;
using HealthCareApp.RepositoryServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareApp.Controllers
{
    public class MedicalRecordController : Controller
    {
        public IGenericRepoServices<MedicalRecord> _genericRepo { get; }

        public MedicalRecordController(IGenericRepoServices<MedicalRecord> genericRepo)
        {
            _genericRepo = genericRepo;
        }

        // GET: MedicalRecordController
        public ActionResult Index()
        {
            return View(_genericRepo.GetAll());
        }

        // GET: MedicalRecordController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: MedicalRecordController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MedicalRecordController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: MedicalRecordController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: MedicalRecordController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: MedicalRecordController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: MedicalRecordController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
