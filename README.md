LTI Library
===========
There are two .NET and two .NET Core projects in this solution to support IMS LTI Tool Providers and Tool Consumers.

## LtiLibrary.Core and LtiLibrary.NetCore
This is the only library you need if you are going to roll your own support for ASP.NET and authentication.

This library includes the classes, properties, and methods to support LTI 1.x launch, outcomes, content items and tool consumer profiles for both Tool Consumers and Tool Providers.

Available on NuGet: https://www.nuget.org/packages/LtiLibrary.Core and https://www.nuget.org/packages/LtiLibrary.NetCore

## LtiLibrary.AspNet and Ltibrary.AspNetCore
This library depends on LtiLibrary.Core and adds useful extensions and helper methods for ASP.NET applications such as an OutcomesApiController which implements the LTI Outcomes API as a WebApi Controller.

Available on NuGet: https://www.nuget.org/packages/LtiLibrary.AspNet and https://www.nuget.org/packages/LtiLibrary.AspNetCore

## Test Projects
There are also four xUnit test projects: one for each project above. These can be helpful examples for how to use the four libraries.