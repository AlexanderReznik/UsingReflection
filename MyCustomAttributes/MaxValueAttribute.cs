using System;

namespace MyCustomAttributes
{
    // +TODO Implement new attribute that should be applied only on class methods parameters.
    [AttributeUsage(AttributeTargets.Parameter)]
    public class MaxValueAttribute : Attribute
    {
        public MaxValueAttribute(int value)
        {
            MaxValue = value;
        }
        public int MaxValue { get; }
    }
}
