using System.Collections.Generic;
using Pressius.ParameterDefinition;

namespace Pressius.Test.PermutationExtension.Model
{
    public class ValidName : DefaultParameterDefinition
    {
        public override List<object> InputCatalogues =>
            new List<object> {
                "Clark Kent",
                "Bruce Wayne",
                "Barry Allen"
            };

        public override ParameterTypeDefinition TypeName =>
            new ParameterTypeDefinition("ValidName");
    }

    public class ValidNameWithCompareParamName : DefaultParameterDefinition
    {
        public override List<object> InputCatalogues =>
            new List<object> {
                "Clark Kent",
                "Bruce Wayne",
                "Barry Allen"
            };

        public override ParameterTypeDefinition TypeName =>
            new ParameterTypeDefinition("name");

        public override bool CompareParamName => true;
    }
}