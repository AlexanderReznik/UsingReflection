using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using MyCustomAttributes;

namespace CustomAttributes
{
    [DebuggerDisplay("{DebuggerDisplay}")]
    public class MyProxyService : IMyService
    {
        private readonly Type _interfaceType;
        private readonly Type _serviceType;
        private readonly object _service;

        public MyProxyService(object service)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            _service = service;
            _interfaceType = typeof(IMyService);
            _serviceType = service.GetType();
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [DebuggerHidden]
        public string MyProperty
        {
            get
            {
                // TODO Add new implementation here:
                // If the DefaultValue attribute is applied to the property, return the attribute value; if the DefaultValue attribute is not applied, return default type value.

                var bindingFlags = BindingFlags.Instance | BindingFlags.Public;

                PropertyInfo interfacePropertyInfo = _interfaceType.GetProperty(nameof(IMyService.MyProperty), typeof(string));

                bool hasDefaultValueAttribute = interfacePropertyInfo.GetCustomAttributes<DefaultValueAttribute>().Any();

                if (hasDefaultValueAttribute)
                {
                    return (string)interfacePropertyInfo.GetCustomAttributes<DefaultValueAttribute>().FirstOrDefault().Value;
                }
                else
                {
                    return default(string);
                }
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [DebuggerHidden]
        public int AnotherProperty
        {
            get
            {
                // TODO Add new implementation here:
                // If the DefaultValue attribute is applied to the property, return the attribute value; if the DefaultValue attribute is not applied, return default type value.

                var bindingFlags = BindingFlags.Instance | BindingFlags.Public;

                PropertyInfo interfacePropertyInfo = _interfaceType.GetProperty(nameof(IMyService.AnotherProperty), typeof(int));

                bool hasDefaultValueAttribute = interfacePropertyInfo.GetCustomAttributes<DefaultValueAttribute>().Any();

                if (hasDefaultValueAttribute)
                {
                    return int.Parse((string)interfacePropertyInfo.GetCustomAttributes<DefaultValueAttribute>().FirstOrDefault().Value);
                }
                else
                {
                    return default(int);
                }
            }
        }

        [DebuggerHidden]
        public int DoSomething(string stringParameter, int parameter)
        {
            var parameterTypes = new[] { typeof(string), typeof(int) };
            var bindingFlags = BindingFlags.Instance | BindingFlags.Public;

            // +TODO Get method from _interfaceType with required name, types and binding flags.
        
            MethodInfo interfaceMethodInfo = _interfaceType.GetMethod(nameof(IMyService.DoSomething), bindingFlags, null, parameterTypes, null );

            // +TODO Get attributes for interface method and set hasLogMethodCallAttribute to true if LogMethodCall attribute exists in the attribute collection.
            bool hasLogMethodCallAttribute = interfaceMethodInfo.GetCustomAttributes<LogMethodCallAttribute>().Any();

            // +TODO Get a required method from _serviceType with required name, types and binding flags.
            MethodInfo serviceMethodInfo = _serviceType.GetMethod(nameof(MyProxyService.DoSomething), bindingFlags, null, parameterTypes, null);

            if (hasLogMethodCallAttribute)
            {
                var sb = new StringBuilder();
                sb.Append($"Enter {_serviceType.Name}.{serviceMethodInfo.Name}(");

                // TODO Add parameter values to the builder to print them to console.
                sb.Append(stringParameter);
                sb.Append(", ");
                sb.Append(parameter);

                sb.Append(")");
                Trace.TraceInformation(sb.ToString());
            }

            // +TODO Implement validation logic for method parameters, and verfy strings for NotNullOrEmpty and integers for MaxValue.
            var paramsInfo = interfaceMethodInfo.GetParameters();
            bool hasNullOrEmptyAttribute = paramsInfo[0].GetCustomAttributes<NotNullOrEmptyAttribute>().Any();
            bool hasMaxValueAttribute = paramsInfo[1].GetCustomAttributes<MaxValueAttribute>().Any();
         
            if (String.IsNullOrEmpty(stringParameter) && hasNullOrEmptyAttribute)
            {
                throw new ArgumentException("{nameof(stringParameter)} is null.");
            }

            if (hasMaxValueAttribute)
            {
                if (parameter > paramsInfo[1].GetCustomAttributes<MaxValueAttribute>().FirstOrDefault().MaxValue)
                {
                    throw new ArgumentException($"{nameof(parameter)} is more than max value.");
                }
            }

            try
            {
                // +TODO Invoke the service method and set the returned result.
                int result = (int)serviceMethodInfo.Invoke(_service, new object[] { stringParameter, parameter });

                if (hasLogMethodCallAttribute)
                {
                    Trace.TraceInformation($"Leave {_serviceType.Name}.{serviceMethodInfo.Name}() = {result}");
                }

                return result;
            }
            catch (Exception e)
            {
                Trace.TraceError($"Exception {_serviceType.Name}.{serviceMethodInfo.Name}() => {e.Message}");
                throw;
            }
        }

        [DebuggerHidden]
        public int DoSomethingElse(int x, int y)
        {
            // TODO Add new implementation here to handle RedirectMethodCall attribute.
            // If the attribute is applied to a method, redirect to a method with specified name; if the attribute is not applied, don't redirect and call the method with DoSomethingElse name.

            var parameterTypes = new[] { typeof(int), typeof(int) };
            var bindingFlags = BindingFlags.Instance | BindingFlags.Public;

            MethodInfo interfaceMethodInfo = _interfaceType.GetMethod(nameof(IMyService.DoSomethingElse), bindingFlags, null, parameterTypes, null);

            bool hasRedirectMethodCallAttribute = interfaceMethodInfo.GetCustomAttributes<RedirectMethodCallAttribute>().Any();

            string methodName;
            MethodInfo serviceMethodInfo;
            if (hasRedirectMethodCallAttribute)
            {
                methodName = interfaceMethodInfo.GetCustomAttributes<RedirectMethodCallAttribute>().FirstOrDefault().RedirectMethodName;
                serviceMethodInfo = _service.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance, null, parameterTypes, null);
            }
            else
            {
                methodName = nameof(MyProxyService.DoSomethingElse);
                serviceMethodInfo = _serviceType.GetMethod(methodName, bindingFlags, null, parameterTypes, null);
            }
            
            int result =  (int)serviceMethodInfo.Invoke(_service, new object[] {x, y});
            return result;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => $"Proxy for {_serviceType.FullName}";
    }
}
