/*
 * From [anchan828](https://github.com/anchan828/property-drawer-collection)

   Copyright (C) 2014 Keigo Ando

   Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

   The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using UnityEngine;
#if UNITY_EDITOR
#endif


namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Animator Paramater attribute.
    /// </summary>
    public class AnimatorParameterAttribute : PropertyAttribute
    {
        /// <summary>
        ///     型のタイプ。型指定をしたい時に使用する
        /// </summary>
        public enum ParameterType
        {
            Float = 1,
            Int = 3,
            Bool = 4,
            Trigger = 9,
            None = 9999
        }


        /// <summary>
        ///     パラメータの型。デフォルトでは型を考慮しない
        /// </summary>
        public readonly ParameterType parameterType = ParameterType.None;

        /// <summary>
        ///     現在選択中のindex
        /// </summary>
        public int selectedValue = 0;


        /// <summary>
        ///     Initializes a new instance of the <see cref="AnimatorParameterAttribute" /> class.
        /// </summary>
        public AnimatorParameterAttribute() : this(ParameterType.None)
        {
        }


        /// <summary>
        ///     Initializes a new instance of the <see cref="AnimatorParameterAttribute" /> class.
        /// </summary>
        /// <param name='ParamaterType'>
        ///     型を指定して選択肢たい場合に設定する
        /// </param>
        public AnimatorParameterAttribute(ParameterType parameterType)
        {
            this.parameterType = parameterType;
        }
    }
}