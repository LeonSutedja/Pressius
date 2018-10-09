using System;
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
                _output.WriteLine("Obj: {0} {1} {2} {3} {4} {5}", 
                    obj.Id, obj.Name, obj.Address,
                    obj.NullableInteger, obj.DecimalValues, obj.Created);
            });
        }
        
        /// <summary>
        /// Constructor permutator will take the first constructor it will find.
        /// </summary>
        [Fact]
        public void PressiusTestObjectWithConstructor_ShouldPermutate()
        {
            var pressiusTestObjectList = Pressius.GenerateWithConstructor<PressiusTestObjectWithConstructor>();
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
                yield return new object[]
                {
                    input.Id, input.Name, input.Address,
                    input.NullableInteger, input.DecimalValues, input.Created
                };
            }
        }
        [Theory]
        [MemberData("ValidPressiusTestObject")]
        public void PressiusTestObject_ShouldBeCreated(int id, string name, string address,
            int? nullableInteger, decimal decimalValues, DateTime created)
        {
            _output.WriteLine("Obj: {0} {1} {2} {3} {4} {5}", id, name, address, nullableInteger,
                decimalValues, created);
        }
    }
}