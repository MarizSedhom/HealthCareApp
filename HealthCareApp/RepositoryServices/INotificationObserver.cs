using HealthCareApp.Models;

namespace HealthCareApp.RepositoryServices
{
    public interface INotificationObserver
    {
        void Update(Notification notification);
    }
}
