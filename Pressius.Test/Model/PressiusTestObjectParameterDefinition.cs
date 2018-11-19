using System.Collections.Generic;
using Pressius.ParameterDefinition;

namespace Pressius.Test.Model
{
    public class PressiusTestObjectParameterDefinition : DefaultParameterDefinition
    {
        private List<object> _inputCatalogues;

        public PressiusTestObjectParameterDefinition()
        {
            _inputCatalogues = new List<object> {
                new PressiusTestObject { Address = "300 Latrobe Street", Id = 1, Name = "My First Object" },
                new PressiusTestObject { Address = "500 Latrobe Street", Id = 2, Name = "My Second Object" }
            };
        }

        public PressiusTestObjectParameterDefinition(List<object> inputCatalogues)
        {
            _inputCatalogues = inputCatalogues;
        }

        public override List<object> InputCatalogues => _inputCatalogues;

        public override ParameterTypeDefinition TypeName =>
            new ParameterTypeDefinition("PressiusTestObject");
    }
}