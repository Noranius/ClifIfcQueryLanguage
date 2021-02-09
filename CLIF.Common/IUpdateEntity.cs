using System;
using System.Collections.Generic;
using System.Text;
using Xbim.Common;
using Xbim.Ifc;

namespace CLIF.Common
{
    /// <summary>
    /// provide functionality for inserting, updating and deleting
    /// </summary>
    public interface IUpdateEntity
    {
        /// <summary>
        /// Modify the model handed over
        /// </summary>
        /// <param name="entitiyToUpdate">Entity to process</param>
        void UpdateEntities(IEnumerable<IPersistEntity> entitiesToUpdate, IfcStore storeWithEntitiesToUpdate);
    }
}
