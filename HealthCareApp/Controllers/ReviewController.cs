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
        private readonly NotificationService notificationService;
        private readonly UserManager<ApplicationUser> userManager;

        public ReviewController(IGenericRepoServices<Review> _reviewService,
                        IGenericRepoServices<ApplicationUser> _userService,
                        IGenericRepoServices<Patient> _patientService,
                        NotificationService _notificationService,
                        UserManager<ApplicationUser> _userManager)
        {
            reviewService = _reviewService;
            userService = _userService;
            patientService = _patientService;
            notificationService = _notificationService;
            userManager = _userManager;
        }


        public ActionResult Index(string doctorId = "d2dffdfa-3168-4fe8-8e52-60ed1c08fc72", string userId = "324a5999-2878-415d-a8c5-7a0b7be468db") // id of patient who views specific doctor reviews
        {
            IEnumerable<Review> doctorReviews = reviewService.FindAll(r => r.DoctorId == doctorId && !r.IsDeleted, r => r.Patient, r => r.Doctor).ToList();
            Patient patient = patientService.Find(u => u.Id == userId);

            ViewBag.userId = userId;
            ViewBag.isPatient = patient != null;

            return View(doctorReviews);
        }

        
        public ActionResult Create(string doctorId = "d2dffdfa-3168-4fe8-8e52-60ed1c08fc72", string patientId = "324a5999-2878-415d-a8c5-7a0b7be468db")
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
                Patient patient = patientService.Find(p => p.Id == review.PatientId);
                review.Patient = patient;

                reviewService.Add(review);
                var notification = new Notification
                {
                    UserId = review.DoctorId,
                    Message = $"A review has been added to you by {patient.FirstName} {patient.LastName}",
                    CreatedDate = DateTime.Now,
                    notificationType = NotificationType.Review
                };

                notificationService.Notify(notification);
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
