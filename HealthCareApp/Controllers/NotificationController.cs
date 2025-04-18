using HealthCareApp.Models;
using HealthCareApp.RepositoryServices;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using System.Threading.Tasks;

namespace HealthCareApp.Controllers
{
    public class NotificationController : Controller
    {
        private readonly IGenericRepoServices<Notification> notificationRepoServices;
        private readonly UserManager<ApplicationUser> userManager;

        public NotificationController(IGenericRepoServices<Notification> _notificatonRepoServices, UserManager<ApplicationUser> _userManager)
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
            return View();
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
