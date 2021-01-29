using CLIF.LibraryFactory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;
using System.Linq;

namespace CLIF.Tests
{

    [TestClass()]
    public class LibraryFactoryTests
    {
        [TestMethod()]
        public void TestCodeCompiliation()
        {
            try
            {
                IfcQueryClassFactory fac = new IfcQueryClassFactory();
                Assembly result = fac.GetQueryAssembly("FactoryTestResult", "CLIF.Tests.CodeCompilation", "CLIF.Tests.CodeCompilation",
                    LinqQueryCollection.BasicQuery);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
        }

        [TestMethod()]
        public void TestCompilationWithQuotes()
        {
            try
            {
                IfcQueryClassFactory fac = new IfcQueryClassFactory();
                string linqQuery = LinqQueryCollection.QuerySelectByTypeName;
                Assembly result = fac.GetQueryAssembly("FactoryTestResult", "CLIF.Tests.TestCompilationWithQuotes", 
                    "CLIF.Tests.TestCompilationWithQuotes", linqQuery);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
        }
    }
}
