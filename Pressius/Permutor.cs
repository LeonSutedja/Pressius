using System.Collections.Generic;

namespace Pressius
{
    /// <summary>
    /// Pressius is an extensible object permutation
    /// For example, a class object that contains 2 attributes of a string and an integer
    /// will generate a list of that object with a default string and integer values.
    ///
    /// Example basic usage:
    /// var pressiusInputs = Permutor.Generate<T>().ToList();
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
    /// var pressiusInputs = Permutor.Generate<CreateRoomCommand>(
    ///     new CreateRoomCommandObjectDefinition(),
    ///     addedParameterDefinitions).ToList();
    ///
    /// </summary>
    public class Permutor
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

        public Permutor()
        {
            _mutatorFactory = new MutatorFactory();
        }

        public Permutor AddParameterDefinition(IParameterDefinition parameterDefinition)
        {
            _mutatorFactory.AddParameterDefinition(parameterDefinition);
            return this;
        }

        public Permutor AddObjectDefinition(IObjectDefinition objectDefinition)
        {
            _mutatorFactory.AddObjectDefinition(objectDefinition);
            return this;
        }

        public Permutor WithConstructor()
        {
            _mutatorFactory.WithConstructor();
            return this;
        }

        public IEnumerable<T> GeneratePermutation<T>()
        {
            return _mutatorFactory.GeneratePermutations<T>();
        }
    }
}