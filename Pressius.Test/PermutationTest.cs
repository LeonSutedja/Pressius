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
            var pressiusTestObjectList = Permutate.Generate<PressiusTestObject>();
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
            var testObjectList = Permutate.Generate<PressiusTestObjectWithDatetime>();
            testObjectList.ShouldNotBeNull();
            testObjectList.ToList().Count.ShouldBeGreaterThan(0);
            var objectList = testObjectList.ToList();
            objectList.ForEach(obj =>
            {
                _output.WriteLine("Obj: {0} {1} {2} {3}", obj.Id, obj.Name, obj.Address, obj.Created);
            });
        }
    }
}