using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using CLIF.Common;
using Xbim.Common;

namespace CLIF.Codegenerator
{
    /// <summary>
    /// Template properties for the template
    /// </summary>
    public partial class LinqModificationTemplate
    {
        public string ClassNamespace { get; set; } = "CLIF.AutoQuery";

        public string ClassName { get; set; } = "AutoQueryClass";

        public string MethodBody { get; set; } = "Console.WriteLine(\"Hello World\");";

        public string LinqQuery { get; set; } = "from ifcEntity in model.instances select ifcEntity;";

        public string InputParameterTypeFullName { get; set; } = LinqModificationTemplate.defaultParameterName;

        public string UpdateEntityName { get; set; } = "entityToUpdate";

        private static readonly Type interfaceType;

        private static readonly MethodInfo interfaceMethod;

        private static readonly string defaultParameterName = typeof(IPersistEntity).FullName;

        static LinqModificationTemplate()
        {
            LinqModificationTemplate.interfaceType = typeof(IUpdateEntity);
            LinqModificationTemplate.interfaceMethod = LinqModificationTemplate.interfaceType.GetMethods()[0];
        }

        public void UpdateEntity<T>(T entitiyToUpdate) where T : IPersistEntity
        {
            throw new NotImplementedException();
        }

        public void UpdateEntity(IInstantiableEntity entity)
        {
            this.UpdateEntity<IInstantiableEntity>(entity);
        }
    }
}
