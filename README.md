# Yaapii.JSON
Parse and validate JSON data using JSONPath.

With this module you can:

- Parse JSON data
- Slice portions of JSON data using [JSONPath](https://goessner.net/articles/JsonPath/)
- Validate JSON data against a [https://json-schema.org/](JSON Schema)

With this module, you can NOT:

- Modify JSON data

# Usage

Given this json:

```json
{
	"name": "jay",
	"purpose": "this json is for testing class JSON which implements IJSON",
	"friends": [
		"Silent Bob"
	],
	"addresses": [
		{
			"name": "Drugstore",
			"type": "default"
		},
		{
			"name": "The mall",
			"type": "alternative"
		}
	]
}
```

## Parse
Parse by creating a new object:
```csharp
    IJSON json = new JSON(jsonString); //multiple ctors for parsing are available
```

## Read

Retrieve a single value:
```csharp
    string value = new JSON(data).Value("addresses[:1].type"));
```

Retrieve multiple values:
```csharp
    IList<string> values = json.Values("addresses[*].type"); //result List will be readonly
```

Retrieve a single node:
```csharp
    IJSON node = json.Node("addresses[:1]");
```

Retrieve multiple nodes:
```csharp
    IList<IJSON> nodes = json.Nodes("addresses[*]"); //result List will be readonly
```

Retrieve the raw token from the IJSON:
```csharp
    JToken token = json.Token(); //JToken is part of NewtonSoft.Json package
```

## Validate

Validate by decorating your object with StrictJSON:
```csharp
    var json = "{ \"test\": \"a word\" }";
    var schema = "{ \"type\": \"object\", \"properties\": { \"test\": { \"type\": \"string\" } } }";

    new StrictJSON(
        new JSON(json),
        schema
    ).Value("test"); //Validation is done when trying to read
```