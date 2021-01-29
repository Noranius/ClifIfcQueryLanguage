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
        public void TestMainMethod()
        {
            CLIF.QueryEnvironment.Program.Main(new string[] { "-l=from ifcEntity in model.Instances select ifcEntity", "-i=Testdata\\B_Damage_Types.ifc" });
        }
    }
}
