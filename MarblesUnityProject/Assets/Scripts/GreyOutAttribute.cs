using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class GreyOutAttribute : PropertyAttribute
{
    // This class can be empty, as it's just a marker attribute
}
