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
    ///     Marks a field for observation/monitoring by triggering specified callbacks.
    /// </summary>
    public class ObserveAttribute : PropertyAttribute
    {
        /// <summary>Name(s) of the callback methods to invoke for observation.</summary>
        public readonly string[] callbackNames;


        /// <summary>
        ///     Creates a new ObserveAttribute with the given callback names.
        /// </summary>
        /// <param name="callbackNames">Callback method names to observe.</param>
        public ObserveAttribute(params string[] callbackNames)
        {
            this.callbackNames = callbackNames;
        }
    }
}
