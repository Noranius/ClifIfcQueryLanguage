using CommandLine;

namespace CLIF.QueryEnvironment
{
    internal class ClifQueryOptions
    {
        /// <summary>
        /// Query string in the format of LINQ
        /// </summary>
        [Option('l',"Linq", HelpText ="Linq query to execute", Required = true)]
        public string LinqQueryString { get; set; }

        /// <summary>
        /// Path to the IFC file to query
        /// </summary>
        [Option('i', "IFC", HelpText = "Ifc file to query ", Required = true)]
        public string IfcPath { get; set; }
    }
}