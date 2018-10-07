using System;
using System.Collections.Generic;
using System.Reflection;

namespace Pressius
{
    public abstract class DefaultParameterDefinition : IParameterDefinition
    {
        public abstract List<object> InputCatalogues { get; }

        public abstract ParameterTypeDefinition TypeName { get; }

        public virtual bool IsMatch(PropertyInfo propertyInfo)
            => TypeName.Equals(propertyInfo.PropertyType.Name);

        public virtual bool IsMatch(ParameterInfo parameterInfo)
            => TypeName.Equals(parameterInfo.ParameterType.Name);
    }

    public class StringParameter : DefaultParameterDefinition
    {
        public override List<object> InputCatalogues =>
            new List<object> {
                string.Empty,
                "Normal String",
                "~!@#$%&*()_+=-`\\][{}|;:,./?><'\"",
                null
            };

        public override ParameterTypeDefinition TypeName => new ParameterTypeDefinition("String");
    }

    public class EmailStringParameter : DefaultParameterDefinition
    {
        public override List<object> InputCatalogues =>
            new List<object>
            {
                "A@a.com",
                "Fake.Email@gmail.com"
            };

        public override ParameterTypeDefinition TypeName => new ParameterTypeDefinition("Email");

        public override bool IsMatch(PropertyInfo propertyInfo)
        {
            var isString = propertyInfo.PropertyType.Name.Contains("String");
            var isEmail = propertyInfo.Name.Contains("Email");
            return isString && isEmail;
        }
    }

    public class IntegerParameter : DefaultParameterDefinition
    {
        public override List<object> InputCatalogues =>
            new List<object> { int.MinValue, 10, int.MaxValue };

        public override ParameterTypeDefinition TypeName => new ParameterTypeDefinition(new List<string> { "Int32", "Int", "Int64" });
    }

    public class DateTimeParameter : DefaultParameterDefinition
    {
        public override List<object> InputCatalogues => new List<object>
            { DateTime.Now, DateTime.MinValue, DateTime.MaxValue };

        public override ParameterTypeDefinition TypeName => new ParameterTypeDefinition("DateTime");
    }

    public class DoubleParameter : DefaultParameterDefinition
    {
        public override List<object> InputCatalogues => new List<object>
            { 0.1, Double.MaxValue, Double.MinValue };

        public override ParameterTypeDefinition TypeName => new ParameterTypeDefinition("Double");
    }
}