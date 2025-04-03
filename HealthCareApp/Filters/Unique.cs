using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using HealthCareApp.RepositoryServices;

namespace HealthCareApp.Filters
{
    public class Unique<T> : ValidationAttribute where T : class
    {
        private readonly string _propertyName;

        public Unique(string propertyName)
        {
            _propertyName = propertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success;

            var repository = validationContext.GetService<IGenericRepoServices<T>>();
            if (repository == null)
            {
                throw new InvalidOperationException($"Repository for {typeof(T).Name} is not available.");
            }

            var exists = repository.FindAll(e => EF.Property<object>(e, _propertyName).Equals(value));

            return exists.Count() == 0 ? ValidationResult.Success : new ValidationResult($"The {_propertyName} is already in use.");
        }
    }
}
