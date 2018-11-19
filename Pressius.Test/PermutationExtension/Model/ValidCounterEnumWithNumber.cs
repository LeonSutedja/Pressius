using System.Collections.Generic;
using Pressius.ParameterDefinition;

namespace Pressius.Test.PermutationExtension.Model
{
    public class ValidCounterEnumWithNumber : DefaultParameterDefinition
    {
        public override List<object> InputCatalogues =>
            new List<object> { 0, 1, 2 };

        public override ParameterTypeDefinition TypeName =>
            new ParameterTypeDefinition("Counter");

        public override bool CompareParamName => true;
    }
}