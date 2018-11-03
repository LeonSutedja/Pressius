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