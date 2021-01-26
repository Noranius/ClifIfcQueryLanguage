using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;

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
        public string GetQueryCode (string linqQuery, string outputClassName = null, string outputNamespace = null)
        {
            LinqQueryTemplate template = new LinqQueryTemplate();
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
    }
}
