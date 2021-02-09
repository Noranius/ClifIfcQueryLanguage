using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;

using CLIF.Common;

namespace CLIF.Codegenerator
{
    public partial class LinqSelectQueryTemplate
    {
        public string ClassNamespace { get; set; } = "CLIF.AutoQuery";

        public string ClassName { get; set; } = "AutoQueryClass";

        public string LinqQuery { get; set; } = "from ifcEntity in model.instances select ifcEntity;";

        private static Type interfaceType;

        private static MethodInfo interfaceMethod;

        static LinqSelectQueryTemplate()
        {
            LinqSelectQueryTemplate.interfaceType = typeof(IIfcSelectQueryClassCreator);
            LinqSelectQueryTemplate.interfaceMethod = LinqSelectQueryTemplate.interfaceType.GetMethods()[0];
        }
    }
}
