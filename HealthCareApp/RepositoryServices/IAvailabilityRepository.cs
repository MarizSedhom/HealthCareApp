using HealthCareApp.Models;

namespace HealthCareApp.RepositoryServices
{
    public interface IAvailabilityRepository:IGenericRepoServices<Availability>
    {
        Availability GetAvailabilitySlotsAppointment(int availabilityId);
        IEnumerable<Availability> GetAvailabilitiesDrInfo();
         AvailabilitySlots GetSlot(int slotId);


    }
}
