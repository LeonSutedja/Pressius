using System.Collections.Generic;
using Pressius.Test.Model;
using Shouldly;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Pressius.Test
{
    public class PermutationTest : BaseTest
    {
        public PermutationTest(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void PressiusTestObject_ShouldPermutate()
        {
            var pressiusTestObjectList = Pressius.Generate<PressiusTestObject>();
            pressiusTestObjectList.ShouldNotBeNull();
            pressiusTestObjectList.ToList().Count.ShouldBeGreaterThan(0);
            var objectList = pressiusTestObjectList.ToList();
            objectList.ForEach(obj =>
            {
                _output.WriteLine("Obj: {0} {1} {2}", obj.Id, obj.Name, obj.Address);
            });
        }

        [Fact]
        public void PressiusTestObjectWithDatetime_ShouldPermutate()
        {
            var testObjectList = Pressius.Generate<PressiusTestObjectWithDatetime>();
            testObjectList.ShouldNotBeNull();
            testObjectList.ToList().Count.ShouldBeGreaterThan(0);
            var objectList = testObjectList.ToList();
            objectList.ForEach(obj =>
            {
                _output.WriteLine("Obj: {0} {1} {2} {3}", obj.Id, obj.Name, obj.Address, obj.Created);
            });
        }
        
        /// <summary>
        /// Constructor permutator will take the first constructor it will find.
        /// </summary>
        [Fact]
        public void PressiusTestObjectWithConstructor_ShouldPermutate()
        {
            var pressiusTestObjectList = Pressius.Generate<PressiusTestObjectWithConstructor>();
            pressiusTestObjectList.ShouldNotBeNull();
            pressiusTestObjectList.ToList().Count.ShouldBeGreaterThan(0);
            var objectList = pressiusTestObjectList.ToList();
            objectList.ForEach(obj =>
            {
                _output.WriteLine("Obj: {0} {1} {2}", obj.Id, obj.Name, obj.Address);
            });
        }

        public static IEnumerable<object[]> ValidPressiusTestObject()
        {
            var pressiusInputs = Pressius.Generate<PressiusTestObject>().ToList();
            foreach (var input in pressiusInputs)
            {
                yield return new object[] { input.Id, input.Name, input.Address };
            }
        }
        [Theory]
        [MemberData("ValidPressiusTestObject")]
        public void PressiusTestObject_ShouldBeCreated(int id, string name, string address)
        {
            _output.WriteLine("Obj: {0} {1} {2}", id, name, address);
        }
    }
}