using System;
using System.Collections.Generic;
using System.Linq;

using CommandLine;

using CLIF.Codegenerator;
using CLIF.LibraryFactory;
using System.Reflection;
using Xbim.Ifc;
using Xbim.Common;
using Xbim.Common.Metadata;
using CLIF.Common;

namespace CLIF.QueryEnvironment
{
    public class Program
    {
        static ClifQueryOptions runOptions;
        private static bool optionParseSuccess = true;

        public static void Main(string[] args)
        {
            //Parameter parsen und auswerten
            ParserResult<ClifQueryOptions> parserResult =
                Parser.Default.ParseArguments<ClifQueryOptions>(args).WithParsed(RunOptions).WithNotParsed(HandleParseError);

            if (optionParseSuccess)
            {
                try
                {
                    //Build and compile Source code for query
                    IfcQueryClassFactory classFactory = new IfcQueryClassFactory();
                    string classNameSpace = "Clif.MainProgram";
                    string className = "MainProgramCompilitation";
                    Assembly tempAssembly = classFactory.GetQueryAssembly(className, classNameSpace, classNameSpace, runOptions.LinqQueryString);
                    Type tempType = tempAssembly.GetType(classNameSpace + "." + className);
                    ConstructorInfo constructorInfo = tempType.GetConstructor(Type.EmptyTypes);
                    IIfcSelectQueryClassCreator queryClassInstance = (IIfcSelectQueryClassCreator)constructorInfo.Invoke(null);

                    //load the ifc file
                    IfcStore temporaryStore = IfcStore.Open(runOptions.IfcPath);
                    IEnumerable<IPersistEntity> queryResult = queryClassInstance.Select(temporaryStore.Model);

                    //generate the output
                    IEnumerable<ExpressType> expressTypes = from resultEntity in queryResult select resultEntity.ExpressType;
                    string output = string.Join("\r\n", expressTypes);
                    Console.WriteLine(output);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex.ToString());
                }
            }
        }

        private static void HandleParseError(IEnumerable<Error> errors)
        {
            optionParseSuccess = false;
        }

        private static void RunOptions(ClifQueryOptions options)
        {
            //Linq="..."
            runOptions = options;
            runOptions.LinqQueryString = runOptions.LinqQueryString.Substring(1);
            runOptions.IfcPath = runOptions.IfcPath.Substring(1);
            optionParseSuccess = true;
        }
    }
}
