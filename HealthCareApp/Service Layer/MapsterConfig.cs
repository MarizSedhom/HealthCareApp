using HealthCareApp.Models;
using Mapster;

public class MapsterConfig
{
    public static void RegisterMappingForSubSpec()
    {
        // one way mapping
        TypeAdapterConfig<SubSpecialization, SubSpecializationVM>.NewConfig()
        .Map(dest => dest.SpecializationName, src => src.Specialization.Name);
    }
}



