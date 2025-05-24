using HealthCare.DAL.Models;
using HealthCare.DAL.Data;
using Microsoft.EntityFrameworkCore;
using HealthCare.BLL.Interface.Repository;

namespace HealthCare.DAL.Repository
{
    public class AvailabilityRepository : GenericRepo<Availability>, IAvailabilityRepository
    {
        private readonly ApplicationDbContext context;

        public AvailabilityRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }

        public Availability GetAvailabilitySlotsAppointment(int availabilityId)
        {
            return context.Availability.Include(v => v.Doctor).Include(v => v.AvailableSlots)
                .ThenInclude(s => s.Appointment)
                .FirstOrDefault(v => v.Id == availabilityId);
        }
        public IEnumerable<Availability> GetAvailabilitiesDrInfo()
        {
            return context.Availability.Include(v => v.Doctor).ThenInclude(d => d.Specialization).Include(v => v.Clinic).ToList();
        }
        public AvailabilitySlots GetSlot(int slotId)
        {
            return context.AvailabilitySlots.Include(s => s.Appointment).Include(a => a.Availability).ThenInclude(d => d.Doctor).FirstOrDefault(s => s.Id == slotId);
        }

    }
}
