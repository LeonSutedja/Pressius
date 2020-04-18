using System.Collections.Generic;

namespace Pressius.ParameterDefinition
{
    public class CustomParameterDefinition : DefaultParameterDefinition
    {
        private List<object> _inputCatalogues;
        public override List<object> InputCatalogues { get { return _inputCatalogues; } }

        private ParameterTypeDefinition _typeName;
        public override ParameterTypeDefinition TypeName { get { return _typeName; } }

        private bool _compareParameterName;
        public override bool CompareParamName { get { return _compareParameterName; } }

        public CustomParameterDefinition(string typeName)
        {
            _inputCatalogues = new List<object>();
            _typeName = new ParameterTypeDefinition(typeName);
            _compareParameterName = base.CompareParamName;
        }

        public CustomParameterDefinition(List<string> typeNames)
        {
            _inputCatalogues = new List<object>();
            _typeName = new ParameterTypeDefinition(typeNames);
            _compareParameterName = base.CompareParamName;
        }

        public void AddInputCatalogues(object input)
        {
            _inputCatalogues.Add(input);
        }

        public void AddInputCatalogues(List<object> inputs)
        {
            _inputCatalogues.AddRange(inputs);
        }

        public void CompareWithParameterName()
        {
            _compareParameterName = true;
        }
    }
}