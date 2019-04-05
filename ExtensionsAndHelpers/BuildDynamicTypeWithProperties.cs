using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;

namespace ExtensionsAndHelpers
{
    public static class ReflectionExtensions
    {
        public static Type BuildDynamicTypeWithProperties(this Type type)
        {
            AppDomain myDomain = Thread.GetDomain();
            AssemblyName myAsmName = new AssemblyName();
            myAsmName.Name = "MyDynamicAssembly";

            // To generate a persistable assembly, specify AssemblyBuilderAccess.RunAndSave.
            AssemblyBuilder myAsmBuilder = AssemblyBuilder.DefineDynamicAssembly(myAsmName,
                                                            AssemblyBuilderAccess.RunAndCollect);
            // Generate a persistable single-module assembly.
            ModuleBuilder myModBuilder =
                myAsmBuilder.DefineDynamicModule(myAsmName.Name);

            TypeBuilder myTypeBuilder = myModBuilder.DefineType($"{type.Name}AutoProxy",
                                                            TypeAttributes.Public);

            CreateProperty(ref myTypeBuilder, "CustomerName", typeof(string));

            Type retval = myTypeBuilder.CreateType();

            return retval;
        }

        private static void CreateProperty(ref TypeBuilder myTypeBuilder, string propertyName, Type propertyType)
        {
            FieldBuilder fieldBldr = myTypeBuilder.DefineField($"_{propertyName.ToLower()}",
                                                            propertyType,
                                                            FieldAttributes.Private);

            // The last argument of DefineProperty is null, because the
            // property has no parameters. (If you don't specify null, you must
            // specify an array of Type objects. For a parameterless property,
            // use an array with no elements: new Type[] {})
            PropertyBuilder propertyBldr = myTypeBuilder.DefineProperty(propertyName,
                                                             PropertyAttributes.HasDefault,
                                                             propertyType,
                                                             null);

            // The property set and property get methods require a special
            // set of attributes.
            MethodAttributes getSetAttr =
                MethodAttributes.Public | MethodAttributes.SpecialName |
                    MethodAttributes.HideBySig;

            MethodBuilder GetterMethoddBuilder = DefineGetterMethod(ref myTypeBuilder, ref fieldBldr, propertyName, propertyType, getSetAttr);

            MethodBuilder custNameSetPropMthdBldr = DefineSetterMethod(ref myTypeBuilder, ref fieldBldr, propertyName, propertyType, getSetAttr);

            // Last, we must map the two methods created above to our PropertyBuilder to 
            // their corresponding behaviors, "get" and "set" respectively. 
            propertyBldr.SetGetMethod(GetterMethoddBuilder);
            propertyBldr.SetSetMethod(custNameSetPropMthdBldr);
        }

        private static MethodBuilder DefineSetterMethod(ref TypeBuilder myTypeBuilder, ref FieldBuilder fieldBldr,
            string propertyName, Type propertyType, MethodAttributes getSetAttr)
        {
            // Define the "set" accessor method for CustomerName.
            MethodBuilder custNameSetPropMthdBldr =
                myTypeBuilder.DefineMethod($"set_{propertyName}",
                                           getSetAttr,
                                           null,
                                           new Type[] { propertyType });

            ILGenerator propertySetIL = custNameSetPropMthdBldr.GetILGenerator();

            propertySetIL.Emit(OpCodes.Ldarg_0);
            propertySetIL.Emit(OpCodes.Ldarg_1);
            propertySetIL.Emit(OpCodes.Stfld, fieldBldr);
            propertySetIL.Emit(OpCodes.Ret);
            return custNameSetPropMthdBldr;
        }

        private static MethodBuilder DefineGetterMethod(ref TypeBuilder myTypeBuilder, ref FieldBuilder fieldBldr,
            string propertyName, Type propertyType, MethodAttributes getSetAttr)
        {
            // Define the "get" accessor method for CustomerName.
            MethodBuilder GetterMethoddBuilder =
                myTypeBuilder.DefineMethod($"get_{propertyName}",
                                           getSetAttr,
                                           propertyType,
                                           Type.EmptyTypes);

            ILGenerator propertyGetIL = GetterMethoddBuilder.GetILGenerator();

            propertyGetIL.Emit(OpCodes.Ldarg_0);
            propertyGetIL.Emit(OpCodes.Ldfld, fieldBldr);
            propertyGetIL.Emit(OpCodes.Ret);
            return GetterMethoddBuilder;
        }
    }
}
