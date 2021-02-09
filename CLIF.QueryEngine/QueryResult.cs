using System;
using System.Collections.Generic;
using System.Text;

namespace CLIF.QueryEngine
{
    /// <summary>
    /// class which delivers the query results
    /// </summary>
    public class QueryResult
    {
        /// <summary>
        /// Was the query successful
        /// </summary>
        public bool Success { get; internal set; }

        /// <summary>
        /// Error during query execution
        /// </summary>
        public Exception Error { get; internal set; }

        /// <summary>
        /// Object which is returned by the query
        /// </summary>
        public object ReturnedObject { get; internal set; }

        /// <summary>
        /// type of the executed query
        /// </summary>
        public QueryTypeEnum QueryType { get; internal set; }
    }
}
