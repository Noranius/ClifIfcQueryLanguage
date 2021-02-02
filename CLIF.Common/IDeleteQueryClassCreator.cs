using System;
using System.Collections.Generic;
using System.Text;
using Xbim.Common;

namespace CLIF.Common
{
    /// <summary>
    /// provide functionality for inserting, updating and deleting
    /// </summary>
    public interface IDeleteQueryClassCreator
    {
        /// <summary>
        /// Modify the model handed over
        /// </summary>
        /// <param name="model">model to operate on</param>
        /// <param name="entitiesToDelete">Entites to delete</param>
        void DeleteEntities(IModel model, IEnumerable<IPersistEntity> entitiesToDelete);
    }
}
