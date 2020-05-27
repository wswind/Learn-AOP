﻿using Castle.DynamicProxy;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

//https://stackoverflow.com/a/28374134/7726468
namespace AutofacAsyncInterceptor
{
    public class CallLoggerAsyncInterceptor : IInterceptor  
    {
        TextWriter _output;

        public CallLoggerAsyncInterceptor(TextWriter output)
        {
            _output = output;
        }

        private static readonly MethodInfo HandleAsyncWithResultMethodInfo = typeof(CallLoggerAsyncInterceptor).GetMethod(nameof(HandleAsyncWithResult), BindingFlags.Instance | BindingFlags.NonPublic);

        public void Intercept(IInvocation invocation)
        {
            //Console.WriteLine("Intercept Begins");
      
            Console.WriteLine("Proceed Begins");
            invocation.Proceed();
            var ret = invocation.ReturnValue;
            var retTask = (Task<string>)ret;
            //var t = retTask.Result;
            retTask.GetAwaiter().GetResult();
            Console.WriteLine("Proceed Ends");
            //var ret = (Task<string>)invocation.ReturnValue;
            //Console.WriteLine("Before get result");
            //var str = ret.Result;
            //Console.WriteLine("After get result");
            //Thread.Sleep(2000);
            //var taskType = invocation.Method.ReturnType;
            //Console.WriteLine("Intercept log 01");
            //var resultType = taskType.GetGenericArguments()[0];
            //Console.WriteLine("Intercept log 02");
            //var mi = HandleAsyncWithResultMethodInfo.MakeGenericMethod(resultType);
            //Console.WriteLine("Intercept log 03");
            //invocation.ReturnValue = mi.Invoke(this, new[] { invocation.ReturnValue, invocation });
            //Console.WriteLine("Intercept log 04");
        }


        private async Task<T> HandleAsyncWithResult<T>(Task<T> task, IInvocation invocation)
        {
            Console.WriteLine("AutofacAsyncInterceptor HandleAsyncWithResult<T> Before await"); //error: the task runs eariler than this line
            var result = await task;
            Console.WriteLine("AutofacAsyncInterceptor HandleAsyncWithResult<T> After await");
            if (IsEnabled(invocation))
            {
                _output.WriteLine("After Invocation, Result is '{0}'.", result);
            }
            return result;
        }

        private bool IsEnabled(IInvocation invocation)
        {
            bool isEnabled = AttributeHelper.IsLoggerEnabled(invocation.Method);
            return isEnabled;
        }
    }
}
