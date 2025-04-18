using HealthCareApp.Models;

namespace HealthCareApp.RepositoryServices
{
    public interface INotificationSubject
    {
        void Notify(Notification notification);
    }
}
