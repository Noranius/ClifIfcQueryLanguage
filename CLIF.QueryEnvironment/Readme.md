# CLIF Query Environment

The CLIF Query Environment is a command line tool to easily query IFC files.

```
CLIF.QueryEnvironment.exe [options]

[options]:
  -q, --query        Linq query to execute

  -i, --ifc          Required. Ifc file to query

  -f, --queryFile    Text file with subsequent queries
  
  -u, --userInput    Type your queries one after another into the interface

  -v, --verbose      Shows additional processing information

  --help             Display this help screen.

  --version          Display version information.
```

## Single query

Of course, the referenced IFC file is mandatory. You have to provide either a query or a query file. Quotes are used to mark start and end of a command line argument. However, you might need quotes in your query. For this purpose, you can escape quotes by using the double quotes two times `""`.

This example selects all property sets from an IFC file. Have a look at the escaped  `IfcPropertySet`. 

```
CLIF.QueryEnvironment.exe -q="from ifcEntity in model.Instances where ifcEntity.ExpressType.ExpressName == ""IfcPropertySet"" select ifcEntity" -i="Testdata\B_Damage_Types.ifc" -v
```

## Query file

For further automation, you can provide a query file for processing. This file has to be a text file. An example for such a file is part of the test data within `CLIF.Tests`. The content of the file is depicted below. Commands are ended with a new line.

```
from ifcEntity in model.Instances where ifcEntity.ExpressType.ExpressName == "IfcPropertySet" select ifcEntity
from ifcEntity in model.Instances where ifcEntity.ExpressType.Name == "IfcBeam" select ifcEntity
```

## Interactive user interface

By using the option -u and provide a file to operate on, a command line interface is started. You can type there your queries and execute them by hitting return. The user interface starts with the lines.

```
CLIF Query Environment
Operating on [your ifc file]
Clif>> 
```

- Going on from this, just type in your queries and retrieve the results. Use `exit` to leave the environment.