using HealthCareApp.RepositoryServices;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

public class UniqueAttribute<T> : ValidationAttribute where T : class
{
    private readonly string[] _propertyNames;
    private readonly string _errorMessage;

    public UniqueAttribute(string errorMessage, params string[] propertyNames)
    {
        if (propertyNames == null || propertyNames.Length == 0)
        {
            throw new ArgumentException("At least one property name must be provided.", nameof(propertyNames));
        }
        _propertyNames = propertyNames;
        _errorMessage = errorMessage;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value == null) return ValidationResult.Success;

        var repo = validationContext.GetService<IGenericRepoServices<T>>();
        if (repo == null)
            throw new InvalidOperationException($"Repository for {typeof(T).Name} is not available.");

        var modelType = validationContext.ObjectInstance.GetType();
        var entityType = typeof(T);

        // Store the properties and their values
        var properties = _propertyNames
            .Select(propName => new
            {
                PropName = propName,
                ModelValue = modelType.GetProperty(propName)?.GetValue(validationContext.ObjectInstance)
            })
            .ToList();

        // Construct expressions for each property
        var parameter = Expression.Parameter(entityType, "e");
        var expressions = properties.Select(p =>
        {
            var dbProp = Expression.Property(parameter, p.PropName);
            var modelValue = Expression.Constant(p.ModelValue);
            return Expression.Equal(dbProp, modelValue);
        }).ToList();

        // Add expression id != current id (to check if any object with another id has the unique values)
        var idProp = modelType.GetProperty("patientId"); // ensure matching property id if there is a view model
        if (idProp != null)
        {
            var idValue = idProp.GetValue(validationContext.ObjectInstance);
            var dbId = Expression.Property(parameter, "patientId");
            var idNotEqual = Expression.NotEqual(dbId, Expression.Constant(idValue));
            expressions.Add(idNotEqual);

            // Check if the db entity & model all properties values not changed to skip validation
            var current = repo.Find(e => EF.Property<object>(e, "patientId") == idValue);
            if (current != null)
            {
                var unchanged = properties.All(p =>
                {
                    var currentVal = entityType.GetProperty(p.PropName)?.GetValue(current);
                    return Equals(currentVal, p.ModelValue);
                });

                if (unchanged) return ValidationResult.Success;
            }
        }

        // Combine all expressions into one (add &&)
        var finalExpr = expressions.Aggregate((a, b) => Expression.AndAlso(a, b));
        var lambda = Expression.Lambda<Func<T, bool>>(finalExpr, parameter);

        // Check if the combination of properties exists
        var exists = repo.FindAllForSearch(lambda).Any();
        return exists
            ? new ValidationResult($"{_errorMessage}")
            : ValidationResult.Success;
    }
}
