using HealthCare.DAL.Models;

namespace HealthCare.BLL.Interface.Service
{
    public interface INotificationObserver
    {
        void Update(Notification notification);
    }
}
