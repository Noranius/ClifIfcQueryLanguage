using System;
using System.IO;
using System.Reflection;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Emit;

using CLIF.Codegenerator;

namespace CLIF.LibraryFactory
{
    public class IfcQueryClassFactory
    {

        private static readonly MetadataReference[] references;

        static IfcQueryClassFactory()
        {
            IfcQueryClassFactory.references = new MetadataReference[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Linq.Enumerable).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Xbim.Ifc.IfcStore).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Xbim.Common.IPersistEntity).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Xbim.Ifc4.EntityFactoryIfc4).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(CLIF.Codegenerator.IIfcSelectQueryClassCreator).Assembly.Location)
            };
        }

        public Assembly GetQueryAssembly(string queryClassName, string classNamespace, string linqQuery)
        {
            CSharpCodeFactory csFactory = new CSharpCodeFactory();
            string classCode = csFactory.GetQueryCode(linqQuery, queryClassName, classNamespace);

            SourceText codeString = SourceText.From(classCode);
            CSharpParseOptions options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp7_3);
            SyntaxTree parsedSyntaxTree = SyntaxFactory.ParseSyntaxTree(codeString, options);

            CSharpCompilation compileResult = CSharpCompilation.Create(classNamespace + ".dll",
                new[] { parsedSyntaxTree },
                references: IfcQueryClassFactory.references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
                    optimizationLevel: OptimizationLevel.Release,
                    assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default));

            MemoryStream tempStream = new MemoryStream();
            EmitResult emitResult = compileResult.Emit(tempStream);
            if (emitResult.Success)
            {
                tempStream.Seek(0, SeekOrigin.Begin);
                Assembly result = System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromStream(tempStream);
                return result; 
            }
            else
            {
                throw new InternalCompilationErrorException(emitResult.Diagnostics.Select(x => x.ToString()));
            }


            //CompilerParameters compileParam = new CompilerParameters();

            //compileParam.GenerateInMemory = true;
            //compileParam.ReferencedAssemblies.Add("System.dll");
            //compileParam.OutputAssembly = Path.Combine(Environment.CurrentDirectory, classNamespace + ".dll");
            //compileParam.GenerateExecutable = false;
            //compileParam.TreatWarningsAsErrors = false;
            ////compileParam.ReferencedAssemblies.Add();

            //CodeDomProvider domProvider = CodeDomProvider.CreateProvider("CSharp");

            //CompilerResults compileResult = domProvider.CompileAssemblyFromSource(compileParam, classCode);

            //if (compileResult.Errors.Count > 0)
            //{
            //    string additionalErrorInformation = string.Join(";\r\n", from CompilerError ex in compileResult.Errors
            //                                                                select ex.ErrorText);

            //    throw new ArgumentException("Error at compiling the provided source code. " + additionalErrorInformation);
            //}
            //return compileResult.CompiledAssembly;
        }
    }
}
