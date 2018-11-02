using System;
using System.Collections.Generic;
using System.Linq;
using Pressius.Extensions;

namespace Pressius
{
    internal class MutatorFactory
    {
        private readonly List<IParameterDefinition> _parameterDefinitions;
        private readonly List<IObjectDefinition> _objectDefinitions;
        private bool _useConstructor;

        public MutatorFactory()
        {
            _parameterDefinitions = new List<IParameterDefinition>
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
            => _parameterDefinitions.Add(parameterDefinition);

        public void AddParameterDefinitions(List<IParameterDefinition> parameterDefinitions)
            => _parameterDefinitions.AddRange(parameterDefinitions);

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
            return objectDefinition != null ? _generatePermutationBasedOnObjectDefinition(objectDefinition, assemblyQualifiedClassName) : _generatePermutation(assemblyQualifiedClassName);
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
            if (objectInitiation == ObjectInitiation.Properties)
            {
                return _generatePropertiesPermutation(classType, objectDefinition);
            }

            throw new Exception("Cannot find object initiation");
        }

        private IEnumerable<object> _generatePermutation(string assemblyQualifiedClassName)
        {
            var classType = Type.GetType(assemblyQualifiedClassName);
            return _useConstructor ? _generateConstructorPermutation(classType) : _generatePropertiesPermutation(classType);
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
                if (param.IsNullableParameter())
                {
                    inputDefinitionType = param.GetNullableParameterName();
                }
                else if (objectDefinition == null)
                {
                    inputDefinitionType = param.ParameterType.Name;
                }
                else if (!objectDefinition.MatcherDictionary.TryGetValue(param.Name, out inputDefinitionType))
                {
                    inputDefinitionType = param.ParameterType.Name;
                }

                var testInput = _parameterDefinitions.FirstOrDefault(
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
                if (prop.IsNullableProperty())
                {
                    inputDefinitionType = prop.GetNullablePropertyName();
                }
                else if (objectDefinition == null)
                {
                    inputDefinitionType = prop.PropertyType.Name;
                }
                else if (!objectDefinition.MatcherDictionary.TryGetValue(prop.Name, out inputDefinitionType))
                {
                    inputDefinitionType = prop.PropertyType.Name;
                }

                var testInput = _parameterDefinitions.FirstOrDefault(
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