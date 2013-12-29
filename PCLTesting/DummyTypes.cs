using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//  Create dummy types in MSTest namespaces so tests which are cross-compiled don't have to use #if's for using statements

//  Namespace for Windows Store app MSTest attributes
namespace Microsoft.VisualStudio.TestPlatform.UnitTestFramework
{
    public class DummyType
    {
    }
}

//  Normal MSTest namespace
namespace Microsoft.VisualStudio.TestTools.UnitTesting
{
    public class DummyType
    {
    }
}
