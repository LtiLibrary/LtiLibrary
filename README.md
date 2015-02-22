LTI Library
===========
There are two .NET projects in this solution to support IMS LTI Tool Providers and Tool Consumers.

## LtiLibrary.Core
This is the only library you need if you are going to roll your own support for ASP.NET and authentication.

This library includes the classes, properties, and methods to support LTI 1.x launch, outcomes, content items and tool consumer profiles for both Tool Consumers and Tool Providers.

Available on NuGet: https://www.nuget.org/packages/LtiLibrary.Core

## LtiLibrary.AspNet
This library depends on LtiLibrary.Core and adds useful extensions and helper methods for ASP.NET applications such as an OutcomesApiController which implements the LTI Outcomes API as a WebApi Controller.

Available on NuGet: https://www.nuget.org/packages/LtiLibrary.AspNet