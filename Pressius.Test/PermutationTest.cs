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
            var pressiusTestObjectList = Permutor.Generate<PressiusTestObject>();
            pressiusTestObjectList.ShouldNotBeNull();
            pressiusTestObjectList.ToList().Count.ShouldBeGreaterThan(0);
            var objectList = pressiusTestObjectList.ToList();
            objectList.ForEach(obj =>
            {
                _output.WriteLine("Obj: {0} {1} {2} {3} {4} {5} {6}", 
                    obj.Id, obj.Name, obj.Address,
                    obj.NullableInteger, obj.DecimalValue, obj.BooleanValue, obj.Created);
            });
        }
        
        /// <summary>
        /// Constructor permutator will take the first constructor it will find.
        /// </summary>
        [Fact]
        public void PressiusTestObjectWithConstructor_ShouldPermutate()
        {
            var pressiusTestObjectList = Permutor.GenerateWithConstructor<PressiusTestObjectWithConstructor>();
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
            var pressiusInputs = Permutor.Generate<PressiusTestObject>().ToList();
            foreach (var input in pressiusInputs)
            {
                yield return new object[]
                {
                    input.Id, input.Name, input.Address,
                    input.NullableInteger, input.DecimalValue, input.BooleanValue, input.Created
                };
            }
        }
        [Theory]
        [MemberData("ValidPressiusTestObject")]
        public void PressiusTestObject_ShouldBeCreated(int id, string name, string address,
            int? nullableInteger, decimal decimalValues, bool booleanValues, DateTime created)
        {
            _output.WriteLine("Obj: {0} {1} {2} {3} {4} {5} {6}", id, name, address, nullableInteger,
                decimalValues, booleanValues, created);
        }

        [Fact]
        public void PressiusTestSimpleObjectWithPrivateSet_ShouldPermutate()
        {
            var pressiusObjectList = Permutor.Generate<PressiusTestSimpleObjectWithPrivateSet>();
            pressiusObjectList.ShouldNotBeNull();
            pressiusObjectList.ToList().Count.ShouldBeGreaterThan(0);
            var objectList = pressiusObjectList.ToList();
            objectList.ForEach(obj =>
            {
                _output.WriteLine("Obj: {0} {1} {2}",
                    obj.Id, obj.Name, obj.Address);
            });
        }
    }
}