using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Common;
using CLIF.QueryEngine;
using Xbim.Ifc4.MeasureResource;

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
        [DeploymentItem(@"TestData\E1_VoidingFeature.ifc")]
        public void TestTypeSelect()
        {
            string ifcFile = @"TestData\E1_VoidingFeature.ifc";
            QueryManager engine = new QueryManager(ifcFile);
            IEnumerable<IPersistEntity> result = engine.Select(LinqQueryCollection.QuerySelectByTypeName);
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
            string ifcFile = @"TestData\E1_VoidingFeature.ifc";
            QueryManager engine = new QueryManager(ifcFile);
            string typeName = "Xbim.Ifc4.SharedBldgElements.IfcBeam";
            string assemblyQualifiedTypeName = engine.GetAssemblyQualifiedTypeName(typeName);
            engine.Update(LinqQueryCollection.QuerySelectByTypeName, assemblyQualifiedTypeName, "beam", "beam.Name = \"Hello World\";");
            IEnumerable<Xbim.Ifc4.Interfaces.IIfcProduct> changes = engine.Select(LinqQueryCollection.QuerySelectByTypeName).Cast<Xbim.Ifc4.Interfaces.IIfcProduct>();
            foreach(var product in changes)
            {
                Assert.IsTrue(product.Name == "Hello World");
            }
            string outPutFile = @"TestData\TestUpdateMethod.ifc";
            engine.SaveModel(outPutFile);
        }

        [TestMethod]
        [DeploymentItem(@"TestData\B_Damage_Types.ifc")]
        public void TestSelectQuery()
        {
            string ifcFile = @"TestData\B_Damage_Types.ifc";
            QueryManager engine = new QueryManager(ifcFile);
            QueryResult res = engine.PerformQuery(LinqQueryCollection.BasicQuery);
            Assert.IsTrue(res.Success);
        }

        [TestMethod]
        [DeploymentItem(@"TestData\B_Damage_Types.ifc")]
        public void TestUpdateQuery()
        {
            string ifcFile = @"TestData\B_Damage_Types.ifc";
            QueryManager engine = new QueryManager(ifcFile);
            QueryResult updateRes = engine.PerformQuery(LinqQueryCollection.UpdateIfcProducts);
            QueryResult selectRes = engine.PerformQuery(LinqQueryCollection.QuerySelectByTypeName);
            Assert.IsTrue(updateRes.Success && selectRes.Success);
            IEnumerable<Xbim.Ifc4.SharedBldgElements.IfcBeam> beams = ((IEnumerable<Xbim.Common.IPersistEntity>)selectRes.ReturnedObject)
                .Cast< Xbim.Ifc4.SharedBldgElements.IfcBeam>();
            foreach (Xbim.Ifc4.SharedBldgElements.IfcBeam beam in beams)
            {
                Assert.IsTrue(beam.Name == "Hello World");
            }
            
        }

        [TestMethod]
        [DeploymentItem(@"TestData\B_Damage_Types.ifc")]
        public void TestInsertQuery()
        {
            string ifcFile = @"TestData\B_Damage_Types.ifc";
            QueryManager engine = new QueryManager(ifcFile);
            QueryResult res = engine.PerformQuery(LinqQueryCollection.InsertNewProxy);
            Assert.IsTrue(res.Success);
        }

        [TestMethod]
        [DeploymentItem(@"TestData\B_Damage_Types.ifc")]
        public void TestDeleteQuery()
        {
            string ifcFile = @"TestData\B_Damage_Types.ifc";
            QueryManager engine = new QueryManager(ifcFile);
            QueryResult preDelete = engine.PerformQuery(LinqQueryCollection.DeleteValidationQuery);
            IEnumerable<IPersistEntity> preDeleteCollection = (IEnumerable<IPersistEntity>)(preDelete.ReturnedObject);
            int preDeleteSize = preDeleteCollection.Count();
            QueryResult res = engine.PerformQuery(LinqQueryCollection.DeleteQuery);
            QueryResult postDelete = engine.PerformQuery(LinqQueryCollection.DeleteValidationQuery);
            IEnumerable<IPersistEntity> postDeleteCollection = (IEnumerable<IPersistEntity>)(postDelete.ReturnedObject);
            Assert.IsTrue(res.Success && preDelete.Success && postDelete.Success && postDeleteCollection.Count() < preDeleteSize);
        }
    }
}
