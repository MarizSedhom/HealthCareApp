using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HealthCareApp.Data
{
    public partial class ApplicationDbContext
    {
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (entityType.BaseType == null)
                {
                    var isDeletedProperty = entityType.GetProperties()
                                                       .FirstOrDefault(p => p.Name == "IsDeleted" && p.ClrType == typeof(bool));

                    if (isDeletedProperty != null)
                    {
                        modelBuilder.Entity(entityType.ClrType)
                                    .HasQueryFilter(BuildSoftDeleteFilter(entityType.ClrType));
                    }
                }
            }
        }

        private static LambdaExpression BuildSoftDeleteFilter(System.Type entityType)
        {
            var parameter = Expression.Parameter(entityType, "e");
            var property = Expression.Property(parameter, "IsDeleted");
            var constant = Expression.Constant(false);
            var body = Expression.Equal(property, constant);

            return Expression.Lambda(body, parameter);
        }
    }
}
