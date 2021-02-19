# Behind the scenes

This section should help developers to understand the software structure and ideas behind CLIF.

## Idea

LINQ offers a comprehensive and elegant approach to query object collections. Furthermore, C# (and also other languages) offer the possibility wo generate and compile code on runtime. So, for the `select` queries, the LINQ select statement is embedded in a C# library, compiled and executed.

The downside is that LINQ does not offer queries for updating, inserting and deleting. For those statements, extensions were necessary.

## Process pipeline

![process pipeline](C:\Users\ripo9018\source\repos\IfcQueryLanguage\figures\process pipeline.svg)

In the first step, a user provides the input, this means the query. Due to the extension of LINQ, the query has to be analyzed and split up into several parts. After that, the code generation and compilation follows. Finally, the results are returned after the execution of the query.

### Query analysis

The query analysis is done by the `CLIF.QueryEngine.QueryParser`. Every query is either a select, insert, update, or delete query. This is decided based on the first word in the query. Based on the query type, several parts are identified. 

In case of a **`select`** query, this is not necessary. Instead, the query is directly forwarded to the code generation.

A **`delete`** query is transformed into a select query, by transforming the `delete` query to a `select` query and delete all of the returned from the list of instances.

For **updating** elements, the query is split into a `select` query to define the elements, which have to be updated. Furthermore, the entity name and type are extracted from the query. Finally, the method body for the updating process is defined.

Lastly, the **`insert`** query is split up into two parts: one for inserting a new element into the list and an **`update` **query, which works on the new generated entity.

### Code Generation

The `CLIF.CodeGenerator` handles the generation of C# source code. Based on the [Text Template Transformation Toolkit (T4)](https://docs.microsoft.com/en-us/visualstudio/modeling/code-generation-and-t4-text-templates?view=vs-2019), the code generator produces C# code. Two templates are part of the library: one template for select queries and one template for update queries. The other queries base on one or both of them. A class for selection implements the interface `CLIF.Common.ISelectEntity`. This interface takes a model to execute the query on. The LINQ query is inserted into that method.

```C#
public class MyQueryClass : ISelectEntity
{
	public IEnumerable<IPersistEntity> Select (IModel model)
    {
        return [query provided by user];
    }
}
```

The update template generates a class, which implements the interface `CLIF.Common.IUpdateEntity`.  This interface has a public method with the entities to update and the related store to start transactions. Furthermore, the generated class has a private method named `InternalUpdateMethod`. This method takes the input type and the method body specified by the user query. Below is the example query

```
update Xbim.Ifc4.SharedBldgElements.IfcBeam beam in model.Instances where beam.ExpressType.Name == \"IfcBeam\" {beam.Name = \"Hello World\";}
```

And this is the code, which is generated from this query.

```C#
public class QueryClass1 : CLIF.Common.IUpdateEntity
{
	public void UpdateEntities(System.Collections.Generic.IEnumerable<Xbim.Common.IPersistEntity> entitiesToUpdate,
    	                       Xbim.Ifc.IfcStore storeWithEntitiesToUpdate)
	{
		storeWithEntitiesToUpdate.ForEach<Xbim.Ifc4.SharedBldgElements.IfcBeam>
        	(entitiesToUpdate.Cast<Xbim.Ifc4.SharedBldgElements.IfcBeam>(), InternalUpdateMethod);
	}

	private void InternalUpdateMethod (Xbim.Ifc4.SharedBldgElements.IfcBeam beam)
	{
		beam.Name = "Hello World";
	}
}
```
### Compilation

On runtime compilation in .Net Core relies on the [Roslyn library](https://docs.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/). `CLIF.LibraryFactoryIfcQueryClassFactory` takes the code from `CLIF.CodeGenerator.CSharpCodeFactory`, produces an assembly with the given class, and stores the class in memory. Important to know: references to external libraries have to be handed over to the method `CSharpCompilation.Create`.

### Query execution

After the compilation of the code, the related interface method is performed.  This part is done by the `CLIF.QueryEngine.QueryManager`.