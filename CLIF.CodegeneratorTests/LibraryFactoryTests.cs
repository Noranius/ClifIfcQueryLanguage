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
                Assembly result = fac.GetQueryAssembly("FactoryTestResult", "CLIF.Tests", "from ifcEntity in model.Instances select ifcEntity");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
        }
    }
}
