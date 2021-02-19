using CommandLine;

namespace CLIF.QueryEnvironment
{
    internal class ClifQueryOptions
    {
        /// <summary>
        /// Query string in the format of LINQ
        /// </summary>
        [Option('q',"query", HelpText ="Linq query to execute", Required = false)]
        public string LinqQueryString { get; set; }

        /// <summary>
        /// Path to the IFC file to query
        /// </summary>
        [Option('i', "ifc", HelpText = "Ifc file to query", Required = true)]
        public string IfcPath { get; set; }

        /// <summary>
        /// Path to the IFC file to query
        /// </summary>
        [Option('f', "queryFile", HelpText = "Text file with subsequent queries", Required = false)]
        public string QueryFile { get; set; }

        /// <summary>
        /// Shows if the verbose mode should be used
        /// </summary>
        [Option('v', "verbose", HelpText = "Shows additional processing information", Required = false)]
        public bool Verbose { get; set; }

        /// <summary>
        /// returns if either a file or a single query has been provided
        /// </summary>
        public bool HasQuery
        {
            get
            {
                return this.HasFile || this.HasSingleQuery;
            }
        }

        /// <summary>
        /// returns if there is a query file to process
        /// </summary>
        public bool HasFile
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.QueryFile);
            }
        }

        /// <summary>
        /// Returns if there is a single query to execute
        /// </summary>
        public bool HasSingleQuery
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.LinqQueryString);
            }
        }
    }
}