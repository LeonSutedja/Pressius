﻿using System.Collections.Generic;

namespace Pressius.Test.PremutationExtension.Model
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
}