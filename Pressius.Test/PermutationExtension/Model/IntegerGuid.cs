using System.Collections.Generic;

namespace Pressius.Test.PermutationExtension.Model
{
    public class IntegerGuid : DefaultParameterDefinition
    {
        public override List<object> InputCatalogues =>
            new List<object> {
                1531,
                9975
            };

        public override ParameterTypeDefinition TypeName =>
            new ParameterTypeDefinition("IntegerGuid");
    }
}