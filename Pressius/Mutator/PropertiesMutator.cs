using Pressius.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Pressius
{
    internal class PropertiesMutator
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
           string definedIdParameterName,
            PropertyInfo[] propertyInfos,
            List<IParameterDefinition> parameterDefinitions,
            IObjectDefinition objectDefinition)
        {
            var propertyPermutationLists = new List<List<object>>();
            var attributeParameterNameDefinitions = parameterDefinitions.Where(pd => pd.CompareParamName).ToList();
            foreach (var prop in propertyInfos)
            {
                var inputDefinitionType = string.Empty;

                var isInputDefinitionTypeNotDefined = (objectDefinition == null) ||
                    ((objectDefinition != null) &&
                        !objectDefinition.MatcherDictionary.TryGetValue(prop.Name, out inputDefinitionType));
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
                else if (isInputDefinitionTypeNotDefined)
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

                var testInput = (string.Equals(prop.PropertyType.Name, "Guid") ||
                    (IsPropertyNameAnId(definedIdParameterName, prop.Name) && isInputDefinitionTypeNotDefined)) ?
                        new List<object> { null } :
                        parameterDefinitions.FirstOrDefault(id => id.TypeName.Equals(inputDefinitionType))?.InputCatalogues;

                if (testInput == null)
                {
                    try
                    {
                        // We can't find anything that matches this type. We will try to generate the list of object of this type recursively.
                        var assemblyQualifiedName = prop.PropertyType.AssemblyQualifiedName;
                        var customTypePropertyMutator = new PropertiesMutator(null, prop.PropertyType, _objectDefinition, _parameterDefinitions);
                        testInput = customTypePropertyMutator.Mutate().ToList();
                    }
                    catch (Exception e)
                    {
                        throw new Exception($"Cannot Found Matching Definition for: { inputDefinitionType }, and have failed to generate permutations with error { e.Message }. Have you assign ObjectDefinition correctly?");
                    }
                }

                propertyPermutationLists.Add(testInput);
            }

            return propertyPermutationLists;
        }

        private List<object> _generateObjectList(
           string definedIdParameterName,
           Type classType,
           List<List<object>> attributePermutations)
        {
            var listOfProperties = classType.GetProperties();
            var results = new List<object>();
            var idCount = 0;
            foreach (var permutationSet in attributePermutations)
            {
                var constructor = classType.GetConstructor(Type.EmptyTypes);
                if (constructor == null)
                    throw new Exception($"No parameterless constructor in the class: { classType.Name }. Please add parameterless constructor.");
                var newObjectType = Activator.CreateInstance(classType);
                for (var i = 0; i < listOfProperties.Length; i++)
                {
                    var prop = listOfProperties[i];
                    var propValue = permutationSet[i];

                    // This is the rule for an ID or Guid type, but must not have any permutation set values.
                    // If it is an integer type, we will create a count and generate an int id
                    // if it is a guid, we will generate a new guid.
                    if ((IsPropertyNameAnId(definedIdParameterName, prop.Name) || prop.PropertyType.Name.Contains("Guid")) &&
                        propValue == null)
                    {
                        if (prop.PropertyType.Name.Contains("Int"))
                        {
                            idCount++;
                            prop.SetValue(newObjectType, idCount);
                        }
                        else if (prop.PropertyType.Name.Contains("Guid"))
                            prop.SetValue(newObjectType, Guid.NewGuid());
                    }
                    else
                    {
                        prop.SetValue(newObjectType, propValue);
                    }
                }
                results.Add(newObjectType);
            }

            return results;
        }

        private bool IsPropertyNameAnId(string idParameterName, string propertyName)
        {
            if (idParameterName != null && propertyName.Equals(idParameterName)) return true;
            if (string.Compare(propertyName, "id", true) == 0) return true;
            if (propertyName.ToLower().EndsWith("id")) return true;
            return false;
        }
    }
}