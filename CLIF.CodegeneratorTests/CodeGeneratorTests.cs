using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CLIF.Codegenerator;

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
            string result = factory.GetQueryCode(LinqQueryCollection.BasicQuery, "ClifQueryTest", "CLIF.QueryTest");
            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void QuotationTest()
        {
            CSharpCodeFactory factory = new CSharpCodeFactory();
            string result = factory.GetQueryCode(LinqQueryCollection.QuerySelectByTypeName, "ClifQueryTest", "CLIF.QueryTest");
            Assert.IsTrue(true);
        }
    }
}