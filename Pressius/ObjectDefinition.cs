using System.Collections.Generic;

namespace Pressius
{
    public interface IObjectDefinition
    {
        string AssemblyQualifiedClassName { get; }
        Dictionary<string, string> MatcherDictionary { get; }
    }

    public class DefinitionMatcher
    {
        public string PropertyName { get; private set; }
        public IParameterDefinition ParameterDefinition { get; private set; }
    }

    public abstract class AbstractObjectDefinition<T> : IObjectDefinition
    {
        public string AssemblyQualifiedClassName => typeof(T).AssemblyQualifiedName;

        public virtual Dictionary<string, string> MatcherDictionary => _propertiesMatcherDictionary();

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
    }

    public abstract class PropertiesObjectDefinition<T> : AbstractObjectDefinition<T>
    {
    }

    public class CustomObjectDefinition<T> : AbstractObjectDefinition<T>
    {
        private Dictionary<string, string> _matcherDictionary;
        public override Dictionary<string, string> MatcherDictionary => _matcherDictionary;

        public CustomObjectDefinition()
        {
            _matcherDictionary = new Dictionary<string, string>();
        }

        public void SetMatcher(Dictionary<string, string> matcher)
        {
            _matcherDictionary = matcher;
        }
    }
}