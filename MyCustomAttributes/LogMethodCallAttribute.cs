using System;

namespace MyCustomAttributes
{
    // +TODO Implement new attribute that should be applied only on class methods.
    [AttributeUsage(AttributeTargets.Method)]
    public class LogMethodCallAttribute : Attribute
    {
        
    }
}
