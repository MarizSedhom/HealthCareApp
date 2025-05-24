using HealthCare.DAL.Models;

namespace HealthCare.BLL.Interface.Service
{
    public interface INotificationSubject
    {
        void Notify(Notification notification);
    }
}
