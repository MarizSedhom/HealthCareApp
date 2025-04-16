using HealthCareApp.Models;

namespace HealthCareApp.RepositoryServices
{
    public class AppNotificationObserver : INotificationObserver
    {
        private readonly IGenericRepoServices<Notification> _notificationRepo;

        public AppNotificationObserver(IGenericRepoServices<Notification> notificationRepo)
        {
            _notificationRepo = notificationRepo;
        }

        public void Update(Notification notification)
        {
            _notificationRepo.Add(notification); 
        }
    }
}
