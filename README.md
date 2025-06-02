# LTI 1.0/1.1/2.0 is legacy and no longer supported
> [!WARNING]  
> This repo is soley maintained to support organisations that are still forced to deal with legacy LTI implementations. All current .Net development should be based on [LtiAdvantage](https://github.com/LtiLibrary/LtiAdvantage) the successor of this repo for LTI 1.3/LtiAdvantage [[1EdTech](https://www.1edtech.org/standards/lti)]

In [1EdTech/IMS Global's LTI Security Bulletin from July 2019](https://www.1edtech.org/lti-security-announcement-and-deprecation-schedule), LTI 1.0/1.1/2.0 were deprecated as they relied on obsolete and no longer secure standards. They ended support three years ago on June 30, 2022:

> All support for LTI versions 1.0, 1.1, 1.1.1, 1.1.2, 1.2, and 2.0 will end on June 30, 2022.
> All institutions and suppliers should adopt LTI 1.3 and LTI Advantage for teaching and learning integration to receive its many benefits.



# LTI Library
> [!WARNING]  
> This repo is deprecated, see notice at the top, all current .Net development should be based on [LtiAdvantage](https://github.com/LtiLibrary/LtiAdvantage)

There are two .NET Standard 1.6/2.0 projects in this solution to support IMS LTI Tool Providers and Tool Consumers. The Visual Studio solution and project files are compatible with Visual Studio 2019.

See the [LtiLibrary1.6 repository](https://github.com/andyfmiller/LtiLibrary1.6) for the last non-.NET Standard source for LtiLibrary.

[![Discord](https://img.shields.io/discord/643735856406069248?label=Discord chat)](https://discord.gg/DQ2t32h)  
![GitHub](https://img.shields.io/github/license/LtiLibrary/LtiLibrary)

## LtiLibrary.NetCore
> [!WARNING]  
> This repo is deprecated, see notice at the top, all current .Net development should be based on [LtiAdvantage](https://github.com/LtiLibrary/LtiAdvantage)

This is the only library you need if you are going to roll your own support for web pages or native apps.

This library includes the classes, properties, and methods to support LTI 1.1.1 launch, outcomes, content items and tool consumer profiles for both Tool Consumers and Tool Providers.

Available on NuGet: [![Nuget](https://img.shields.io/nuget/dt/LtiLibrary.NetCore?label=LtiLibrary.NetCore)](https://www.nuget.org/packages/LtiLibrary.NetCore)

## Ltibrary.AspNetCore
> [!WARNING]  
> This repo is deprecated, see notice at the top, all current .Net development should be based on [LtiAdvantage](https://github.com/LtiLibrary/LtiAdvantage)

This library depends on LtiLibrary.NetCore and adds useful extensions and helper methods for ASP.NET Core 2.0+ applications such as an OutcomesApiController which implements the LTI Outcomes API as a Controller.

Available on NuGet: [![Nuget](https://img.shields.io/nuget/dt/LtiLibrary.NetCore?label=LtiLibrary.AspNetCore)](https://www.nuget.org/packages/LtiLibrary.AspNetCore)

## Test Projects
There are also two xUnit test projects: one for each project above. These can be helpful examples for how to use the libraries.

## Build Status

| CI | Env | Status |
| --- | --- | --- |
| GitHub | Windows | ![GitHub Workflow Status](https://img.shields.io/github/workflow/status/LtiLibrary/LtiLibrary/.NET%20Core)  


## NuGet Status

| Assembly | Status |
| --- | --- | 
| LtiLibrary.NetCore | [![NuGet Status](https://img.shields.io/nuget/v/LtiLibrary.NetCore.svg)](https://www.nuget.org/packages/LtiLibrary.NetCore/) | 
| LtiLibrary.AspNetCore | [![NuGet Status](https://img.shields.io/nuget/v/LtiLibrary.AspNetCore.svg)](https://www.nuget.org/packages/LtiLibrary.AspNetCore/) |
