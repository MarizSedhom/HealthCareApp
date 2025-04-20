using HealthCareApp.RepositoryServices;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HealthCareApp.ViewModel.Notification
{
    public class NotificationViewComponent : ViewComponent
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IGenericRepoServices<Models.Notification> notificationRepoServices;

        public NotificationViewComponent(UserManager<ApplicationUser> userManager, IGenericRepoServices<Models.Notification> _notificationRepo)
        {
            _userManager = userManager;
            notificationRepoServices = _notificationRepo;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.GetUserAsync((ClaimsPrincipal)User);
            var count = notificationRepoServices
                        .FindAll(n => n.UserId == user.Id && !n.IsDeleted && !n.IsRead)
                        .Count();

            return View(count);
        }
    }
}