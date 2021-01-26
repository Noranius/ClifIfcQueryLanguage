using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;

namespace CLIF.Codegenerator
{
    public partial class LinqQueryTemplate
    {
        public string ClassNamespace { get; set; } = "CLIF.AutoQuery";

        public string ClassName { get; set; } = "AutoQueryClass";

        public string LinqQuery { get; set; } = "from ifcEntity in model.instances select ifcEntity;";

        private static Type interfaceType;

        private static MethodInfo interfaceMethod;

        static LinqQueryTemplate()
        {
            LinqQueryTemplate.interfaceType = typeof(IIfcSelectQueryClassCreator);
            LinqQueryTemplate.interfaceMethod = LinqQueryTemplate.interfaceType.GetMethods()[0];
        }
    }
}
