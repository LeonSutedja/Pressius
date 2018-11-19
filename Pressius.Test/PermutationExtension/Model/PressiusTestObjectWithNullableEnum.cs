using Pressius.Test.PermutationExtension.Model;

namespace Pressius.Test.Model
{
    public class PressiusTestObjectWithNullableEnum
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public Counter? Counter { get; set; }
    }
}