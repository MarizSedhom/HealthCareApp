namespace HealthCareApp.RepositoryServices
{
    public interface IDoctorRepository:IGenericRepoServices<Doctor>
    {
        Doctor GetDrWithClinicAvailabilities(string doctorId);

    }
}
