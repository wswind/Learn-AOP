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
            var dynamicProxy = (ITwitterService)Activator.CreateInstance(
            type, new object[] { new MyTwitterService() });
            dynamicProxy.Tweet("My tweet message!");
        }
        static Type CreateDynamicProxyType()
        {
            var assemblyName = new AssemblyName("MyProxies");
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName,
                                                       AssemblyBuilderAccess.Run);
            var modBuilder = assemblyBuilder.DefineDynamicModule("MyProxies");

            var typeBuilder = modBuilder.DefineType(
                "MyTwitterServiceProxy",
                TypeAttributes.Public | TypeAttributes.Class,
                typeof(object),
                new[] { typeof(ITwitterService) });
           
            var fieldBuilder = typeBuilder.DefineField(
                "_realObject",
                typeof (MyTwitterService),
                FieldAttributes.Private);
            var constructorBuilder = typeBuilder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.HasThis,
                new[] {typeof (MyTwitterService)});
                var contructorIl = constructorBuilder.GetILGenerator();
            contructorIl.Emit(OpCodes.Ldarg_0);
            contructorIl.Emit(OpCodes.Ldarg_1);
            contructorIl.Emit(OpCodes.Stfld, fieldBuilder);
            contructorIl.Emit(OpCodes.Ret);
            var methodBuilder = typeBuilder.DefineMethod("Tweet",
                                MethodAttributes.Public | MethodAttributes.Virtual,
                                typeof (void),
                                new[] {typeof (string)});
                                typeBuilder.DefineMethodOverride(methodBuilder,
                                typeof (ITwitterService).GetMethod("Tweet"));
                                var tweetIl = methodBuilder.GetILGenerator();

            //Console.Writeline
            tweetIl.Emit(OpCodes.Ldstr, "Hello before!");
            tweetIl.Emit(OpCodes.Call, typeof (Console).GetMethod("WriteLine", new[] {typeof (string)}));
            //load arg0 (this)
            tweetIl.Emit(OpCodes.Ldarg_0);
            //load _realObject
            tweetIl.Emit(OpCodes.Ldfld, fieldBuilder);
            //load argument1
            tweetIl.Emit(OpCodes.Ldarg_1);
            //call tweet
            tweetIl.Emit(OpCodes.Call,fieldBuilder.FieldType.GetMethod("Tweet"));
            //Console.Writeline
            tweetIl.Emit(OpCodes.Ldstr, "Hello after!");
            tweetIl.Emit(OpCodes.Call, typeof (Console).GetMethod("WriteLine", new[] {typeof (string)}));
            tweetIl.Emit(OpCodes.Ret);
            return  typeBuilder.CreateType();

        }
    }
}
