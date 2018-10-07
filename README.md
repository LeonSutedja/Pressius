# Pressius
Pressius is a naive object creator permutation for test purpose.

Pressius is an extensible object permutation
For example, a class object that contains 2 attributes of a string and an integer
will generate a list of that object with a default string and integer values.

Example basic usage:

This is the class that we want to permutate:
    public class PressiusTestObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
    }

To permutate, simply call:
var permutationList = Pressius.Generate<PressiusTestObject>().ToList();

Pressius is easily extendable. The extension points are able to provide more definition in the test cases.
To extend the function with a custom input values to be mutated, a class extended from PropertiesObjectDefinition<T> is required.
This is where T is the type of the object to be mutated. Example of the function is below.
    public class PressiusTestObjectObjectDefinition : PropertiesObjectDefinition<PressiusTestObject>
    {
        public override Dictionary<string, string> MatcherDictionary =>
            new Dictionary<string, string>
            {
                { "Address", "ValidLocation" }
            };
    }

To create a set of custom values as an input, a class extended from DefaultParameterDefinition is required.
Below is an example of such class for 'ValidLocation' above.
    public class ValidLocation : DefaultParameterDefinition
    {
        public override List<object> InputCatalogues =>
            new List<object> {
               "Mens Building, 10 Latrobe Street, VIC 3000, Melbourne, Australia",
               "111 St Kilda, VIC 3004, Melbourne, Australia" };

        public override ParameterTypeDefinition TypeName =>
            new ParameterTypeDefinition("ValidLocation");
    }

To bind them all together, the following is a sample usage:
var addedParameterDefinitions = new List<IParameterDefinition>()
{
    new ValidLocation()
};
var pressiusInputs = Pressius.Generate<PressiusTestObject>(
    new PressiusTestObjectObjectDefinition(),
    addedParameterDefinitions).ToList();
	
Or, we can also do like:
var pressius = new Pressius();
var permutations = pressius
   .AddParameterDefinition(new ValidLocation())
   .AddObjectDefinition(new PressiusTestObjectObjectDefinition())
   .GeneratePermutation<PressiusTestObject>();
   
Object Constructor is also supported.
For example, the following object will be constructed, using the first constructor it finds.
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

