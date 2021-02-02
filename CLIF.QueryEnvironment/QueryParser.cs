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
    internal class QueryParser
    {
        internal enum QueryTypeEnum
        {
            SELECT,
            DELETE,
            UPDATE,
            INSERT
        }

        /// <summary>
        /// class to represent the information for update queries
        /// </summary>
        internal class ModifyQueryInformation
        {
            /// <summary>
            /// select query to define entities for update
            /// </summary>
            internal string SelectQuery { get; set; }

            /// <summary>
            /// body for the action to perform on every selected entity
            /// </summary>
            internal string MethodBody { get; set; }

            /// <summary>
            /// object type of the modified objects
            /// </summary>
            internal string ObjectType { get; set; }
        }

        /// <summary>
        /// class to represent the information for an insert statement
        /// </summary>
        internal class InsertQueryInformation
        {
            /// <summary>
            /// type to insert
            /// </summary>
            internal string ObjectType { get; set; }

            /// <summary>
            /// optional method to perform
            /// </summary>
            internal string MethodBody { get; set; }
        }

        /// <summary>
        /// Checks whether the query is a query for select
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        internal QueryTypeEnum GetQueryType(string query)
        {
            string[] parts = query.Split(" ");
            if (parts.Length < 1)
            {
                throw new ArgumentException("Empty Query");
            }
            if (parts[0].Equals("from"))
            {
                return QueryTypeEnum.SELECT;
            }
            else if (parts[0].Equals("delete", StringComparison.InvariantCultureIgnoreCase))
            {
                return QueryTypeEnum.DELETE;
            }
            else if (parts[0].Equals("update"))
            {
                return QueryTypeEnum.UPDATE;
            }
            else if (parts[0].Equals("insert"))
            {
                return QueryTypeEnum.INSERT;
            }
            else
            {
                throw new ArgumentException("Could not detect query type");
            }
        }

        internal string GetLinqStringDelete(string linqDeleteQuery)
        {
            string[] parts = linqDeleteQuery.Split(" ");
            parts[0] = "select";
            return string.Join(" ", parts);
        }

        /// <summary>
        /// returns the select string from the update string
        /// e.g.
        /// input: update Xbim.Ifc4.Kernel.IfcProduct ifcEntity from model.Instances where ifcEntity.Label == 1 {ifcEntity.Name = "new Name";}
        /// </summary>
        /// <param name="linqModify"></param>
        /// <returns>query string to select entities for update</returns>
        internal ModifyQueryInformation GetUpdateInformation(string linqModify)
        {
            string[] selectAndMethod = new string[2];
            int bracketPosition = linqModify.IndexOf('{');
            selectAndMethod[0] = linqModify.Substring(0, bracketPosition);
            selectAndMethod[1] = linqModify.Substring(bracketPosition);
            ModifyQueryInformation result = new ModifyQueryInformation();

            //replace only first update
            string updateString = "update";
            int update = selectAndMethod[0].IndexOf(updateString);
            result.SelectQuery = "from" + selectAndMethod[0].Substring(update + updateString.Length);

            //retrieve data type
            string[] selectQueryParts = result.SelectQuery.Split(" ");
            result.ObjectType = selectQueryParts[1];
            selectQueryParts[1] = string.Empty;
            result.SelectQuery = string.Join(" ", selectQueryParts).Replace("  ", " ") + "select (" + result.ObjectType + ")ifcEntity";

            result.MethodBody = selectAndMethod[1].Substring(1, selectAndMethod[1].Length - 2);
            return result;
        }

        /// <summary>
        /// returns the method body and object type for an insert
        /// </summary>
        /// <param name="linqInsert"></param>
        /// <returns></returns>
        internal string GetInsertInformation(string linqInsert)
        {

        }
    }
}
