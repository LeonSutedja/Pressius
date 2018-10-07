using System;
using System.Linq;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace Pressius.Test
{
    public class PressiusTestObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
    }

    public class PressiusTestObjectWithDatetime
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public DateTime Created { get; set; }
    }

    public class PermutationTest
    {
        private readonly ITestOutputHelper output;

        public PermutationTest(ITestOutputHelper output)
        {
            this.output = output;
        }


        [Fact]
        public void GeneratePressiusTestObject_ShouldPermutate()
        {
            var pressiusTestObjectList = Permutate.Generate<PressiusTestObject>();
            pressiusTestObjectList.ShouldNotBeNull();
            pressiusTestObjectList.ToList().Count.ShouldBeGreaterThan(0);
            var objectList = pressiusTestObjectList.ToList();
            objectList.ForEach(obj =>
            {
                output.WriteLine("Obj: {0} {1} {2}", obj.Id, obj.Name, obj.Address);
            });
        }

        [Fact]
        public void GeneratePressiusTestObjectWithDatetime_ShouldPermutate()
        {
            var testObjectList = Permutate.Generate<PressiusTestObjectWithDatetime>();
            testObjectList.ShouldNotBeNull();
            testObjectList.ToList().Count.ShouldBeGreaterThan(0);
            var objectList = testObjectList.ToList();
            objectList.ForEach(obj =>
            {
                output.WriteLine("Obj: {0} {1} {2} {3}", obj.Id, obj.Name, obj.Address, obj.Created);
            });
        }
    }
}
