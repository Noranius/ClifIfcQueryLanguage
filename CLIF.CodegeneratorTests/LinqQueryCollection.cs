using System;
using System.Collections.Generic;
using System.Text;

namespace CLIF.Tests
{
    public static class LinqQueryCollection
    {
        public static readonly string QuerySelectByTypeName = "from ifcEntity in model.Instances where ifcEntity.ExpressType.Name == \"IFCBEAM\" select ifcEntity";
        public static readonly string BasicQuery = "from ifcEntity in model.Instances select ifcEntity";
        public static readonly string DefectTypeQuery = "from ifcProduct in (model.Instances.OfType<Xbim.Ifc4.Interfaces.IIfcProduct>()) " +
            "where (ifcProduct.IsTypedBy.Any() && ifcProduct.IsTypedBy.Any(x => x.RelatingType.Name.ToString().StartsWith(\"Damage type:\"))) " + 
            "select ifcProduct as Xbim.Ifc4.Interfaces.IIfcProduct";
    }
}
