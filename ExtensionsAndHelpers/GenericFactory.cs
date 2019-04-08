using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace ExtensionsAndHelpers
{
    public static class Factory<TKey, TBaseType>
        where TBaseType: class
    {
        private delegate TBaseType BaseTypeInvoker();
        private static Dictionary<TKey, BaseTypeInvoker> _delegates = new Dictionary<TKey, BaseTypeInvoker>();
        private static object _locker = new object();

        private static BaseTypeInvoker CreateInvoker(Type type)
        {
            ConstructorInfo constructor = type.GetConstructor(new Type[0]);
            if(constructor == null)
            {
                throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "{0} doesn't have a public default constructor.", type.FullName));
            }

            DynamicMethod method = new DynamicMethod(type.Name, type, new Type[0], typeof(TBaseType));

            ILGenerator il = method.GetILGenerator();
            il.Emit(OpCodes.Newobj, constructor);
            il.Emit(OpCodes.Ret);

            return (BaseTypeInvoker)method.CreateDelegate(typeof(BaseTypeInvoker));
        }
        public static void Add(TKey key, Type type)
        {
            // Some checks on the type argument
            if (type == null)
                throw new ArgumentNullException("type");

            // Check if object is not a class
            if (type.IsClass == false)
                throw new ArgumentException(
                String.Format(CultureInfo.InvariantCulture,
                "{0} is not a reference type.",
                type.FullName), "type");

            // Check if object is abstract
            if (type.IsAbstract == true)
                throw new ArgumentException(
                String.Format(CultureInfo.InvariantCulture,
                "{0} is an abstract class, which can not be created.",
                type.FullName), "type");

            // Check whether the given type is assignable from
            if (typeof(TBaseType).IsAssignableFrom(type) == false)
                throw new ArgumentException(String.Format(
                "The given type {0} should be derivable from {1}.",
                type.FullName, typeof(TBaseType).FullName), "type");

            lock (_locker)
            {
                if (_delegates.ContainsKey(key) == false)
                {
                    try
                    {
                        BaseTypeInvoker invoke = CreateInvoker(type);
                        invoke();
                        _delegates.Add(key, invoke);
                    }
                    catch (InvalidCastException)
                    {
                        throw new InvalidCastException(
                            String.Format(CultureInfo.InvariantCulture,
                            "{0} couldn't be casted to {1}.",
                            type.FullName, typeof(TBaseType).FullName)
                        );
                    }
                }
                else
                {
                    _delegates[key]();
                }
            }
        }

        public static TBaseType CreateInstance(TKey key)
        {
            BaseTypeInvoker invoke = null;
            lock (_locker)
            {
                _delegates.TryGetValue(key, out invoke);
            }
            return invoke?.Invoke() ?? null;
        }
    }

    // http://joelabrahamsson.com/a-neat-little-type-inference-trick-with-c/

    public static class Wrapper
    {
        public static Wrapper<T> Create<T>(T wrapped)
        {
            return new Wrapper<T>(wrapped);
        }

        public static Wrapper<T> Wrap<T>(this T wrapped)
        {
            return Create(wrapped);
        }
    }

    public class Wrapper<T>
    {
        readonly Dictionary<string, MethodBase> _getters;
        readonly Dictionary<string, MethodBase> _setters;

        public Wrapper(T wrapped)
        {
            Wrapped = wrapped;

            //_getters = new Dictionary<string, MethodBase>();

            _getters = typeof(T).GetProperties().ToDictionary(p=> p.Name, p => MethodBase.GetMethodFromHandle(p.GetMethod.MethodHandle));
            _setters = typeof(T).GetProperties().ToDictionary(p => p.Name, p => MethodBase.GetMethodFromHandle(p.SetMethod.MethodHandle));

            // var delegateType = typeof(Func<,>).MakeGenericType(typeof(T), p.GetMethod.ReturnType);
            //Func<T, object> getter;
            //if (!getters.TryGetValue(name, out getter))
            //{
            //    getter = (Func<T, object>)Delegate.CreateDelegate(
            //        typeof(Func<T, object>),
            //        typeof(T).GetProperty(name).GetMethod
            //    );
            //    getters.Add(name, getter);
            //}

            //T o;
            //value = getter(o);
        }

        public object this[string index]
        {
            get => _getters.ContainsKey(index) ? _getters[index].Invoke(Wrapped, null) : null;//{ /* return the specified index here */ }
            set
            {
                if (_setters.ContainsKey(index)) _setters[index].Invoke(Wrapped, new[] { value });
                else throw new ArgumentException();
            }
        }
        public T Wrapped { get; set; }
    }


}
