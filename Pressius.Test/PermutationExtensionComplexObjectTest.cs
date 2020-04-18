using Pressius.Test.Model;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using Pressius.Test.Shared;
using Xunit;
using Xunit.Abstractions;

namespace Pressius.Test
{
    public class PermutationExtensionComplexObjectTest : BaseTest
    {
        public PermutationExtensionComplexObjectTest(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void PressiusTestComplexObject_ShouldPermutate()
        {
            var pressiusTestObjectList = new Permutor()
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

        [Fact]
        public void PressiusTestComplexObject_WithPermutorInterface_ShouldPermutate()
        {
            var paramDefinitionInputCatalogues = new List<object> {
                new PressiusTestObject { Address = "300 Latrobe Street", Id = 1, Name = "My First Object" },
                new PressiusTestObject { Address = "500 Latrobe Street", Id = 2, Name = "My Second Object" },
                new PressiusTestObject { Address = "600 Latrobe Street", Id = 3, Name = "My Third Object" }
            };
            var otherVariableCatalogues = new List<object>
            {
                "Test1", "Test2", "Test3", "Test4", "Test5", "Test6", "Test7","Test8","Test9", "Test10"
            };
            var pressiusTestObjectList = new Permutor()
                .WithId("Id")
                .AddParameterDefinition("PressiusTestObject", paramDefinitionInputCatalogues)
                .AddParameterDefinition("OtherVariable", otherVariableCatalogues, true)
                .WithCustomObjectDefinition<PressiusTestComplexObject>()
                .WithObjectDefinitionMatcher("PressiusTestObject", "PressiusTestObject")
                .GeneratePermutation<PressiusTestComplexObject>();

            pressiusTestObjectList.ShouldNotBeNull();
            pressiusTestObjectList.ToList().Count.ShouldBeGreaterThan(0);
            var objectList = pressiusTestObjectList.ToList();

            var addressList = paramDefinitionInputCatalogues.Select(c => ((PressiusTestObject)c).Address);
            var nameList = paramDefinitionInputCatalogues.Select(c => ((PressiusTestObject)c).Name);
            var otherVariableList = otherVariableCatalogues.Select(c => (string)c);
            objectList.ForEach(obj =>
            {
                _output.WriteLine("Obj: {0} {1} {2} {3}", obj.Id, obj.OtherVariable, obj.PressiusTestObject.Name, obj.PressiusTestObject.Address);
                nameList.ShouldContain(obj.PressiusTestObject.Name);
                addressList.ShouldContain(obj.PressiusTestObject.Address);
                otherVariableList.ShouldContain(obj.OtherVariable);
            });
        }

        [Fact]
        public void PressiusTestComplexObjectWithPressiusGeneratedObject_ShouldPermutate()
        {
            var permutator = new Permutor();
            var pressiusTestObjectList = permutator.GeneratePermutation<PressiusTestObject>();
            var genericObjectList = new List<object>();
            genericObjectList.AddRange(pressiusTestObjectList);
            var pressiusTestComplexObjectList = permutator
                .AddParameterDefinition(new PressiusTestObjectParameterDefinition(genericObjectList))
                .AddObjectDefinition(new PressiusTestComplexObjectDefinition())
                .GeneratePermutation<PressiusTestComplexObject>();

            pressiusTestComplexObjectList.ShouldNotBeNull();
            pressiusTestComplexObjectList.ToList().Count.ShouldBeGreaterThan(0);
            var objectList = pressiusTestComplexObjectList.ToList();
            objectList.ForEach(obj =>
            {
                _output.WriteLine("Obj: {0} {1} {2} {3}", obj.Id, obj.OtherVariable, obj.PressiusTestObject.Name, obj.PressiusTestObject.Address);
            });
        }
    }
}