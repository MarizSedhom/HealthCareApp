using HealthCareApp.Models;

namespace HealthCareApp.RepositoryServices
{
    public class NotificationService : INotificationSubject
    {
        private readonly List<INotificationObserver> _observers;

        public NotificationService(IEnumerable<INotificationObserver> observers)
        {
            _observers = observers.ToList();
        }

        public void Notify(Notification notification)
        {
            foreach (var observer in _observers)
            {
                observer.Update(notification);
            }
        }
    }

}
