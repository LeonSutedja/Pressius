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

    public class IntegerGuid : DefaultParameterDefinition
    {
        public override List<object> InputCatalogues =>
            new List<object> {
                1531,
                9975
            };

        public override ParameterTypeDefinition TypeName =>
            new ParameterTypeDefinition("IntegerGuid");
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

    public class PermutationExtensionTest : BaseTest
    {
        public PermutationExtensionTest(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void PressiusTestObject_ShouldPermutateWithStaticGenerator()
        {
            var addedParameterDefinitions =
                new List<IParameterDefinition> { new ValidLocation() };
            var pressiusTestObjectList = Permutor.Generate<PressiusTestObject>(
                new PressiusTestObjectObjectDefinition(),
                addedParameterDefinitions).ToList();
            pressiusTestObjectList.ShouldNotBeNull();
            pressiusTestObjectList.ToList().Count.ShouldBeGreaterThan(0);
            var objectList = pressiusTestObjectList.ToList();
            objectList.ForEach(obj =>
            {
                _output.WriteLine("Obj: {0} {1} {2}", obj.Id, obj.Name, obj.Address);
            });
        }

        [Fact]
        public void PressiusTestObject_ShouldPermutate()
        {
            var pressius = new Permutor();
            var pressiusTestObjectList = pressius
                .AddParameterDefinition(new ValidLocation())
                .AddParameterDefinition(new IntegerGuid())
                .AddObjectDefinition(new PressiusTestObjectObjectDefinitionWithIntegerGuid())
                .GeneratePermutation<PressiusTestObject>();

            pressiusTestObjectList.ShouldNotBeNull();
            pressiusTestObjectList.ToList().Count.ShouldBeGreaterThan(0);
            var objectList = pressiusTestObjectList.ToList();
            objectList.ForEach(obj =>
            {
                _output.WriteLine("Obj: {0} {1} {2}", obj.Id, obj.Name, obj.Address);
            });
        }
    }
}