using Pressius.ParameterDefinition;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pressius
{
    internal partial class MutatorFactory
    {
        private readonly List<IParameterDefinition> _parameterDefinitions;
        private readonly List<IObjectDefinition> _objectDefinitions;
        private bool _useConstructor;
        private string _idName;

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

        public void WithId(string idParamName) => _idName = idParamName;

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
            var mutator = new ConstructorMutator(classType, objectDefinition, _parameterDefinitions, _idName);
            return mutator.Mutate();
        }

        private IEnumerable<object> _generatePropertiesPermutation(
            Type classType,
            IObjectDefinition objectDefinition = null)
        {
            var mutator = new PropertiesMutator(_idName, classType, objectDefinition, _parameterDefinitions);
            return mutator.Mutate();
        }
    }
}