using System;
using System.Collections.Generic;
using System.Linq;

namespace Pressius
{
    /// <summary>
    /// Pressius is an extensible object permutation
    /// For example, a class object that contains 2 attributes of a string and an integer
    /// will generate a list of that object with a default string and integer values.
    ///
    /// Example basic usage:
    /// var pressiusInputs = Permutate.Generate<T>().ToList();
    /// where T is the object type that aimed to be permutated.
    ///
    /// To extend the function with a custom input values to be mutated, a class extended from PropertiesObjectDefinition<T> is required.
    /// This is where T is the type of the object to be mutated. Example of the function is below.
    /// public class CreateRoomCommandObjectDefinition : PropertiesObjectDefinition<CreateRoomCommand>
    /// {
    ///    public override Dictionary<string, string> MatcherDictionary =>
    ///        new Dictionary<string, string>
    ///        {
    ///            { "ShortName", "ValidShortRoomName" },
    ///            { "Name", "ValidRoomName" },
    ///            { "Location", "ValidLocation" }
    ///        };
    /// }
    ///
    /// To create a set of custom values as an input, a class extended from DefaultParameterDefinition is required.
    /// Below is an example of such class for 'ValidLocation' above.
    /// public class ValidLocation : DefaultParameterDefinition
    /// {
    ///    public override List<object> InputCatalogues =>
    ///        new List<object> {
    ///            "Mens Building, 10 Latrobe Street, VIC 3000, Melbourne, Australia",
    ///            "~!@#$%&*()_+=-`\\][{}|;:,./?><'\"" };
    ///    public override ParameterTypeDefinition TypeName => new ParameterTypeDefinition("ValidLocation");
    /// }
    ///
    /// To bind them all together, the following is a sample usage:
    /// var addedParameterDefinitions = new List<IParameterDefinition>()
    /// {
    ///     new ValidShortRoomName(),
    ///     new ValidRoomName(),
    ///     new ValidLocation()
    /// };
    /// var pressiusInputs = Permutate.Generate<CreateRoomCommand>(
    ///     new CreateRoomCommandObjectDefinition(),
    ///     addedParameterDefinitions).ToList();
    ///
    /// </summary>
    public class PressiusPermutator
    {
        public static IEnumerable<T> Generate<T>()
        {
            var inputGenerator = new MutatorFactory();
            var inputs = inputGenerator.GeneratePermutations<T>();
            return inputs;
        }

        public static IEnumerable<T> GenerateWithConstructor<T>()
        {
            var inputGenerator = new MutatorFactory();
            inputGenerator.WithConstructor();
            var inputs = inputGenerator.GeneratePermutations<T>();
            return inputs;
        }

        public static IEnumerable<T> Generate<T>(
            IObjectDefinition objectDefinition,
            List<IParameterDefinition> parameterDefinitions)
        {
            var inputGenerator = new MutatorFactory();
            inputGenerator.AddObjectDefinition(objectDefinition);
            inputGenerator.AddParameterDefinitions(parameterDefinitions);
            var inputs = inputGenerator.GeneratePermutations<T>();
            return inputs;
        }

        private MutatorFactory _mutatorFactory { get; }

        public PressiusPermutator()
        {
            _mutatorFactory = new MutatorFactory();
        }

        public PressiusPermutator AddParameterDefinition(IParameterDefinition parameterDefinition)
        {
            _mutatorFactory.AddParameterDefinition(parameterDefinition);
            return this;
        }

        public PressiusPermutator AddObjectDefinition(IObjectDefinition objectDefinition)
        {
            _mutatorFactory.AddObjectDefinition(objectDefinition);
            return this;
        }

        public PressiusPermutator WithConstructor()
        {
            _mutatorFactory.WithConstructor();
            return this;
        }

        public IEnumerable<T> GeneratePermutation<T>()
        {
            return _mutatorFactory.GeneratePermutations<T>();
        }

        private class MutatorFactory
        {
            private readonly List<IParameterDefinition> _inputDefinitions;
            private readonly List<IObjectDefinition> _objectDefinitions;
            private bool _useConstructor;

            public MutatorFactory()
            {
                _inputDefinitions = new List<IParameterDefinition>
                {
                    new StringParameter(),
                    new IntegerParameter(),
                    new DecimalParameter(),
                    new BooleanParameter(),
                    new DateTimeParameter(),
                    new DoubleParameter()
                };
                _objectDefinitions = new List<IObjectDefinition>();
                _useConstructor = false;
            }

            public void AddObjectDefinition(IObjectDefinition objectDefinition)
                => _objectDefinitions.Add(objectDefinition);

            public void AddParameterDefinition(IParameterDefinition parameterDefinition)
                => _inputDefinitions.Add(parameterDefinition);

            public void AddParameterDefinitions(List<IParameterDefinition> parameterDefinitions)
                => _inputDefinitions.AddRange(parameterDefinitions);

