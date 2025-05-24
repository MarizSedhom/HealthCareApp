using HealthCare.BLL.Interface.Repository;
using HealthCare.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using System.Threading.Tasks;

namespace HealthCareApp.Controllers
{
    [Authorize("Doctor,Patient")]
    public class NotificationController : Controller
    {
        private readonly IGenericRepo<Notification> notificationRepoServices;
        private readonly UserManager<ApplicationUser> userManager;

        public NotificationController(IGenericRepo<Notification> _notificatonRepoServices, UserManager<ApplicationUser> _userManager)
        {
            notificationRepoServices = _notificatonRepoServices;
            userManager = _userManager;
        }
        // GET: NotificationController
        //show all notification for specific user
        public async Task<ActionResult> Index()
        {
            var currentUser = await userManager.GetUserAsync(User);
            var notifications = notificationRepoServices
                .FindAll(u=> u.UserId == currentUser.Id && !u.IsDeleted)
                .OrderByDescending(u=>u.CreatedDate);

            ViewBag.UserFirstName = currentUser.FirstName;
            return View(notifications);
        }

        // GET: NotificationController/Details/5
        public ActionResult Details(int id)
        {
            return View(notificationRepoServices.Find(n=>n.Id==id));
        }

        [HttpPost]
        public IActionResult MarkAsRead(int id)
        {
            var notification = notificationRepoServices.Find(n => n.Id == id);
            if (notification != null && !notification.IsRead)
            {
                notification.IsRead = true;
                notificationRepoServices.Update(notification);
            }
            return Ok();
        }


        // GET: NotificationController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: NotificationController/Create
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

        // GET: NotificationController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: NotificationController/Edit/5
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

        public ActionResult Delete(int id)
        {
            notificationRepoServices.SoftDelete(notificationRepoServices.GetById(id));
            return RedirectToAction(nameof(Index));
        }

        //// POST: NotificationController/Delete/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Delete(int id, IFormCollection collection)
        //{
        //    try
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
    }
}
