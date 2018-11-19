using System.Collections.Generic;

namespace Pressius.Test.Model
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