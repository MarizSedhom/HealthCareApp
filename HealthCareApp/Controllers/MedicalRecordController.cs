using HealthCareApp.Models;
using HealthCareApp.RepositoryServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


namespace HealthCareApp.Controllers
{
    public class MedicalRecordController : Controller
    {

        public IGenericRepoServices<MedicalRecord> _medicalRecordService {  get; set; }
        public IGenericRepoServices<Patient> _patientService { get; set; }
        public IGenericRepoServices<HealthCareApp.Models.Doctor> _doctorService { get; set; }


        public MedicalRecordController(IGenericRepoServices<MedicalRecord> medicalRecordService, IGenericRepoServices<HealthCareApp.Models.Doctor> doctorService, IGenericRepoServices<Patient> patientService)
        {
            _medicalRecordService = medicalRecordService;
            _doctorService = doctorService;
            _patientService = patientService;

        }
        // GET: MedicalReportController
        public ActionResult Index(string doctorId = "E123")
        {
            return View(_medicalRecordService.FindAll(med => med.DoctorId == doctorId, med => med.Patient ,med => med.Doctor));
        }

        // GET: MedicalReportController/Details/5
        public ActionResult Details(int id)
        {
            return View(_medicalRecordService.Find(med => med.Id == id, med => med.Patient, med => med.Doctor));
        }

        // GET: MedicalReportController/Create
        public ActionResult Create(string doctorId = "E123")
        {

            var patients = _patientService.GetAll()
             .Select(d => new SelectListItem
             {
                 Value = d.Id,
                 Text = d.FirstName + " " + d.LastName
             }).ToList();

            var doctor = _doctorService.GetById(doctorId);

            ViewBag.Doctor = doctor.FirstName + " " + doctor.LastName;

            ViewBag.Patients = patients;

            return View();
        }

        // POST: MedicalReportController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MedicalRecord medicalRecord, string doctorId = "E123")
        {
            var patients = _patientService.GetAll()
                .Select(d => new SelectListItem
                {
                    Value = d.Id,
                    Text = d.FirstName + " " + d.LastName
                }).ToList();

            var doctor = _doctorService.GetById(doctorId);

            ViewBag.Doctor = doctor.FirstName + " " + doctor.LastName;

            ViewBag.Patients = patients;
            try
            {
                medicalRecord.CreatedAt = DateTime.Now;
                medicalRecord.DoctorId = doctorId;


                _medicalRecordService.Add(medicalRecord);
                return RedirectToAction("Index");
            }
            catch
            {
                return View(medicalRecord);
            }
        }



        //GET: MedicalReportController/Edit/5
        public ActionResult Edit(int id)
        {
            var medicalRecord = _medicalRecordService.Find(med => med.Id == id, med => med.Patient, med => med.Doctor);

            if (medicalRecord == null)
                return NotFound();

            var doctor = _doctorService.GetById(medicalRecord.DoctorId);
            ViewBag.Doctor = doctor.FirstName + " " + doctor.LastName;

            ViewBag.Patients = _patientService.GetAll()
                .Select(p => new SelectListItem
                {
                    Value = p.Id,
                    Text = p.FirstName + " " + p.LastName
                }).ToList();

            return View(medicalRecord);
        }

        // POST: MedicalReportController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MedicalRecord medicalRecord)
        {
            var doctor = _doctorService.GetById(medicalRecord.DoctorId);

            ViewBag.Doctor = doctor.FirstName + " " + doctor.LastName;

            ViewBag.Patients = _patientService.GetAll()
                .Select(p => new SelectListItem
                {
                    Value = p.Id,
                    Text = p.FirstName + " " + p.LastName
                }).ToList();


            try
            {
                medicalRecord.CreatedAt = DateTime.Now;
                _medicalRecordService.Update(medicalRecord);

                return RedirectToAction("Index");
            }
            catch
            {
                return View(medicalRecord);
            }
        }


      

        // POST: MedicalReportController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {

            var medicalRecord = _medicalRecordService.GetById(id);
                _medicalRecordService.SoftDelete(medicalRecord);
                return RedirectToAction(nameof(Index));
            
           
 
    
        }

      

   
    }
}
