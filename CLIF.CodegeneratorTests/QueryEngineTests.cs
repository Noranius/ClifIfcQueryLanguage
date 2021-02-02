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
            IEnumerable<IPersistEntity> result = engine.Select(LinqQueryCollection.BasicQuery);
        }

        [TestMethod]
        [DeploymentItem(@"TestData\B_Damage_Types.ifc")]
        public void TestRecurringCompiliation()
        {
            string ifcFile = @"TestData\B_Damage_Types.ifc";
            QueryManager engine = new QueryManager(ifcFile);
            IEnumerable<IPersistEntity> result = engine.Select(LinqQueryCollection.BasicQuery);
            result = engine.Select(LinqQueryCollection.DefectTypeQuery);
        }

        [TestMethod]
        [DeploymentItem(@"TestData\B_Damage_Types.ifc")]
        public void TestDefectSelection()
        {
            try
            {
                string ifcFile = @"TestData\B_Damage_Types.ifc";
                QueryManager engine = new QueryManager(ifcFile);
                IEnumerable<IPersistEntity> result = engine.Select(LinqQueryCollection.DefectTypeQuery);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Assert.Fail();
            }
        }

        [TestMethod]
        [DeploymentItem(@"TestData\B_Damage_Types.ifc")]
        public void TestDeleteEntity()
        {
            string ifcFile = @"TestData\B_Damage_Types.ifc";
            QueryManager engine = new QueryManager(ifcFile);
            engine.Delete(LinqQueryCollection.DeleteSelectionQuery);
        }

        [TestMethod]
        [DeploymentItem(@"TestData\B_Damage_Types.ifc")]
        public void TestOverwriteModel()
        {
            string ifcFile = @"TestData\B_Damage_Types.ifc";
            QueryManager engine = new QueryManager(ifcFile);
            engine.Delete(LinqQueryCollection.DeleteSelectionQuery);
            string pathToStore = @"Testdata\OverridableTestFile.ifc";
            engine.SaveModel(pathToStore);
        }
    }
}
