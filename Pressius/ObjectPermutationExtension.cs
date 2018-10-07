using System.Collections.Generic;
using System.Linq;

namespace Pressius
{
    internal static class ObjectPermutationExtension
    {
        public static void GeneratePermutations(this List<List<object>> permutationLists, List<List<object>> results)
        {
            _generateMinimalPermutations(permutationLists, results);
        }

        private static void _generateMinimalPermutations(
            List<List<object>> permutationLists,
            List<List<object>> results,
            List<object> currentPropertyValues = null,
            int propertyIndex = 0)
        {
            if (propertyIndex == permutationLists.Count)
            {
                results.Add(currentPropertyValues); 
                return;
            }

            var propertyLists = permutationLists[propertyIndex];

            for (var i = 0; i < propertyLists.Count; i++)
            {
                var propertyToBeAded = propertyLists[i];

                var newPropertyValues = new List<object>();
                currentPropertyValues = (currentPropertyValues == null || propertyIndex == 0)
                    ? new List<object>()
                    : currentPropertyValues;
                newPropertyValues.AddRange(currentPropertyValues);
               
                if (results.Any())
                {
                    var allValuesInCurrentPropertyIndex = results.Select(r => r[propertyIndex]).ToList();
                    if (allValuesInCurrentPropertyIndex.Contains(propertyToBeAded))
                    {
                        propertyToBeAded = propertyLists.First();
                        currentPropertyValues.Add(propertyToBeAded);
                        _generateMinimalPermutations(permutationLists, results, currentPropertyValues, propertyIndex + 1);
                        break;
                    }
                    currentPropertyValues.Add(propertyToBeAded);
                    _generateMinimalPermutations(permutationLists, results, currentPropertyValues, propertyIndex + 1);
                }
                else
                {
                    currentPropertyValues.Add(propertyToBeAded);
                    _generateMinimalPermutations(permutationLists, results, currentPropertyValues, propertyIndex + 1);
                }

                currentPropertyValues = newPropertyValues;
            }
        }
    }
}