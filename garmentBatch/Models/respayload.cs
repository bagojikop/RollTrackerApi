using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace garmentBatch.Models
{

   

    public class respayload
    {

      

        public byte status_cd { get; set; } = 1;
        public object data { get; set; }
        public errors error { get; set; } = new errors();
        public object PageDetails { get; set; }
    }

        public class errors
        {
            public string error_cd { get; set; }
            public string message { get; set; }
            public object exception { get; set; }
        
        }

    public class QueryStringParameters
    {
        const int maxPageSize = 25;
        public int PageNumber { get; set; } = 1;
        private int _pageSize = 10;
        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = (value > maxPageSize) ? maxPageSize : value;
            }
        }
        public List<values> keys { get; set; }
    }
    public class PageDetail<T>
    {
        public int CurrentPage { get; private set; }
        public int TotalPages { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;
        public PageDetail(int count, int pageNumber, int pageSize)
        {
            TotalCount = count;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

        }
        public static PageDetail<T> ToPagedList(IQueryable<T> source, int pageNumber, int pageSize)
        {
            var count = source.Count();

            return new PageDetail<T>(count, pageNumber, pageSize);
        }

    }





    public class filterExpression<T> : Expression
    {

        public static Expression<Func<T, bool>> GetFilterExpression(List<values> keys)
        {
            var parameter = Expression.Parameter(typeof(T), "c");
            var conditions = new List<Expression>();


            foreach (var kvp in keys)
            {
                var propertyName = kvp.Key;
                var constantValue = Expression.Constant(kvp.Value);
                var propertyExpression = Expression.Property(parameter, propertyName);
                var methodInfo = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                var callExpression = Expression.Call(propertyExpression, methodInfo, constantValue);
                var condition = Expression.Lambda<Func<T, bool>>(callExpression, parameter);

                conditions.Add(condition);
            }

            var combinedCondition = conditions.Aggregate(Expression.AndAlso); // Use Expression.OrElse to combine with OR operator
            var lambdaExpression = Expression.Lambda<Func<T, bool>>(combinedCondition, parameter);

            return lambdaExpression;

        }


    }


    public class EntityFrameworkFilter<T>
    {
        private ParameterExpression parameter;
        private Expression expression;

        public EntityFrameworkFilter()
        {
            parameter = Expression.Parameter(typeof(T), "entity");
            expression = Expression.Constant(true); // Start with a 'true' expression
        }

        public IQueryable<T> Filter(IQueryable<T> queryable, List<values> keys)
        {
            IQueryable<T> query = queryable;
            foreach (var kvp in keys.Where(w => !string.IsNullOrEmpty(w.Value)))
            {

                var parameter = Expression.Parameter(typeof(T), "x");
                Expression propertyExpression = parameter;

                if (kvp.Key.Contains("."))
                {
                    var relatedTypes = kvp.Key.Split('.');
                    var relatedPropertyName = relatedTypes[0];
                    var relatedProperty = typeof(T).GetProperty(relatedPropertyName);

                    if (relatedProperty == null)
                    {
                        throw new ArgumentException($"Property {relatedPropertyName} not found in type {typeof(T)}");
                    }

                    // Create an expression for the related entity
                    propertyExpression = Expression.Property(parameter, relatedPropertyName);

                    // Load the related entity into memory
                    query = query.Include(relatedPropertyName);

                    // Check if the subtable is related to another subtable
                    for (int i = 1; i < relatedTypes.Length - 1; i++)
                    {
                        var subRelatedPropertyName = relatedTypes[i];
                        var subRelatedProperty = relatedProperty.PropertyType.GetProperty(subRelatedPropertyName);

                        if (subRelatedProperty == null)
                        {
                            throw new ArgumentException($"Property {subRelatedPropertyName} not found in type {relatedProperty.PropertyType}");
                        }

                        // Create an expression for the related subentity
                        propertyExpression = Expression.Property(propertyExpression, subRelatedPropertyName);

                        // Load the related subentity into memory
                        query = query.Include($"{relatedPropertyName}.{subRelatedPropertyName}");

                        // Update the related property and type for the next iteration
                        relatedProperty = subRelatedProperty;
                        relatedPropertyName = subRelatedPropertyName;
                    }


                    //if (relatedTypes.Length > 2)
                    //{
                    //    var subRelatedPropertyName = relatedTypes[1];
                    //    var subRelatedProperty = relatedProperty.PropertyType.GetProperty(subRelatedPropertyName);

                    //    if (subRelatedProperty == null)
                    //    {
                    //        throw new ArgumentException($"Property {subRelatedPropertyName} not found in type {relatedProperty.PropertyType}");
                    //    }

                    //    // Create an expression for the related subentity
                    //    propertyExpression = Expression.Property(propertyExpression, subRelatedPropertyName);

                    //    // Load the related subentity into memory
                    //    query = query.Include($"{relatedPropertyName}.{subRelatedPropertyName}");
                    //}
                }

                // Create an expression for the filter
                var columnExpression = Expression.Property(propertyExpression, kvp.Key.Split('.').Last());
                var containsMethod = Expression.Call(columnExpression, "Contains", Type.EmptyTypes, Expression.Constant(kvp.Value));
                var lambda = Expression.Lambda<Func<T, bool>>(containsMethod, parameter);




                //var property = Expression.Property(parameter, kvp.Key);
                //var value = Expression.Constant(kvp.Value);
                //var containsMethod = Expression.Call(property, "Contains", Type.EmptyTypes, value);
                //var lambda = Expression.Lambda<Func<T, bool>>(containsMethod, parameter);

                query = query.Where(lambda);


                //var property = Expression.Property(parameter, kvp.Key);
                //var value = Expression.Constant(kvp.Value);
                //var containsMethod = Expression.Call(property, "Contains", Type.EmptyTypes, value);
                //var lambda = Expression.Lambda<Func<T, bool>>(containsMethod, parameter);

                //query = query.Where(lambda);
            }
            return query;
        }

 
    }
    public class values
    {
        public string Key { get; set; }
        public string Value { get; set; }


    }
}