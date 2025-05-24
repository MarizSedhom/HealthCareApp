﻿using System.Security.Claims;
using HealthCare.BLL.Interface.Repository;
using HealthCare.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;



namespace HealthCareApp.Controllers
{
    public class MedicalRecordController : Controller
    {

        public IGenericRepo<MedicalRecord> _medicalRecordService {  get; set; }
        public IGenericRepo<Patient> _patientService { get; set; }
        public IGenericRepo<HealthCare.DAL.Models.Doctor> _doctorService { get; set; }


        public MedicalRecordController(IGenericRepo<MedicalRecord> medicalRecordService, IGenericRepo<HealthCare.DAL.Models.Doctor> doctorService, IGenericRepo<Patient> patientService)
        {
            _medicalRecordService = medicalRecordService;
            _doctorService = doctorService;
            _patientService = patientService;

        }
        // GET: MedicalReportController
        [Authorize(Roles = "Doctor")]
        public ActionResult Index(string doctorId = null)
        {

            if(doctorId == null)
            {
                doctorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            }
            return View(_medicalRecordService.FindAll(med => med.DoctorId == doctorId, med => med.Patient ,med => med.Doctor));
        }
        
        [Authorize(Roles = "Patient")]//***
        public ActionResult getAllRecordsForPatient(string patientId = null)
        {
            if(patientId == null)
            {
                patientId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            }

            return View(_medicalRecordService.FindAll(med => med.PatientId == patientId, med => med.Doctor, med => med.Patient, d=>d.Doctor.Specialization));
        }

        // GET: MedicalReportController/Details/5
        [Authorize(Roles = "Doctor")]//****
        public ActionResult Details(int id, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(_medicalRecordService.Find(med => med.Id == id, med => med.Patient, med => med.Doctor));
        }

        //public ActionResult Details(string doctorId, string patientId)
        //{
        //    return View(_medicalRecordService.Find(med => med.DoctorId == doctorId && med.PatientId == patientId, med => med.Patient, med => med.Doctor));
        //}
        // GET: MedicalReportController/Create

        [Authorize(Roles = "Doctor")]
        public ActionResult Create(string patientId, string returnUrl, string doctorId = null)
        {
            if (doctorId == null)
            {
                doctorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            }

            var doctor = _doctorService.GetById(doctorId);
           

            ViewBag.Doctor = doctor.FirstName + " " + doctor.LastName;

            var patient = _patientService.GetById(patientId);
          

            ViewBag.Patient = patient;  
            ViewBag.ReturnUrl = returnUrl;

            return View();
        }

        [Authorize(Roles = "Doctor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MedicalRecord medicalRecord, string returnUrl, string doctorId = null)
        {
            if (doctorId == null)
            {
                doctorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            }

            var doctor = _doctorService.GetById(doctorId);
          

            ViewBag.Doctor = doctor.FirstName + " " + doctor.LastName;

            var patient = _patientService.GetById(medicalRecord.PatientId);
          

            ViewBag.Patient = patient;
            ViewBag.ReturnUrl = returnUrl;

            try
            {
                medicalRecord.CreatedAt = DateTime.Now;
                medicalRecord.DoctorId = doctorId;
                _medicalRecordService.Add(medicalRecord);
                return Redirect(returnUrl ?? Url.Action("DisplayPatientsForDoctor", "Patient"));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error creating record: " + ex.Message);
                return View(medicalRecord);
            }
        }


        //GET: MedicalReportController/Edit/5
        [Authorize(Roles = "Doctor")]

        public ActionResult Edit(int id, string returnUrl)
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
            ViewBag.ReturnUrl = returnUrl;

            return View(medicalRecord);
        }

        //POST: MedicalReportController/Edit/5
        [Authorize(Roles = "Doctor")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MedicalRecord medicalRecord, string returnUrl)
        {
            var existingRecord = _medicalRecordService.Find(m => m.Id == medicalRecord.Id);
            if (existingRecord == null)
                return NotFound();

            existingRecord.Diagnosis = medicalRecord.Diagnosis;
            existingRecord.Prescription = medicalRecord.Prescription;

            var doctor = _doctorService.GetById(existingRecord.DoctorId);
           
                ViewBag.Doctor = doctor.FirstName + " " + doctor.LastName;
           
           
            

            ViewBag.Patients = _patientService.GetAll()
                .Select(p => new SelectListItem
                {
                    Value = p.Id,
                    Text = p.FirstName + " " + p.LastName
                }).ToList();

            ViewBag.ReturnUrl = returnUrl;

            try
            {
              
                _medicalRecordService.Update(existingRecord);
                return RedirectToAction("DisplayPatientsForDoctor", "Patient");

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error updating record: " + ex.Message);
                return View(existingRecord);
            }
        }




        // POST: MedicalReportController/Delete/5
        [Authorize(Roles = "Doctor")]

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
