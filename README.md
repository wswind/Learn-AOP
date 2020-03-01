# AOP in .NET

AOP是所有现代OOP语言开发框架中的基础功能，随着Spring框架的普及，对于AOP的使用已经像喝水一样普通。可是知其然还要其所以然。本文将基于.NET环境探讨实现AOP的底层原理。

> 本文为读书笔记
>
> 文中部分代码样例摘自Matthew D. Groves的《AOP in .NET》，推荐大家购买阅读。
>
> 中间件与过滤器原理截图摘自微软官方文档，请查看文中链接。

![](https://img2018.cnblogs.com/blog/1114902/202002/1114902-20200225235856077-620526288.png)
			

本文主要包含以下内容：

1. 基础概念
2. ASP.NET Core框架内置的AOP
   1. 中间件
   2. 过滤器
   
3. AOP in .NET
   1. 编译时/运行时织入

   2. 代理模式

   3. 手动编写动态代理代码

   4. Castle DynamicProxy

   5. Autofac + Castle.DynamicProxy

下载文中样例代码请访问 <https://github.com/wswind/aop-learn>



## 基础概念

面向对象编程通过类的继承机制来复用代码，这在大多数情况下这很有用。但是随着软件系统的越来复杂，出现了一些通过OOP处理起来相当费力的关注点，比如：日志记录，权限控制，缓存，数据库事务提交等等。它们的处理逻辑分散于各个模块，各个类方法之中，这违反了DRY原则(Don't Repeat Yourself)以及关注度点分离原则（Separation of Concerns），不利于后期的代码维护。所谓AOP(面向切面编程)，就是将这些关注点，看作一个个切面，捕获这些切面并将其处理程序模块化的过程。

以一个简单的日志记录切面处理为例。如果不应用AOP，日志处理的代码逻辑分散于模块的各个方法中，如下图

![](https://img2018.cnblogs.com/blog/1114902/202002/1114902-20200226114316256-1666642467.png)

**要实现AOP，关键在于捕捉切面，然后将切面织入（“weaving”）到业务模块中。**

如下图代码中，我们将分散的日志处理代码模块化成了一个统一的切面处理程序：LogAspect。然后将其织入到BusinessModule1中，这就实现了日志处理的AOP。

![](https://img2018.cnblogs.com/blog/1114902/202002/1114902-20200226000324991-2046549754.png)





## ASP.NET Core框架内置的AOP机制

在.ASP.NET Core框架中，微软内置了一些处理AOP逻辑的机制。虽然这与传统意义上的AOP不同，但是这里还是简单提一下。

### 中间件机制

![](https://img2018.cnblogs.com/blog/1114902/202002/1114902-20200225235911798-1436753340.png)


<https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/>

<https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/write>

ASP.NET Core框架本身就是由一系列中间件组成的，它本身内置的异常处理，路由转发，权限控制，也就是在上述图中的请求管道中实现的。所以我们也完全可以基于中间件机制，实现AOP。

以异常处理为例，我可以将try catch加入到next方法的前后，以捕获后续运行过程中未处理的异常，并进行统一处理。代码如下：

```csharp
public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    public ExceptionHandlerMiddleware(RequestDelegate next )
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, IHostingEnvironment env,ILogger<ExceptionHandlerMiddleware> logger)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(new EventId(ex.HResult), ex, ex.Message);
            await context.HandleExceptionAsync(ex, env.IsDevelopment());
        }
    }
}
```

### 过滤器机制

https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/filters

过滤器本质上是由路由中间件(Routing Middleware)的请求管道实现的，如下图所示。

![](https://img2018.cnblogs.com/blog/1114902/202002/1114902-20200225235927198-1726151277.png)


开发者通过定义并注册相应的过滤器，就能基于这个请求管道，来处理对应的关注点，如权限控制，结果转换，日志记录等等。Asp.NET Core 的过滤器执行顺序如下图：

![](https://img2018.cnblogs.com/blog/1114902/202002/1114902-20200225235936330-226692176.png)



我们可以基于中间件或者过滤器机制，完成简单的开发。**可惜的是，这些并不是语言级别的aop。**asp.net core是一个开发框架，它为了方便你开发，给你内置了一些条条框框，你照着做确实能够解决大部分问题。

但是脱离了它，该如何自己借助语言特性实现AOP呢？下面我们开始真正进入主题。

### 

## AOP in .NET

### 编译时/运行时织入

在基础概念中，我们已经简单的说明了什么是AOP的织入。<u>**实现织入的方式分为两种：编译时织入、运行时织入。**</u>

> 当你使用C＃创建.NET项目时，该项目将被编译为CIL（也称为MSIL，IL和bytecode）作为程序集（DLL或EXE文件）。 下图说明了这个过程。然后，公共语言运行时（CLR）可以将CIL转换成真实的机器指令（通过即时编译过程，JIT）。 
>
> 《aop in .net》

![](https://img2018.cnblogs.com/blog/1114902/202002/1114902-20200226000115210-46561410.png)

所谓编译时织入，就是在编译过程中修改产生的CIL文件，来达到织入的效果，如下图所示。编译时织入主要通过PostSharp实现。

![](https://img2018.cnblogs.com/blog/1114902/202002/1114902-20200226000139343-1919172574.png)

运行时织入则是在程序运行时来完成的织入，一般是通过DynamicProxy(动态代理)程序（Castle.Core）配合IoC容器(Autofac，StructureMap等)来实现的。
在IoC容器解析服务实例(Service Instance)时，动态代理程序会基于服务实例创建动态代理对象，并在动态代理对象方法中，织入拦截器(interceptor)的执行逻辑，以此完成动态织入。
这里的拦截器就是我们处理切面逻辑的地方，我们会在后面通过代码样例详细讲解这种动态代理模式的实现原理。

DynamicProxy与PostSharp这两种织入模式各有利弊：

1. PostSharp是在编译时进行的，DynamicProxy在运行时进行。所以一个会增加编译时间，一个会降低运行效率。
2. 由于PostSharp需要安装额外的编译程序，这意味着没有安装PostSharp的机器，无法正确编译你开发的程序。这不利于应用在开源项目中，也不利于部署CI/CD的自动化编译服务。
3. PostSharp为收费的商业项目，需要付费使用。而运行时织入所需的Castle.Core以及IoC框架，都是开源免费的。
4. DynamicProxy必须使用IoC容器，对于UI对象或领域对象，并不适合或不可能通过容器获取实例。PostSharp没有这个问题。
5. DynamicProxy比PostSharp更易于进行单元测试。
6. DynamicProxy在运行时执行，因此在编译完成后，你仍可以通过修改配置文件来修改切面配置。PostSharp做不到这一点。
7. DynamicProxy的拦截器被附加到类的所有方法中，而PostSharp能够更精准的拦截。
8. PostSharp能够在static方法、private方法、属性中织入AOP，而DynamicProxy做不到这一点。

你可以根据自己的需要选择合适的织入方式，不过由于PostSharp为商业付费项目，我后面不再对其进行过多讲解，需要的朋友可自行阅读《AOP in .NET》中的相关内容，或查阅PostSharp官网。

本文后面将主要通过代码样例讲述如何基于动态代理实现运行时织入。

### 代理模式

回顾之前基础概念一节中的例子。我们需要在Mehtod1的执行前后，分别调用LogAspect的BeginMethod以及EndMethod方法来处理日志记录逻辑。

![](https://img2018.cnblogs.com/blog/1114902/202002/1114902-20200226225431023-1684925223.png)


我们现在通过运用一个简单的代理模式模拟这个过程：

定义一个接口 IBusinessModule，并实现它

```csharp
public interface IBusinessModule
{
    void Method1();
}

public class BusinessModule : IBusinessModule
{
    public void Method1()
    {
        Console.WriteLine("Method1");
    }
}
```

我现在需要在Method1方法调用前后，添加日志记录。在不改变BusinessModule原有代码的情况下，我们可以添加一个代理中间层来实现。代理类调用Method1，并在调用前后来打印日志。

```csharp
public class BusinessModuleProxy : IBusinessModule
{
    BusinessModule _realObject;
    public BusinessModuleProxy()
    {
        _realObject = new BusinessModule();
    }
    public void Method1()
    {
        Console.WriteLine("BusinessModuleProxy before");
        _realObject.Method1();
        Console.WriteLine("BusinessModuleProxy after");
    }
}
```

在执行时，我们通过调用代理类来执行Method1，输出便可以实现日志的输出

```csharp
class Program
{
    static void Main(string[] args)
    {
        IBusinessModule module = new BusinessModuleProxy();
        module.Method1();
    }
}
```

**越是简单的东西越接近事物的本质，代理模式就是后面一切运行时织入实现的根本。**

其实如果你在实际开发过程中，如果你的程序较小，对AOP的需要没有那么迫切，你也完全可以考虑通过IoC容器 + 代理模式(将对象的创建改为DI)来替代后面即将讲的重型AOP实现。因为引入动态代理实现重型AOP会降低你的程序运行速度。

### 手动编写动态代理代码

上个例子中的代理模式虽然很有用，但是如果你需要为多个类的多个接口编写切面处理程序，你就需要为每个接口编写一个代理类，这是一个不小的工作量，也不易于代码的维护。因此我们需要使用动态代理技术来动态生成代理类。

虽然我们能够通过Castle的DynamicProxy工具来实现动态代理，但是为了了解底层原理，我们还是先手动编写动态代理代码。

为了更好的展示动态代理类的构建，我们对上面的例子进行一些调整。
我们不再自行定义代理类，而是需要通过IL生成器（ILGenerator）来生成它。

BusinessModule之前的例子很类似，但是也有些不同，Method1方法加入了参数，这主要是为了便于演示IL生成器的用法。


```csharp
public interface IBusinessModule
{
    void Method1(string message);
}
public class BusinessModule : IBusinessModule
{
    public void Method1(string message)
    {
        Console.WriteLine("Method1: {0}", message);
    }
}
```
我们希望通过IL生成器构造以下的代理类。和之前不同的是，这个代理类的构造函数传入了BusinessModule对象实例而不是通过new方法自己创建（这有些类似装饰器模式）。
之所以这样做，是为了简化IL生成器的代码量（这个代码真的不是很好写）。
代理类定义如下，需要说明的是，这个类只是一个伪代码，用于讲解IL生成器的逻辑。在运行中不会被调用。
```csharp
public class BusinessModuleProxy
{
    BusinessModule _realObject;

    public BusinessModuleProxy(BusinessModule svc)
    {
        _realObject = svc;
    }
    public void Method1(string message)
    {
        Console.WriteLine("Method1 before!");
        _realObject.Method1(message);
        Console.WriteLine("Method1 after!");
    }
}
```
手动创建创建代理类的CreateDynamicProxyType方法代码如下（你可以在文章开头提到的github仓库中下载）。
```csharp
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
```

CreateDynamicProxyType方法构造出的类型，其实就是伪代码展示过的BusinessModuleProxy。通过ILGenerator.Emit方法，我们插入了控制台提示。

Main函数调用代码如下：

```csharp
static void Main(string[] args)
{
    var type = CreateDynamicProxyType();
    var dynamicProxy = (IBusinessModule)Activator.CreateInstance(
    type, new object[] { new BusinessModule() });
    dynamicProxy.Method1("Hello DynamicProxy!");
}
```

执行结果展示：

```
Method1 before!
Method1: Hello DynamicProxy!
Method1 after!
```

虽然我们在实际开发中，不会自己手动这样构造程序集来构造代理类。但是这个例子展示了运行时织入的动态代理原理。和之前的编译时织入类似，它也是对程序集的IL进行了修改。只不过它修改的时机是在对象实例创建时进行的。

希望这个例子能够帮助你理解动态代理的底层原理。

### Castle DynamicProxy

在实际开发中，我们往往通过Castle.Core来实现DynamicProxy。Castle.Core是一个开源且被广泛使用的动态代理组件，你可以通过nuget安装并使用它。

IInterceptor是Castle.Core定义的拦截器接口。我们首先定义一个简单的拦截器，在方法执行的前后，在控制台打印消息。

```csharp
public class MyInterceptorAspect : IInterceptor
{
    public void Intercept(IInvocation invocation)
    {
        Console.WriteLine("Interceptor before");
        invocation.Proceed();
        Console.WriteLine("Interceptor after");
    }
}
```

在定义一个消息发送类，用于打印消息。

```csharp
public class MessageClient
{
    public virtual void Send(string msg)
    {
        Console.WriteLine("Sending: {0}", msg);
    }
}
```

我们希望在Send方法调用前后，织入上面的拦截器。则可在Main函数中添加以下代码

```csharp
var proxyGenerator = new ProxyGenerator();
var svc = proxyGenerator.CreateClassProxy<MessageClient>(new MyInterceptorAspect());
svc.Send("hi");
```

控制台结果如下

```
Interceptor before
Sending: hi
Interceptor after
```

我们可以看到，使用Castle.Core织入非常简单。不过也有一点需要额外注意：

Send必须是虚方法，这是因为**CreateClassProxy返回的类型，并不是MessageClient，它是以MessageClient为父类的动态代理类**，如果你看懂了上一节的内容，这里应该很好理解。所以，所有需要拦截的方法，都需要声明为虚方法，这样才能使拦截生效。如果你使用过NHibernate或者EntityFramework的.NET Framework版本，这个要求你应该很熟悉。

不过虚方法要求是因为MessageClient是一个具体类（concrete class）。如果通过接口进行拦截，我们可以使用CreateInterfaceProxyWithTarget方法，而避免必须要求为虚方法的限制。下面我们来通过代码演示：

我们定义一个HelloClient，它继承了IHelloClient接口

```csharp
public class HelloClient : IHelloClient
{
    public void Hello()
    {
        Console.WriteLine("Hello");
    }
}

public interface IHelloClient
{
    void Hello();
}
```

通过CreateInterfaceProxyWithTarget即可完成MyInterceptorAspect接口拦截。通过接口拦截不再要求Hello方法为虚方法。

```csharp
var svc2 = proxyGenerator.CreateInterfaceProxyWithTarget<IHelloClient>(new HelloClient(), new MyInterceptorAspect());
svc2.Hello();
```

Castle.Core是一个很有用的动态代理插件，很多开源组件都使用了它，学习与掌握它的基本使用是很有必要的。

### Autofac + Castle.DynamicProxy

通过IoC容器配合动态代理，是实际开发中，最常用的方式。这里使用autofac来进行演示。

> autofac拦截器的详细文档请浏览：<https://autofac.readthedocs.io/en/latest/advanced/interceptors.html>
>



和之前一样，我创建了一个拦截器，拦截特定方法的执行，并在执行前后进行控制台打印。

另外，我定义了一个自定义属性(Attribute)来设置方法是否需要使用日志，如果开启了，才进行日志打印。

**通过自定义属性对方法进行声明，从而影响AOP拦截器的方式，可以使代码更加直观，简化代码逻辑。**

拦截器CallLogger代码如下：

```csharp
public class CallLogger : IInterceptor
{
    TextWriter _output;

    public CallLogger(TextWriter output)
    {
        _output = output;
    }

    public void Intercept(IInvocation invocation)
    {
        _output.WriteLine("Calling method '{0}' with parameters '{1}'... ",
            invocation.Method.Name,
            string.Join(", ", invocation.Arguments.Select(a => (a ?? "").ToString()).ToArray()));

        //校验方法是否需要开启了Logger
        bool isEnabled = AttributeHelper.IsLoggerEnabled(invocation.Method);

        //方法执行前
        if (isEnabled)
        {
            _output.WriteLine("Logger is Enabled");
        }
        //被拦截的方法执行
        invocation.Proceed();

        //方法执行后
        if (isEnabled)
        {
            _output.WriteLine("Done: result was '{0}'.", invocation.ReturnValue);
        }
    }
}
```

要拦截的接口ISomeType及其实现类定义如下，[Intercept]标签将接口与拦截器进行了关联。

```csharp
[Intercept(typeof(CallLogger))] 
public interface ISomeType
{
    [Custom(StartLog = true)]
    string Show(string input);
}

public class SomeType : ISomeType
{
    //di called interface ,the attribute should be at interface
    public string Show(string input)
    {
        Console.WriteLine($"showdemo");
        return "resultdemo";
    }
}
```

代码中[Custom(StartLog = true)]是我自定义的标签，用于设定日志开关。

CustomAttribute定义代码如下

```csharp
[AttributeUsage(AttributeTargets.Method)]
public class CustomAttribute : Attribute
{
    public bool StartLog { get; set; }
}
```

我编写了一个帮助类来处理这个Attribute

```csharp
public static class AttributeHelper
{
    public static bool IsLoggerEnabled(MethodInfo type)
    {
        return GetStartLog(type);
    }
    
    public static bool HasCustomAttribute(MemberInfo methodInfo)
    {
       return methodInfo.IsDefined(typeof(CustomAttribute), true);
    }

    private static bool GetStartLog(MethodInfo methodInfo)
    {
        var attrs = methodInfo.GetCustomAttributes(true).OfType<CustomAttribute>().ToArray();
        if (attrs.Any())
        {
            CustomAttribute customAttribute = attrs.First();
            return customAttribute.StartLog;
        }
        return false;
    }
}
```

通过控制台的Main函数进行代码调用

```csharp
static void Main(string[] args)
{
    // create builder
    var builder = new ContainerBuilder();
	// 注册接口及其实现类
    builder.RegisterType<SomeType>()
        .As<ISomeType>()
        .EnableInterfaceInterceptors();
    // 注册拦截器
    builder.Register(c => new CallLogger(Console.Out));
    // 创建容器
    var container = builder.Build();
    // 解析服务
    var willBeIntercepted = container.Resolve<ISomeType>();
    // 执行
    willBeIntercepted.Show("this is a test");
}   
```

输出结果如下：

```
Calling method 'Show' with parameters 'this is a test'...
Logger is Enabled
showdemo
Done: result was 'resultdemo'.
```

关于异步方法的拦截，这一补充一点：

Castle.Core目前没有原生支持异步方法的拦截，你需要在拦截器对异步方法进行一些额外的处理。

Autofac对这个问题也没有内置支持：<https://autofac.readthedocs.io/en/latest/advanced/interceptors.html#asynchronous-method-interception>

你可以通过Task.ContinueWith()来处理异步情况，或者通过第三方的帮助库来实现异步方法的拦截：<https://github.com/JSkimming/Castle.Core.AsyncInterceptor>

对于Autofac的异步拦截器的代码样例，可查看：<https://github.com/wswind/aop-learn/tree/master/AutofacAsyncInterceptor>

另外：Structuremap(sunsetted) 是有异步拦截器支持的，可查看拓展阅读中链接。



最后，希望本文对你有帮助。如果本文有错误欢迎在评论中指出。




> 拓展阅读：
>
> 
>
> 装饰模式 <https://www.tutorialspoint.com/design_pattern/decorator_pattern.htm>
>
> 代理模式 <https://www.tutorialspoint.com/design_pattern/proxy_pattern.htm>
>
> 代理模式与装饰模式的区别 <https://stackoverflow.com/questions/18618779/differences-between-proxy-and-decorator-pattern>
>
> .NET Core 默认IoC容器结合适配器模式  <https://medium.com/@willie.tetlow/net-core-dependency-injection-decorator-workaround-664cd3ec1246>>
>
> Simple .NET Aspect-Oriented Programming ：<https://github.com/TylerBrinks/Snap>
>
> Structuremap Interception and Decorators: <https://structuremap.github.io/interception-and-decorators>
>
> StructuremapAspect Oriented Programming with StructureMap.DynamicInterception: <https://structuremap.github.io/dynamic-interception/>
> 
> Castle.Core 异步拦截器文档： <https://github.com/castleproject/Core/blob/master/docs/dynamicproxy-async-interception.md>