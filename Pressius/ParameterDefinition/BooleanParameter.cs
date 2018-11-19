using System.Collections.Generic;

namespace Pressius.ParameterDefinition
{
    public class BooleanParameter : DefaultParameterDefinition
    {
        public override List<object> InputCatalogues => new List<object>
            { true, false };

        public override ParameterTypeDefinition TypeName => new ParameterTypeDefinition("Boolean");
    }
}