using System;

namespace MyCustomAttributes
{
    // +TODO Implement new attribute that should be applied only on class methods parameters.
    [AttributeUsage(AttributeTargets.Parameter)]
    public class NotNullOrEmptyAttribute : Attribute
    {
    }
}
