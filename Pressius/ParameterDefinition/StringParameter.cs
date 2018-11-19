using System.Collections.Generic;

namespace Pressius.ParameterDefinition
{
    public class StringParameter : DefaultParameterDefinition
    {
        public override List<object> InputCatalogues =>
            new List<object> {
                "The quick brown fox jumps over the lazy dog", // normal string
                "1234567890 Cozy lummox gives smart squid who asks for job pen", // Alpha Numeric
                string.Empty, // Empty string
                "~!@#$%&*()_+=-`\\][{}|;:,./?><'\"", // Characters and symbols
                null, // null value
                new string('x', 1024 * 1024 / 2) // really long string
            };

        public override ParameterTypeDefinition TypeName => new ParameterTypeDefinition("String");
    }
}