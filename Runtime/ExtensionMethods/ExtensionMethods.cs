using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;


namespace SOSXR.SeaShark
{
    public static class ExtensionMethods
    {
        /// <summary>
        ///     How far & in what direction do I need to go?
        ///     For each axis in 'axisToUse' that is set to 0, the displacement will also be 0.
        /// </summary>
        /// <returns></returns>
        public static Vector3 CalculateDisplacement(this Transform originTrans, Transform targetTrans, Vector3 axisToUse)
        {
            return CalculateDisplacement(originTrans.position, targetTrans.position, axisToUse);
        }


        /// <summary>
        ///     How far & in what direction do I need to go?
        ///     For each axis in 'axisToUse' that is set to 0, the displacement will also be 0.
        /// </summary>
        /// <returns></returns>
        public static Vector3 CalculateDisplacement(this Transform originTrans, Vector3 targetPos, Vector3 axisToUse)
        {
            return CalculateDisplacement(originTrans.position, targetPos, axisToUse);
        }


        /// <summary>
        ///     How far & in what direction do I need to go?
        ///     For each axis in 'axisToUe' that is set to 0, the displacement will also be 0.
        /// </summary>
        /// <returns></returns>
        public static Vector3 CalculateDisplacement(this Vector3 originPos, Vector3 targetPos, Vector3 axisToUse)
        {
            var displacement = targetPos - originPos;

            if (axisToUse.x == 0)
            {
                displacement.x = 0;
            }

            if (axisToUse.y == 0)
            {
                displacement.y = 0;
            }

            if (axisToUse.z == 0)
            {
                displacement.z = 0;
            }

            return displacement;
        }


        /// <summary>
        ///     Creates Vector with max 1
        /// </summary>
        /// <param name="displacement"></param>
        /// <returns></returns>
        public static Vector3 CalculateDirection(this Vector3 displacement)
        {
            return displacement.normalized;
        }


        /// <summary>
        ///     Calculates how far away the target is.
        ///     Magnitude is the long side (C) of the triangle: A^2+B^2 = C^2
        /// </summary>
        /// <param name="displacement"></param>
        /// <returns></returns>
        public static float CalculateDistance(this Vector3 displacement)
        {
            return displacement.magnitude;
        }


        /// <summary>
        ///     Calculates how far away the target is from the origin
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static float CalculateDistance(this Transform origin, Transform target)
        {
            return origin.CalculateDisplacement(target, Vector3.one).CalculateDistance();
        }


        /// <summary>
        ///     Calculates whether I am looking at the target.
        ///     Defaults to -1 to 1, with 1 looking directly at, -1 looking directly away.
        ///     Can remap to a 0-1 value, 0 being looking directly away, 1 being looking directly at.
        /// </summary>
        /// <param name="transformOne"></param>
        /// <param name="lookAtTarget"></param>
        /// <param name="remap"></param>
        /// <returns></returns>
        public static float CalculateLookAtDotProduct(this Transform transformOne, Transform lookAtTarget, bool remap = false)
        {
            var displacement = transformOne.CalculateDisplacement(lookAtTarget.position, Vector3.one);
            var direction = displacement.CalculateDirection();
            var dotProduct = Vector3.Dot(direction, transformOne.forward.normalized); // -1 to 1, with 1 looking directly at, -1 looking directly away

            if (remap)
            {
                var remapped = dotProduct.RemapValue(new Vector2(-1, 1), new Vector2(0, 1));

                return remapped;
            }

            return dotProduct;
        }


        /// <summary>
        ///     Returns 1 for Right, -1 for Left, 0 for neither.
        /// </summary>
        /// <param name="transformOne"></param>
        /// <param name="lookAtTarget"></param>
        /// <returns></returns>
        public static int LeftRight(this Transform transformOne, Transform lookAtTarget)
        {
            var dot = transformOne.CalculateLookAtDotProduct(lookAtTarget);

            float upDotProduct = 0;

            if (dot != 0)
            {
                var displacement = transformOne.CalculateDisplacement(lookAtTarget.position, Vector3.one);
                var direction = displacement.CalculateDirection();
                var perpendicular = Vector3.Cross(transformOne.forward, direction);
                upDotProduct = Vector3.Dot(perpendicular, transformOne.up.normalized);
            }

            if (upDotProduct > 0)
            {
                return 1; // Right
            }

            if (upDotProduct < 0)
            {
                return -1; // Left
            }

            return 0;
        }


        /// <summary>
        ///     Returns 1 for Up, -1 for Down, 0 for neither.
        /// </summary>
        /// <param name="transformOne"></param>
        /// <param name="lookAtTarget"></param>
        /// <returns></returns>
        public static int UpDown(this Transform transformOne, Transform lookAtTarget)
        {
            var dot = transformOne.CalculateLookAtDotProduct(lookAtTarget);

            float upDotProduct = 0;

            if (dot != 0)
            {
                var displacement = transformOne.CalculateDisplacement(lookAtTarget.position, Vector3.one);
                var direction = displacement.CalculateDirection();
                upDotProduct = Vector3.Dot(direction, transformOne.up.normalized);
            }

            if (upDotProduct > 0)
            {
                return 1; // Up
            }

            if (upDotProduct < 0)
            {
                return -1; // Down
            }

            return 0;
        }


