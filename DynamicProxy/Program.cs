using System;
using System.Reflection;
using System.Reflection.Emit;

namespace DynamicProxy
{
    class Program
    {
        static void Main(string[] args)
        {
            var type = CreateDynamicProxyType();
            var dynamicProxy = (IBusinessModule)Activator.CreateInstance(
            type, new object[] { new BusinessModule() });
            dynamicProxy.Method1("Hello DynamicProxy!");
        }
        static Type CreateDynamicProxyType()
        {
            var assemblyName = new AssemblyName("MyProxies");
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName,
                                                       AssemblyBuilderAccess.Run);      
            var modBuilder = assemblyBuilder.DefineDynamicModule("MyProxies");

            var typeBuilder = modBuilder.DefineType(
                "BusinessModuleProxy",
                TypeAttributes.Public | TypeAttributes.Class,
                typeof(object),
                new[] { typeof(IBusinessModule) });
           
            var fieldBuilder = typeBuilder.DefineField(
                "_realObject",
                typeof (BusinessModule),
                FieldAttributes.Private);
            var constructorBuilder = typeBuilder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.HasThis,
                new[] {typeof (BusinessModule)});
                var contructorIl = constructorBuilder.GetILGenerator();
            contructorIl.Emit(OpCodes.Ldarg_0);
            contructorIl.Emit(OpCodes.Ldarg_1);
            contructorIl.Emit(OpCodes.Stfld, fieldBuilder);
            contructorIl.Emit(OpCodes.Ret);
            var methodBuilder = typeBuilder.DefineMethod("Method1",
                                MethodAttributes.Public | MethodAttributes.Virtual,
                                typeof (void),
                                new[] {typeof (string)});
                                typeBuilder.DefineMethodOverride(methodBuilder,
                                typeof (IBusinessModule).GetMethod("Method1"));
                                var method1 = methodBuilder.GetILGenerator();

            //Console.Writeline
            method1.Emit(OpCodes.Ldstr, "Method1 before!");
            method1.Emit(OpCodes.Call, typeof (Console).GetMethod("WriteLine", new[] {typeof (string)}));
            //load arg0 (this)
            method1.Emit(OpCodes.Ldarg_0);
            //load _realObject
            method1.Emit(OpCodes.Ldfld, fieldBuilder);
            //load argument1
            method1.Emit(OpCodes.Ldarg_1);
            //call Method1
            method1.Emit(OpCodes.Call,fieldBuilder.FieldType.GetMethod("Method1"));
            //Console.Writeline
            method1.Emit(OpCodes.Ldstr, "Method1 after!");
            method1.Emit(OpCodes.Call, typeof (Console).GetMethod("WriteLine", new[] {typeof (string)}));
            method1.Emit(OpCodes.Ret);
            return  typeBuilder.CreateType();

        }
    }
}
