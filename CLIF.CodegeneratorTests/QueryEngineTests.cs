using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Common;
using CLIF.QueryEngine;

namespace CLIF.Tests
{
    [TestClass()]
    public class QueryEngineTests
    {
        [TestMethod]
        [DeploymentItem(@"TestData\B_Damage_Types.ifc")]
        public void TestSimpleQuery()
        {
            string ifcFile = @"TestData\B_Damage_Types.ifc";
            QueryManager engine = new QueryManager(ifcFile);
            IEnumerable<IPersistEntity> result = engine.SelectEntities(LinqQueryCollection.BasicQuery);
        }

        [TestMethod]
        [DeploymentItem(@"TestData\B_Damage_Types.ifc")]
        public void TestRecurringCompiliation()
        {
            string ifcFile = @"TestData\B_Damage_Types.ifc";
            QueryManager engine = new QueryManager(ifcFile);
            IEnumerable<IPersistEntity> result = engine.SelectEntities(LinqQueryCollection.BasicQuery);
            result = engine.SelectEntities(LinqQueryCollection.DefectTypeQuery);
        }

        [TestMethod]
        [DeploymentItem(@"TestData\B_Damage_Types.ifc")]
        public void TestDefectSelection()
        {
            try
            {
                string ifcFile = @"TestData\B_Damage_Types.ifc";
                QueryManager engine = new QueryManager(ifcFile);
                IEnumerable<IPersistEntity> result = engine.SelectEntities(LinqQueryCollection.DefectTypeQuery);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Assert.Fail();
            }
        }
    }
}