        /// <summary>
        ///     Returns the difference in height between the two transforms
        ///     transformTwo - transformOne
        /// </summary>
        /// <param name="transformOne"></param>
        /// <param name="transformTwo"></param>
        /// <returns></returns>
        public static float CompareHeight(this Transform transformOne, Transform transformTwo)
        {
            return transformTwo.position.y - transformOne.position.y;
        }


        /// <summary>
        ///     Sets a transform to a new world position
        /// </summary>
        /// <param name="thisGameObject"></param>
        /// <param name="newPosition"></param>
        public static void MoveToNewPosition(this GameObject thisGameObject, Transform newPosition)
        {
            MoveToNewPosition(thisGameObject.transform, newPosition.position);
        }


        /// <summary>
        ///     Sets a transform to a new world position
        /// </summary>
        /// <param name="thisGameObject"></param>
        /// <param name="newPosition"></param>
        public static void MoveToNewPosition(this GameObject thisGameObject, Vector3 newPosition)
        {
            MoveToNewPosition(thisGameObject.transform, newPosition);
        }


        /// <summary>
        ///     Sets a transform to a new world position
        /// </summary>
        /// <param name="thisTransform"></param>
        /// <param name="newPosition"></param>
        public static void MoveToNewPosition(this Transform thisTransform, Transform newPosition)
        {
            MoveToNewPosition(thisTransform, newPosition.position);
        }


        /// <summary>
        ///     Sets a transform to a new world position
        /// </summary>
        /// <param name="thisTransform"></param>
        /// <param name="newPosition"></param>
        public static void MoveToNewPosition(this Transform thisTransform, Vector3 newPosition)
        {
            thisTransform.position = newPosition;
        }


        /// <summary>
        ///     Lerps a transform to a new world position
        /// </summary>
        /// <param name="thisTransform"></param>
        /// <param name="toTransform"></param>
        /// <param name="speed"></param>
        public static void LerpToNewPosition(this Transform thisTransform, Transform toTransform, float speed)
        {
            LerpToNewPosition(thisTransform, toTransform.position, speed);
        }


        /// <summary>
        ///     Lerps a transform to a new world position
        /// </summary>
        /// <param name="thisTransform"></param>
        /// <param name="newPosition"></param>
        /// <param name="speed"></param>
        public static void LerpToNewPosition(this Transform thisTransform, Vector3 newPosition, float speed)
        {
            thisTransform.position = Vector3.Lerp(thisTransform.position, newPosition, Time.deltaTime * speed);
        }


        /// <summary>
        ///     Sets a transform to a new world position
        /// </summary>
        /// <param name="thisTransform"></param>
        /// <param name="newPosition"></param>
        /// <param name="basedOn"></param>
        public static void MovePositionTo(this Transform thisTransform, Transform newPosition, Transform basedOn)
        {
            var distanceDiff = newPosition.position - basedOn.position;

            thisTransform.position += distanceDiff;
        }


        /// <summary>
        ///     Adjusts the height by the given float value
        ///     Can be positive or negative
        /// </summary>
        /// <param name="thisPosition"></param>
        /// <param name="heightAdjust"></param>
        public static Vector3 AdjustHeightBy(this Vector3 thisPosition, float heightAdjust)
        {
            var offset = new Vector3(0, heightAdjust, 0);

            thisPosition += offset;

            return thisPosition;
        }


        /// <summary>
        ///     Sorts GameObjects by name
        /// </summary>
        /// <param name="gameObjects"></param>
        public static void SortByName(this List<GameObject> gameObjects)
        {
            gameObjects.Sort((x, y) =>
                string.Compare(x?.name, y?.name, StringComparison.OrdinalIgnoreCase));
        }


        /// <summary>
        ///     Sorts GameObjects by name
        /// </summary>
        /// <param name="gameObjects"></param>
        public static void SortByName(this GameObject[] gameObjects)
        {
            Array.Sort(gameObjects, (x, y) =>
                string.Compare(x?.name, y?.name, StringComparison.OrdinalIgnoreCase));
        }


        /// <summary>
        ///     Sorts Transforms by name
        /// </summary>
        /// <param name="transforms"></param>
        public static void SortByName(this List<Transform> transforms)
        {
            transforms.Sort((x, y) =>
                string.Compare(x?.name, y?.name, StringComparison.OrdinalIgnoreCase));
        }


        /// <summary>
        ///     Sorts Transforms by name
        /// </summary>
        /// <param name="transforms"></param>
        public static void SortByName(this Transform[] transforms)
        {
            Array.Sort(transforms, (x, y) =>
                string.Compare(x?.name, y?.name, StringComparison.OrdinalIgnoreCase));
        }


        /// <summary>
        ///     Draws debug ray from Transform in the Vector3 direction displacement
        /// </summary>
        /// <param name="originTrans"></param>
        /// <param name="displacement"></param>
        public static void DrawRay(this Transform originTrans, Vector3 displacement)
        {
            Debug.DrawRay(originTrans.position, displacement);
        }


        /// <summary>
        ///     Checks if given string has text, and whether that text is more than just whitespace
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool ContainsText(this string str)
        {
            return !string.IsNullOrEmpty(str) && !string.IsNullOrWhiteSpace(str);
        }


