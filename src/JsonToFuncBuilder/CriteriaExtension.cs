using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace JsonToFuncBuilder
{
    public static class CriteriaExtensions
    {
        public static Expression<Func<TSource, bool>> SimpleComparison<TSource>(string property, object value, ComparisonEnum comparison)
        {
            var type = typeof(TSource);
            var pe = Expression.Parameter(type, "p");
            var propertyReference = Expression.Property(pe, property);
            var constantReference = Expression.Constant(value);


            int intValue;
            if (Int32.TryParse(value.ToString(), out intValue))
            {
                constantReference = Expression.Constant(intValue);
            }

            DateTime dateTimeValue;
            if (DateTime.TryParse(value.ToString(), out dateTimeValue))
            {
                constantReference = Expression.Constant(dateTimeValue);
            }

            Expression compare = null;

            switch (comparison)
            {
                case ComparisonEnum.Equal:
                    compare = Expression.Equal(propertyReference, constantReference);
                    break;


                case ComparisonEnum.NotEqual:
                    compare = Expression.Not(Expression.Equal(propertyReference, constantReference));
                    break;

                case ComparisonEnum.GreaterThanEqualTo:
                    compare = Expression.GreaterThanOrEqual(propertyReference, constantReference);
                    break;
            }

            return Expression.Lambda<Func<TSource, bool>>(compare, new[] { pe });
        }

        public static Func<T, bool> BuildCriteria<T>(IList<CriteriaGroup> criteria)
        {
            Expression<Func<T, bool>> groups = null;
            

            foreach (var criteriaGroup in criteria)
            {

                Expression<Func<T, bool>> result = null;

                foreach (var criterion in criteriaGroup.Criteria)
                {
                                      
                    var op = (ComparisonEnum)Enum.Parse(typeof(ComparisonEnum), criterion.Operator, true);


                    if (result != null)
                    {
                        result = result.And(SimpleComparison<T>(criterion.Field, criterion.Value, op));
                    }
                    else
                    {
                        result = SimpleComparison<T>(criterion.Field, criterion.Value, op);
                    }

                }

                groups = groups == null ? result : groups.And(result);
            }



            return groups.Compile();
        }

        public static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
        {
            // build parameter map (from parameters of second to parameters of first)
            var map = first.Parameters.Select((f, i) => new { f, s = second.Parameters[i] }).ToDictionary(p => p.s, p => p.f);

            // replace parameters in the second lambda expression with parameters from the first
            var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

            // apply composition of lambda expression bodies to parameters from the first expression 
            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.And);
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.Or);
        }
    }
}