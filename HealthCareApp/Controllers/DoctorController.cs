using HealthCareApp.RepositoryServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Xml.Linq;

namespace HealthCareApp.Controllers
{
    public class DoctorController : Controller
    {
        private readonly IGenericRepoServices<Doctor> DoctorRepo;

        public DoctorController(IGenericRepoServices<Doctor> doctorRepo)
        {
            DoctorRepo = doctorRepo;
        }

        /*public IActionResult DoctorFilteration(string prop, string val)
        {
            // If prop is Gender, convert the val string back to the enum
            if (prop == "Gender")
            {
                if (val == "Gender.Male")
                {
                    val = Gender.Male.ToString();
                }
                else if (val == "Gender.Female")
                {
                    val = Gender.Female.ToString();
                }
            }

            // If the property is "SubSpecializations", filter by SubSpecialization Name
            if (prop == "SubSpecializations")
            {
                var subSpecResult = DoctorRepo.FindAll(dr =>
                    dr.SubSpecializations.Any(ss => ss.Name == val) // Assuming 'Name' is the property in SubSpecialization
                );

                return Json(subSpecResult);
            }

            //if (prop == "availabilities")
            //{
            //    var availResult = DoctorRepo.FindAll(dr =>
            //        dr.availabilities.Any(av =>
            //            av.AvailableSlots.Any(slot => slot.State == val) // the added col
            //        )
            //    );

            //    return Json(availResult);
            //}

            if (prop == "Fees")
            {
                IEnumerable<Doctor> feesResult = new List<Doctor>();

                if (val == "أقل من ٥٠")
                {
                    feesResult = DoctorRepo.FindAll(dr => dr.Fees < 50);
                }
                else if (val == "من ٥٠ حتى ١٠٠")
                {
                    feesResult = DoctorRepo.FindAll(dr => dr.Fees >= 50 && dr.Fees <= 100);
                }
                else if (val == "من ١٠٠ حتى ٢٠٠")
                {
                    feesResult = DoctorRepo.FindAll(dr => dr.Fees > 100 && dr.Fees <= 200);
                }
                else if (val == "من ٢٠٠ حتى ٣٠٠")
                {
                    feesResult = DoctorRepo.FindAll(dr => dr.Fees > 200 && dr.Fees <= 300);
                }
                else
                {
                    feesResult = DoctorRepo.GetAllNoTracking();
                }
                return Json(new { items = feesResult });
            }


            // Otherwise, build an expression tree to filter based on other properties
            var parameter = Expression.Parameter(typeof(Doctor), "dr");
            Expression body = null;

            // Build the expression for filtering based on the property and value
            var property = Expression.Property(parameter, prop); // Get the property of Doctor (e.g., Gender, Title, etc.)

            // Handle case where the property is a string or enum
            if (property.Type == typeof(string))
            {
                var constant = Expression.Constant(val); // The constant value (from the query parameter)
                body = Expression.Equal(property, constant); // Check if the property equals the value
            }
            else if (property.Type.IsEnum)
            {
                var enumValue = Enum.Parse(property.Type, val); // Convert string value to the enum value
                var constant = Expression.Constant(enumValue); // Create the constant for the enum value
                body = Expression.Equal(property, constant); // Check if the property equals the enum value
            }
            else if (property.Type == typeof(decimal) || property.Type == typeof(int))
            {
                // Handle numeric properties (e.g., Fees, ExperienceYears)
                var numericValue = Convert.ChangeType(val, property.Type); // Convert string to the appropriate numeric type
                var constant = Expression.Constant(numericValue);
                body = Expression.Equal(property, constant); // Check if the property equals the value
            }
            else
            {
                // Add more cases if necessary
                return Json(new { items = new List<object>() }); // Return empty list if the type is unhandled
            }

            // Create the expression tree for the lambda
            var lambda = Expression.Lambda<Func<Doctor, bool>>(body, parameter);

            // Apply the expression tree to filter the doctors
            var filteredDoctors = DoctorRepo.FindAll(lambda);

            return Json(new { items = filteredDoctors });
        }*/

        public class DoctorFilter
        {
            public List<string>? Gender { get; set; }
            public List<string>? Title { get; set; }
            public string? Fees { get; set; }
            public List<string>? SubSpecializations { get; set; }
        }

        public IActionResult DoctorFilteration(DoctorFilter filter)
        {
            // Ensure the query is IQueryable, even if DoctorRepo returns IEnumerable
            var query = DoctorRepo.GetAllNoTracking().AsQueryable(); // Convert to IQueryable

            // Filter by Title (Multiple selections)
            if (filter.Title?.Any() == true)
            {
                query = query.Where(dr => filter.Title.Contains(dr.Title));  // Apply Title filter
            }

            // Filter by SubSpecializations (Any of the selected)
            if (filter.SubSpecializations?.Any() == true)
            {
                query = query.Where(dr => dr.SubSpecializations
                    .Any(ss => filter.SubSpecializations.Contains(ss.Name)));  // Filter by SubSpecialization
            }

            // Filter by Fees (example: less than 200)
            if (!string.IsNullOrEmpty(filter.Fees))
            {
                query = ApplyFeeFilter(query, filter.Fees);  // Apply Fees filter using a separate method
            }

            // Filter by Gender (Multiple selections)
            if (filter.Gender?.Any() == true)
            {
                var genderEnums = filter.Gender
                    .Select(gender => Enum.TryParse<Gender>(gender, true, out var genderEnum) ? genderEnum : (Gender?)null)
                    .Where(g => g.HasValue)
                    .Select(g => g.Value)
                    .ToList();

                if (genderEnums.Any())
                {
                    query = query.Where(dr => genderEnums.Contains(dr.gender));  // Apply Gender filter
                }
            }

            // Execute the query and get the filtered doctors
            var filteredDoctors = query.ToList();

            return Json(new { items = filteredDoctors });
        }

        // Helper method to apply the Fees filter
        private IQueryable<Doctor> ApplyFeeFilter(IQueryable<Doctor> query, string feeFilter)
        {
            switch (feeFilter)
            {
                case "أقل من ٥٠":
                    query = query.Where(dr => dr.Fees < 50);
                    break;
                case "من ٥٠ حتى ١٠٠":
                    query = query.Where(dr => dr.Fees >= 50 && dr.Fees <= 100);
                    break;
                case "من ١٠٠ حتى ٢٠٠":
                    query = query.Where(dr => dr.Fees > 100 && dr.Fees <= 200);
                    break;
                case "من ٢٠٠ حتى ٣٠٠":
                    query = query.Where(dr => dr.Fees > 200 && dr.Fees <= 300);
                    break;
                default:
                    // No filtering applied if fee is not recognized
                    break;
            }

            return query;
        }

    }
}
