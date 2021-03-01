# LINQ IFC Query Engine CLIF

## CLIF in a Nutshell

CLIF is a query engine to read and manipulate IFC files on your local machine. CLIF is based on LINQ and the [xBIM.Essentials](https://github.com/xBimTeam/XbimEssentials) library. With a LINQ query you can filter entities regarding attributes, types, or other parameters. Furthermore, there are LINQ like queries to add, delete, or update entities.

CLIF is fully based on `.Net Core 3.1`. Hence, you can compile the code for Windows, Unix or Mac. 

## Using CLIF

Depending on your requirements, you can either include the CLIF query engine into your project and query your IFC files by creating a new `QueryManager` and call the `PerformQuery` method. The `QueryResult` contains returns information about the success , errors, and, in case of a select query, the returned object.

```C#
string ifcPath = @"TestData\B_Damage_Types.ifc";
CLIF.QueryEngine.QueryManager qManager = new QueryManager(ifcFile);
string ifcQuery = "from ifcEntity in model.Instances where ifcEntity.ExpressType.Name == \"IfcBeam\" select ifcEntity"
CLIF.QueryEngine.QueryResult result = qManager.PerformQuery(ifcQuery);
```

In case of changes in the IFC model, remember to save the new model.

```
qManager.SaveModel("C:\\my\\path\\newmodel.ifc")
```

A second possibility is to use the console application `CLIF.QueryEnvironment` which loads an IFC file and processes a command or text file with multiple commands. The example below shows the command line to execute a single query.

```
QueryEnvironment.exe -i="TestData\B_Damage_Types.ifc" -q="from ifcEntity in model.Instances where ifcEntity.ExpressType.Name == ""IfcBeam"" select ifcEntity" -s="C:\my\path\newmodel.ifc"
```

Furthermore, the command line application offers an interactive user interface to execute queries.

## Using basic LINQ for the IFC files

Please refer to the [Introduction to LINQ queries](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/linq/introduction-to-linq-queries) for basic information about Language Integrated Query (LINQ). All IFC entities are stored in `model.Instances`.  Whereby, `model` is an instance of `Xbim.Common.IModel` (https://github.com/xBimTeam/XbimEssentials/blob/master/Xbim.Common/IModel.cs). Hence, to access the entities, after the `in` statement in the query, `model.Instances` has to follow. 

```C#
from ifcEntity in model.Instances select ifcEntity
```

This query language is case sensitive, which means that `model.instances` with a lower case `i` leads to an error. However, you are free to choose an entity name. Instead of `ifcEntity`, you can use everything else. 

```c#
from myEntity in model.Instances select myEntity
```

Please omit to use statements or keywords from `C#` as name for the entity, for instance, `class`, `enum`, `string`, et cetera. Extension methods from LINQ are not usable. 

In case you need the returned types as another type, you can cast them within the select section

```c#
from ifcEntity in model.Instances where ifcEntity.ExpressType.Name == "IfcBeam" select (Xbim.Ifc4.SharedBldgElements.IfcBeam) ifcEntity
```

or

```c#
from ifcEntity in model.Instances where ifcEntity.ExpressType.Name == "IfcBeam" select ifcEntity as Xbim.Ifc4.SharedBldgElements.IfcBeam
```

This is helpful for encapsulated queries. Think of the case you want to select all entities with a specified type object.

```C#
from ifcProduct in 
(from pr in model.Instances where (pr is Xbim.Ifc4.Interfaces.IIfcProduct select pr as Xbim.Ifc4.Interfaces.IIfcProduct)
where (ifcProduct.IsTypedBy.Any(x => x.RelatingType.Name.ToString().StartsWith("Damage type:"))) select ifcProduct as Xbim.Ifc4.Interfaces.IIfcProduct 
```

If you dive deeper into xBIM, you can also do the same with the Method `OfType` from the `IModel` interface.

```c#
from ifcProduct in (model.Instances.OfType<Xbim.Ifc4.Interfaces.IIfcProduct>()) where (ifcProduct.IsTypedBy.Any(x => x.RelatingType.Name.ToString().StartsWith("Damage type:"))) select ifcProduct as Xbim.Ifc4.Interfaces.IIfcProduct 
```

## LINQ extensions for update, insert and delete

You find a short summary of the CLIF query structure in the remarks of the class `CLIF.QueryEngine.QueryParser`. Check out the class `CLIF.Tests.LinqQueryCollection` for a collection of query examples. Please consider to replace `\"` with `"`.

### Update

The general structure for the `update` query is as follows.

```C#
update [full xBIM type name of the entity to update] [name of the variable] in model.Instances where [condition] { [C# code] }
```

##### [full xBIM type name of the entity to update]

The query needs the exact type of the entities to update. This might be an interface from `Xbim.Essentials` or a full specified type.

##### [name of the variable]

This is the name of the variable or entity, which is used within the where part of the query and within the later code for updating.

##### [condition]

The condition for filtering the elements for updating. Please refer to [Basic LINQ Query Operations (C#)](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/linq/basic-linq-query-operations) for further information about filtering.

##### [C# code]

This source code is meant to update your entity. Please use the `[variable name]`to access a single entity after filtering. **Every line of code has to end with a semicolon**.

##### Example

The following code snippet shows a query, to set the name of all `IfcBeam` entities to `Hello World`.

```C#
update Xbim.Ifc4.SharedBldgElements.IfcBeam beam in model.Instances where beam.ExpressType.Name == "IfcBeam" {beam.Name = "Hello World";};
```

### Insert

The `insert` command is comparable to the update command. The general structure is as follows.

```
insert [full xBIM type name of the entity to insert] [name of the variable] in model.Instances { [C# code] }
```

##### [full xBIM type name of the entity to update]

The query needs the exact type of the entities to update. This might be an interface from `Xbim.Essentials` or a full specified type.

##### [name of the variable]

This is the name of the variable or entity, which is used within the where part of the query and within the later code for updating.

##### [C# code]

This source code is meant to initialize your entity. Please use the `[variable name]`to access the new created entity. **Every line of code has to end with a semicolon**.

##### Example

The example below illustrates a query to add a new proxy with the name `"My new Proxy"`.

```c#
insert Xbim.Ifc4.Kernel.IfcProxy proxy in model.Instances {proxy.Name = "My new Proxy";}
```

### Delete

The `delete` command looks as follows.

```c#
delete [name of the variable] in model.Instances where [condition]
```

##### [name of the variable]

This is the name of the variable or entity, which is used within the where part of the query and within the later code for updating.

##### [condition]

The condition for filtering the elements for updating. Please refer to [Basic LINQ Query Operations (C#)](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/linq/basic-linq-query-operations) for further information about filtering.

##### Example

The example below deletes the entity with the IFC number 9010.

```c#
delete ifcEntity in model.Instances where ifcEntity.EntityLabel == 9010
```

## Further documentation

[CLIF Query Environment](CLIF.QueryEnvironment/Readme.md)

[Developer Documentation](Readme_developer.md)

