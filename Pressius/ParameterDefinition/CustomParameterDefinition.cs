using System.Collections.Generic;

namespace Pressius.ParameterDefinition
{
    public class CustomParameterDefinition : DefaultParameterDefinition
    {
        private List<object> _inputCatalogues;
        public override List<object> InputCatalogues { get { return _inputCatalogues; } }

        private ParameterTypeDefinition _typeName;
        public override ParameterTypeDefinition TypeName { get { return _typeName; } }

        public override bool CompareParamName => true;

        public CustomParameterDefinition(string typeName)
        {
            _inputCatalogues = new List<object>();
            _typeName = new ParameterTypeDefinition(typeName);
        }

        public CustomParameterDefinition(List<string> typeNames)
        {
            _inputCatalogues = new List<object>();
            _typeName = new ParameterTypeDefinition(typeNames);
        }

        public void AddInputCatalogues(object input)
        {
            _inputCatalogues.Add(input);
        }

        public void AddInputCatalogues(List<object> inputs)
        {
            _inputCatalogues.AddRange(inputs);
        }
    }
}