using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace CLIF.Tests
{
    [TestClass()]
    public class MainProgramTestClass
    {
        [TestMethod()]
        public void TestCommand()
        {
            CLIF.QueryEnvironment.Program.Main(new string[] { "--query=\"from ifcEntity in model.Instances where ifcEntity.ExpressType.ExpressName == \"IfcPropertySet\" select ifcEntity", "-i=\"Testdata\\B_Damage_Types.ifc\"", "-v" });
        }

        [TestMethod()]
        public void TestQueryFile()
        {
            CLIF.QueryEnvironment.Program.Main(new string[] { "-f=\"TestData\\QueryTestFile.txt\"", "-i=\"Testdata\\B_Damage_Types.ifc\"", "-v" });
        }
    }
}
