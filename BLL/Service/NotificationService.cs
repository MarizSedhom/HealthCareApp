using HealthCare.BLL.Interface.Service;
using HealthCare.DAL.Models;

namespace HealthCare.BLL.Service
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
