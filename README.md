LTI Library
===========
There are two .NET Core projects in this solution to support IMS LTI Tool Providers and Tool Consumers. See the LtiLibrary1.6 repository for the last non-.NET Core source for LtiLibrary.

## LtiLibrary.NetCore
This is the only library you need if you are going to roll your own support for web pages or native apps.

This library includes the classes, properties, and methods to support LTI 1.x launch, outcomes, content items and tool consumer profiles for both Tool Consumers and Tool Providers.

Available on NuGet: https://www.nuget.org/packages/LtiLibrary.NetCore

## Ltibrary.AspNetCore
This library depends on LtiLibrary.NetCore and adds useful extensions and helper methods for ASP.NET Core applications such as an OutcomesApiController which implements the LTI Outcomes API as a Controller.

Available on NuGet: https://www.nuget.org/packages/LtiLibrary.AspNetCore

## Test Projects
There are also two xUnit test projects: one for each project above. These can be helpful examples for how to use the libraries.

[![Build status](https://ci.appveyor.com/api/projects/status/qpkjtvp91mra9ogr/branch/master?svg=true)](https://ci.appveyor.com/project/andyfmiller/ltilibrary/branch/master)
