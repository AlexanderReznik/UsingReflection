using System;

namespace MyCustomAttributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RedirectMethodCallAttribute : Attribute
    {
        public string RedirectMethodName { get; }

        public RedirectMethodCallAttribute(string redirectMethodName)
        {
            RedirectMethodName = redirectMethodName;
        }
    }
}
