using System.Collections.Generic;

namespace Pressius
{
    public interface IObjectDefinition
    {
        string AssemblyQualifiedClassName { get; }
        ObjectInitiation InitiationMethod { get; }
        Dictionary<string, string> MatcherDictionary { get; }
    }

    public class DefinitionMatcher
    {
        public string PropertyName { get; private set; }
        public IParameterDefinition ParameterDefinition { get; private set; }
    }

    public enum ObjectInitiation
    {
        Properties,
        Constructor
    }

    public abstract class AbstractObjectDefinition<T> : IObjectDefinition
    {
        public string AssemblyQualifiedClassName => typeof(T).AssemblyQualifiedName;

        public virtual Dictionary<string, string> MatcherDictionary
        {
            get
            {
                if (InitiationMethod == ObjectInitiation.Constructor) return _constructorMatcherDictionary();
                if (InitiationMethod == ObjectInitiation.Properties) return _propertiesMatcherDictionary();
                return null;
            }
        }

        private Dictionary<string, string> _constructorMatcherDictionary()
        {
            var result = new Dictionary<string, string>();
            var constructors = typeof(T).GetConstructors();
            var firstConstructorParameters = constructors[0].GetParameters();
            foreach (var param in firstConstructorParameters)
            {
                var paramType = param.ParameterType.Name;
                var paramName = param.Name;
                result.Add(paramName, paramType);
            }
            return result;
        }

        private Dictionary<string, string> _propertiesMatcherDictionary()
        {
            var result = new Dictionary<string, string>();
            var listOfProperties = typeof(T).GetProperties();

            foreach (var prop in listOfProperties)
            {
                var propertyType = prop.PropertyType.Name;
                var propertyName = prop.Name;
                result.Add(propertyName, propertyType);
            }
            return result;
        }

        public abstract ObjectInitiation InitiationMethod { get; }
    }

    public abstract class ConstructorObjectDefinition<T> : AbstractObjectDefinition<T>
    {
        public override ObjectInitiation InitiationMethod => ObjectInitiation.Constructor;
    }

    public abstract class PropertiesObjectDefinition<T> : AbstractObjectDefinition<T>
    {
        public override ObjectInitiation InitiationMethod => ObjectInitiation.Properties;
    }
}