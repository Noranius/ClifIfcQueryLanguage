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
        private CSharpCodeFactory internalCodeFactory = new CSharpCodeFactory();

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
                MetadataReference.CreateFromFile(typeof(Xbim.Ifc2x3.Functions).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(CLIF.Common.IIfcSelectQueryClassCreator).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.ComponentModel.INotifyPropertyChanged).Assembly.Location)
            };
        }

        public Assembly GetSelectQueryAssembly(string queryClassName, string classNamespace, string assemblyName, string linqQuery)
        {
            string classCode = this.internalCodeFactory.GetSelectQueryCode(linqQuery, queryClassName, classNamespace);
            return this.GenerateAssemblyFromCode(queryClassName, classNamespace, assemblyName, classCode);
        }

        public Assembly GetUpdateQueryAssembly (string queryClassName, Type inputParameterType, string inputParameterName, 
            string classNamespace, string assemblyName, string methodBody)
        {
            string classCode = this.internalCodeFactory.GetUpdateQueryCode(methodBody, inputParameterType, inputParameterName, queryClassName, classNamespace);
            return this.GenerateAssemblyFromCode(queryClassName, classNamespace, assemblyName, classCode);
        }

        private Assembly GenerateAssemblyFromCode(string queryClassName, string classNamespace, string assemblyName, string sourceCode)
        {
            SourceText codeString = SourceText.From(sourceCode);
            CSharpParseOptions options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp7_3);
            SyntaxTree parsedSyntaxTree = SyntaxFactory.ParseSyntaxTree(codeString, options);

            CSharpCompilation compileResult = CSharpCompilation.Create(assemblyName + ".dll",
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
                throw new InternalCompilationErrorException(emitResult.Diagnostics.Select(x => x.GetMessage()).ToArray());
            }
        }
    }
}
