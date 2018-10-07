namespace Pressius.Test.Model
{
    public class PressiusTestObjectWithConstructor
    {
        public int Id { get; }
        public string Name { get; }
        public string Address { get; }

        public PressiusTestObjectWithConstructor(int id, string name)
        {
            Id = id;
            Name = name;
            Address = "Default Address";
        }

        public PressiusTestObjectWithConstructor(int id, string name, string address)
        {
            Id = id;
            Name = name;
            Address = address;
        }

    }
}