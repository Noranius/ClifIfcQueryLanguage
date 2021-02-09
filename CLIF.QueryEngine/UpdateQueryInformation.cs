using System;
using System.Collections.Generic;
using System.Text;

namespace CLIF.QueryEngine
{
    /// <summary>
    /// class to represent the information for update queries
    /// </summary>
    public class UpdateQueryInformation
    {
        /// <summary>
        /// select query to define entities for update
        /// </summary>
        public string SelectQuery { get; set; }

        /// <summary>
        /// body for the action to perform on every selected entity
        /// </summary>
        public string MethodBody { get; set; }

        /// <summary>
        /// object type of the modified objects
        /// </summary>
        public string ObjectType { get; set; }

        /// <summary>
        /// name of the entity to update
        /// </summary>
        public string EntityName { get; set; }
    }
}
