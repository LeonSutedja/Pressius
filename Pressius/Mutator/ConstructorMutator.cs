using Pressius.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Pressius
{
    internal partial class MutatorFactory
    {
        private class ConstructorMutator
        {
            private readonly Type _classType;
            private readonly IObjectDefinition _objectDefinition;
            private readonly List<IParameterDefinition> _parameterDefinitions;
            private readonly string _idParameterName;

            public ConstructorMutator(
                Type classType,
                IObjectDefinition objectDefinition,
                List<IParameterDefinition> parameterDefinitions,
                string idParameterName)
            {
                _classType = classType;
                _objectDefinition = objectDefinition;
                _parameterDefinitions = parameterDefinitions;
                _idParameterName = idParameterName;
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

                var attributePermutationsWithId = _generateAttributePermutationsId(listOfParameters,
                    attributePermutations, _idParameterName);

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

                    if (objectDefinition == null)
                    {
                        if (attributeParameterNameDefinitions.Any(pd => pd.TypeName.Equals(param.Name)))
                        {
                            inputDefinitionType = attributeParameterNameDefinitions
                                .First(pd => pd.TypeName.Equals(param.Name)).TypeName.NameList.First();
                        }
                        else
                        {
                            inputDefinitionType = param.IsNullableParameter()
                                ? param.GetNullableParameterName()
                                : param.ParameterType.Name;
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
                            inputDefinitionType = param.IsNullableParameter()
                                ? param.GetNullableParameterName()
                                : param.ParameterType.Name;
                        }
                    }

                    var testInput = parameterDefinitions.FirstOrDefault(
                        id => id.TypeName.Equals(inputDefinitionType))?.InputCatalogues;

                    if(string.Equals(param.ParameterType.Name, "Guid"))
                        testInput = new List<object> { null };

                    if (testInput == null)
                        throw new Exception($"Cannot Found Matching Definition for: { inputDefinitionType }, have you assign ObjectDefinition correctly?");

                    propertyPermutationLists.Add(testInput);
                }

                return propertyPermutationLists;
            }

            private List<List<object>> _generateAttributePermutationsId(
                ParameterInfo[] listOfParameters,
                List<List<object>> currentAttributePermutations,
                string idParameterName)
            {
                ParameterInfo parameter;
                if(_idParameterName == null)
                    parameter = listOfParameters.FirstOrDefault(pd => string.Equals(pd.ParameterType.Name, "Guid"));
                else
                    parameter = listOfParameters.FirstOrDefault(pd => string.Equals(pd.Name, idParameterName));
                
                if(parameter == null)
                {                   
                    if (idParameterName != null)
                        throw new Exception("Cannot find id with attribute name: " + idParameterName);
                    return currentAttributePermutations;
                }

                var idIndex = listOfParameters.ToList().IndexOf(parameter);

                var count = 0;
                var idType = parameter.ParameterType.Name;
                foreach (var attributePermutation in currentAttributePermutations)
                {
                    count++;
                    if (idType.Contains("Guid"))
                        attributePermutation[idIndex] = Guid.NewGuid();
                    else if(idType.Contains("Int"))
                        attributePermutation[idIndex] = count;
                    else
                        throw new Exception("Only Int and Guid is supported for Id generator");                    
                }

                return currentAttributePermutations;
            }            
        }
    }
}