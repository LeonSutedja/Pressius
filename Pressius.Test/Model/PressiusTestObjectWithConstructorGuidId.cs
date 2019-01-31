using System;

namespace Pressius.Test.Model
{
    public class PressiusTestObjectWithConstructorGuidId
    {
        public Guid Id { get; }
        public string Name { get; }
        public string Address { get; }

        public PressiusTestObjectWithConstructorGuidId(Guid id, string name)
        {
            Id = id;
            Name = name;
            Address = "Default Address";
        }

        public PressiusTestObjectWithConstructorGuidId(Guid id, string name, string address)
        {
            Id = id;
            Name = name;
            Address = address;
        }
    }
}