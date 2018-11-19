using System;
using System.Collections.Generic;

namespace Pressius.ParameterDefinition
{
    public class IntegerParameter : DefaultParameterDefinition
    {
        public override List<object> InputCatalogues =>
            new List<object> { 10, Int32.MinValue, Int32.MaxValue };

        public override ParameterTypeDefinition TypeName 
            => new ParameterTypeDefinition(new List<string> { "Int32", "Int" });
    }
}