using Pressius.Test.Model;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Pressius.Test
{
    public class PressiusTestComplexObject
    {
        public int Id { get; set; }
        public PressiusTestObject PressiusTestObject { get; set; }
        public string OtherVariable { get; set; }
    }

    public class PressiusTestComplexObjectDefinition
        : PropertiesObjectDefinition<PressiusTestObject>
    {
        public override Dictionary<string, string> MatcherDictionary =>
            new Dictionary<string, string>
            {
                { "PressiusTestObject", "PressiusTestObject" }
            };
    }

    public class PressiusTestObjectParameterDefinition : DefaultParameterDefinition
    {
        public override List<object> InputCatalogues =>
            new List<object> {
                new PressiusTestObject { Address = "300 Latrobe Street", Id = 1, Name = "My First Object" },
                new PressiusTestObject { Address = "500 Latrobe Street", Id = 2, Name = "My Second Object" }
            };

        public override ParameterTypeDefinition TypeName =>
            new ParameterTypeDefinition("PressiusTestObject");
    }

    public class PermutationExtensionComplexObjectTest : BaseTest
    {
        public PermutationExtensionComplexObjectTest(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void PressiusTestComplexObject_ShouldPermutate()
        {
            var pressius = new Pressius();
            var pressiusTestObjectList = pressius
                .AddParameterDefinition(new PressiusTestObjectParameterDefinition())
                .AddObjectDefinition(new PressiusTestComplexObjectDefinition())
                .GeneratePermutation<PressiusTestComplexObject>();

            pressiusTestObjectList.ShouldNotBeNull();
            pressiusTestObjectList.ToList().Count.ShouldBeGreaterThan(0);
            var objectList = pressiusTestObjectList.ToList();
            objectList.ForEach(obj =>
            {
                _output.WriteLine("Obj: {0} {1} {2} {3}", obj.Id, obj.OtherVariable, obj.PressiusTestObject.Name, obj.PressiusTestObject.Address);
            });
        }
    }
}