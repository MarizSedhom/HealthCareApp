using HealthCare.DAL.Models;

namespace HealthCare.BLL.Interface.Repository
{
    public interface IAvailabilityRepository : IGenericRepo<Availability>
    {
        Availability GetAvailabilitySlotsAppointment(int availabilityId);
        IEnumerable<Availability> GetAvailabilitiesDrInfo();
        AvailabilitySlots GetSlot(int slotId);


    }
}
