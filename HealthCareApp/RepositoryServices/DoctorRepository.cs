using HealthCareApp.Data;
using Microsoft.EntityFrameworkCore;

namespace HealthCareApp.RepositoryServices
{
    public class DoctorRepository:GenericRepo<Doctor>,IGenericRepoServices<Doctor>, IDoctorRepository
    {
        readonly private ApplicationDbContext context;
        public DoctorRepository(ApplicationDbContext context):base(context)
        {
            this.context = context;
        }
        public Doctor GetDrWithClinicAvailabilities(string doctorId)
        {
            return context.Doctors.Include(d=>d.Clinics).ThenInclude(c=>c.Region).ThenInclude(c=>c.City)
                .Include(d=>d.availabilities).ThenInclude(v=>v.AvailableSlots).ThenInclude(s => s.Appointment)  // Including the Appointment property inside AvailableSlots
                .FirstOrDefault(d=>d.Id==doctorId);
        }
    }
}
