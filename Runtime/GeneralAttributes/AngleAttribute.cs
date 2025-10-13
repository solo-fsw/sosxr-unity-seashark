/*
 * From [anchan828](https://github.com/anchan828/property-drawer-collection)

   Copyright (C) 2014 Keigo Ando

   Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

   The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using UnityEngine;

namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Angle attribute.
    /// </summary>
    public class AngleAttribute : PropertyAttribute
    {
        public readonly float min = -1;
        public readonly float max = -1;
        public readonly string unit = string.Empty;
        public Color backgroundColor = Color.gray, activeColor = Color.red;
        public readonly bool showValue = true;
        public float knobSize = 32;


        public AngleAttribute()
        {
        }


        public AngleAttribute(float min, float max)
        {
            this.min = min;
            this.max = max;
        }


        public AngleAttribute(float min, float max, string unit)
            : this(min, max)
        {
            this.unit = unit;
        }


        public AngleAttribute(float min, float max, string unit, Color backgroundColor)
            : this(min, max, unit)
        {
            this.backgroundColor = backgroundColor;
        }


        public AngleAttribute(float min, float max, string unit, Color backgroundColor, Color activeColor)
            : this(min, max, unit, backgroundColor)
        {
            this.activeColor = activeColor;
        }


        public AngleAttribute(float min, float max, string unit, Color backgroundColor, Color activeColor, bool showValue)
            : this(min, max, unit, backgroundColor, activeColor)
        {
            this.showValue = showValue;
        }


        public AngleAttribute(float min, float max, string unit, Color backgroundColor, Color activeColor, bool showValue, float knobSize)
            : this(min, max, unit, backgroundColor, activeColor, showValue)
        {
            this.knobSize = knobSize;
        }
    }
}