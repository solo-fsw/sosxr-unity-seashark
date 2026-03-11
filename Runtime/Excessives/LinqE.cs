using System;
using System.Collections.Generic;
using System.Linq;


namespace SOSXR.SeaShark.Excessives
{
    /// <summary>
    /// LINQ extension methods for enumerable operations including loops, combinations, permutations, and utilities.
    /// Provides additional functionality beyond standard LINQ for iteration, searching, and manipulation.
    /// </summary>
    public static class LinqE
    {
        #region Loops

        /// <summary>
        /// Iterates through each element in the enumerable and invokes an action on each element.
        /// Returns the original enumerable for method chaining.
        /// </summary>
        /// <typeparam name="TSource">The type of elements in the enumerable.</typeparam>
        /// <param name="enumerable">The enumerable to iterate through.</param>
        /// <param name="action">The action to invoke on each element.</param>
        /// <returns>The original enumerable.</returns>
        public static IEnumerable<TSource> ForEach<TSource>(
            this IEnumerable<TSource> enumerable,
            Action<TSource> action
        )
        {
            using (var enumerator = enumerable.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    action(enumerator.Current);
                }
            }

            return enumerable;
        }


        /// <summary>
        /// Iterates through each element in the enumerable with its index and invokes an action on each.
        /// Returns the original enumerable for method chaining.
        /// </summary>
        /// <typeparam name="TSource">The type of elements in the enumerable.</typeparam>
        /// <param name="enumerable">The enumerable to iterate through.</param>
        /// <param name="action">The action to invoke on each element and its index.</param>
        /// <returns>The original enumerable.</returns>
        public static IEnumerable<TSource> For<TSource>(
            this IEnumerable<TSource> enumerable,
            Action<TSource, int> action
        )
        {
            var i = 0;

            using (var enumerator = enumerable.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    action(enumerator.Current, i);
                    i++;
                }
            }

