using System.Collections.Generic;

namespace Pressius.Test.PermutationExtension.Model
{
    public class ValidLocation : DefaultParameterDefinition
    {
        public override List<object> InputCatalogues =>
            new List<object> {
               "Mens Building, 10 Latrobe Street, VIC 3000, Melbourne, Australia",
               "111 St Kilda, VIC 3004, Melbourne, Australia" };

        public override ParameterTypeDefinition TypeName =>
            new ParameterTypeDefinition("ValidLocation");
    }
}