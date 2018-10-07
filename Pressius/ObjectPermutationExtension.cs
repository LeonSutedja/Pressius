using System.Collections.Generic;
using System.Linq;

namespace Pressius
{
    public static class ObjectPermutationExtension
    {
        public static List<List<object>> GenerateMinimalPermutations(this List<List<object>> propertyPermutationLists)
        {
            var results = new List<List<object>>();
            var firstOfEveryList = propertyPermutationLists.Select(ppl => ppl[0]).ToList();
            for (var i = 0; i < propertyPermutationLists.Count; i++)
            {
                var permutationList = propertyPermutationLists[i];
                for (var j = 0; j < permutationList.Count; j++)
                {
                    if (i > 0 && j == 0) continue;

                    var value = permutationList[j];
                    var newObjectList = new List<object>();
                    for (var x = 0; x < propertyPermutationLists.Count; x++)
                    {
                        var valueToBeAdded = (x == i) ? value : firstOfEveryList[i];
                        newObjectList.Add(valueToBeAdded);
                    }
                    results.Add(newObjectList);
                }
            }

            return results.Distinct().ToList();
        }

        public static List<List<object>> GenerateMinimalPermutationsTakeTwo(
            this List<List<object>> propertyPermutationLists)
        {
            var results = new List<List<object>>();
            var firstOfEveryList = propertyPermutationLists.Select(ppl => ppl[0]).ToList();
            results.Add(firstOfEveryList);
            for (var i = 0; i < propertyPermutationLists.Count; i++)
            {
                var permutationList = propertyPermutationLists[i];
                foreach (var item in permutationList)
                {
                    var indexFromToGet = i + 1;
                    var theRestPropertyPermutationList = propertyPermutationLists.GetRange(
                        indexFromToGet, 
                        propertyPermutationLists.Count - indexFromToGet);
                    foreach (var nextList in theRestPropertyPermutationList)
                    {

                    }
                }
            }

            return results.Distinct().ToList();
        }

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