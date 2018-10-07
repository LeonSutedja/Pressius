using Pressius.Test.Model;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Pressius.Test
{
    public class PressiusTestObjectObjectDefinition
        : PropertiesObjectDefinition<PressiusTestObject>
    {
        public override Dictionary<string, string> MatcherDictionary =>
            new Dictionary<string, string>
            {
                { "Address", "ValidLocation" }
            };
    }

    public class ValidLocation : DefaultParameterDefinition
    {
       public override List<object> InputCatalogues =>
           new List<object> {
               "Mens Building, 10 Latrobe Street, VIC 3000, Melbourne, Australia",
               "111 St Kilda, VIC 3004, Melbourne, Australia" };
       public override ParameterTypeDefinition TypeName => 
           new ParameterTypeDefinition("ValidLocation");
    }

    public class PermutationExtensionTest
    {
        private readonly ITestOutputHelper output;

        public PermutationExtensionTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void GeneratePressiusTestObject_ShouldPermutate()
        {
            var addedParameterDefinitions = 
                new List<IParameterDefinition> { new ValidLocation() };
            var pressiusTestObjectList = Permutate.Generate<PressiusTestObject>(
                new PressiusTestObjectObjectDefinition(),
                addedParameterDefinitions).ToList();
            pressiusTestObjectList.ShouldNotBeNull();
            pressiusTestObjectList.ToList().Count.ShouldBeGreaterThan(0);
            var objectList = pressiusTestObjectList.ToList();
            objectList.ForEach(obj =>
            {
                output.WriteLine("Obj: {0} {1} {2}", obj.Id, obj.Name, obj.Address);
            });
        }
    }
}