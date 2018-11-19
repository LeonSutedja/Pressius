using System.Collections.Generic;
using Pressius.Test.Model;

namespace Pressius.Test.PermutationExtension.Model
{
    public class PressiusTestObjectObjectDefinitionWithIntegerGuid
        : PropertiesObjectDefinition<PressiusTestObject>
    {
        public override Dictionary<string, string> MatcherDictionary =>
            new Dictionary<string, string>
            {
                { "Address", "ValidLocation" },
                { "Id", "IntegerGuid" }
            };
    }
}