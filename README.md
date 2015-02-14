BoundedLayers
=============

BoundedLayers is a C# library to validate and enforce layer boundaries in your solutions.

Installation
------------

Grab the **BoundedLayers** package from NuGet by typing this in the Visual Studio package manager powershell console:

`PM> install-package BoundedLayers`

or by running:

```Shell
cd your-project-directory # the one with the .csproj
mono NuGet.exe install BoundedLayers
```

Description
-----------

To see BoundedLayers in action we first need to introduce a hypothetical solution with multiple layer boundaries, so here it goes.

Imagine you have a solution composed of different apps (or bounded contexts). These apps can be anything from web services, web sites, worker services, command-line apps, etc. Each app has more or less the same structure, a "Core" project for business logic, an "Infrastructure" project for external service and database access, a "Hosting" project for hosting strategies, a "Test" project for your unit test, etc.

Eventually patterns of reusability start to emerge in you apps so you create shared libraries for each project, that is, a "Shared.Core", a "Shared.Infrastructure", "Shared.Hosting", "Shared.Test", etc.

By this point you probably have a clear idea of which projects should reference which other projects and which references are forbidden. A simplified version of this could be:

```
  +-------------+       +-------------+
  | Shared.Core | <---- | Shared.Host |  Shared layer
  +-------------+       +-------------+
         ^   ^                 ^
         |   +-------------+   |
         |                 |   |
  +-------------+       +-------------+
  |  App.Core   | <---- |  App.Host   |  App layer
  +-------------+       +-------------+

       Core                  Host
    comopnents            components
```

In words this can be specified as:
- The "Shared" layer cannot reference any other layer
- The "App" layer can reference only the "Shared" layer
- The "Core" components cannot reference any other component
- The "Host" components can reference only the "Core" component

As the project evolves and more artifacts are added to each project, you'd like to enforce the boundaries and prevent automagic tools like Re-sharper from adding a reference to the wrong project just to satisfy a need of the moment.

Using BoundedLayers this can be specified and enforced as:

```C#
  Layers.Configure()
    .Layer("Shared").HasNoReferences()
    .Layer("App").References("Shared")
    .Component("Core").HasNoReferences()
    .Component("Host").References("Core")
    .Validate(solutionPath)
    .Assert();
```

Where ```solutionPath``` can be either an absolute solution path or a solution path relative to the location of the BoundedLayers.dll assembly (i.e. the ```OutputPath``` of your project).

Ideally you could include this code in a unit test, as in [BoundedLayers.Test/SolutionTest.cs](https://github.com/pzavolinsky/BoundedLayers/tree/master/BoundedLayers.Test/SolutionTest.cs).

Expressions
-----------

Layers, components and project references can be specified using the following matching strategies:
- ```Expression.Type.NamePart```: Matches any part of an project name, where project name parts are the strings delimited by dots. For example, System.Web.Http contains the following parts: "System", "Web" and "Http". This is the default expression strategy.
- ```Expression.Type.RegularExpression```: Uses regular expressions to match project names. Note that the regular expressions have implicit boundaries (```^``` and ```$```) so the expression "Shared" is actually "^Shared$".

You can specify the expression strategy as an argument to the ```Layers.Configure()``` method, for example:

```C#
  Layers.Configure(Expression.Type.RegularExpression)
    .Layer(@"BoundedLayers.*").HasNoReferences()         // anything that starts with "BoundedLayers."
    .Component(@"BoundedLayers").HasNoReferences()       // only the "BoundedLayers" project
    .Component(@".*\.Test").References(@"BoundedLayers") // anything that ends with ".Test" ...
                                                         // ... can reference "BoundedLayers"
    .Validate(solutionPath)
    .Assert();
```
