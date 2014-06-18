namespace PCLTesting.Runner
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    internal static class HelperExtensions
    {
        internal static void ReportIfNotNull<T>(this IProgress<T> progress, T value)
        {
            if (progress != null)
            {
                progress.Report(value);
            }
        }
    }
}
