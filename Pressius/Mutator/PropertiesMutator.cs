using Pressius.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Pressius
{
    internal partial class MutatorFactory
    {
        private class PropertiesMutator
        {
            private readonly string _idName;
            private readonly Type _classType;
            private readonly IObjectDefinition _objectDefinition;
            private readonly List<IParameterDefinition> _parameterDefinitions;

            public PropertiesMutator(
                string idName,
                Type classType,
                IObjectDefinition objectDefinition,
                List<IParameterDefinition> parameterDefinitions)
            {
                _idName = idName;
                _classType = classType;
                _objectDefinition = objectDefinition;
                _parameterDefinitions = parameterDefinitions;
            }

            public IEnumerable<object> Mutate()
            {
                var listOfProperties = _classType.GetProperties();
                var propertyPermutationLists = _generatePermutationList(
                    _idName,
                    listOfProperties,
                    _parameterDefinitions,
                    _objectDefinition);

                var attributePermutations = new List<List<object>>();
                propertyPermutationLists.GeneratePermutations(attributePermutations);
                return _generateObjectList(_idName, _classType, attributePermutations);
            }           

            private List<List<object>> _generatePermutationList(
                string idName,
                PropertyInfo[] propertyInfos,
                List<IParameterDefinition> parameterDefinitions,
                IObjectDefinition objectDefinition)
            {
                var propertyPermutationLists = new List<List<object>>();
                var attributeParameterNameDefinitions = parameterDefinitions.Where(pd => pd.CompareParamName).ToList();
                foreach (var prop in propertyInfos)
                {
                    var inputDefinitionType = string.Empty;                    

                    if (objectDefinition == null)
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
                            inputDefinitionType = prop.IsNullableProperty()
                                ? prop.GetNullablePropertyName()
                                : prop.PropertyType.Name;
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
                            inputDefinitionType = prop.IsNullableProperty()
                                ? prop.GetNullablePropertyName()
                                : prop.PropertyType.Name;
                        }
                    }

                    var testInput = parameterDefinitions.FirstOrDefault(
                        id => id.TypeName.Equals(inputDefinitionType))?.InputCatalogues;
                    
                    if (string.Equals(prop.PropertyType.Name, "Guid"))
                        testInput = new List<object> { null };

                    if (testInput == null)                    
                        throw new Exception($"Cannot Found Matching Definition for: { inputDefinitionType }, have you assign ObjectDefinition correctly?");
                    

                    propertyPermutationLists.Add(testInput);
                }

                return propertyPermutationLists;
            }

            private List<object> _generateObjectList(
               string idParameterName,
               Type classType,
               List<List<object>> attributePermutations)
            {
                var listOfProperties = classType.GetProperties();
                var results = new List<object>();
                var idCount = 0;
                foreach(var permutationSet in attributePermutations)
                {
                    var constructor = classType.GetConstructor(Type.EmptyTypes);
                    if(constructor == null)
                        throw new Exception($"No parameterless constructor in the class: { classType.Name }. Please add parameterless constructor.");
                    var newInput = Activator.CreateInstance(classType);
                    for(var i = 0; i < listOfProperties.Length; i++)
                    {
                        var prop = listOfProperties[i];
                        if(idParameterName != null && prop.Name.Equals(idParameterName))
                        {
                            idCount++;
                            if(prop.PropertyType.Name.Contains("Int"))
                                prop.SetValue(newInput, idCount);
                            else
                            {
                                if(prop.PropertyType.Name.Contains("Guid"))
                                    throw new Exception("Guid does not require WithId");

                                throw new Exception("Only Int and Guid is supported for Id generator");
                            }
                        }
                        else
                        {
                            var propValue = permutationSet[i];
                            if(prop.PropertyType.Name.Contains("Guid"))
                                propValue = Guid.NewGuid();                                
                            prop.SetValue(newInput, propValue);
                        }
                    }
                    results.Add(newInput);
                }

                return results;
            }
        }
    }
}