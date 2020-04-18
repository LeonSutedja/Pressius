using System.Collections.Generic;
using System.Linq;
using Pressius.ParameterDefinition;
using Pressius.Test.Model;
using Pressius.Test.PermutationExtension.Model;
using Pressius.Test.Shared;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace Pressius.Test.PermutationExtension
{
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

            var integerParams = new IntegerParameter();
            var stringParams = new StringParameter();
            var validLocation = new ValidLocation();
            objectList.ForEach(obj =>
            {
                _output.WriteLine("Obj: {0} {1} {2}", obj.Id, obj.Name, obj.Address);
                integerParams.InputCatalogues.ShouldContain(obj.Id);
                validLocation.InputCatalogues.ShouldContain(obj.Address);
                stringParams.InputCatalogues.ShouldContain(obj.Name);
            });
        }

        [Fact]
        public void PressiusTestObject_WithValidLocationAndIntegerGuid_ShouldPermutateWithCustomValues()
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
            var integerGuid = new IntegerGuid();
            var stringParams = new StringParameter();
            var validLocation = new ValidLocation();
            objectList.ForEach(obj =>
            {
                _output.WriteLine("Obj: {0} {1} {2}", obj.Id, obj.Name, obj.Address);
                integerGuid.InputCatalogues.ShouldContain(obj.Id);
                validLocation.InputCatalogues.ShouldContain(obj.Address);
                stringParams.InputCatalogues.ShouldContain(obj.Name);
            });
        }

        [Fact]
        public void PressiusTestObject_WithPermutorNewInterfaces_ShouldPermutateWithCustomValues()
        {
            var stringParams = new StringParameter();
            var validLocation = new List<object> {
               "Mens Building, 10 Latrobe Street, VIC 3000, Melbourne, Australia",
               "111 St Kilda, VIC 3004, Melbourne, Australia" };
            var integerGuid = new List<object> {
                1531,
                9975
            };

            var pressius = new Permutor();
            var pressiusTestObjectList = pressius
                .AddParameterDefinition("ValidLocation", new List<object> {
                   "Mens Building, 10 Latrobe Street, VIC 3000, Melbourne, Australia",
                   "111 St Kilda, VIC 3004, Melbourne, Australia" })
                .AddParameterDefinition("IntegerGuid", new List<object> { 1531, 9975 })
                .WithObjectDefinitionMatcher("Address", "ValidLocation")
                .WithObjectDefinitionMatcher("Id", "IntegerGuid")
                .GeneratePermutation<PressiusTestObject>();

            pressiusTestObjectList.ShouldNotBeNull();
            pressiusTestObjectList.ToList().Count.ShouldBeGreaterThan(0);
            var objectList = pressiusTestObjectList.ToList();
            objectList.ForEach(obj =>
            {
                _output.WriteLine("Obj: {0} {1} {2}", obj.Id, obj.Name, obj.Address);
                integerGuid.ShouldContain(obj.Id);
                validLocation.ShouldContain(obj.Address);
                stringParams.InputCatalogues.ShouldContain(obj.Name);
            });
        }

        [Fact]
        public void PressiusTestObject_WithValidLocationAttribute_ShouldPermutateWithCustomValues()
        {
            var pressius = new Permutor();
            var pressiusTestObjectList = pressius
                .AddParameterDefinition(new ValidLocationWithAttribute())
                .GeneratePermutation<PressiusTestObject>();

            pressiusTestObjectList.ShouldNotBeNull();
            pressiusTestObjectList.ToList().Count.ShouldBeGreaterThan(0);
            var objectList = pressiusTestObjectList.ToList();
            var integerParameter = new IntegerParameter();
            var stringParams = new StringParameter();
            var validLocation = new ValidLocationWithAttribute();
            objectList.ForEach(obj =>
            {
                _output.WriteLine("Obj: {0} {1} {2}", obj.Id, obj.Name, obj.Address);
                integerParameter.InputCatalogues.ShouldContain(obj.Id);
                validLocation.InputCatalogues.ShouldContain(obj.Address);
                stringParams.InputCatalogues.ShouldContain(obj.Name);
            });
        }

        [Fact]
        public void PressiusTestObject_WithValidNameAndConstructorPermutation_ShouldPermutateWithCustomValues()
        {
            var pressius = new Permutor();
            var pressiusTestObjectList = pressius
                .AddParameterDefinition(new ValidName())
                .AddObjectDefinition(new PressiusTestObjectWithConstructorObjectDefinition())
                .WithConstructor()
                .GeneratePermutation<PressiusTestObjectWithConstructor>();

            pressiusTestObjectList.ShouldNotBeNull();
            pressiusTestObjectList.ToList().Count.ShouldBeGreaterThan(0);
            var objectList = pressiusTestObjectList.ToList();
            var integerParams = new IntegerParameter();
            var validName = new ValidName();
            objectList.ForEach(obj =>
            {
                _output.WriteLine("Obj: {0} {1} {2}", obj.Id, obj.Name, obj.Address);
                integerParams.InputCatalogues.ShouldContain(obj.Id);
                obj.Address.ShouldBe("Default Address");
                validName.InputCatalogues.ShouldContain(obj.Name);
            });
        }

        [Fact]
        public void PressiusTestObject_WithValidNameConstructorAndNoObjectDefinition_ShouldPermutateWithCustomValues()
        {
            var pressius = new Permutor();
            var pressiusTestObjectList = pressius
                .AddParameterDefinition(new ValidNameWithCompareParamName())
                .WithConstructor()
                .GeneratePermutation<PressiusTestObjectWithConstructor>();

            pressiusTestObjectList.ShouldNotBeNull();
            pressiusTestObjectList.ToList().Count.ShouldBeGreaterThan(0);
            var objectList = pressiusTestObjectList.ToList();
            var integerParams = new IntegerParameter();
            var validName = new ValidNameWithCompareParamName();
            objectList.ForEach(obj =>
            {
                _output.WriteLine("Obj: {0} {1} {2}", obj.Id, obj.Name, obj.Address);
                integerParams.InputCatalogues.ShouldContain(obj.Id);
                obj.Address.ShouldBe("Default Address");
                validName.InputCatalogues.ShouldContain(obj.Name);
            });
        }

        [Fact]
        public void PressiusTestObjectWithEnum_ShouldPermutate()
        {
            var permutor = new Permutor();
            var pressiusTestObjectList = permutor
                .AddParameterDefinition(new ValidCounterEnum())
                .GeneratePermutation<PressiusTestObjectWithEnum>();
            pressiusTestObjectList.ShouldNotBeNull();
            var objectList = pressiusTestObjectList.ToList();
            objectList.Count.ShouldBeGreaterThan(0);
            var integerParams = new IntegerParameter();
            var stringParams = new StringParameter();
            var counterParams = new ValidCounterEnum();
            objectList.ForEach(obj =>
            {
                _output.WriteLine("Obj: {0} {1} {2} {3}",
                    obj.Id, obj.Name, obj.Address, obj.Counter);
                integerParams.InputCatalogues.ShouldContain(obj.Id);
                stringParams.InputCatalogues.ShouldContain(obj.Name);
                stringParams.InputCatalogues.ShouldContain(obj.Address);
                counterParams.InputCatalogues.ShouldContain(obj.Counter);
            });
        }

        [Fact]
        public void PressiusTestObjectWithNullableEnum_ShouldPermutate()
        {
            var permutor = new Permutor();
            var pressiusTestObjectList = permutor
                .AddParameterDefinition(new ValidCounterEnum())
                .AddParameterDefinition(new ValidCounterTwoEnum())
                .GeneratePermutation<PressiusTestObjectWithNullableEnum>();
            pressiusTestObjectList.ShouldNotBeNull();
            var objectList = pressiusTestObjectList.ToList();
            objectList.Count.ShouldBeGreaterThan(0);
            var integerParams = new IntegerParameter();
            var stringParams = new StringParameter();
            var counterParams = new ValidCounterEnum();
            objectList.ForEach(obj =>
            {
                _output.WriteLine("Obj: {0} {1} {2} {3}",
                    obj.Id, obj.Name, obj.Address, obj.Counter);
                integerParams.InputCatalogues.ShouldContain(obj.Id);
                stringParams.InputCatalogues.ShouldContain(obj.Name);
                stringParams.InputCatalogues.ShouldContain(obj.Address);
                counterParams.InputCatalogues.ShouldContain(obj.Counter);
            });
        }

        [Fact]
        public void PressiusTestObjectWithEnum_ShouldPermutateWithIntegerEnum()
        {
            var permutor = new Permutor();
            var pressiusTestObjectList = permutor
                .AddParameterDefinition(new ValidCounterEnumWithNumber())
                .GeneratePermutation<PressiusTestObjectWithEnum>();
            pressiusTestObjectList.ShouldNotBeNull();
            var objectList = pressiusTestObjectList.ToList();
            objectList.Count.ShouldBeGreaterThan(0);
            var integerParams = new IntegerParameter();
            var stringParams = new StringParameter();
            var counterParams = new ValidCounterEnum();
            objectList.ForEach(obj =>
            {
                _output.WriteLine("Obj: {0} {1} {2} {3}",
                    obj.Id, obj.Name, obj.Address, obj.Counter);
                integerParams.InputCatalogues.ShouldContain(obj.Id);
                stringParams.InputCatalogues.ShouldContain(obj.Name);
                stringParams.InputCatalogues.ShouldContain(obj.Address);
                counterParams.InputCatalogues.ShouldContain(obj.Counter);
            });
        }
    }
}