using System;

namespace Pressius.Test.Model
{
    public class PressiusTestSuperComplexObject
    {
        public Guid Id { get; set; }
        public PressiusTestComplexObject PressiusTestObject1 { get; set; }
        public PressiusTestComplexObject PressiusTestObject2 { get; set; }
        public PressiusTestComplexObject PressiusTestObject3 { get; set; }
        public string OtherVariable { get; set; }
    }
}