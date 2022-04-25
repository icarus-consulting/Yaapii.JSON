# Yaapii.JSON
[![Build status](https://ci.appveyor.com/api/projects/status/nmrvv76mehs9hwll/branch/main?svg=true)](https://ci.appveyor.com/project/icarus-consulting/yaapii-json/branch/main)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg?style=flat-square)](http://makeapullrequest.com)
[![EO principles respected here](http://www.elegantobjects.org/badge.svg)](http://www.elegantobjects.org)

## Repo Guidelines

Mainly responsible for this repository is [jaanpeeter](https://github.com/jaanpeeter).
Please request a review in every single PR from him. 

He will try to review the PRs within **1 week** and merge applied PRs within **2 weeks** with a new release. Main review day is thursday.

# Overview
Parse and validate JSON data using JSONPath.

With this module you can:

- Parse JSON data
- Patch JSON data
- Slice portions of JSON data using [JSONPath](https://goessner.net/articles/JsonPath/)
- Validate JSON data against a [https://json-schema.org](JSON Schema)

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
    string value = new JSON(data).Value("addresses[0].type"));
```

Retrieve multiple values:
```csharp
    IList<string> values = json.Values("addresses[*].type"); //result List will be readonly
```

Retrieve a single node:
```csharp
    IJSON node = json.Node("addresses[0]");
```

Retrieve multiple nodes:
```csharp
    IList<IJSON> nodes = json.Nodes("addresses[*]"); //result List will be readonly
```

Retrieve the raw token from the IJSON:
```csharp
    JToken token = json.Token(); //JToken is part of NewtonSoft.Json package
```

## Patch
```csharp
var patched = new JSONPatched(unpatched,$"$.addresses[0].name","Quick Stop");
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

# Contribute
# Elegant Objects Rules
This project respects the *Elegant Objects Rules*. These rules are suggested in the two "[Elegant Objects](https://www.amazon.de/Elegant-Objects-Yegor-Bugayenko/dp/1519166915)" books.
	
Here is an overview:
- [no null](http://www.yegor256.com/2014/05/13/why-null-is-bad.html)
- [no getters or setters](http://www.yegor256.com/2014/09/16/getters-and-setters-are-evil.html)
- [no code execution in ctors](http://www.yegor256.com/2015/05/07/ctors-must-be-code-free.html)
- [no mutable objects](http://www.yegor256.com/2014/06/09/objects-should-be-immutable.html)
- [no static methods](http://www.yegor256.com/2014/05/05/oop-alternative-to-utility-classes.html)
- [no type casting](http://www.yegor256.com/2015/04/02/class-casting-is-anti-pattern.html)
- [implementation inheritance](http://www.yegor256.com/2016/09/13/inheritance-is-procedural.html)
- [no data transfer objects](http://www.yegor256.com/2016/07/06/data-transfer-object.html)
- Four member variables maximum
- Five public methods maximum
- Strict method naming
- Every type is an interface
- No code execution in constructors
- No class inheritance, except for design pattern "Envelopes"
- [and more](http://www.yegor256.com/2014/09/10/anti-patterns-in-oop.html)

