using Pressius.Test.Model;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using Pressius.Test.PermutationExtension.Model;
using Pressius.Test.Shared;
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
            var objectList = pressiusTestObjectList.ToList();
            objectList.Count.ShouldBeGreaterThan(0);
            var integerParams = new IntegerParameter();
            var stringParams = new StringParameter();
            var booleanParams = new BooleanParameter();
            var decimalParams = new DecimalParameter();
            objectList.ForEach(obj =>
            {
                _output.WriteLine("Obj: {0} {1} {2} {3} {4} {5} {6}",
                    obj.Id, obj.Name, obj.Address,
                    obj.NullableInteger, obj.DecimalValue, obj.BooleanValue, obj.Created);
                integerParams.InputCatalogues.ShouldContain(obj.Id);
                integerParams.InputCatalogues.ShouldContain(obj.NullableInteger);
                stringParams.InputCatalogues.ShouldContain(obj.Name);
                stringParams.InputCatalogues.ShouldContain(obj.Address);
                booleanParams.InputCatalogues.ShouldContain(obj.BooleanValue);
                decimalParams.InputCatalogues.ShouldContain(obj.DecimalValue);
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
            var objectList = pressiusTestObjectList.ToList();
            objectList.Count.ShouldBeGreaterThan(0);
            var integerParams = new IntegerParameter();
            var stringParams = new StringParameter();
            objectList.ForEach(obj =>
            {
                _output.WriteLine("Obj: {0} {1} {2}", obj.Id, obj.Name, obj.Address);
                integerParams.InputCatalogues.ShouldContain(obj.Id);
                stringParams.InputCatalogues.ShouldContain(obj.Name);
                obj.Address.ShouldBe("Default Address");
            });
        }

        public static IEnumerable<object[]> ValidPressiusTestObject()
        {
            var pressiusInputs = Permutor.Generate<PressiusTestObject>();
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
        public void PressiusTestObject_UsingXunitTheory_ShouldCreateMultiple(int id, string name, string address,
            int? nullableInteger, decimal decimalValues, bool booleanValues, DateTime created)
        {
            var integerParams = new IntegerParameter();
            var stringParams = new StringParameter();
            var booleanParams = new BooleanParameter();
            var decimalParams = new DecimalParameter();
            _output.WriteLine("Obj: {0} {1} {2} {3} {4} {5} {6}", id, name, address, nullableInteger,
                decimalValues, booleanValues, created);
            integerParams.InputCatalogues.ShouldContain(id);
            integerParams.InputCatalogues.ShouldContain(nullableInteger);
            stringParams.InputCatalogues.ShouldContain(name);
            stringParams.InputCatalogues.ShouldContain(address);
            booleanParams.InputCatalogues.ShouldContain(booleanValues);
            decimalParams.InputCatalogues.ShouldContain(decimalValues);
        }

        [Fact]
        public void PressiusTestSimpleObjectWithPrivateSet_ShouldPermutate()
        {
            var pressiusObjectList = Permutor.Generate<PressiusTestSimpleObjectWithPrivateSet>();
            pressiusObjectList.ShouldNotBeNull();
            var objectList = pressiusObjectList.ToList();
            objectList.Count.ShouldBeGreaterThan(0);
            objectList.ForEach(obj =>
            {
                _output.WriteLine("Obj: {0} {1} {2}",
                    obj.Id, obj.Name, obj.Address);
            });
        }
    }
}