using HealthCare.BLL.Interface.Repository;
using HealthCare.DAL.Models;
using HealthCareApp.ViewModel.Review;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

using System.Security.Claims;

namespace HealthCareApp.Controllers
{
    [Authorize]
    public class ReviewController : Controller
    {
        IGenericRepo<Review> reviewService;
        IGenericRepo<ApplicationUser> userService;
        IGenericRepo<Patient> patientService;
        IGenericRepo<HealthCare.DAL.Models.Doctor> doctorService;

        IGenericRepo<Notification> notificationRepoService;
        NotificationService notificationService;


        public ReviewController(IGenericRepo<Review> _reviewService, IGenericRepo<ApplicationUser> _userService, IGenericRepo<Patient> _patientService, IGenericRepo<HealthCare.DAL.Models.Doctor> _docttorService, IGenericRepo<Notification> _notificationRepoService, NotificationService _notificationService)
        {
            reviewService = _reviewService;
            userService = _userService;
            patientService = _patientService;
            doctorService = _docttorService;
            notificationRepoService = _notificationRepoService;
            notificationService = _notificationService;
        }

        [Authorize("Doctor,Patient")]
        public ActionResult GetDoctorReviews(string doctorId = null)
        {
            // Doctor view
            if (doctorId == null)
            {
                doctorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            }

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

            return View(doctorReviews);
            //if (User.IsInRole("Patient"))
            //{
                
            //}
            //else
            //{
            //    return View("GetReviewsForDr", doctorReviews);
            //}
        }

        [Authorize("Patient")]
        public ActionResult AddReview(string doctorId)
        {
         
            var review = new AddReviewVM()
            {
                DoctorId = doctorId,
            };
            return View(review);
        }

        [Authorize("Patient")]
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
                return RedirectToAction("ViewDoctorDetails", "Doctor", new { doctorId = review.DoctorId});
            }
            else
            {
                return View(reviewVM);
            }
        }

        [Authorize("Patient")]
        public ActionResult EditReview(int id)
        {
            Review review = reviewService.Find(r => r.Id == id);

            return View(review);
        }

        [Authorize("Patient")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditReview(int id, Review review)
        {
            if (ModelState.IsValid)
            {
                review.IsEdited = true;
                review.IsApproved = false;
                reviewService.Update(review);
                return RedirectToAction("ViewDoctorDetails", "Doctor", new { review.DoctorId });
            }
            else
            {
                return View(review);
            }
        }
        [Authorize("Patient")]
        public ActionResult DeleteReview(int id)
        {
            var review = reviewService.GetById(id);
            var DoctorId = review.DoctorId;

            reviewService.SoftDelete(review);

            return RedirectToAction("ViewDoctorDetails", "Doctor", new { DoctorId });
        }



        // admin : approave, delete
        [Authorize("Admin")]

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
        [Authorize("Admin")]
        public ActionResult ApproveReview(int reviewId)
        {
            Review review = reviewService.GetById(reviewId);
            review.IsApproved = true;
            reviewService.Update(review);

            Patient patient = patientService.Find(p => p.Id == review.PatientId);
            review.Patient = patient;

            HealthCare.DAL.Models.Doctor doctor = doctorService.Find(d => d.Id == review.DoctorId);
            review.Doctor = doctor;

            //notification for Patient
            var notificationPt = new Notification
            {
                UserId = review.PatientId,
                Message = $"Your review for Dr {doctor.FirstName} {doctor.LastName} has been approved and published successfully",
                CreatedDate = DateTime.Now,
                notificationType = NotificationType.Review
            };
            notificationService.Notify(notificationPt);

            //notification for dr
            var notificationDr = new Notification
            {
                UserId = review.DoctorId,
                Message = $"A review has been added to you by {patient.FirstName} {patient.LastName}",
                CreatedDate = DateTime.Now,
                notificationType = NotificationType.Review
            };

            notificationService.Notify(notificationDr);

            return RedirectToAction(nameof(DisplayPendingReviews));
        }

        [Authorize("Admin")]
        public ActionResult RejectReview(int reviewId)
        {
            Review review = reviewService.GetById(reviewId);
            review.IsDeleted = true;
            reviewService.Update(review);

            return RedirectToAction(nameof(DisplayPendingReviews));
        }
    }
}
