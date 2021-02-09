using System;
using System.Collections.Generic;
using System.Text;

namespace CLIF.QueryEngine
{
    /// <summary>
    /// class to represent the information for an insert statement
    /// </summary>
    public class InsertQueryInformation
    {
        /// <summary>
        /// type to insert
        /// </summary>
        public string ObjectType { get; set; }

        /// <summary>
        /// optional method to perform
        /// </summary>
        public string MethodBody { get; set; }
    }
}
