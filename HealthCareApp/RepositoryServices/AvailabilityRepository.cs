using HealthCareApp.Data;
using HealthCareApp.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthCareApp.RepositoryServices
{
    public class AvailabilityRepository:GenericRepo<Availability>,IAvailabilityRepository
    {
        private readonly ApplicationDbContext context;

        public AvailabilityRepository(ApplicationDbContext context):base(context)
        {
            this.context = context;
        }

        public Availability GetAvailabilitySlotsAppointment(int availabilityId)
        {
            return context.Availability.Include(v => v.AvailableSlots)
                .ThenInclude(s => s.Appointment)
                .FirstOrDefault(v => v.Id == availabilityId);
        }
        public IEnumerable<Availability> GetAvailabilitiesDrInfo()
        {
            return context.Availability.Include(v => v.Doctor).ThenInclude(d => d.Specialization).Include(v => v.Clinic).ToList();
        }
    }
}
