using System.Collections.Generic;
using Pressius.Test.Model;

namespace Pressius.Test.PermutationExtension.Model
{
    public class ValidCounterEnum : DefaultParameterDefinition
    {
        public override List<object> InputCatalogues =>
            new List<object> { Counter.One, Counter.Two, Counter.Three };

        public override ParameterTypeDefinition TypeName =>
            new ParameterTypeDefinition("Counter");

        public override bool CompareParamName => true;
    }
}