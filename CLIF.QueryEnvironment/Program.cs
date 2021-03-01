using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

using CommandLine;

using CLIF.QueryEngine;
using System.IO;

namespace CLIF.QueryEnvironment
{
    public class Program
    {
        /// <summary>
        /// run options for this program
        /// </summary>
        private static ClifQueryOptions runOptions;

        /// <summary>
        /// parsing of the options successful
        /// </summary>
        private static bool optionParseSuccess = true;

        /// <summary>
        /// query engine for processing the queries
        /// </summary>
        private static QueryManager engine;

        /// <summary>
        /// List of parsing errors
        /// </summary>
        private static List<Error> parseErrors;

        /// <summary>
        /// maximum length of query to display as info
        /// </summary>
        private static int maximumQueryLengthToDisplay = 80;

        public static void Main(string[] args)
        {
            //Parameter parsen und auswerten
            ParserResult<ClifQueryOptions> parserResult =
                Parser.Default.ParseArguments<ClifQueryOptions>(args).WithParsed(RunOptions).WithNotParsed(HandleParseError);

            if (Program.optionParseSuccess)
            {
                try
                {
                    //start query engine
                    if (Program.engine == null)
                    {
                        Program.engine = new QueryManager(runOptions.IfcPath);
                    }
                    
                    //process single query
                    if (Program.runOptions.HasSingleQuery)
                    {
                        Program.ProcessQuery(Program.runOptions.LinqQueryString);
                    }
                    else if (Program.runOptions.HasFile)
                    {
                        //open the file
                        using (StreamReader reader = new StreamReader(Program.runOptions.QueryFile))
                        {
                            int lineCounter = 1;
                            while (!reader.EndOfStream)
                            {
                                string query = reader.ReadLine();
                                Program.ProcessQuery(query, lineCounter);
                                lineCounter++;
                            }
                        }
                    }
                    else if (Program.runOptions.UserInputInterface)
                    {
                        string query = string.Empty;
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine("CLIF Query Environment");
                        Console.WriteLine("Operating on " + Path.GetFullPath(Program.runOptions.IfcPath));
                        bool exit = false;
                        while (!exit)
                        {
                            Console.ForegroundColor = ConsoleColor.Gray;
                            Console.Write("CLIF>> ");
                            query = Console.ReadLine();
                            if (string.IsNullOrWhiteSpace(query))
                            {
                                continue;
                            }
                            else if (query.Equals("exit"))
                            {
                                exit = true;
                            }
                            else
                            {
                                try
                                {
                                    Program.ProcessQuery(query);
                                }
                                catch (Exception ex)
                                {
                                    Program.ReportError(ex.ToString());
                                }
                            }
                        }
                    }
                    else
                    {
                        Program.ReportError("Please use either a single command (-q), a query file (-f) or the user interface (-u)");
                        return;
                    }
                    
                    //save the results
                    if (Program.runOptions.SaveFile)
                    {
                        Program.engine.SaveModel(Program.runOptions.PathToSave);
                    }
                }
                catch (Exception ex)
                {
                    Program.ReportError(ex.ToString());
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="errors"></param>
        private static void HandleParseError(IEnumerable<Error> errors)
        {
            Program.optionParseSuccess = false;
            Program.parseErrors = errors.ToList();
        }

        private static void RunOptions(ClifQueryOptions options)
        {
            Program.runOptions = options;
            if (!Program.runOptions.HasQuery
                && !Program.runOptions.UserInputInterface)
            {
                Program.ReportError("Neither a query file, a single query, nor the user input has been provided. Please check the help via --help");
                Program.optionParseSuccess = false;
                return;
            }

            if (Program.runOptions.HasSingleQuery && (Program.runOptions.HasFile || Program.runOptions.UserInputInterface)
                || Program.runOptions.HasFile && Program.runOptions.UserInputInterface)
            {
                Program.ReportError("Please provide either a single query, a query file or use the user interface, but not multiple in parallel");
                Program.optionParseSuccess = false;
                return;
            }

            //Linq="..."
            if (Program.runOptions.HasSingleQuery && Program.runOptions.LinqQueryString[0] == '=')
            {
                Program.runOptions.LinqQueryString = Program.runOptions.LinqQueryString.Substring(1);
            }

            if (Program.runOptions.HasFile && Program.runOptions.QueryFile[0] == '=')
            {
                Program.runOptions.QueryFile = Program.runOptions.QueryFile.Substring(1).Replace("\"", string.Empty);
            }

            Program.runOptions.IfcPath = Program.TrimPath(runOptions.IfcPath); 
            Program.runOptions.PathToSave = Program.TrimPath(Program.runOptions.PathToSave);

            Program.optionParseSuccess = true;
        }

        /// <summary>
        /// Trims a path to the proper style
        /// </summary>
        /// <param name="pathToTrim"></param>
        /// <returns></returns>
        private static string TrimPath(string pathToTrim)
        {
            string result = pathToTrim;
            if (!string.IsNullOrWhiteSpace(result))
            {
                if (result[0] == '=')
                {
                    result = result.Substring(1);
                }
                return result.Replace("\"", string.Empty);
            }
            return pathToTrim;
        }

        /// <summary>
        /// processes a single query
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        private static void ProcessQuery (string query, int lineCounter = -1)
        {
            if (Program.runOptions.Verbose)
            {

                if (lineCounter != -1)
                {
                    Program.ReportVerbose("Processing query file line " + lineCounter);
                }

                Program.ReportVerbose("Processed query: ");
                if (query.Length > Program.maximumQueryLengthToDisplay)
                {
                    Program.ReportVerbose(query.Substring(0, Program.maximumQueryLengthToDisplay) + "...");
                }
                else
                {
                    Program.ReportVerbose(query);
                } 
            }

            QueryResult queryResult = Program.engine.PerformQuery(query);

            if (!queryResult.Success)
            {
                if (queryResult.Error != null)
                {
                    Program.ReportError("Error at executing query. " + queryResult.Error.ToString());
                }
                return;
            }

            if (queryResult.ReturnedObject != null)
            {
                if (queryResult.ReturnedObject is IEnumerable resultList)
                {
                    int resultCount = 0;
                    foreach (var resultEntity in resultList)
                    {
                        Program.ReportQueryResult(resultEntity.ToString());
                        resultCount++;
                    }

                    if (resultCount < 1)
                    {
                        Program.ReportQueryResult("Info: Empty list returned. Consider to check the query.");
                    }
                }
                else if (queryResult.ReturnedObject is Xbim.Common.IPersistEntity)
                {
                    Program.ReportQueryResult(queryResult.ReturnedObject.ToString());
                }
            }
            else
            {
                Program.ReportQueryResult("Query sucessful executed.");
            }
        }

        /// <summary>
        /// print error on screen
        /// </summary>
        /// <param name="errorText"></param>
        private static void ReportError(string errorText)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine(errorText);
        }

        private static void ReportQueryResult (string result)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(result);
        }

        /// <summary>
        /// return information
        /// </summary>
        /// <param name="info"></param>
        private static void ReportVerbose(string info)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(info);
        }
    }
}
