using System;
using System.Collections.Generic;

namespace Pressius.ParameterDefinition
{
    public class DateTimeParameter : DefaultParameterDefinition
    {
        public override List<object> InputCatalogues => new List<object>
            { DateTime.Now, DateTime.MinValue, DateTime.MaxValue };

        public override ParameterTypeDefinition TypeName => new ParameterTypeDefinition("DateTime");
    }
}