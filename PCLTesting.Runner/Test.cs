using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PCLTesting.Infrastructure
{
    public enum TestState
    {
        NotRun,
        Passed,
        Failed,
        Skipped,
    }

    public class Test
    {
        private static readonly object[] EmptyParameters = new object[0];
        MethodInfo _method;

        public Test(MethodInfo method)
        {
            _method = method;
            TestState = TestState.NotRun;
            FailureException = null;
        }

        public string FullName
        {
            get
            {
                return _method.DeclaringType.FullName + "." + _method.Name;
            }
        }

        public TestState TestState { get; private set; }

        public Exception FailureException { get; private set; }

        public async Task RunAsync()
        {
            TestState = TestState.NotRun;
            FailureException = null;
            try
            {
                object testClassInstance = _method.IsStatic ? null : Activator.CreateInstance(_method.ReflectedType);
                if (_method.ReturnType == typeof(void))
                {
                    _method.Invoke(testClassInstance, EmptyParameters);
                }
                else
                {
                    await (Task)_method.Invoke(testClassInstance, EmptyParameters);
                }

                TestState = TestState.Passed;
            }
            catch (TargetInvocationException ex)
            {
                TestState = TestState.Failed;
                FailureException = ex.InnerException;
            }
            catch (Exception ex)
            {
                TestState = TestState.Failed;
                FailureException = ex;
            }
        }
    }
}
