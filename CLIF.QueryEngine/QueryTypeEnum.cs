using System;
using System.Collections.Generic;
using System.Text;

namespace CLIF.QueryEngine
{
    /// <summary>
    /// enum to represent the different query type
    /// </summary>
    public enum QueryTypeEnum
    {
        /// <summary>
        /// select statement
        /// </summary>
        SELECT,

        /// <summary>
        /// delete statement
        /// </summary>
        DELETE,

        /// <summary>
        /// update statement
        /// </summary>
        UPDATE,

        /// <summary>
        /// insert statement
        /// </summary>
        INSERT
    }
}
