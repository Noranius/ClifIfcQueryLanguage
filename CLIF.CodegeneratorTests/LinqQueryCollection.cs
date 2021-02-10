using System;
using System.Collections.Generic;
using System.Text;

namespace CLIF.Tests
{
    public static class LinqQueryCollection
    {
        public static readonly string QuerySelectByTypeName = "from ifcEntity in model.Instances where ifcEntity.ExpressType.Name == \"IfcBeam\" select ifcEntity";
        public static readonly string BasicQuery = "from ifcEntity in model.Instances select ifcEntity";
        public static readonly string DefectTypeQuery = "from ifcProduct in (model.Instances.OfType<Xbim.Ifc4.Interfaces.IIfcProduct>()) " +
            "where (ifcProduct.IsTypedBy.Any() && ifcProduct.IsTypedBy.Any(x => x.RelatingType.Name.ToString().StartsWith(\"Damage type:\"))) " + 
            "select ifcProduct as Xbim.Ifc4.Interfaces.IIfcProduct";
        public static readonly string DeleteSelectionQuery = "from ifcEntity in model.Instances where ifcEntity.EntityLabel == 9010 select ifcEntity";
        public static readonly string MethodBodySimpleModification = "entityToUpdate.Name = \"Hello World\";";
        public static readonly string DeleteReinforcement = "from ifcRebar in model.Instances where ifcRebar.ExpressType.Name.Contains(\"IfcReinforcingBar\") select ifcRebar";
        public static readonly string UpdateIfcProducts = "update Xbim.Ifc4.SharedBldgElements.IfcBeam beam in model.Instances where beam.ExpressType.Name == \"IfcBeam\" {beam.Name = \"Hello World\";}";
        public static readonly string InsertNewProxy = "insert Xbim.Ifc4.Kernel.IfcProxy proxy in model.instances {proxy.Name = \"My new Proxy\";}";
        public static readonly string DeleteQuery = "delete ifcEntity in model.Instances where ifcEntity.EntityLabel == 9010";
        public static readonly string DeleteValidationQuery = "from ifcEntity in model.Instances where ifcEntity.EntityLabel == 9010 select ifcEntity";
    }
}
