using Microsoft.VisualStudio.TestTools.UnitTesting;
using CLIF.Codegenerator;
using System;
using System.Collections.Generic;
using System.Text;

namespace CLIF.Tests
{
    [TestClass()]
    public class CodeGeneratorTests
    {
        /// <summary>
        /// Tests the generation of the source code
        /// </summary>
        [TestMethod()]
        public void GetQueryCodeTest()
        {
            CSharpCodeFactory factory = new CSharpCodeFactory();
            string result = factory.GetQueryCode("from ifcEntity in model.instances select ifcEntity", 
                "ClifQueryTest", "CLIF.QueryTest");
            Assert.IsTrue(true);
        }
    }
}