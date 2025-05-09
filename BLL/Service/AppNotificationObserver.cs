using HealthCare.DAL.Models;
using HealthCare.BLL.Interface.Repository;
using HealthCare.BLL.Interface.Service;

namespace HealthCare.BLL.Service
{
    public class AppNotificationObserver : INotificationObserver
    {
        private readonly IGenericRepo<Notification> _notificationRepo;

        public AppNotificationObserver(IGenericRepo<Notification> notificationRepo)
        {
            _notificationRepo = notificationRepo;
        }

        public void Update(Notification notification)
        {
            _notificationRepo.Add(notification);
        }
    }
}
