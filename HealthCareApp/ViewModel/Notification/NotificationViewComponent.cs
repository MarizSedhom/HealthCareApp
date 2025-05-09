using HealthCare.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using HealthCare.BLL.Interface.Repository;
namespace HealthCareApp.ViewModel.Notification
{
    public class NotificationViewComponent : ViewComponent
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IGenericRepo<HealthCare.DAL.Models.Notification> notificationRepoServices;

        public NotificationViewComponent(UserManager<ApplicationUser> userManager, IGenericRepo<HealthCare.DAL.Models.Notification> _notificationRepo)
        {
            _userManager = userManager;
            notificationRepoServices = _notificationRepo;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.GetUserAsync((ClaimsPrincipal)User);
            int count = 0;

            if (user != null)
            {
                count = notificationRepoServices
                        .FindAll(n => n.UserId == user.Id && !n.IsDeleted && !n.IsRead)
                        .Count();
            }

            return View(count);
        }
    }
}