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
        [DeploymentItem(@"TestData\G_Beam_with_corroded_reinforcement_Round_IfcVoidingFeature.ifc")]
        public void TestOverwriteModel()
        {
            //string ifcFile = @"TestData\B_Damage_Types.ifc";
            string ifcFile = @"TestData\G_Beam_with_corroded_reinforcement_Round_IfcVoidingFeature.ifc";
            QueryManager engine = new QueryManager(ifcFile);
            engine.Delete(LinqQueryCollection.DeleteReinforcement);
            string pathToStore = @"Testdata\G_Beam_without_reinforcement_and_with_spalling.ifc";
            engine.SaveModel(pathToStore);
        }

        [TestMethod]
        [DeploymentItem(@"TestData\E1_VoidingFeature.ifc")]
        public void TestUpdateMethod()
        {
            string ifcFile = @"TestData\G_Beam_with_corroded_reinforcement_Round_IfcVoidingFeature.ifc";
            QueryManager engine = new QueryManager(ifcFile);
            engine.Update(LinqQueryCollection.QuerySelectByTypeName, "Xbim.Ifc4.SharedBldgElements.IfcBeam", "beam", "beam.Name = \"Hello World\";");
        }
    }
}
