using HealthCareApp.Models;
using HealthCareApp.RepositoryServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Claims;

namespace HealthCareApp.Controllers
{
    public class ReviewController : Controller
    {
        IGenericRepoServices<Review> reviewService;
        IGenericRepoServices<ApplicationUser> userService;
        IGenericRepoServices<Patient> patientService;

        public ReviewController(IGenericRepoServices<Review> _reviewService, IGenericRepoServices<ApplicationUser> _userService, IGenericRepoServices<Patient> _patientService)
        {
            reviewService = _reviewService;
            userService = _userService;
            patientService = _patientService;
        }

        
        public ActionResult Index(string doctorId = "hggvftgf55555555", string userId = null) // id of patient who views specific doctor reviews
        {
            IEnumerable<Review> doctorReviews = reviewService.FindAll(r => r.DoctorId == doctorId && !r.IsDeleted, r => r.Patient, r => r.Doctor).ToList();
            Patient patient = patientService.Find(u => u.Id == userId);

            ViewBag.userId = userId;
            ViewBag.isPatient = patient != null;

            return View(doctorReviews);
        }

        
        public ActionResult Create(string doctorId = "hggvftgf55555555", string patientId = "2")
        {
            ViewBag.patientId = patientId;
            ViewBag.doctorId = doctorId;

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Review review)
        {
            if (ModelState.IsValid)
            {
                reviewService.Add(review);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(review);
            }
        }


        public ActionResult Edit(int id)
        {
            Review review = reviewService.Find(r => r.Id == id, r => r.Patient, r => r.Doctor);

            return View(review);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Review review)
        {
            if(ModelState.IsValid)
            {
                review.IsEdited = true;
                reviewService.Update(review);

                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(review);
            }
        }

        public ActionResult Delete(int id)
        {
            reviewService.SoftDelete(reviewService.GetById(id));

            return RedirectToAction(nameof(Index));
        }



        // admin : approave, delete
        public ActionResult DisplayPendingReviews()
        {
            IEnumerable<Review> pendingReviews= reviewService.FindAll(r => !r.IsApproved && !r.IsDeleted, r => r.Patient, r => r.Doctor).ToList();
            return View(pendingReviews);
        }

        public ActionResult ApproveReview(int reviewId)
        {
            Review review = reviewService.GetById(reviewId);
            review.IsApproved = true;
            reviewService.Update(review);

            return RedirectToAction(nameof(DisplayPendingReviews));
        }

        public ActionResult RejectReview(int reviewId)
        {
            Review review = reviewService.GetById(reviewId);
            review.IsDeleted = true;
            reviewService.Update(review);

            return RedirectToAction(nameof(DisplayPendingReviews));
        }
    }
}
