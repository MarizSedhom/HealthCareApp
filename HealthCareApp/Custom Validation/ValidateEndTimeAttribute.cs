using System.ComponentModel.DataAnnotations;

namespace HealthCareApp.CustomValidation
{
    public class ValidateEndTimeAttribute:ValidationAttribute
    {
        string startTimePropertyName;
        string IsAvailableName;
        public ValidateEndTimeAttribute(string _starttime, string isAvailableName)
        {
            startTimePropertyName = _starttime;
            IsAvailableName = isAvailableName;
        }
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null) return new ValidationResult("endTime is required");
            if (value is TimeOnly endTime){
                var startTimePropertyInfo = validationContext.ObjectType.GetProperty(startTimePropertyName);
                var IsAvailable =(bool) validationContext.ObjectType.GetProperty(IsAvailableName).GetValue(validationContext.ObjectInstance);
                if (!IsAvailable)
                {
                    return ValidationResult.Success;

                }
                if (startTimePropertyInfo == null)
                    return new ValidationResult($"Unknown property: {startTimePropertyName}");
                TimeOnly startTime = (TimeOnly)startTimePropertyInfo.GetValue(validationContext.ObjectInstance);
                if (endTime > startTime)
                    return ValidationResult.Success;
                else
                    return new ValidationResult("endTime must be after start time");
            }
            return new ValidationResult("Not matched Type");
        }

    }
}
