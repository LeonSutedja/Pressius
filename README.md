# Pressius

## Release Notes 1.0.3
- Fixes issues with constructor permutation
- Added 'CompareParamName' attributes in the parameter definition. When this is set to true, the parameter definition will be compared to attribute name by default. With CompareParamName attribute, object definition class is not needed. Pressius takes precedence of the attribute names over the object definition.

## Documentation

### What is Pressius

Pressius is an extensible object permutation
For example, a class object that contains 2 attributes of a string and an integer
will generate a list of that object with a default string and integer values.

### Example basic usage:

This is the class that we want to permutate:

	public class PressiusTestObject
	{
			public int Id { get; set; }
			public string Name { get; set; }
			public string Address { get; set; }
	}

To permutate, simply call:

	var permutationList = Permutor.Generate<PressiusTestObject>();

### Example with extension:
	
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
	var pressiusInputs = Permutor.Generate<PressiusTestObject>(
		new PressiusTestObjectObjectDefinition(),
		addedParameterDefinitions)
		.ToList();
	
Or, the following will also works:

	var pressius = new Permutor();
	var permutations = pressius
   		.AddParameterDefinition(new ValidLocation())
   		.AddObjectDefinition(new PressiusTestObjectObjectDefinition())
   		.GeneratePermutation<PressiusTestObject>();
		
### Example with constructor:		

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

### Example to compare with attribute name

**New in 1.0.3**

Release 1.0.3 contains CompareParamName variable which can be set in the parameter definition.
Once this is set, the permutation will take this, and tries to compare to the attribute name by default.

An example is like below:

    public class ValidNameWithCompareParamName : DefaultParameterDefinition
    {
        public override List<object> InputCatalogues =>
            new List<object> {
                "Clark Kent",
                "Bruce Wayne",
                "Barry Allen"
            };

        public override ParameterTypeDefinition TypeName =>
            new ParameterTypeDefinition("name");

        public override bool CompareParamName => true;
    }
	
This allows to ommit object definition class, and we can permutate with the following:

	var pressius = new Permutor();
	var pressiusTestObjectList = pressius
		.AddParameterDefinition(new ValidNameWithCompareParamName())
		.GeneratePermutation<PressiusTestObjectWithConstructor>();	
	
### Nullable value
	
Nullable value is supported. For example, a class that contains:

	public int? Id { get; set; }

will use the integer type generator.

### Pressius Algorithm

The permutation will generate a minimum permutation that will use all the values at least once.
For example, consider the following attributes set:

	[1, 2, 3]
	["", "abc", "xyz"]
	[78, 77]

will generate the following:

	[1, "", 78]
	[1, "", 77"]
	[1, "abc", 78]
	[1, "xyz", 78]
	[2, "", 78]
	[3, "", 78]

This is to keep the list of the objects minimal, whilst still able to test all the values.

### Default Types and Values

Current supported default types are:

* int 
  - 10 
  - Int32.MinValue
  - Int32.MaxValue
* string 
  - "The quick brown fox jumps over the lazy dog" // normal string
  - "1234567890 Cozy lummox gives smart squid who asks for job pen" // Alpha Numeric
  - string.Empty // Empty string
  - "~!@#$%&*()_+=-`\\][{}|;:,./?><'\"" // Characters and symbols
  - null // null value
  - new string('x', 1024 * 1024 / 2) // really long string
* DateTime
  - DateTime.Now
  - DateTime.MinValue
  - DateTime.MaxValue
* decimal
  - (decimal)0.0
  - decimal.MinValue
  - decimal.MaxValue
  - decimal.MinusOne
  - decimal.Zero
  - decimal.One
* double
  - 0.1
  - Double.MaxValue
  - Double.MinValue
* boolean
  - true
  - false
