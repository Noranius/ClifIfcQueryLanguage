using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace CLIF.QueryEngine
{
    /// <summary>
    /// Class to split up queries for further usage
    /// </summary>
    /// <remarks>
    /// from ... in ... --> select statement;
    /// delete ... in ... --> delete statement;
    /// insert [insert type] ... in ... { ... }--> add statement
    /// update [update type] ... in ... where ... { ... }--> update statement</remarks>
    public class QueryParser
    {
        /// <summary>
        /// Checks whether the query is a query for select
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public QueryTypeEnum GetQueryType(string query)
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

        /// <summary>
        /// retrieve the data to delete elements
        /// </summary>
        /// <param name="linqDeleteQuery"></param>
        /// <returns></returns>
        public DeleteQueryInformation GetDeleteIInformation(string linqDeleteQuery)
        {
            string[] parts = linqDeleteQuery.Split(" ");
            parts[0] = "from";
            string entityName = null;
            string entityType = null;
            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i] == "in")
                {
                    entityName = parts[i - 1];
                    if (i == 3)
                    {
                        entityType = parts[1];
                    }
                    break;
                }
            }

            if (string.IsNullOrWhiteSpace(entityName))
            {
                throw new ArgumentException("In token not found");
            }

            string selectString;
            if (entityType == null)
            {
                selectString = string.Join(" ", parts) + " select " + entityName;
            }
            else
            {
                selectString = string.Join(" ", parts) + " select (" + entityType + ")" + entityName;
            }
            return new DeleteQueryInformation() { SelectStatement = selectString };
        }

        /// <summary>
        /// returns the select string from the update string
        /// e.g.
        /// input: update Xbim.Ifc4.Kernel.IfcProduct ifcEntity from model.Instances where ifcEntity.Label == 1 {ifcEntity.Name = "new Name";}
        /// </summary>
        /// <param name="linqModify"></param>
        /// <returns>query string to select entities for update</returns>
        public UpdateQueryInformation GetUpdateInformation(string linqModify)
        {
            if (linqModify.IndexOf("{") == -1)
            {
                throw new ArgumentException("Start of Method body not found. The Method body starts with '{'.");
            }

            string[] selectAndMethod = this.SplitByFirstOccurence(linqModify, "{");
            UpdateQueryInformation result = new UpdateQueryInformation();

            //replace only first update
            string updateString = "update";
            int update = selectAndMethod[0].IndexOf(updateString);
            result.SelectQuery = "from" + selectAndMethod[0].Substring(update + updateString.Length);

            //retrieve data type
            string[] selectQueryParts = result.SelectQuery.Split(" ");
            result.ObjectType = selectQueryParts[1];
            result.EntityName = selectQueryParts[2];
            selectQueryParts[1] = string.Empty;
            result.SelectQuery = string.Join(" ", selectQueryParts).Replace("  ", " ") + "select (" + result.ObjectType + ")" + result.EntityName;

            result.MethodBody = selectAndMethod[1].Substring(1, selectAndMethod[1].Length - 2);
            return result;
        }

        /// <summary>
        /// returns the method body and object type for an insert
        /// </summary>
        /// <param name="linqInsert"></param>
        /// <returns></returns>
        public UpdateQueryInformation GetInsertInformation(string linqInsert)
        {
            UpdateQueryInformation result = new UpdateQueryInformation();
            string objectTypeString;

            //extract method body
            if (linqInsert.IndexOf("{") == -1)
            {
                result.MethodBody = string.Empty;
                objectTypeString = linqInsert;
            }
            else
            {
                string[] partsEntireQuery = this.SplitByFirstOccurence(linqInsert, "{");
                result.MethodBody = partsEntireQuery[1].Substring(1, partsEntireQuery[1].Length - 2);
                objectTypeString = partsEntireQuery[0];
            }

            //extract object type and entity name
            string[] partsInsert = objectTypeString.Split(" ");
            result.ObjectType = partsInsert[1];
            result.EntityName = partsInsert[2];
            return result;
        }

        private string[] SplitByFirstOccurence (string sourceString, string delimiter)
        {
            string[] result = new string[2];
            int bracketPosition = sourceString.IndexOf(delimiter);
            result[0] = sourceString.Substring(0, bracketPosition);
            result[1] = sourceString.Substring(bracketPosition);
            return result;
        }
    }
}
