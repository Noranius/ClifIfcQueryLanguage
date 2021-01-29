using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace CLIF.QueryEnvironment
{
    /// <summary>
    /// Class to split up queries for further usage
    /// </summary>
    /// <remarks>
    /// from ... in ... --> select statement; = could be prefix
    /// delete ... in ... --> delete statement; no prefix
    /// insert ... in ... --> add statement
    /// update ... in ... set ... --> update statement</remarks>
    public class QueryParser
    {
        private enum QueryTypeEnum
        {
            SELECT,
            DELETE,
            UPDATE,
            INSERT
        }

        /// <summary>
        /// Checks whether the query is a query for select
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        private QueryTypeEnum GetQueryType(string query)
        {
            string[] parts = query.Split(" ");
            if (parts.Length < 1)
            {
                throw new ArgumentException("Empty Query");
            }
            if (parts.Length > 1 && parts[1] == "="
                || parts[0].Contains("select"))
            {
                return QueryTypeEnum.SELECT;
            }
            else if (parts[0].Contains("delete"))
            {
                return QueryTypeEnum.DELETE;
            }
            else if (parts[0].Contains("update"))
            {
                return QueryTypeEnum.UPDATE;
            }
            else if (parts[0].Contains("insert"))
            {
                return QueryTypeEnum.INSERT;
            }
            else
            {
                throw new ArgumentException("Could not detect query type");
            }
        }
    }
}
