using HealthCareApp.Models;
using HealthCareApp.RepositoryServices;
using HealthCareApp.ViewModel.Review;
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
        IGenericRepoServices<Models.Doctor> doctorService;

        public ReviewController(IGenericRepoServices<Review> _reviewService, IGenericRepoServices<ApplicationUser> _userService, IGenericRepoServices<Patient> _patientService, IGenericRepoServices<Models.Doctor> _docttorService)
        {
            reviewService = _reviewService;
            userService = _userService;
            patientService = _patientService;
            doctorService = _docttorService;
        }

        
        public ActionResult GetDoctorReviewsForPatient(string doctorId = "hggvftgf55555555")
        {
            var dr = doctorService.GetById(doctorId);
            IEnumerable<Review> reviews = reviewService.FindAll(r => r.DoctorId == doctorId && !r.IsDeleted, r => r.Patient, r => r.Doctor).ToList();

            var doctorReviews = new DoctorReviewsVM
            {
                TotalRating = reviews.Any() ? reviews.Where(r => r.IsApproved).Average(r => r.Rating) : 0.0,
                ReviewsCount = reviews.Where(r => r.IsApproved).Count(),
                DoctorName = $"{dr.Title} {dr.FirstName} {dr.LastName}"
            };

            doctorReviews.Reviews = reviews.Select(r => new ReviewVM
            {
                Id = r.Id,
                Rating = r.Rating,
                ReviewText = r.ReviewText,
                ReviewDate = r.ReviewDate,
                IsApproved = r.IsApproved,
                IsEdited = r.IsEdited,
                PatientId = r.PatientId,
                PatientName = $"{r.Patient.FirstName} {r.Patient.LastName}",
                Age = DateOnly.FromDateTime(DateTime.Now).Year - r.Patient.DateOfBirth.Year,
                IsDeleted = r.IsDeleted
            });
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
                return RedirectToAction(nameof(GetDoctorReviewsForPatient));
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

                return RedirectToAction(nameof(GetDoctorReviewsForPatient));
            }
            else
            {
                return View(review);
            }
        }

        public ActionResult Delete(int id)
        {
            reviewService.SoftDelete(reviewService.GetById(id));

            return RedirectToAction(nameof(GetDoctorReviewsForPatient));
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
