using System;
using System.ComponentModel;
using System.Reflection;
using System.Reflection.Emit;

namespace ExtensionsAndHelpers
{
    public class DynamicProxyGenerator
    {
        private static TypeBuilder typeBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(""), AssemblyBuilderAccess.RunAndCollect).DefineDynamicModule("").DefineType("");

        MethodInfo DelegateCombine = typeof(Delegate).GetMethod("Combine", new Type[] { typeof(Delegate), typeof(Delegate) });
        MethodInfo DelegateRemove = typeof(Delegate).GetMethod("Remove", new Type[] { typeof(Delegate), typeof(Delegate) });
        MethodInfo InvokeDelegate = typeof(PropertyChangedEventHandler).GetMethod("Invoke");
        FieldBuilder eventBack = typeBuilder.DefineField("PropertyChanged", typeof(PropertyChangingEventHandler), FieldAttributes.Private);
        ConstructorInfo CreateEventArgs = typeof(PropertyChangingEventArgs).GetConstructor(new Type[] { typeof(String) });
        MethodBuilder raiseEvent;

        private void Init(string typeName, string assemblyName = "DynamicAssembly", 
            string moduleName = "DynamicModule")
        {
            TypeBuilder typeBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.RunAndCollect).DefineDynamicModule("").DefineType("");
            AssemblyName mAsmName = new AssemblyName();
            mAsmName.Name = "DynamicAssembly.dll";
            AppDomain currentDomain = System.AppDomain.CurrentDomain;

