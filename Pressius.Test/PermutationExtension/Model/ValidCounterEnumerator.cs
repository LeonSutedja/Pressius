using Pressius.Test.Model;
using System.Collections.Generic;

namespace Pressius.Test
{
    public class ValidCounterEnumerator : DefaultParameterDefinition
    {
        public override List<object> InputCatalogues =>
            new List<object> { Counter.One, Counter.Two, Counter.Three };

        public override ParameterTypeDefinition TypeName =>
            new ParameterTypeDefinition("Counter");

        public override bool CompareParamName => true;
    }
}