            return enumerable;
        }


        /// <summary>
        /// Invokes an action on all unique pairs of elements (combinations) in the enumerable.
        /// Each pair is processed only once (i, j) where i &lt; j.
        /// </summary>
        /// <typeparam name="TSource">The type of elements in the enumerable.</typeparam>
        /// <param name="enumerable">The enumerable to process.</param>
        /// <param name="action">The action to invoke on each pair of elements.</param>
        /// <returns>The original enumerable.</returns>
        public static IEnumerable<TSource> Combination<TSource>(
            this IEnumerable<TSource> enumerable,
            Action<TSource, TSource> action
        )
        {
            for (var i = 0; i < enumerable.Count(); i++)
            {
                for (var j = i + 1; j < enumerable.Count(); j++)
                {
                    action(enumerable.ElementAt(i), enumerable.ElementAt(j));
                }
            }

            return enumerable;
        }


        /// <summary>
        /// Invokes an action on all permutations of pairs in the enumerable.
        /// Each element is paired with every other element, including itself.
        /// </summary>
        /// <typeparam name="TSource">The type of elements in the enumerable.</typeparam>
        /// <param name="enumerable">The enumerable to process.</param>
        /// <param name="action">The action to invoke on each pair permutation.</param>
        /// <returns>The original enumerable.</returns>
        public static IEnumerable<TSource> Permuation<TSource>(
            this IEnumerable<TSource> enumerable,
            Action<TSource, TSource> action
        )
        {
            var enumerator1 = enumerable.GetEnumerator();
            var enumerator2 = enumerable.GetEnumerator();

            while (enumerator1.MoveNext())
            {
                while (enumerator2.MoveNext())
                {
                    action(enumerator1.Current, enumerator2.Current);
                }
            }

            return enumerable;
        }

        #endregion

        #region Loops Backward

        /// <summary>
        /// Iterates through the enumerable in reverse order and invokes an action on each element.
        /// Returns the original enumerable for method chaining.
        /// </summary>
        /// <typeparam name="TSource">The type of elements in the enumerable.</typeparam>
        /// <param name="enumerable">The enumerable to iterate through in reverse.</param>
        /// <param name="action">The action to invoke on each element.</param>
        /// <returns>The original enumerable.</returns>
        public static IEnumerable<TSource> ForEachBack<TSource>(
            this IEnumerable<TSource> enumerable,
            Action<TSource> action
        )
        {
            for (var i = enumerable.Count() - 1; i >= 0; i--)
            {
                action(enumerable.ElementAt(i));
            }

            return enumerable;
        }


        /// <summary>
        /// Iterates through the enumerable in reverse order with its index and invokes an action on each.
        /// Returns the original enumerable for method chaining.
        /// </summary>
        /// <typeparam name="TSource">The type of elements in the enumerable.</typeparam>
        /// <param name="enumerable">The enumerable to iterate through in reverse.</param>
        /// <param name="action">The action to invoke on each element and its index.</param>
        /// <returns>The original enumerable.</returns>
        public static IEnumerable<TSource> ForBack<TSource>(
            this IEnumerable<TSource> enumerable,
            Action<TSource, int> action
        )
        {
            for (var i = enumerable.Count() - 1; i >= 0; i--)
            {
                action(enumerable.ElementAt(i), i);
            }

            return enumerable;
        }

        #endregion

        #region Get Sub Array

        /// <summary>
        /// Creates a sub-array from the enumerable starting at the specified index with the specified length.
        /// </summary>
        /// <typeparam name="TSource">The type of elements in the enumerable.</typeparam>
        /// <param name="enumerable">The enumerable to extract from.</param>
        /// <param name="startIndex">The starting index of the sub-array.</param>
        /// <param name="length">The length of the sub-array.</param>
        /// <returns>A new enumerable containing the sub-array elements.</returns>
        public static IEnumerable<TSource> SubArray<TSource>(
            this IEnumerable<TSource> enumerable,
            int startIndex, int length
        )
        {
            var final = new TSource[length];

            Array.Copy(enumerable.ToArray(), startIndex, final, 0, length);

            return final.AsEnumerable();
        }


        /// <summary>
        /// Creates a sub-array by stepping through the enumerable with wrapping.
        /// Starts at startIndex and steps by stepsize for the specified number of cycles, wrapping around if necessary.
        /// </summary>
        /// <typeparam name="TSource">The type of elements in the enumerable.</typeparam>
        /// <param name="enumerable">The enumerable to extract from.</param>
        /// <param name="startIndex">The starting index.</param>
        /// <param name="cycles">The number of elements to extract.</param>
        /// <param name="stepsize">The step size between elements (default: 1).</param>
        /// <returns>A new enumerable containing the stepped elements with wrapping.</returns>
        public static IEnumerable<TSource> SubArraySmart<TSource>(
            this IEnumerable<TSource> enumerable,
            int startIndex, int cycles, int stepsize = 1
        )
        {
            var final = new TSource[cycles];

            var currentIndex = startIndex;

            for (var i = 0; i < cycles; i++)
            {
                currentIndex =
                    MathE.ClampWrap(
                        startIndex + i * stepsize,
                        0,
                        enumerable.Count()
                    );

                final[i] = enumerable.ElementAt(currentIndex);
            }

            return final.AsEnumerable();
        }

        #endregion

        #region Min/Max

        /// <summary>
        /// Finds the element with the minimum value according to the provided selector function.
        /// </summary>
        /// <typeparam name="TSource">The type of elements in the enumerable.</typeparam>
        /// <param name="enumerable">The enumerable to search.</param>
        /// <param name="selector">A function to extract the comparable value from each element.</param>
        /// <returns>The element with the minimum value, or default if enumerable is empty.</returns>
        public static TSource Minimum<TSource>(
            this IEnumerable<TSource> enumerable,
            Func<TSource, IComparable> selector
        )
        {
            var minimum = default(TSource);

            var assignedMinimum = false; //Used so we don't have to use a 'default' value

            enumerable.ForEach(n =>
                {
                    if (!assignedMinimum || selector(n).CompareTo(selector(minimum)) < 0)
                    {
                        minimum = n;
                        assignedMinimum = true;
                    }
                }
            );

            return minimum;
        }


        /// <summary>
        /// Finds the element with the maximum value according to the provided selector function.
        /// </summary>
        /// <typeparam name="TSource">The type of elements in the enumerable.</typeparam>
        /// <param name="enumerable">The enumerable to search.</param>
        /// <param name="selector">A function to extract the comparable value from each element.</param>
        /// <returns>The element with the maximum value, or default if enumerable is empty.</returns>
        public static TSource Maximum<TSource>(
            this IEnumerable<TSource> enumerable,
            Func<TSource, IComparable> selector
        )
        {
            var maximum = default(TSource);

            var assignedMaximum = false; //Used so we don't have to use a 'default' value

            enumerable.ForEach(n =>
                {
                    if (!assignedMaximum || selector(n).CompareTo(selector(maximum)) > 0)
                    {
                        maximum = n;
                        assignedMaximum = true;
                    }
                }
            );

            return maximum;
        }

        #endregion

        #region Misc

        /// <summary>
        /// Returns the element at the specified index (nth element) in the enumerable.
        /// </summary>
        /// <typeparam name="TSource">The type of elements in the enumerable.</typeparam>
        /// <param name="enumerable">The enumerable to search.</param>
        /// <param name="n">The zero-based index of the element to return.</param>
        /// <returns>The element at index n, or default if index is out of range.</returns>
        public static TSource Nth<TSource>(
            this IEnumerable<TSource> enumerable,
            int n
        )
        {
            using (var enumerator = enumerable.GetEnumerator())
            {
                var i = 0;

                while (enumerator.MoveNext())
                {
                    if (i == n)
                    {
                        return enumerator.Current;
                    }

                    i++;
                }

                return default;
            }
        }


        /// <summary>
        /// Finds the index of the specified element in the enumerable.
        /// </summary>
        /// <typeparam name="TSource">The type of elements in the enumerable.</typeparam>
        /// <param name="enumerable">The enumerable to search.</param>
        /// <param name="instance">The element to find.</param>
        /// <returns>The zero-based index of the element, or -1 if not found.</returns>
        public static int FindIndex<TSource>(
            this IEnumerable<TSource> enumerable,
            TSource instance
        )
        {
            var i = 0;
            var enumerator = enumerable.GetEnumerator();

            while (enumerator.MoveNext())
            {
                if (enumerator.Current.Equals(instance))
                {
                    return i;
                }

                i++;
            }

            return -1; //Could not find it in the array
        }


        /// <summary>
        /// Swaps the elements at the specified indices in the enumerable.
        /// </summary>
        /// <typeparam name="TSource">The type of elements in the enumerable.</typeparam>
        /// <param name="enumerable">The enumerable to modify.</param>
        /// <param name="index1">The index of the first element to swap.</param>
        /// <param name="index2">The index of the second element to swap.</param>
        /// <returns>A new enumerable with the elements swapped.</returns>
        /// <exception cref="IndexOutOfRangeException">Thrown if either index is out of range.</exception>
        public static IEnumerable<TSource> Swap<TSource>(
            this IEnumerable<TSource> enumerable,
            int index1, int index2
        )
        {
            // Optimized swap that avoids ToArray() conversion and uses the index-based access pattern
            var list = enumerable.ToList();
            if (index1 < 0 || index1 >= list.Count || index2 < 0 || index2 >= list.Count)
            {
                throw new IndexOutOfRangeException("Swap indices are out of range");
            }
            
            TSource tmp = list[index1];
            list[index1] = list[index2];
            list[index2] = tmp;
            
            return list.AsEnumerable();
        }


        /// <summary>
        /// Sets the element at the specified index to the provided value.
        /// </summary>
        /// <typeparam name="TSource">The type of elements in the enumerable.</typeparam>
        /// <param name="enumerable">The enumerable to modify.</param>
        /// <param name="value">The new value to set.</param>
        /// <param name="index">The index of the element to set.</param>
        /// <returns>A new enumerable with the element at the specified index replaced.</returns>
        /// <exception cref="Exception">Thrown if index is out of range.</exception>
        public static IEnumerable<TSource> SetAt<TSource>(
            this IEnumerable<TSource> enumerable,
            TSource value, int index
        )
        {
            if (index >= enumerable.Count())
            {
                throw new Exception("IndexOutOfRangeException"); //{TODO} Finish this
            }

            var array = enumerable.ToArray();

            array[index] = value;

            return array.AsEnumerable();
        }

        #endregion

        #region Random

        /// <summary>
        /// Randomly selects and returns a single element from the enumerable.
        /// </summary>
        /// <typeparam name="TSource">The type of elements in the enumerable.</typeparam>
        /// <param name="enumerable">The enumerable to pick from.</param>
        /// <returns>A randomly selected element from the enumerable.</returns>
        public static TSource Pick<TSource>(
            this IEnumerable<TSource> enumerable
        )
        {
            return CryptoRand.Pick(enumerable.ToArray());
        }


        /// <summary>
        /// Randomly shuffles the elements in the enumerable using the Fisher-Yates algorithm.
        /// </summary>
        /// <typeparam name="TSource">The type of elements in the enumerable.</typeparam>
        /// <param name="enumerable">The enumerable to shuffle.</param>
        /// <returns>A new enumerable with elements in random order.</returns>
        public static IEnumerable<TSource> Shuffle<TSource>(
            this IEnumerable<TSource> enumerable
        )
        {
            //TSource[] newArray = new TSource[enumerable.Count()];

            for (var i = 0; i < enumerable.Count(); i++)
            {
                enumerable = enumerable.Swap(i, (int) CryptoRand.Range(0.0, enumerable.Count()));
            }

            //for (int i = 0; i < newArray.Length; i++)
            //	newArray[i] = enumerable.Where(n => !newArray.Contains(n)).Pick(); //Kinda slow?
            return enumerable;
        }

        #endregion
    }
}