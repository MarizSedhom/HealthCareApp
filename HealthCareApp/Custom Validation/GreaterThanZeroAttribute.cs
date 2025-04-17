namespace HealthCareApp.Custom_Validation
{
    using HealthCareApp.RepositoryServices;
    using System;
    using System.ComponentModel.DataAnnotations;

    public class GreaterThanZeroAttribute<T> : ValidationAttribute where T : class
    {
        private readonly string _propertyName;
        private readonly string _errorMessage;

        public GreaterThanZeroAttribute(string propertyName, string errorMessage)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentException("Property name cannot be null or empty.", nameof(propertyName));
            }

            _propertyName = propertyName;
            _errorMessage = errorMessage;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var repo = validationContext.GetService<IGenericRepoServices<T>>();

            if (repo == null)
            {
                throw new InvalidOperationException($"Repository for {typeof(T).Name} is not available.");
            }

            var modelType = validationContext.ObjectInstance.GetType();
            var entityType = typeof(T);

            var propertyValue = modelType.GetProperty(_propertyName)?.GetValue(validationContext.ObjectInstance);

            if (propertyValue == null)
                return ValidationResult.Success;

            if (propertyValue is int intValue && intValue <= 0)
            {
                return new ValidationResult($"{_errorMessage}");
            }
            else if (propertyValue is decimal decimalValue && decimalValue <= 0)
            {
                return new ValidationResult($"{_errorMessage}");
            }
            else if (propertyValue is double doubleValue && doubleValue <= 0)
            {
                return new ValidationResult($"{_errorMessage}");
            }

            return ValidationResult.Success;
        }
    }

}
