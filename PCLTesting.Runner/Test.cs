namespace PCLTesting.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using PCLCommandBase;
    using Validation;

    public enum TestState
    {
        NotRun,
        Passed,
        Failed,
        Skipped,
    }

    public class Test : BindableBase
    {
        private static readonly object[] EmptyParameters = new object[0];
        private readonly MethodInfo method;

        public Test(MethodInfo method)
        {
            Requires.NotNull(method, "method");
            this.method = method;
            this.RegisterDependentProperty(() => FailureException, () => OneLineFailureExplanation);
            this.RegisterDependentProperty(() => FailureException, () => FullFailureExplanation);
        }

        public string FullName
        {
            get { return this.method.DeclaringType.FullName + "." + this.method.Name; }
        }

        public string Name
        {
            get { return this.method.Name; }
        }

        private TestState result;
        public TestState Result
        {
            get { return this.result; }
            private set { this.SetProperty(ref this.result, value); }
        }

        private Exception failureException;
        public Exception FailureException
        {
            get { return this.failureException; }
            private set { this.SetProperty(ref this.failureException, value); }
        }

        public string FullFailureExplanation
        {
            get { return this.FailureException != null ? this.FailureException.ToString() : String.Empty; }
        }

        public string OneLineFailureExplanation
        {
            get
            {
                return this.FailureException == null
                    ? string.Empty
                    : this.FailureException.GetType().Name + ": " + this.FailureException.Message;
            }
        }

        public async Task RunAsync()
        {
            this.Result = TestState.NotRun;
            this.FailureException = null;
            try
            {
                object testClassInstance = this.method.IsStatic ? null : Activator.CreateInstance(this.method.ReflectedType);
                if (this.method.ReturnType == typeof(void))
                {
                    this.method.Invoke(testClassInstance, EmptyParameters);
                }
                else
                {
                    await (Task)this.method.Invoke(testClassInstance, EmptyParameters);
                }

                this.Result = TestState.Passed;
            }
            catch (TargetInvocationException ex)
            {
                this.Result = TestState.Failed;
                this.FailureException = ex.InnerException;
            }
            catch (Exception ex)
            {
                this.Result = TestState.Failed;
                this.FailureException = ex;
            }
        }
    }
}
