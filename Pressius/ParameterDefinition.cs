using System.Collections.Generic;
using System.Reflection;

namespace Pressius
{
    public class ParameterTypeDefinition
    {
        public List<string> NameList { get; }

        public ParameterTypeDefinition(string name)
        {
            NameList = new List<string> { name };
        }

        public ParameterTypeDefinition(List<string> nameList)
        {
            NameList = nameList;
        }

        public bool Equals(string obj)
            => obj != null && NameList.Contains(obj);
    }

    public interface IParameterDefinition
    {
        List<object> InputCatalogues { get; }

        ParameterTypeDefinition TypeName { get; }

        bool CompareParamName { get; }

        bool IsMatch(PropertyInfo propertyInfo);

        bool IsMatch(ParameterInfo parameterInfo);
    }
}