            public void WithConstructor() => _useConstructor = true;

            public IEnumerable<T> GeneratePermutations<T>()
            {
                var assemblyQualifiedClassName = typeof(T).AssemblyQualifiedName;
                var permutations = _generateObjectPermutation(assemblyQualifiedClassName);
                return permutations.Select(objectResult => (T)objectResult);
            }

            private IEnumerable<object> _generateObjectPermutation(string assemblyQualifiedClassName)
            {
                var objectDefinition = _objectDefinitions.FirstOrDefault(
                       od => string.Compare(od.AssemblyQualifiedClassName, assemblyQualifiedClassName) == 0);
                if (objectDefinition != null)
                {
                    return _generatePermutationBasedOnObjectDefinition(objectDefinition, assemblyQualifiedClassName);
                }

                return _generatePermutation(assemblyQualifiedClassName);
            }

            private IEnumerable<object> _generatePermutationBasedOnObjectDefinition(
                IObjectDefinition objectDefinition,
                string assemblyQualifiedClassName)
            {
                var classType = Type.GetType(assemblyQualifiedClassName);

                var objectInitiation = objectDefinition.InitiationMethod;
                if (objectInitiation == ObjectInitiation.Constructor)
                {
                    return _generateConstructorPermutation(classType, objectDefinition);
                }
                else if (objectInitiation == ObjectInitiation.Properties)
                {
                    return _generatePropertiesPermutation(classType, objectDefinition);
                }

                throw new Exception("Cannot find object initiation");
            }

            private IEnumerable<object> _generatePermutation(string assemblyQualifiedClassName)
            {
                var classType = Type.GetType(assemblyQualifiedClassName);
                if (_useConstructor)
                {
                    return _generateConstructorPermutation(classType);
                }
                return _generatePropertiesPermutation(classType);
            }

            private IEnumerable<object> _generateConstructorPermutation(
               Type classType,
               IObjectDefinition objectDefinition = null)
            {
                var classConstructors = classType.GetConstructors().FirstOrDefault();
                var listOfParameters = classConstructors.GetParameters();

                var propertyPermutationLists = new List<List<object>>();
                foreach (var param in listOfParameters)
                {
                    var inputDefinitionType = string.Empty;
                    if (objectDefinition == null)
                    {
                        inputDefinitionType = param.ParameterType.Name;
                    }
                    else if (!objectDefinition.MatcherDictionary.TryGetValue(param.Name, out inputDefinitionType))
                    {
                        inputDefinitionType = param.ParameterType.Name;
                    }

                    var testInput = _inputDefinitions.FirstOrDefault(
                        id => id.TypeName.Equals(inputDefinitionType))?.InputCatalogues;
                    if (testInput == null)
                        throw new Exception("Cannot Found Matching Input Definition");

                    propertyPermutationLists.Add(testInput);
                }

                var attributePermutations = new List<List<object>>();
                propertyPermutationLists.GeneratePermutations(attributePermutations);

                var results = attributePermutations
                    .Select(ap => Activator.CreateInstance(classType, ap.ToArray()));

                return results;
            }

            private IEnumerable<object> _generatePropertiesPermutation(
                Type classType,
                IObjectDefinition objectDefinition = null)
            {
                var listOfProperties = classType.GetProperties();
                var propertyPermutationLists = new List<List<object>>();
                foreach (var prop in listOfProperties)
                {
                    var inputDefinitionType = string.Empty;
                    if (prop.PropertyType.IsGenericType &&
                        prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        var propertyType = prop.PropertyType.GetGenericArguments().First();
                        inputDefinitionType = propertyType.Name;
                    }
                    else if (objectDefinition == null)
                    {
                        inputDefinitionType = prop.PropertyType.Name;
                    }
                    else if (!objectDefinition.MatcherDictionary.TryGetValue(prop.Name, out inputDefinitionType))
                    {
                        inputDefinitionType = prop.PropertyType.Name;
                    }

                    var testInput = _inputDefinitions.FirstOrDefault(
                        id => id.TypeName.Equals(inputDefinitionType))?.InputCatalogues;
                    if (testInput == null)
                        throw new Exception("Cannot Found Matching Input Definition");

                    propertyPermutationLists.Add(testInput);
                }

                var attributePermutations = new List<List<object>>();
                propertyPermutationLists.GeneratePermutations(attributePermutations);

                var results = new List<object>();
                foreach (var permutationSet in attributePermutations)
                {
                    var constructor = classType.GetConstructor(Type.EmptyTypes);
                    if (constructor == null) throw new Exception("No parameterless constructor in the class.");
                    var newInput = Activator.CreateInstance(classType);
                    for (var i = 0; i < listOfProperties.Length; i++)
                    {
                        var prop = listOfProperties[i];
                        var propValue = permutationSet[i];
                        prop.SetValue(newInput, propValue);
                    }
                    results.Add(newInput);
                }

                return results;
            }
        }
    }
}