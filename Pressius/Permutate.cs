using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
    public class Permutate
    {
        public static IEnumerable<T> Generate<T>()
        {
            var inputGenerator = new MutatorFactory();
            var inputs = inputGenerator.GeneratePermutations<T>();
            return inputs;
        }

        public static IEnumerable<T> Generate<T>(
            IObjectDefinition objectDefinition)
        {
            var inputGenerator = new MutatorFactory();
            var inputs = inputGenerator
                .SetObjectDefinition(objectDefinition)
                .GeneratePermutations<T>();
            return inputs;
        }

        public static IEnumerable<T> Generate<T>(
            IObjectDefinition objectDefinition,
            List<IParameterDefinition> parameterDefinitions)
        {
            var inputGenerator = new MutatorFactory();
            var inputs = inputGenerator
                .SetObjectDefinition(objectDefinition)
                .AddParameterDefinitions(parameterDefinitions)
                .GeneratePermutations<T>();
            return inputs;
        }

        private class MutatorFactory
        {
            private readonly List<IParameterDefinition> _inputDefinitions;
            private readonly List<IObjectDefinition> _objectDefinitions;

            private Type[] GetTypesInNamespace(string nameSpace)
            {
                var assembly = Assembly.GetExecutingAssembly();
                return assembly.GetTypes().Where(t => String.Equals(t.Namespace, nameSpace, StringComparison.Ordinal)).ToArray();
            }

            public MutatorFactory()
            {
                _inputDefinitions = new List<IParameterDefinition>()
                {
                    new StringParameter(),
                    new EmailStringParameter(),
                    new IntegerParameter(),
                    new DateTimeParameter(),
                    new DoubleParameter()
                };
                _objectDefinitions = new List<IObjectDefinition>();
            }

            public MutatorFactory SetObjectDefinition(IObjectDefinition objectDefinition)
            {
                _objectDefinitions.Add(objectDefinition);
                return this;
            }

            public MutatorFactory AddParameterDefinitions(List<IParameterDefinition> parameterDefinitions)
            {
                _inputDefinitions.AddRange(parameterDefinitions);
                return this;
            }

            public IEnumerable<object> GeneratePermutationsFromAssemblyQualifiedClassName(
                string assemblyQualifiedClassName)
            {
                return _generateObjectPermutation(assemblyQualifiedClassName);
            }

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

                try
                {
                    // we try to generate the object by properties permutation.
                    // If it fails, we will generate via constructor instead.
                    return _generatePropertiesPermutation(classType);
                }
                catch
                {
                    try
                    {
                        return _generateConstructorPermutation(classType);
                    }
                    catch
                    {
                        throw new Exception("Cannot automatically generate permutations. Please define object definition first");
                    }
                }
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

                var attributePermutationsMinimal = propertyPermutationLists.GenerateMinimalPermutationsTakeTwo();

                var results = attributePermutationsMinimal
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
                    if (objectDefinition == null)
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
                //propertyPermutationLists.GeneratePermutations(attributePermutations);

                ObjectPermutationExtension.GeneratePermutations(propertyPermutationLists, attributePermutations);

                //attributePermutations = propertyPermutationLists.GenerateMinimalPermutationsTakeTwo();

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