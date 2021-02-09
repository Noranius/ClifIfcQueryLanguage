using System;
using Xbim.Common;

namespace CLIF.Codegenerator
{
    public class CSharpCodeFactory
    {
        /// <summary>
        /// Generates the source code for the query
        /// based on the provided query string
        /// </summary>
        /// <param name="linqQuery">LINQ query string</param>
        /// <param name="outputClassName">Name of the class for the output</param>
        /// <returns>The final code for the later compilation</returns>
        public string GetSelectQueryCode (string linqQuery, string outputClassName = null, string outputNamespace = null)
        {
            LinqSelectQueryTemplate template = new LinqSelectQueryTemplate();
            template.LinqQuery = linqQuery;
            if (!string.IsNullOrWhiteSpace(outputClassName))
            {
                template.ClassName = outputClassName;
            }
            if (!string.IsNullOrWhiteSpace(outputNamespace))
            {
                template.ClassNamespace = outputNamespace;
            }
            
            return template.TransformText();
        }

        /// <summary>
        /// Generates the source code for a modification or update class
        /// </summary>
        /// <param name="methodBody">Method body within the change method</param>
        /// <param name="outputClassName">name of the class for the output</param>
        /// <param name="outputNamespace">namespace for the output class</param>
        /// <param name="inputParameterType">Type of the input parameter</param>
        /// <returns></returns>
        public string GetUpdateQueryCode (string methodBody, Type inputParameterType, string inputParameterName, string outputClassName = null, string outputNamespace = null)
        {
            LinqModificationTemplate template = new LinqModificationTemplate();
            template.MethodBody = methodBody;
            template.UpdateEntityName = inputParameterName;

            if (inputParameterType.GetInterface(typeof(IPersistEntity).FullName) == null)
            {
                throw new ArgumentException("The type " + inputParameterType.FullName + " does not implement the interface " + typeof(IPersistEntity).FullName);
            }

            template.InputParameterTypeFullName = inputParameterType.FullName;
            if (!string.IsNullOrWhiteSpace(outputClassName))
            {
                template.ClassName = outputClassName;
            }
            if (!string.IsNullOrWhiteSpace(outputNamespace))
            {
                template.ClassNamespace = outputNamespace;
            }
            
            return template.TransformText();
        }
    }
}
