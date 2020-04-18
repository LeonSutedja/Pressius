# Pressius

## Release Notes 1.2.0
- Added interfaces on Permutor to create object definition. The new interface deprecates the requirement to create object definition as a class.
- Added interfaces on Permutor to create parameter type definition. The new interface deprecates the requirement to create parameter definition as a class.

## Release Notes 1.1.0
- Added support for int Id permutation and Guid id permutation

## Release Notes 1.0.4
- Fixes issues with compare param name with nullable value

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

```csharp
public class PressiusTestObject
{
	public int Id { get; set; }
	public string Name { get; set; }
	public string Address { get; set; }
}
```

To permutate, simply call:

```csharp
var permutationList = Permutor.Generate<PressiusTestObject>();
```

### Example to use your own custom values to permutate:
	
Pressius is easily extendable. The extension points are able to provide more definition in the test cases.
The following are sample code to use your own values to permutate.

**New in 1.2.0**

```csharp
var pressius = new Permutor();
var pressiusTestObjectList = pressius
	.AddParameterDefinition("ValidLocation", new List<object> {
		"Mens Building, 10 Latrobe Street, VIC 3000, Melbourne, Australia",
		"111 St Kilda, VIC 3004, Melbourne, Australia" }) // "ValidLocation" is the collection name of values
	.AddParameterDefinition("IntegerCollection", new List<object> { 1531, 9975 }) // "IntegerCollection" is the collection name of values
	.WithObjectDefinitionMatcher("Address", "ValidLocation") // We are telling a property name "Address" to use the collection of "ValidLocation"
	.WithObjectDefinitionMatcher("Id", "IntegerCollection") // We are telling the property name "Id" to use collection of "IntegerCollection"
	.GeneratePermutation<PressiusTestObject>();
```

### Example to compare with property name

**New in 1.2.0**

The test complex object that we want to mutate is as follow:
```csharp
public class PressiusTestComplexObject
{
	public int Id { get; set; }
	public PressiusTestObject PressiusTestObject { get; set; }
	public string OtherVariable { get; set; }
}
```

To mutate the above, we simply need to do:
```csharp
var pressiusTestObjectList = new Permutor()
	.WithId("Id")
	.AddParameterDefinition("CollectionName", paramDefinitionInputCatalogues) // In here, we are creating a collection of name "CollectionName"
	.AddParameterDefinition("OtherVariable", otherVariableCatalogues, true) // The 'true', in here sets that, this collection is for a property name "OtherVarible"
	.WithObjectDefinitionMatcher("PressiusTestObject", "CollectionName") // We are using telling to match PressiusTestObject type to a collection values with name "CollectionName"
	.GeneratePermutation<PressiusTestComplexObject>();
```

### Example with constructor:		

Object Constructor is also supported.
For example, the following object will be constructed, using the first constructor it finds.

```csharp
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
```

### Example to generate Id

Only Integer and Guid for Id is currently supported by default.
To permutate an integer Id we need to use .WithId("{Id name}"). This is as, integer is a generic type, and it is required to distinguished between normal integer, an id.
 
```csharp
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
```

Then pressius can be call with:
```csharp
var pressius = new Permutor();
var pressiusTestObjectList = pressius
	.WithId("Id")
    .GeneratePermutation<PressiusTestObject>();
```

Constructor is also similar. An object like:
```csharp
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
}
```
Can be permutated with the following command:
```csharp
var pressius = new Permutor();
var pressiusTestObjectList = pressius
    .WithConstructor()
    .WithId("Id") // Id is the property name for the id. 
    .GeneratePermutation<PressiusTestObjectWithConstructor>();
```

Guid permutation is more straightforward, and does not require WithId. This is as, Guid itself is a type, and hence we can identify it.
An example of a guid object:
```csharp
public class PressiusTestObjectWithGuid
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
}
```

can be permutated with simply the following:
```csharp
var pressius = new Permutor();
var pressiusTestObjectList = pressius
    .GeneratePermutation<PressiusTestObjectWithGuid>();
```

### Nullable value
	
Nullable value is supported. For example, a class that contains:

```csharp
public int? Id { get; set; }
```

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
