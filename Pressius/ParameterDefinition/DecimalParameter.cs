using System.Collections.Generic;

namespace Pressius.ParameterDefinition
{
    public class DecimalParameter : DefaultParameterDefinition
    {
        public override List<object> InputCatalogues =>
            new List<object> { (decimal)0.0, decimal.MinValue, decimal.MaxValue, decimal.MinusOne, decimal.Zero, decimal.One };

        public override ParameterTypeDefinition TypeName
            => new ParameterTypeDefinition("Decimal");
    }
}