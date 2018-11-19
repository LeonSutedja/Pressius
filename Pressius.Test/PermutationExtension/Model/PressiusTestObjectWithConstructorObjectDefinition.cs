using System.Collections.Generic;
using Pressius.Test.Model;

namespace Pressius.Test.PremutationExtension.Model
{
    public class PressiusTestObjectWithConstructorObjectDefinition
        : PropertiesObjectDefinition<PressiusTestObjectWithConstructor>
    {
        public override Dictionary<string, string> MatcherDictionary =>
            new Dictionary<string, string>
            {
                { "name", "ValidName" }
            };
    }
}