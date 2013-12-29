using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCLTesting
{
	public class AssertFailedException : Exception
	{
		public AssertFailedException(string message)
			: base(message)
		{

		}

		public AssertFailedException(string message, Exception innerException)
			: base(message, innerException)
		{

		}
	}

	public static class Assert
	{
		public static void AreEqual(object expected, object actual, string message = null)
		{
			bool equal;
			if (expected == null)
			{
				equal = (actual == null);
			}
			else
			{
				equal = expected.Equals(actual);
			}

			if (!equal)
			{
				string failMessage = string.Format("Expected: {0} Actual: {1}", expected, actual);
				HandleFail("AreEqual", failMessage, message);
			}
		}

		public static void IsTrue(bool condition, string message = null)
		{
			if (!condition)
			{
				HandleFail("IsTrue", null, message);
			}
		}

		public static void IsFalse(bool condition, string message = null)
		{
			if (condition)
			{
				HandleFail("IsFalse", null, message);
			}
		}

        public static void IsNull(object obj, string message = null)
        {
            if (!object.ReferenceEquals(obj, null))
            {
                HandleFail("IsNull", null, message);
            }
        }

        public static void IsNotNull(object obj, string message = null)
        {
            if (object.ReferenceEquals(obj, null))
            {
                HandleFail("IsNotNull", null, message);
            }
        }

		

        static void HandleFail(string assertName, string failMessage, string message, Exception innerException = null)
        {
			string finalMessage = "Assert." + assertName + " failed.";
			if (!string.IsNullOrEmpty(failMessage))
			{
				finalMessage += "  " + failMessage;
			}
            if (!string.IsNullOrEmpty(message))
            {
                finalMessage += "  " + message;
            }

            if (innerException == null)
            {
                throw new AssertFailedException(finalMessage);
            }
            else
            {
                throw new AssertFailedException(finalMessage, innerException);
            }
        }
    }

}
