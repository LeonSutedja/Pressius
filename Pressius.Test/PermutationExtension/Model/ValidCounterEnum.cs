using System.Collections.Generic;
using Pressius.ParameterDefinition;

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

    public class ValidCounterTwoEnum : DefaultParameterDefinition
    {
        public override List<object> InputCatalogues =>
            new List<object> { null, CounterTwo.One, CounterTwo.Two, CounterTwo.Three };

        public override ParameterTypeDefinition TypeName =>
            new ParameterTypeDefinition("CounterAgain");

        public override bool CompareParamName => true;
    }
}