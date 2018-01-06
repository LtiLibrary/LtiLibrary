# LTI Library
There are two .NET Core 2.0 projects in this solution to support IMS LTI Tool Providers and Tool Consumers. The Visual Studio solution and project files are compatible with Visual Studio 2017 (v15.3).

See the [LtiLibrary1.6 repository](https://github.com/andyfmiller/LtiLibrary1.6) for the last non-.NET Core source for LtiLibrary.

## LtiLibrary.NetCore
This is the only library you need if you are going to roll your own support for web pages or native apps.

This library includes the classes, properties, and methods to support LTI 1.x launch, outcomes, content items and tool consumer profiles for both Tool Consumers and Tool Providers.

Available on NuGet: https://www.nuget.org/packages/LtiLibrary.NetCore

## Ltibrary.AspNetCore
This library depends on LtiLibrary.NetCore and adds useful extensions and helper methods for ASP.NET Core 2.0 applications such as an OutcomesApiController which implements the LTI Outcomes API as a Controller.

Available on NuGet: https://www.nuget.org/packages/LtiLibrary.AspNetCore

## Test Projects
There are also two xUnit test projects: one for each project above. These can be helpful examples for how to use the libraries.

## Build Status

| CI | Env | Status |
| --- | --- | --- |
| AppVeyor | Windows | [![Build status](https://ci.appveyor.com/api/projects/status/qpkjtvp91mra9ogr?svg=true)](https://ci.appveyor.com/project/andyfmiller/ltilibrary) |
| travis-ci | Ubuntu | [![Build Status](https://travis-ci.org/andyfmiller/LtiLibrary.svg?branch=master)](https://travis-ci.org/andyfmiller/LtiLibrary) |

## NuGet Status

| Assembly | Status |
| --- | --- | 
| LtiLibrary.NetCore | [![NuGet Status](https://img.shields.io/nuget/v/LtiLibrary.NetCore.svg)](https://www.nuget.org/packages/LtiLibrary.NetCore/) | 
| LtiLibrary.AspNetCore | [![NuGet Status](https://img.shields.io/nuget/v/LtiLibrary.AspNetCore.svg)](https://www.nuget.org/packages/LtiLibrary.AspNetCore/) |