        /// <summary>
        ///     Calculate the standard deviation of an array
        ///     Adapted from: https://stackoverflow.com/questions/5336457/how-to-calculate-a-standard-deviation-array
        /// </summary>
        /// <param name="floatArray"></param>
        /// <returns></returns>
        public static float CalculateSD(this float[] floatArray)
        {
            var average = floatArray.Average();
            var sumOfSquaresOfDifferences = floatArray.Select(val => (val - average) * (val - average)).Sum();

            return Mathf.Sqrt(sumOfSquaresOfDifferences / floatArray.Length);
        }


        /// <summary>
        ///     Fisher Yates shuffle. From : https://answers.unity.com/questions/16531/randomizing-arrays.html
        /// </summary>
        /// <param name="arr">Arr.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static void Shuffle<T>(this T[] arr)
        {
            for (var i = arr.Length - 1; i > 0; i--)
            {
                var r = Random.Range(0, i + 1);
                (arr[i], arr[r]) = (arr[r], arr[i]);
            }
        }


        /// <summary>
        ///     Fisher Yates shuffle on List. From
        ///     https://forum.unity.com/threads/clever-way-to-shuffle-a-list-t-in-one-line-of-c-code.241052/
        /// </summary>
        /// <param name="ts"></param>
        /// <typeparam name="T"></typeparam>
        public static void Shuffle<T>(this IList<T> ts)
        {
            var count = ts.Count;
            var last = count - 1;

            for (var i = 0; i < last; ++i)
            {
                var r = Random.Range(i, count);
                (ts[i], ts[r]) = (ts[r], ts[i]);
            }
        }


        /// <summary>
        ///     Just get the children, without the parent transform.
        ///     Adapted from https://answers.unity.com/questions/496958/how-can-i-get-only-the-childrens-of-a-gameonbject.html¬
        /// </summary>
        /// <returns>The first children.</returns>
        /// <param name="parent">Parent.</param>
        public static Transform[] GetChildrenWithoutParent(this Transform parent)
        {
            var children = parent.GetComponentsInChildren<Transform>();
            var firstChildren = new Transform[parent.childCount];
            var index = 0;

            foreach (var child in children)
            {
                if (child.parent != parent)
                {
                    continue;
                }

                firstChildren[index] = child;
                index++;
            }

            return firstChildren;
        }


