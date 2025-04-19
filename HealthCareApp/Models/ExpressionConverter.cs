//using System.Linq.Expressions;

//namespace HealthCareApp.Mapster_Config
//{
//    public static class ExpressionConverter
//    {
//        public static Expression<Func<A, bool>> Convert<A, V>(Expression<Func<V, bool>> expr)
//            where A : class
//            where V : class
//        {
//            var parameter = Expression.Parameter(typeof(A), expr.Parameters[0].Name);
//            var visitor = new ExpressionVisitorForConversion<A, V>(parameter);
//            var body = visitor.Visit(expr.Body);
//            return Expression.Lambda<Func<A, bool>>(body, parameter);
//        }
//    }

//    public class ExpressionVisitorForConversion<A, V> : ExpressionVisitor
//        where A : class
//        where V : class
//    {
//        private readonly ParameterExpression _parameter;

//        public ExpressionVisitorForConversion(ParameterExpression parameter)
//        {
//            _parameter = parameter;
//        }

//        protected override Expression VisitMember(MemberExpression node)
//        {
//            var sourceType = typeof(V);
//            var destinationType = typeof(A);

//            var sourceProperty = sourceType.GetProperty(node.Member.Name);
//            var destinationProperty = destinationType.GetProperty(node.Member.Name);

//            if (sourceProperty != null && destinationProperty != null)
//            {
//                var newMember = Expression.Property(_parameter, destinationProperty);
//                return base.VisitMember(newMember);
//            }
//            return base.VisitMember(node);
//        }
//    }
//}