            // mAssemblyBuilder = currentDomain.DefineDynamicAssembly(mAsmName, AssemblyBuilderAccess.RunAndSave);
            // mModuleBuilder = mAssemblyBuilder.DefineDynamicModule(mAsmName.Name, mAsmName.Name, false);
        }

        public Type CreateType<TType>(String typeName, Type parent)
        {
            Init(typeName);
            typeBuilder.AddInterfaceImplementation(typeof(INotifyPropertyChanged));

            MethodBuilder raiseEvent = GenerateILForINotifyPropertyChanged();
            SubclassVirtualProperties(typeof(TType), raiseEvent);

            Type genType = typeBuilder.CreateType();
            return genType;
        }


        private void SubclassVirtualProperties(Type parent, MethodBuilder raiseEvent)
        {
            foreach (PropertyInfo pinfo in parent.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (pinfo.GetSetMethod().IsVirtual)
                {
                    PropertyBuilder pb = typeBuilder.DefineProperty(
                        pinfo.Name, PropertyAttributes.None, pinfo.PropertyType, Type.EmptyTypes);
                    MethodAttributes getSetAttr =
                        MethodAttributes.Public | MethodAttributes.SpecialName |
                        MethodAttributes.HideBySig | MethodAttributes.Virtual;
                    MethodBuilder getMethod =
                        typeBuilder.DefineMethod(
                            "get_" + pinfo.Name, getSetAttr, pinfo.PropertyType, Type.EmptyTypes);
                    ILGenerator gen = getMethod.GetILGenerator();
                    gen.Emit(OpCodes.Ldarg_0);
                    gen.Emit(OpCodes.Call, pinfo.GetGetMethod());
                    gen.Emit(OpCodes.Ret);
                    pb.SetGetMethod(getMethod);

                    //MethodBuilder setMethod =
                    //   mTypeBuilder.DefineMethod(
                    //     "set_" + currentPropertyName, setSetAttr, null, new Type[] { currentPropertyType });

                    MethodBuilder setMethod =
                        typeBuilder.DefineMethod(
                            "set_" + pinfo.Name, getSetAttr, null, new Type[] {
                                pinfo.PropertyType
                            });
                    gen = setMethod.GetILGenerator();
                    gen.Emit(OpCodes.Ldarg_0);
                    gen.Emit(OpCodes.Ldstr, pinfo.Name);
                    gen.Emit(OpCodes.Call, raiseEvent);
                    gen.Emit(OpCodes.Ldarg_0);
                    gen.Emit(OpCodes.Ldarg_1);
                    gen.Emit(OpCodes.Call, pinfo.GetSetMethod());
                    gen.Emit(OpCodes.Ret);
                    pb.SetSetMethod(setMethod);
                }
            }
        }

        private MethodBuilder GenerateILForINotifyPropertyChanged()
        {
            MethodInfo DelegateCombine = typeof(Delegate).GetMethod("Combine", new Type[] {
                typeof (Delegate), typeof (Delegate)
            });
            MethodInfo DelegateRemove = typeof(Delegate).GetMethod("Remove", new Type[] {
                typeof (Delegate), typeof (Delegate)
            });
            MethodInfo InvokeDelegate = typeof(PropertyChangedEventHandler).GetMethod("Invoke");
            FieldBuilder eventBack = typeBuilder.DefineField("PropertyChanged", typeof(PropertyChangingEventHandler), FieldAttributes.Private);
            ConstructorInfo CreateEventArgs = typeof(PropertyChangingEventArgs).GetConstructor(new Type[] {
                typeof (String)
            });


            MethodBuilder AddPropertyChanged = typeBuilder.DefineMethod(
                "add_PropertyChanged", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.SpecialName | MethodAttributes.Final | MethodAttributes.HideBySig | MethodAttributes.NewSlot,
                typeof(void), new Type[] {
                    typeof (PropertyChangedEventHandler)
                });
            ILGenerator gen = AddPropertyChanged.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldfld, eventBack);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Call, DelegateCombine);
            gen.Emit(OpCodes.Castclass, typeof(PropertyChangedEventHandler));
            gen.Emit(OpCodes.Stfld, eventBack);
            gen.Emit(OpCodes.Ret);

            MethodBuilder RemovePropertyChanged = typeBuilder.DefineMethod(
                "remove_PropertyChanged", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.SpecialName | MethodAttributes.Final | MethodAttributes.HideBySig | MethodAttributes.NewSlot,
                typeof(void), new Type[] {
                    typeof (PropertyChangedEventHandler)
                });
            gen = RemovePropertyChanged.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldfld, eventBack);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Call, DelegateRemove);
            gen.Emit(OpCodes.Castclass, typeof(PropertyChangedEventHandler));
            gen.Emit(OpCodes.Stfld, eventBack);
            gen.Emit(OpCodes.Ret);

            MethodBuilder RaisePropertyChanged = typeBuilder.DefineMethod(
                "OnPropertyChanged", MethodAttributes.Public,
                typeof(void), new Type[] {
                    typeof (String)
                });
            gen = RaisePropertyChanged.GetILGenerator();
            Label lblDelegateOk = gen.DefineLabel();
            gen.DeclareLocal(typeof(PropertyChangedEventHandler));
            gen.Emit(OpCodes.Nop);
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldfld, eventBack);
            gen.Emit(OpCodes.Stloc_0);
            gen.Emit(OpCodes.Ldloc_0);
            gen.Emit(OpCodes.Ldnull);
            gen.Emit(OpCodes.Ceq);
            gen.Emit(OpCodes.Brtrue, lblDelegateOk);
            gen.Emit(OpCodes.Ldloc_0);
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Newobj, CreateEventArgs);
            gen.Emit(OpCodes.Callvirt, InvokeDelegate);
            gen.MarkLabel(lblDelegateOk);
            gen.Emit(OpCodes.Ret);

            EventBuilder pcevent = typeBuilder.DefineEvent("PropertyChanged", EventAttributes.None, typeof(PropertyChangedEventHandler));
            pcevent.SetRaiseMethod(RaisePropertyChanged);
            pcevent.SetAddOnMethod(AddPropertyChanged);
            pcevent.SetRemoveOnMethod(RemovePropertyChanged);

            return RaisePropertyChanged;
        }


        public void GeneratePropertyChangedEvent()
        {
            Action<ILGenerator> gen = (il) =>
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, eventBack);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Call, DelegateCombine);
                il.Emit(OpCodes.Castclass, typeof(PropertyChangedEventHandler));
                il.Emit(OpCodes.Stfld, eventBack);
                il.Emit(OpCodes.Ret);
            };
            var methodAttributes = MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.SpecialName
                | MethodAttributes.HideBySig | MethodAttributes.Final | MethodAttributes.NewSlot;
            var returnType = typeof(void);
            var parameterTypes = new Type[] { typeof(PropertyChangedEventHandler) };
            CreateMethod("add_PropertyChanged", returnType, parameterTypes, methodAttributes, gen);
        }

        public void CreateOnPropertyChanged()
        {
            Action<ILGenerator> gen = (il) =>
            {
                Label lblDelegateOk = il.DefineLabel();
                il.Emit(OpCodes.Nop);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, eventBack);
                il.Emit(OpCodes.Stloc_0);
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Ldnull);
                il.Emit(OpCodes.Ceq);
                il.Emit(OpCodes.Brtrue, lblDelegateOk);
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Newobj, CreateEventArgs);
                il.Emit(OpCodes.Callvirt, InvokeDelegate);
                il.MarkLabel(lblDelegateOk);
                il.Emit(OpCodes.Ret);
            };
            var methodAttributes = MethodAttributes.Public;
            var returnType = typeof(void);
            var parameterTypes = new Type[] { typeof(string) };
            CreateMethod("OnPropertyChanged", returnType, parameterTypes, methodAttributes, gen);
        }

        public void ApplyToProperties<TParent>()
        {
            foreach (PropertyInfo pinfo in typeof(TParent).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (pinfo.GetSetMethod().IsVirtual)
                {
                    PropertyBuilder pb = typeBuilder.DefineProperty(
                        pinfo.Name, PropertyAttributes.None, pinfo.PropertyType, Type.EmptyTypes);
                    MethodAttributes getSetAttr =
                        MethodAttributes.Public | MethodAttributes.SpecialName |
                        MethodAttributes.HideBySig | MethodAttributes.Virtual;
                    MethodBuilder getMethod =
                        typeBuilder.DefineMethod(
                            "get_" + pinfo.Name, getSetAttr, pinfo.PropertyType, Type.EmptyTypes);
                    ILGenerator gen = getMethod.GetILGenerator();
                    gen.Emit(OpCodes.Ldarg_0);
                    gen.Emit(OpCodes.Call, pinfo.GetGetMethod());
                    gen.Emit(OpCodes.Ret);
                    pb.SetGetMethod(getMethod);


                    MethodBuilder setMethod =
                        typeBuilder.DefineMethod(
                            "set_" + pinfo.Name, getSetAttr, null, new Type[] { pinfo.PropertyType });
                    gen = setMethod.GetILGenerator();
                    gen.Emit(OpCodes.Ldarg_0);
                    gen.Emit(OpCodes.Ldstr, pinfo.Name);
                    gen.Emit(OpCodes.Call, raiseEvent);
                    gen.Emit(OpCodes.Ldarg_0);
                    gen.Emit(OpCodes.Ldarg_1);
                    gen.Emit(OpCodes.Call, pinfo.GetSetMethod());
                    gen.Emit(OpCodes.Ret);
                    pb.SetSetMethod(setMethod);
                }
            }
        }

        public MethodBuilder CreateMethod(string name, 
            Type returnType,
            Type[] parameterTypes, 
            MethodAttributes methodAttributes, 
            Action<ILGenerator> il_gen)
        {
            MethodBuilder methodToBuild = typeBuilder.DefineMethod(name, methodAttributes, returnType, parameterTypes);
            ILGenerator il = methodToBuild.GetILGenerator();
            il_gen(il);
            return methodToBuild;
        }
    }
}
