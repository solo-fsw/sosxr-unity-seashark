using System;
using UnityEngine;


[AttributeUsage(AttributeTargets.Field)]
public class PostfixAttribute : PropertyAttribute
{
    public readonly string Postfix;


    public PostfixAttribute(string postfix)
    {
        Postfix = postfix;
    }
}