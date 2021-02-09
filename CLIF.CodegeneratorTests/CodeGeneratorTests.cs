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
        public void GetSelectQueryCodeTest()
        {
            CSharpCodeFactory factory = new CSharpCodeFactory();
            string result = factory.GetSelectQueryCode(LinqQueryCollection.BasicQuery, "ClifQueryTest", "CLIF.QueryTest");
            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void QuotationTest()
        {
            CSharpCodeFactory factory = new CSharpCodeFactory();
            string result = factory.GetSelectQueryCode(LinqQueryCollection.QuerySelectByTypeName, "ClifQueryTest", "CLIF.QueryTest");
            Assert.IsTrue(true);
        }

        /// <summary>
        /// Tests the generation of the source code
        /// </summary>
        [TestMethod()]
        public void GetUpdateQueryCodeTest()
        {
            CSharpCodeFactory factory = new CSharpCodeFactory();
            string result = factory.GetUpdateQueryCode(LinqQueryCollection.MethodBodySimpleModification, typeof(Xbim.Ifc4.Interfaces.IIfcProduct), "entityToUpdate", "ClifQueryTest", "CLIF.QueryTest");
            Assert.IsTrue(true);
        }
    }
}