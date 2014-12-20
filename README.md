### LTI Library
There are four .NET projects in this solution to support IMS LTI Tool Providers and Tool Consumers.

## LtiLibrary.Core
This is the only library you need if you are going to roll your own support for ASP.NET and authentication.

This library includes the classes, properties, and methods to support LTI 1.x launch, outcomes, and tool consumer profiles for both Tool Consumers and Tool Providers.

## LtiLibrary.AspNet
This library depends on LtiLibrary.Core and adds useful extensions and helper methods for ASP.NET applications such as an OutcomesApiController which implements the LTI Outcomes API as a WebApi Controller.

## LtiLibrary.Owin.Security.Lti
This library also depends on LtiLibrary.Core and implements OWIN middleware for LTI. This is useful if you are using OWIN middleware to authenticate your web application and want to add LTI authentication.

## LtiLibrary.AspNet.Identity.Owin
And finally, this library depends on both LtiLibrary.Owin.Security.Lti and LtiLibrary.Core. It provides ASP.NET application support for the OWIN middleware for LTI.
