using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Pressius.Extensions;
using Pressius.ParameterDefinition;

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
            return objectDefinition != null ? 
                _generatePermutationBasedOnObjectDefinition(objectDefinition, assemblyQualifiedClassName) : 
                _generatePermutation(assemblyQualifiedClassName);
        }

        private IEnumerable<object> _generatePermutationBasedOnObjectDefinition(
            IObjectDefinition objectDefinition,
            string assemblyQualifiedClassName)
        {
            var classType = Type.GetType(assemblyQualifiedClassName);

            return _useConstructor
                ? _generateConstructorPermutation(classType, objectDefinition) 
                : _generatePropertiesPermutation(classType, objectDefinition);
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
            var mutator = new ConstructorMutator(classType, objectDefinition, _parameterDefinitions);
            return mutator.Mutate();
        }

        private IEnumerable<object> _generatePropertiesPermutation(
            Type classType,
            IObjectDefinition objectDefinition = null)
        {
            var mutator = new PropertiesMutator(classType, objectDefinition, _parameterDefinitions);
            return mutator.Mutate();
        }

        private class PropertiesMutator
        {
            private readonly Type _classType;
            private readonly IObjectDefinition _objectDefinition;
            private readonly List<IParameterDefinition> _parameterDefinitions;

            public PropertiesMutator(
                Type classType, 
                IObjectDefinition objectDefinition, 
                List<IParameterDefinition> parameterDefinitions)
            {
                _classType = classType;
                _objectDefinition = objectDefinition;
                _parameterDefinitions = parameterDefinitions;
            }

            public IEnumerable<object> Mutate()
            {
                var listOfProperties = _classType.GetProperties();
                var propertyPermutationLists = _generatePermutationList(
                    listOfProperties,
                    _parameterDefinitions,
                    _objectDefinition);

                var attributePermutations = new List<List<object>>();
                propertyPermutationLists.GeneratePermutations(attributePermutations);
                return _generateObjectList(_classType, attributePermutations);
            }

            private List<object> _generateObjectList(
                Type classType,
                List<List<object>> attributePermutations)
            {
                var listOfProperties = classType.GetProperties();
                var results = new List<object>();
                foreach (var permutationSet in attributePermutations)
                {
                    var constructor = classType.GetConstructor(Type.EmptyTypes);
                    if (constructor == null)
                        throw new Exception($"No parameterless constructor in the class: { classType.Name }. Please add parameterless constructor.");
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

            private List<List<object>> _generatePermutationList(
                PropertyInfo[] propertyInfos,
                List<IParameterDefinition> parameterDefinitions,
                IObjectDefinition objectDefinition)
            {
                var propertyPermutationLists = new List<List<object>>();
                var attributeParameterNameDefinitions = parameterDefinitions.Where(pd => pd.CompareParamName).ToList();
                foreach (var prop in propertyInfos)
                {
                    var inputDefinitionType = string.Empty;

                    if (prop.IsNullableProperty())
                    {
                        inputDefinitionType = prop.GetNullablePropertyName();
                    }
                    else if (objectDefinition == null)
                    {
                        // when there is no object definition, we will check if there is
                        // an attribute parameter name definition that matches with the prop name.
                        // otherwise, we will assign the default prop.PropertyType.Name value
                        if (attributeParameterNameDefinitions.Any(pd => pd.TypeName.Equals(prop.Name)))
                        {
                            inputDefinitionType = attributeParameterNameDefinitions
                                .First(pd => pd.TypeName.Equals(prop.Name)).TypeName.NameList.First();
                        }
                        else
                        {
                            inputDefinitionType = prop.PropertyType.Name;
                        }
                    }
                    else if (!objectDefinition.MatcherDictionary.TryGetValue(prop.Name, out inputDefinitionType))
                    {
                        // when there is an object definition and cannot be matched, 
                        // we will check if there is an attribute parameter name definition 
                        // that matches with the prop name.
                        // otherwise, we will assign the default prop.PropertyType.Name value
                        if (attributeParameterNameDefinitions.Any(pd => pd.TypeName.Equals(prop.Name)))
                        {
                            inputDefinitionType = attributeParameterNameDefinitions
                                .First(pd => pd.TypeName.Equals(prop.Name)).TypeName.NameList.First();
                        }
                        else
                        {
                            inputDefinitionType = prop.PropertyType.Name;
                        }
                    }

                    var testInput = parameterDefinitions.FirstOrDefault(
                        id => id.TypeName.Equals(inputDefinitionType))?.InputCatalogues;

                    if (testInput == null)
                        throw new Exception($"Cannot Found Matching Definition for: { inputDefinitionType }, have you assign ObjectDefinition correctly?");

                    propertyPermutationLists.Add(testInput);
                }

                return propertyPermutationLists;
            }
        }

        private class ConstructorMutator
        {
            private readonly Type _classType;
            private readonly IObjectDefinition _objectDefinition;
            private readonly List<IParameterDefinition> _parameterDefinitions;

            public ConstructorMutator(
                Type classType, 
                IObjectDefinition objectDefinition, 
                List<IParameterDefinition> parameterDefinitions)
            {
                _classType = classType;
                _objectDefinition = objectDefinition;
                _parameterDefinitions = parameterDefinitions;
            }

            public IEnumerable<object> Mutate()
            {
                // Default, get the first constructor.
                var classConstructors = _classType.GetConstructors().FirstOrDefault();
                if (classConstructors == null)
                    throw new Exception($"Cannot found any constructor for this class type: {_classType.Name}");
                var listOfParameters = classConstructors.GetParameters();

                var propertyPermutationLists = _generatePermutationList(listOfParameters,
                    _objectDefinition, _parameterDefinitions);

                var attributePermutations = new List<List<object>>();
                propertyPermutationLists.GeneratePermutations(attributePermutations);

                var results = attributePermutations
                    .Select(ap => Activator.CreateInstance(_classType, ap.ToArray()));

                return results;
            }

            private List<List<object>> _generatePermutationList(
                ParameterInfo[] listOfParameters,
                IObjectDefinition objectDefinition,
                List<IParameterDefinition> parameterDefinitions
                )
            {
                var propertyPermutationLists = new List<List<object>>();
                foreach (var param in listOfParameters)
                {
                    var inputDefinitionType = string.Empty;
                    var attributeParameterNameDefinitions = parameterDefinitions.Where(pd => pd.CompareParamName).ToList();
                    if (param.IsNullableParameter())
                    {
                        inputDefinitionType = param.GetNullableParameterName();
                    }
                    else if (objectDefinition == null)
                    {
                        if (attributeParameterNameDefinitions.Any(pd => pd.TypeName.Equals(param.Name)))
                        {
                            inputDefinitionType = attributeParameterNameDefinitions
                                .First(pd => pd.TypeName.Equals(param.Name)).TypeName.NameList.First();
                        }
                        else
                        {
                            inputDefinitionType = param.ParameterType.Name;
                        }
                    }
                    else if (!objectDefinition.MatcherDictionary.TryGetValue(param.Name, out inputDefinitionType))
                    {
                        if (attributeParameterNameDefinitions.Any(pd => pd.TypeName.Equals(param.Name)))
                        {
                            inputDefinitionType = attributeParameterNameDefinitions
                                .First(pd => pd.TypeName.Equals(param.Name)).TypeName.NameList.First();
                        }
                        else
                        {
                            inputDefinitionType = param.ParameterType.Name;
                        }
                    }

                    var testInput = parameterDefinitions.FirstOrDefault(
                        id => id.TypeName.Equals(inputDefinitionType))?.InputCatalogues;
                    if (testInput == null)
                        throw new Exception($"Cannot Found Matching Definition for: { inputDefinitionType }, have you assign ObjectDefinition correctly?");

                    propertyPermutationLists.Add(testInput);
                }

                return propertyPermutationLists;
            }
        }
    }
}