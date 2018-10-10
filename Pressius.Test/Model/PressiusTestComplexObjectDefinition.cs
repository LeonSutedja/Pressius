using System.Collections.Generic;
using Pressius.Test.Model;

namespace Pressius.Test
{
    public class PressiusTestComplexObjectDefinition
        : PropertiesObjectDefinition<PressiusTestObject>
    {
        public override Dictionary<string, string> MatcherDictionary =>
            new Dictionary<string, string>
            {
                { "PressiusTestObject", "PressiusTestObject" }
            };
    }
}