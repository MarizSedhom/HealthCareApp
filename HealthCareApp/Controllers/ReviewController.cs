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



        public ActionResult GetDoctorReviews(string doctorId = "1")
        {
            var dr = doctorService.GetById(doctorId);
            IEnumerable<Review> reviews = reviewService.FindAll(r => r.DoctorId == doctorId && !r.IsDeleted, r => r.Patient, r => r.Doctor).ToList();

            var approvedReviews = reviews.Where(r => r.IsApproved).ToList();
            var doctorReviews = new DoctorReviewsVM
            {
                TotalRating = approvedReviews.Any() ? approvedReviews.Average(r => r.Rating) : 0.0,
                ReviewsCount = approvedReviews.Count(),
                DoctorName = $"{dr.Title} {dr.FirstName} {dr.LastName}",
                DoctorId = doctorId
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

            if(User.IsInRole("Patient"))
            {
                return View(doctorReviews);
            }
            else
            {
                return View("GetReviewsForDr", doctorReviews);
            }
        }


        public ActionResult AddReview(string doctorId = "80ac78e2-def2-4e42-a1db-a3b58939f63b")
        {
            var review = new AddReviewVM()
            {
                DoctorId = doctorId,
            };
            return View(review);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddReview(AddReviewVM reviewVM)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (ModelState.IsValid)
            {
                var review = new Review()
                {
                    Rating = reviewVM.Rating,
                    ReviewText = reviewVM.ReviewText,
                    PatientId = userId,
                    DoctorId = reviewVM.DoctorId
                };

                reviewService.Add(review);
                return RedirectToAction(nameof(GetDoctorReviews));
            }
            else
            {
                return View(reviewVM);
            }
        }


        public ActionResult EditReview(int id)
        {
            Review review = reviewService.Find(r => r.Id == id);

            return View(review);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditReview(int id, Review review)
        {
            if (ModelState.IsValid)
            {
                review.IsEdited = true;
                reviewService.Update(review);
                return RedirectToAction(nameof(GetDoctorReviews));
            }
            else
            {
                return View(review);
            }
        }

        public ActionResult DeleteReview(int id)
        {
            reviewService.SoftDelete(reviewService.GetById(id));

            return RedirectToAction(nameof(GetDoctorReviews));
        }



        // admin : approave, delete
        public ActionResult DisplayPendingReviews()
        {
            IEnumerable<Review> pendingReviews = reviewService.FindAll(r => !r.IsApproved && !r.IsDeleted, r => r.Doctor, r => r.Patient).ToList();
            var pendingReviewsVM = pendingReviews.Select(r => new ReviewVM
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
                IsDeleted = r.IsDeleted,
                DoctorName = $"{r.Doctor.Title} {r.Doctor.FirstName} {r.Doctor.LastName}"

            });
            return View(pendingReviewsVM);
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
