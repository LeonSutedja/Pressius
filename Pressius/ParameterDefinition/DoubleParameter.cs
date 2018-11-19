using System;
using System.Collections.Generic;

namespace Pressius.ParameterDefinition
{
    public class DoubleParameter : DefaultParameterDefinition
    {
        public override List<object> InputCatalogues => new List<object>
            { 0.1, Double.MaxValue, Double.MinValue };

        public override ParameterTypeDefinition TypeName => new ParameterTypeDefinition("Double");
    }
}