using System;
using System.Collections.Generic;
using System.Reflection;

namespace Pressius
{
    public abstract class DefaultParameterDefinition : IParameterDefinition
    {
        public abstract List<object> InputCatalogues { get; }

        public abstract ParameterTypeDefinition TypeName { get; }

        public virtual bool CompareParamName => false;

        public virtual bool IsMatch(PropertyInfo propertyInfo)
            => TypeName.Equals(propertyInfo.PropertyType.Name);

        public virtual bool IsMatch(ParameterInfo parameterInfo)
            => TypeName.Equals(parameterInfo.ParameterType.Name);
    }

    public class StringParameter : DefaultParameterDefinition
    {
        public override List<object> InputCatalogues =>
            new List<object> {
                "The quick brown fox jumps over the lazy dog", // normal string
                "1234567890 Cozy lummox gives smart squid who asks for job pen", // Alpha Numeric
                string.Empty, // Empty string
                "~!@#$%&*()_+=-`\\][{}|;:,./?><'\"", // Characters and symbols
                null, // null value
                new string('x', 1024 * 1024 / 2) // really long string
            };

        public override ParameterTypeDefinition TypeName => new ParameterTypeDefinition("String");
    }
    
    public class IntegerParameter : DefaultParameterDefinition
    {
        public override List<object> InputCatalogues =>
            new List<object> { 10, Int32.MinValue, Int32.MaxValue };

        public override ParameterTypeDefinition TypeName 
            => new ParameterTypeDefinition(new List<string> { "Int32", "Int" });
    }

    public class DecimalParameter : DefaultParameterDefinition
    {
        public override List<object> InputCatalogues =>
            new List<object> { (decimal)0.0, decimal.MinValue, decimal.MaxValue, decimal.MinusOne, decimal.Zero, decimal.One };

        public override ParameterTypeDefinition TypeName
            => new ParameterTypeDefinition("Decimal");
    }

    public class DoubleParameter : DefaultParameterDefinition
    {
        public override List<object> InputCatalogues => new List<object>
            { 0.1, Double.MaxValue, Double.MinValue };

        public override ParameterTypeDefinition TypeName => new ParameterTypeDefinition("Double");
    }

    public class DateTimeParameter : DefaultParameterDefinition
    {
        public override List<object> InputCatalogues => new List<object>
            { DateTime.Now, DateTime.MinValue, DateTime.MaxValue };

        public override ParameterTypeDefinition TypeName => new ParameterTypeDefinition("DateTime");
    }
    
    public class BooleanParameter : DefaultParameterDefinition
    {
        public override List<object> InputCatalogues => new List<object>
            { true, false };

        public override ParameterTypeDefinition TypeName => new ParameterTypeDefinition("Boolean");
    }
}