using System;
using System.Collections.Generic;
using Xbim.Common;

namespace CLIF.Common
{
    /// <summary>
    /// An interface to define the methods to query IFC information
    /// </summary>
    public interface IIfcSelectQueryClassCreator
    {
        /// <summary>
        /// Searches with the linq query in the provided IFC model
        /// </summary>
        /// <param name="model">Model to search within</param>
        /// <returns>A collection of resulting entities</returns>
        IEnumerable<IPersistEntity> Select(IModel model);
    }
}