        /// <summary>
        ///     Find by name the transform of (sub)child in a given parent, recursively.
        ///     Adapted from: https://forum.unity.com/threads/solved-find-a-child-by-name-searching-all-subchildren.40684/
        /// </summary>
        public static Transform FindChildByName(this Transform transform, string name)
        {
            if (transform.name.Equals(name))
            {
                return transform;
            }

            foreach (Transform child in transform)
            {
                var result = FindChildByName(child, name);

                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }


        /// <summary>
        ///     Find the transform of (sub)child in a given parent by the start of a name, recursively.
        ///     Adapted from: https://forum.unity.com/threads/solved-find-a-child-by-name-searching-all-subchildren.40684/
        /// </summary>
        public static Transform FindChildByNameStart(this Transform transform, string nameStart)
        {
            if (transform.name.StartsWith(nameStart))
            {
                return transform;
            }

            foreach (Transform child in transform)
            {
                var result = FindChildByNameStart(child, nameStart);

                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }


        /// <summary>
        ///     Find the transform of (sub)child in a given parent by the end of a name, recursively.
        ///     Adapted from: https://forum.unity.com/threads/solved-find-a-child-by-name-searching-all-subchildren.40684/
        /// </summary>
        public static Transform FindChildByNameEnd(this Transform transform, string nameEnd)
        {
            if (transform.name.EndsWith(nameEnd))
            {
                return transform;
            }

            foreach (Transform child in transform)
            {
                var result = FindChildByNameEnd(child, nameEnd);

                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }


        /// <summary>
        ///     Find the transform of (sub)child in a given parent containing part of a name, recursively.
        ///     Adapted from: https://forum.unity.com/threads/solved-find-a-child-by-name-searching-all-subchildren.40684/
        /// </summary>
        public static Transform FindChildByNameContains(this Transform transform, string nameContains)
        {
            if (transform.name.Contains(nameContains))
            {
                return transform;
            }

            foreach (Transform child in transform)
            {
                var result = FindChildByNameContains(child, nameContains);

                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }


        /// <summary>
        ///     Function to search through all child Transforms and return matching Transforms
        ///     By ChatGPT
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="nameEndsWith"></param>
        /// <returns></returns>
        public static Transform[] FindChildrenByNameEnd(this Transform parent, string nameEndsWith)
        {
            var matchingTransforms = new List<Transform>();

            foreach (Transform child in parent)
            {
                if (child.name.EndsWith(nameEndsWith))
                {
                    matchingTransforms.Add(child);
                }

                // Recursive call to search through child's children
                var childMatches = FindChildrenByNameEnd(child, nameEndsWith);
                matchingTransforms.AddRange(childMatches);
            }

            return matchingTransforms.ToArray();
        }


        /// <summary>
        ///     Function to search through all child Transforms and return matching Transforms
        ///     By ChatGPT
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="nameContains"></param>
        /// <returns></returns>
        public static List<Transform> FindChildrenByNameContains(this Transform parent, string nameContains)
        {
            var matchingTransforms = new List<Transform>();

            foreach (Transform child in parent)
            {
                if (child.name.Contains(nameContains))
                {
                    matchingTransforms.Add(child);
                }

                // Recursive call to search through child's children
                var childMatches = FindChildrenByNameContains(child, nameContains);
                matchingTransforms.AddRange(childMatches);
            }

            return matchingTransforms;
        }


        /// <summary>
        ///     Find by tag the transform of (sub)child in a given parent, recursively.
        ///     Adapted from: https://forum.unity.com/threads/solved-find-a-child-by-name-searching-all-subchildren.40684/
        /// </summary>
        public static Transform FindChildByTag(this Transform transform, string tag)
        {
            if (transform.tag.Equals(tag))
            {
                return transform;
            }

            foreach (Transform child in transform)
            {
                var result = FindChildByTag(child, tag);

                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }


        /// <summary>
        ///     Find by tag the transform of the first found ancestor, recursively.
        ///     Adapted from: https://forum.unity.com/threads/solved-find-a-child-by-name-searching-all-subchildren.40684/
        /// </summary>
        public static Transform FindAncestorByTag(this Transform transform, string tag)
        {
            return FindAncestorByTag(transform.gameObject, tag).transform;
        }


        /// <summary>
        ///     Find by tag the GameObject of the first found ancestor, recursively.
        ///     Adapted from: https://forum.unity.com/threads/solved-find-a-child-by-name-searching-all-subchildren.40684/
        /// </summary>
        public static GameObject FindAncestorByTag(this GameObject gameObject, string tag)
        {
            if (gameObject.transform.parent == null)
            {
                return null;
            }

            if (gameObject.transform.parent.tag.Equals(tag))
            {
                return gameObject;
            }

            foreach (GameObject parent in gameObject.transform)
            {
                var result = FindAncestorByTag(parent, tag);

                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }


        /// <summary>
        ///     Cleans up an array and takes out any null items
        ///     From ChatGPT
        /// </summary>
        /// <param name="transformsArray"></param>
        /// <returns></returns>
        public static Transform[] CleanupCollection(this IEnumerable<Transform> transformsArray)
        {
            return CleanupCollection(transformsArray.ToList()).ToArray();
        }


        /// <summary>
        ///     Cleans up an array and takes out any null items
        ///     From ChatGPT
        /// </summary>
        /// <param name="transformsArray"></param>
        /// <returns></returns>
        public static List<Transform> CleanupCollection(this List<Transform> transformsArray)
        {
            return transformsArray.Where(transform => transform != null && transform.gameObject.activeSelf).ToList();
        }


        /// <summary>
        ///     Puts the X and Y axis of the Vector3 into a new Vector2
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Vector2 ToVector2XY(this Vector3 input)
        {
            return new Vector2(input.x, input.y);
        }


        /// <summary>
        ///     Clamps the float to a value between minMax.x and minMax.y
        /// </summary>
        /// <param name="input"></param>
        /// <param name="minMax"></param>
        /// <returns></returns>
        public static float Clamp(this float input, Vector2 minMax)
        {
            return Mathf.Clamp(input, minMax.x, minMax.y);
        }


        /// <summary>
        ///     Clamps the input.x and input.y to a value between minMax.x and minMax.y
        /// </summary>
        /// <param name="input"></param>
        /// <param name="minMax"></param>
        /// <returns></returns>
        public static Vector2 Clamp(this Vector2 input, Vector2 minMax)
        {
            return new Vector2(input.x.Clamp(minMax), input.y.Clamp(minMax));
        }


        /// <summary>
        ///     Clamps the input.x, the input.y, and the input.z to a value between minMax.x and minMax.y
        /// </summary>
        /// <param name="input"></param>
        /// <param name="minMax"></param>
        /// <returns></returns>
        public static Vector3 Clamp(this Vector3 input, Vector2 minMax)
        {
            return new Vector3(input.x.Clamp(minMax), input.y.Clamp(minMax), input.z.Clamp(minMax));
        }


        /// <summary>
        ///     Clamps the float to a value between 0 and 1
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static float Clamped01(this float input)
        {
            return Mathf.Clamp01(input);
        }


        /// <summary>
        ///     Clamps the Vector2 to a value between 0 and 1
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Vector2 Clamped01(this Vector2 input)
        {
            return new Vector2(Clamped01(input.x), Clamped01(input.y));
        }


        /// <summary>
        ///     Clamps the Vector3 to a value between 0 and 1
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Vector3 Clamped01(this Vector3 input)
        {
            return new Vector3(Clamped01(input.x), Clamped01(input.y), Clamped01(input.z));
        }


        /// <summary>
        ///     If > 0.5 then 1, else 0
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static float ReverseClamped01(this float input)
        {
            if (input > 0.5f)
            {
                input = 1;
            }
            else
            {
                input = 0;
            }

            return input;
        }


        /// <summary>
        ///     If > 0.5 then 1, else 0
        ///     For both x and y-axis
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Vector2 ReverseClamped01(this Vector2 input)
        {
            return new Vector2(ReverseClamped01(input.x), ReverseClamped01(input.y));
        }


        /// <summary>
        ///     If > 0.5 then 1, else 0
        ///     For all axis
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Vector3 ReverseClamped01(this Vector3 input)
        {
            return new Vector3(ReverseClamped01(input.x), ReverseClamped01(input.y), ReverseClamped01(input.z));
        }


        /// <summary>
        ///     Returns a new Vector3 with Y axis set to 0
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Vector3 Flatten(this Vector3 input)
        {
            return new Vector3(input.x, 0, input.z);
        }


        /// <summary>
        ///     Returns the Transform but with the Y axis set to 0
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Transform Flatten(this Transform input)
        {
            input.position = input.position.Flatten();

            return input;
        }


        /// <summary>
        ///     For each of the Axis of the 'axisToUse' Vector3, if it's set to 0, the corresponding input axis will also be set to
        ///     0
        /// </summary>
        /// <param name="original"></param>
        /// <param name="axisToUse"></param>
        public static Vector3 ZeroOutVector3(this Vector3 original, Vector3Int axisToUse)
        {
            return ZeroOutVector3(original, (Vector3) axisToUse);
        }


        /// <summary>
        ///     For each of the Axis of the 'axisToUse' Vector3, if it's set to 0, the corresponding input axis will also be set to
        ///     0
        /// </summary>
        /// <param name="original"></param>
        /// <param name="axisToUse"></param>
        public static Vector3 ZeroOutVector3(this Vector3 original, Vector3 axisToUse)
        {
            var newVector = original;

            if (axisToUse.x == 0)
            {
                newVector.x = 0;
            }

            if (axisToUse.y == 0)
            {
                newVector.y = 0;
            }

            if (axisToUse.z == 0)
            {
                newVector.z = 0;
            }

            return newVector;
        }


        /// <summary>
        ///     Sets local scale uniformly. Cannot be smaller than 0.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="scale"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void SetLocalScale(this Transform transform, float scale)
        {
            if (scale < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(scale));
            }

            transform.localScale = new Vector3(scale, scale, scale);
        }


        /// <summary>
        ///     Set the Transform's Position and Rotation to 0,0,0. Set the LocalScale to 1,1,1
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        public static Transform ZeroOutTransform(this Transform original)
        {
            original.position = Vector3.zero;
            original.rotation = Quaternion.identity;
            original.localScale = Vector3.one;

            return original;
        }


        /// <summary>
        ///     Set the Transform's LocalPosition and LocalRotation to 0,0,0. Set the LocalScale to 1,1,1
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        public static Transform LocalZeroOutTransform(this Transform original)
        {
            original.localPosition = Vector3.zero;
            original.localRotation = Quaternion.identity;
            original.localScale = Vector3.one;

            return original;
        }


        /// <summary>
        ///     Casts normal Vector3 to the closest Vector3Int
        /// </summary>
        /// <param name="v3"></param>
        /// <returns></returns>
        public static Vector3Int ToVector3Int(this Vector3 v3)
        {
            return new Vector3Int((int) v3.x, (int) v3.y, (int) v3.y);
        }


        /// <summary>
        ///     Example: a value can range from 0 to 1. If for example 0.5 is the 'center' / 'high point', then 0.75 will be as far
        ///     away from that center as 0.25 is.
        ///     This recalculates that initial value to reflect this, and it linearly scales the value to be between 0 and 1 again.
        ///     Mentioned values in this example are not required to be used, any value can be used.
        /// </summary>
        /// <param name="rangedValue"></param>
        /// <param name="highPoint"></param>
        /// <returns></returns>
        public static float NormalizeFromHighPoint(this float rangedValue, float highPoint)
        {
            if (rangedValue > highPoint)
            {
                rangedValue = highPoint - (rangedValue - highPoint);
            }

            return rangedValue;
        }


        /// <summary>
        ///     Example: a value can range from 0 to 1. If for example 0.5 is the 'center' / 'high point', then 0.75 will be as far
        ///     away from that center as 0.25 is.
        ///     This recalculates that initial value to reflect this, and it linearly scales the value to be between 0 and 1 again.
        ///     Mentioned values in this example are not required to be used, any value can be used.
        /// </summary>
        /// <param name="rangedValues"></param>
        /// <param name="highPoint"></param>
        /// <returns></returns>
        public static Vector2 NormalizeFromHighPoint(this Vector2 rangedValues, float highPoint)
        {
            if (rangedValues.x > highPoint)
            {
                rangedValues.x = NormalizeFromHighPoint(rangedValues.x, highPoint);
            }

            if (rangedValues.y > highPoint)
            {
                rangedValues.y = NormalizeFromHighPoint(rangedValues.y, highPoint);
            }

            return rangedValues;
        }


        /// <summary>
        ///     Gets minimum value of Vector2
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static float GetMin(this Vector2 input)
        {
            return Mathf.Min(input.x, input.y);
        }


        /// <summary>
        ///     Gets minimum value of Vector3
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static float GetMin(this Vector3 input)
        {
            return Mathf.Min(input.x, input.y, input.z);
        }


        /// <summary>
        ///     Gets maximum value of Vector2
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static float GetMax(this Vector2 input)
        {
            return Mathf.Max(input.x, input.y);
        }


        /// <summary>
        ///     Gets maximum value of Vector3
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static float GetMax(this Vector3 input)
        {
            return Mathf.Max(input.x, input.y, input.z);
        }


        /// <summary>
        ///     Runs through children of given Transform, and destroys all;
        /// </summary>
        /// <param name="t"></param>
        public static void DestroyChildren(this Transform t)
        {
            foreach (Transform child in t)
            {
                Object.Destroy(child.gameObject);
            }
        }


        /// <summary>
        ///     Runs through children of given Transform, and destroys all immediately
        /// </summary>
        /// <param name="t"></param>
        public static void DestroyChildrenImmediately(this Transform t)
        {
            foreach (Transform child in t)
            {
                Object.DestroyImmediate(child.gameObject);
            }
        }


        /// <summary>
        ///     Sets the layer of the given GameObject and all of its children to the given layer.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="layer"></param>
        public static void SetLayersRecursively(this GameObject gameObject, int layer)
        {
            gameObject.layer = layer;

            foreach (Transform t in gameObject.transform)
            {
                t.gameObject.SetLayersRecursively(layer);
            }
        }


        /// <summary>
        ///     Sets Alpha of SpriteRenderer to given alpha.
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="alpha"></param>
        public static void FadeAlpha(this SpriteRenderer renderer, float alpha)
        {
            var color = renderer.color;
            color.a = alpha;
            renderer.color = color;
        }


        /// <summary>
        ///     Sets Alpha of material on given Renderer to given alpha.
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="alpha"></param>
        public static void FadeAlpha(this Renderer renderer, float alpha)
        {
            var material = renderer.material;
            var color = material.color;
            color.a = alpha;
            material.color = color;
        }


        /// <summary>
        ///     Picks a random item from this list
        /// </summary>
        /// <param name="list"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetRandomItemFromList<T>(this IList<T> list)
        {
            return list[Random.Range(0, list.Count)];
        }


        /// <summary>
        ///     Splits a string into a string array, with each character separated by a space.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string SplitString(this string input)
        {
            return string.Join(" ", input.ToCharArray());
        }


        /// <summary>
        ///     Extracts the file name from a full path, and removes the given character and everything before it.
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="removeBeforeCharacter"></param>
        /// <param name="removeExtension"></param>
        /// <returns></returns>
        public static string ExtractedFileNameFromPath(this string fullPath, string removeBeforeCharacter = "/", string removeExtension = ".unity")
        {
            var lastIndex = fullPath.LastIndexOf(removeBeforeCharacter, StringComparison.Ordinal);
            var removeIncluding = lastIndex + 1;
            var shortened = fullPath.Remove(0, removeIncluding);
            var withoutExtension = shortened.Replace(removeExtension, "");

            return withoutExtension;
        }


        /// <summary>
        ///     Usage example:
        ///     var gameObject = new GameObject;
        ///     gameObject.DrawCircle(1); or gameObject.DrawCircle(1, 0.5f, true);
        ///     Adapted from: https://www.loekvandenouweland.com/content/use-linerenderer-in-unity-to-draw-a-circle.html
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="radius"></param>
        /// <param name="lineWidth"></param>
        /// <param name="useWorldSpace"></param>
        public static void DrawCircle(this GameObject gameObject, float radius = 1f, float lineWidth = 0.025f, bool useWorldSpace = false)
        {
            const int segments = 360;
            const int closedSegments = segments + 1; // add extra point to make start point and endpoint the same to close the circle
            var line = gameObject.AddComponent<LineRenderer>();

            line.useWorldSpace = useWorldSpace;
            line.startWidth = lineWidth;
            line.endWidth = lineWidth;
            line.positionCount = closedSegments;

            var points = new Vector3[closedSegments];

            for (var i = 0; i < closedSegments; i++)
            {
                var rad = Mathf.Deg2Rad * (i * 360f / segments);
                points[i] = new Vector3(Mathf.Sin(rad) * radius, 0, Mathf.Cos(rad) * radius);
            }

            line.SetPositions(points);
        }


        /// <summary>
        ///     This rounds a float to specified decimals: otherwise issues to round .5 values (e.g. 3.5 to 4, or 6.555 to 6.56)
        /// </summary>
        /// <param name="originalValue"></param>
        /// <param name="decimals"></param>
        /// <returns></returns>
        public static float RoundCorrectly(this float originalValue, int decimals)
        {
            // Standard Rounding gave problems when rounding values like 0.5 / 1.12125 / 2.45 etc. to 1 / 1.1213 / 2.5 respectively
            // Value needs to be cast to decimal, and MidpointRounding needs to be set to AwayFromZero to fix that issue:
            var originalAsDecimal = (decimal) originalValue; // Float needs to be cast to decimal. Does not work properly when using double.
            const MidpointRounding midwayRounding = MidpointRounding.AwayFromZero; // As per: https://stackoverflow.com/questions/37290845/incorrect-result-of-math-round-function-in-vb-net  /// And: https://docs.microsoft.com/en-us/dotnet/api/system.midpointrounding?redirectedfrom=MSDN&view=net-6.0

            var rounded = (float) Math.Round(originalAsDecimal, decimals, midwayRounding);

            return rounded;
        }


        /// <summary>
        ///     This rounds a double to specified decimals: otherwise issues to round .5 values (e.g. 3.5 to 4, or 6.555 to 6.56)
        /// </summary>
        /// <param name="originalValue"></param>
        /// <param name="decimals"></param>
        /// <returns></returns>
        public static double RoundCorrectly(this double originalValue, int decimals)
        {
            return RoundCorrectly((float) originalValue, decimals);
        }


        /// <summary>
        ///     This rounds a Vector2 to specified decimals: otherwise issues to round .5 values (e.g. 3.5 to 4, or 6.555 to 6.56)
        /// </summary>
        /// <param name="originalValue"></param>
        /// <param name="decimals"></param>
        /// <returns></returns>
        public static Vector2 RoundCorrectly(this Vector2 originalValue, int decimals)
        {
            var rounded = new Vector3
            {
                x = RoundCorrectly(originalValue.x, decimals),
                y = RoundCorrectly(originalValue.y, decimals)
            };

            return rounded;
        }


        /// <summary>
        ///     This rounds a Vector3 to specified decimals: otherwise issues to round .5 values (e.g. 3.5 to 4, or 6.555 to 6.56)
        /// </summary>
        /// <param name="originalValue"></param>
        /// <param name="decimals"></param>
        /// <returns></returns>
        public static Vector3 RoundCorrectly(this Vector3 originalValue, int decimals)
        {
            var rounded = new Vector3
            {
                x = RoundCorrectly(originalValue.x, decimals),
                y = RoundCorrectly(originalValue.y, decimals),
                z = RoundCorrectly(originalValue.z, decimals)
            };

            return rounded;
        }


        /// <summary>
        ///     Remaps float from old range to new range.
        ///     Based on https://forum.unity.com/threads/re-map-a-number-from-one-range-to-another.119437/
        /// </summary>
        /// <param name="from"></param>
        /// <param name="oldRange"></param>
        /// <param name="newRange"></param>
        /// <returns></returns>
        public static float RemapValue(this float from, Vector2 oldRange, Vector2 newRange)
        {
            var fromAbs = from - oldRange.x;
            var fromMaxAbs = oldRange.y - oldRange.x;

            var normal = fromAbs / fromMaxAbs;

            var toMaxAbs = newRange.y - newRange.x;
            var toAbs = toMaxAbs * normal;

            var to = toAbs + newRange.x;

            return to;
        }


        /// <summary>
        ///     Returns all UnityActions from a UnityEvent that are assigned to it in the Inspector
        /// </summary>
        /// <param name="unityEvent"></param>
        /// <returns></returns>
        public static IEnumerable<UnityAction> GetPersistentCallersFromUnityEvent(this UnityEvent unityEvent)
        {
            var persistentCallers = new List<UnityAction>();

            for (var i = 0; i < unityEvent.GetPersistentEventCount(); i++)
            {
                var target = unityEvent.GetPersistentTarget(i);
                var targetType = target.GetType();
                var methodName = unityEvent.GetPersistentMethodName(i);
                var methodInfo = targetType.GetMethod(methodName);

                if (methodInfo != null)
                {
                    var methodParameters = methodInfo.GetParameters();
                    UnityAction method = null;

                    if (methodParameters.Length == 0)
                    {
                        method = (UnityAction) methodInfo.CreateDelegate(typeof(UnityAction), target);
                    }
                    else if (methodParameters.Length > 0)
                    {
                        Debug.LogWarning("GetPersistentCallersFromUnityEvent" + "Cannot handle multiple parameters. Make sure to add" + methodName + "from" + target.name + "manually to the new UnityEvent");

                        continue;
                    }

                    if (method == null)
                    {
                        Debug.LogWarning("GetPersistentCallersFromUnityEvent" + "Could not create delegate for" + methodName + "on" + target + "with Type" + targetType);

                        continue;
                    }


                    Debug.Log("GetPersistentCallersFromUnityEvent" + "Found" + methodName + "for" + target + "with Type" + targetType);

                    persistentCallers.Add(method);
                }
            }

            return persistentCallers;
        }


        /// <summary>
        ///     Returns true if this is the first child Transform of its parent.
        /// </summary>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static bool IsFirstChild(this Transform trans)
        {
            var siblingIndex = trans.GetSiblingIndex();

            return siblingIndex == 0;
        }


        /// <summary>
        ///     Returns true if this is the last child Transform of its parent.
        /// </summary>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static bool IsLastChild(this Transform trans)
        {
            var siblingIndex = trans.GetSiblingIndex();
            var maxIndex = trans.parent.childCount - 1;

            return siblingIndex == maxIndex;
        }


        /// <summary>
        ///     Safely find a GameObject in the scene
        /// </summary>
        /// <param name="monoBehaviour"></param>
        /// <param name="foundObject"></param>
        /// <typeparam name="T"></typeparam>
        public static void SafeFindObjectOfType<T>(this MonoBehaviour monoBehaviour, out T foundObject) where T : MonoBehaviour
        {
            foundObject = Object.FindFirstObjectByType<T>();

            if (foundObject == null)
            {
                Debug.LogError("SafeFindObjectOfType" + "Could not find" + typeof(T) + "in the scene");
            }
        }


        /// <summary>
        ///     Get or add a component to a GameObject
        ///     Gets if exists
        ///     Adds if not
        /// </summary>
        /// <param name="go"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            if (go.GetComponent<T>() == null)
            {
                go.AddComponent<T>();
            }

            return go.GetComponent<T>();
        }


        /// <summary>
        ///     Recreates a temporary AudioSource when the original is buy.
        /// </summary>
        /// <param name="original"></param>
        /// <param name="clip"></param>
        /// <returns></returns>
        public static void PlayEnsured(this AudioSource original, AudioClip clip)
        {
            if (!original.isPlaying)
            {
                original.clip = clip;
                original.Play();

                return;
            }

            var tempSource = original.gameObject.AddComponent<AudioSource>();

            tempSource.spatialize = original.spatialize;
            tempSource.spatialBlend = original.spatialBlend;
            tempSource.minDistance = original.minDistance;
            tempSource.maxDistance = original.maxDistance;
            tempSource.pitch = original.pitch;
            tempSource.spread = original.spread;
            tempSource.panStereo = original.panStereo;
            tempSource.priority = original.priority;
            tempSource.volume = original.volume;
            tempSource.outputAudioMixerGroup = original.outputAudioMixerGroup;
            tempSource.bypassEffects = original.bypassEffects;
            tempSource.bypassReverbZones = original.bypassReverbZones;
            tempSource.bypassListenerEffects = original.bypassListenerEffects;
            tempSource.reverbZoneMix = original.reverbZoneMix;
            tempSource.dopplerLevel = original.dopplerLevel;
            tempSource.playOnAwake = original.playOnAwake;
            tempSource.loop = original.loop;

            tempSource.rolloffMode = original.rolloffMode;

            if (tempSource.rolloffMode == AudioRolloffMode.Custom)
            {
                tempSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, original.GetCustomCurve(AudioSourceCurveType.CustomRolloff));
            }

            tempSource.clip = clip;
            tempSource.Play();

            var destroyDelay = clip.length + 1f; // +1 sec for safety.
            Object.Destroy(tempSource, destroyDelay);
        }


        /// <summary>
        ///     Checks to see whether a list contains an index. For this the list needs to be not null.
        ///     Returns false if null or if provided index is less than 0 or more than the Count-1
        /// </summary>
        /// <param name="list"></param>
        /// <param name="index"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool ContainsIndex<T>(this List<T> list, int index)
        {
            return list != null && index >= 0 && index <= list.Count - 1;
        }


        /// <summary>
        ///     Checks to see whether an array contains an index. For this the array needs to be not null.
        ///     Returns false if null or if provided index is less than 0 or more than the Length-1
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool ContainsIndex<T>(this T[] array, int index)
        {
            return array != null && index >= 0 && index <= array.Length - 1;
        }


        /// <summary>
        ///     Get Random float from Vector2 range
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public static float GetRandomFromRange(this Vector2 range)
        {
            return Random.Range(range.x, range.y);
        }


        /// <summary>
        ///     Gets last digit of a number
        /// </summary>
        public static int GetLastDigit(this int number)
        {
            var positiveNumber = Math.Abs(number);

            return positiveNumber % 10;
        }


        /// <summary>
        ///     Divides the dividend by the divisor and returns the remainder.
        ///     E.g. 10.GetRemainder(3) returns 1
        ///     E.g. 10.GetRemainder(0) returns 0
        ///     E.g. 5.GetRemainder(6) returns 5
        /// </summary>
        /// <param name="dividend"></param>
        /// <param name="divisor"></param>
        /// <returns></returns>
        public static int GetRemainder(this int dividend, int divisor)
        {
            if (divisor == 0)
            {
                Debug.LogError("GetRemainder " + " Cannot divide by 0");

                return 0;
            }

            if (divisor > dividend)
            {
                Debug.Log("GetRemainder " + " A larger divisor will always return the dividend");

                return dividend;
            }

            return dividend % divisor;
        }


        /// <summary>
        ///     Add item to list if unique: if it is not already in list, please add.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="item"></param>
        /// <typeparam name="T"></typeparam>
        public static void AddIfUnique<T>(this List<T> list, T item)
        {
            if (list == null)
            {
                return;
            }

            if (item == null)
            {
                list.Add(default);

                return;
            }

            if (!list.Contains(item))
            {
                list.Add(item);
            }
        }


        /// <summary>
        ///     Add items to list if unique: if they are not already in list, please add.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="items"></param>
        /// <typeparam name="T"></typeparam>
        public static void AddRangeUnique<T>(this List<T> list, IEnumerable<T> items)
        {
            if (list == null)
            {
                return;
            }

            if (items == null)
            {
                list.AddIfUnique(default);

                return;
            }

            foreach (var item in items)
            {
                list.AddIfUnique(item);
            }
        }


        /// <summary>
        ///     Remove item from list if it exists in list.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="item"></param>
        /// <typeparam name="T"></typeparam>
        public static void RemoveSafe<T>(this List<T> list, T item)
        {
            if (list == null)
            {
                return;
            }

            if (item == null)
            {
                return;
            }

            if (list.Contains(item))
            {
                list.Remove(item);
            }
        }


        /// <summary>
        ///     Remove items from list if they exist in list.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="items"></param>
        /// <typeparam name="T"></typeparam>
        public static void RemoveRangeSafe<T>(this List<T> list, IEnumerable<T> items)
        {
            if (list == null)
            {
                return;
            }

            if (items == null)
            {
                return;
            }

            foreach (var item in items)
            {
                list.Remove(item);
            }
        }
    }
}