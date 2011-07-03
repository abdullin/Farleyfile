using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FarleyFile
{
    public static class RedirectToWhen
    {
        static class Cache<T>
        {
            public static readonly IDictionary<Type, MethodInfo> Dict = typeof(T)
                .GetMethods()
                .Where(m => m.Name == "When")
                .Where(m => m.GetParameters().Length == 1)
                .ToDictionary(m => m.GetParameters().First().ParameterType, m => m);
        }
        public static void InvokeEventOptional<T>(T instance, IEvent command)
        {
            MethodInfo info;
            var type = command.GetType();
            if (!Cache<T>.Dict.TryGetValue(type, out info))
            {
                // we don't care if state does not consume events
                // they are persisted anyway
                return;
            }
            info.Invoke(instance, new[] { command });

        }

        public static void InvokeCommand<T>(T instance, ICommand command)
        {
            MethodInfo info;
            var type = command.GetType();
            if (!Cache<T>.Dict.TryGetValue(type, out info))
            {
                var s = string.Format("Failed to locate {0}.When({1})", typeof(T).Name, type.Name);
                throw new InvalidOperationException(s);
            }
            info.Invoke(instance, new[] { command });
        }
    }
}