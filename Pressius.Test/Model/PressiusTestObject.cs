using System;

namespace Pressius.Test.Model
{
    public class PressiusTestObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public int? NullableInteger { get; set; }
        public decimal DecimalValue { get; set; }
        public bool BooleanValue { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Modified { get; set; }
    }
}