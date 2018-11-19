using System.Collections.Generic;
using System.Reflection;

namespace Pressius.ParameterDefinition
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
}