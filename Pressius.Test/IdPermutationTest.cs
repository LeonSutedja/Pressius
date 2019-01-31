using Pressius.Test.Model;
using Pressius.Test.Shared;
using Shouldly;
using System.Linq;
using Pressius.ParameterDefinition;
using Xunit;
using Xunit.Abstractions;

namespace Pressius.Test
{
    public class IdPermutationTest : BaseTest
    {
        public IdPermutationTest(ITestOutputHelper output) : base(output)
        {
        }

        /// <summary>
        /// Permutator with Id as one of the object
        /// </summary>
        [Fact]
        public void PressiusTestObjectWithId_ShouldPermutate()
        {
            var pressius = new Permutor();
            var pressiusTestObjectList = pressius
                .WithId("Id")
                .GeneratePermutation<PressiusTestObject>();

            pressiusTestObjectList.ShouldNotBeNull();
            var objectList = pressiusTestObjectList.ToList();
            objectList.Count.ShouldBeGreaterThan(0);

            var stringParams = new StringParameter();
            var idCount = 1;
            objectList.ForEach(obj =>
            {
                _output.WriteLine("Obj: {0} {1} {2}", obj.Id, obj.Name, obj.Address);
                obj.Id.ShouldBe(idCount);
                idCount++;
                stringParams.InputCatalogues.ShouldContain(obj.Name);
            });
        }

        /// <summary>
        /// Permutator with Id as one of the object
        /// </summary>
        [Fact]
        public void PressiusTestObjectWithGuidId_ShouldPermutate()
        {
            var pressius = new Permutor();
            var pressiusTestObjectList = pressius
                .GeneratePermutation<PressiusTestObjectWithGuid>();

            pressiusTestObjectList.ShouldNotBeNull();
            var objectList = pressiusTestObjectList.ToList();
            objectList.Count.ShouldBeGreaterThan(0);

            var stringParams = new StringParameter();
            var idCount = 1;
            objectList.ForEach(obj =>
            {
                _output.WriteLine("Obj: {0} {1} {2}", obj.Id, obj.Name, obj.Address);
                idCount++;
                stringParams.InputCatalogues.ShouldContain(obj.Name);
            });
        }

        /// <summary>
        /// Constructor permutator with Id as one of the object
        /// </summary>
        [Fact]
        public void PressiusTestObjectWithConstructorAndId_ShouldPermutate()
        {
            var pressius = new Permutor();
            var pressiusTestObjectList = pressius
                .WithConstructor()
                .WithId("id")
                .GeneratePermutation<PressiusTestObjectWithConstructor>();

            pressiusTestObjectList.ShouldNotBeNull();
            var objectList = pressiusTestObjectList.ToList();
            objectList.Count.ShouldBeGreaterThan(0);

            var stringParams = new StringParameter();
            var idCount = 1;
            objectList.ForEach(obj =>
            {
                _output.WriteLine("Obj: {0} {1} {2}", obj.Id, obj.Name, obj.Address);
                obj.Id.ShouldBe(idCount);
                idCount++;
                stringParams.InputCatalogues.ShouldContain(obj.Name);
            });
        }

        /// <summary>
        /// Constructor permutator with Id as one of the object
        /// </summary>
        [Fact]
        public void PressiusTestObjectWithConstructorAndGuidId_ShouldPermutate()
        {
            var pressius = new Permutor();
            var pressiusTestObjectList = pressius
                .WithConstructor()
                .GeneratePermutation<PressiusTestObjectWithConstructorGuidId>();

            pressiusTestObjectList.ShouldNotBeNull();
            var objectList = pressiusTestObjectList.ToList();
            objectList.Count.ShouldBeGreaterThan(0);

            var stringParams = new StringParameter();
            var idCount = 1;
            objectList.ForEach(obj =>
            {
                _output.WriteLine("Obj: {0} {1} {2}", obj.Id, obj.Name, obj.Address);
                idCount++;
                stringParams.InputCatalogues.ShouldContain(obj.Name);
            });
        }
    }
}