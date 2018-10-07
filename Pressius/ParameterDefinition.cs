using System.Collections.Generic;
using System.Reflection;

namespace Pressius
{
    public class ParameterTypeDefinition
    {
        public List<string> NameList { get; private set; }

        public ParameterTypeDefinition(string name)
        {
            NameList = new List<string> { name };
        }

        public ParameterTypeDefinition(List<string> nameList)
        {
            NameList = nameList;
        }

        public override bool Equals(object obj)
        {
            var stringObject = obj as string;
            return stringObject != null ? NameList.Contains(stringObject) : base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return 116380930 + EqualityComparer<List<string>>.Default.GetHashCode(NameList);
        }
    }

    public interface IParameterDefinition
    {
        List<object> InputCatalogues { get; }

        ParameterTypeDefinition TypeName { get; }

        bool IsMatch(PropertyInfo propertyInfo);

        bool IsMatch(ParameterInfo parameterInfo);
    }
}