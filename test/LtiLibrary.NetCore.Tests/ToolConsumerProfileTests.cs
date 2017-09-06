﻿using System;
using LtiLibrary.NetCore.Tests.SimpleHelpers;
using LtiLibrary.NetCore.Common;
using LtiLibrary.NetCore.Lti.v2;
using NodaTime;
using Xunit;


namespace LtiLibrary.NetCore.Tests
{
    public class ToolConsumerProfileTests
    {
        private static readonly Instant DefaultDateTime = Instant.FromUtc(2015, 1, 1, 0, 0);

        [Fact]
        public void ToolConsumerProfile_MatchesReferenceJson()
        {
            var profile = new ToolConsumerProfile
            {
                Id = new Uri("http://lms.example.com/profile/b6ffa601-ce1d-4549-9ccf-145670a964d4"),
                LtiVersion = LtiConstants.LtiVersion,
                Guid = "b6ffa601-ce1d-4549-9ccf-145670a964d4",
                ProductInstance = new ProductInstance
                {
                    Guid = "c86542d5-fde1-4aae-ae18-7018089fddcd",
                    ProductInfo = new ProductInfo
                    {
                        Description = new ProductDescription("A fictitious Learning Management System"),
                        ProductFamily = new ProductFamily
                        {
                            Code = "omega",
                            Vendor = new Vendor
                            {
                                Code = "lms.example.com",
                                Contact = new Contact("support@lms.example.com"),
                                Description = new VendorDescription("A fictitious vendor of a Learning Management System"),
                                VendorName = new VendorName("LMS Corporation"),
                                Timestamp = DefaultDateTime.ToDateTimeUtc(),
                                Website = "http://lms.example.com/products/omega"
                            }
                        },
                        ProductName = new ProductName("Omega LMS"),
                        ProductVersion = "2.3",
                        TechnicalDescription = new ProductTechnicalDescription("LTI 1.2 compliant")
                    },
                    ServiceOwner = new ServiceOwner
                    {
                        Id = new Uri("http://state.university.edu/"),
                        Timestamp = DefaultDateTime.ToDateTimeUtc(),
                        Name = new ServiceOwnerName("State University"),
                        Description = new ServiceOwnerDescription("A fictitious university."),
                        Support = new Contact("techsupport@university.edu")
                    },
                    Support = new Contact("techsupport@university.edu"),
                    ServiceProvider = new ServiceProvider
                    {
                        Id = new Uri("http://yasp.example.com/ServiceProvider"),
                        Guid = "yasp.example.com",
                        Timestamp = DefaultDateTime.ToDateTimeUtc(),
                        ServiceProviderName = new ServiceProviderName("Your Application Service Provider"),
                        Description = new ServiceProviderDescription("YASP is a fictitious application service provider"),
                        Support = new Contact("support@yasp.example.com")
                    }
                },
                CapabilityOffered = new[] { "basic-lti-launch-request" },
                ServiceOffered = new[]
                {
                    new RestService
                    {
                        Id = new Uri("MyLTIService.all", UriKind.Relative),
                        EndPoint = new Uri("http://lms.example.com/ltiservice/my/{contextId}"),
                        Format = new [] { "application/x-vnd.ims.lti.v1.myltiservice+json" },
                        Action = new [] { "GET" }
                    }
                }
            };
            profile.Terms.Add("tcp", "https://lms.example.com/profile/b6ffa601-ce1d-4549-9ccf-145670a964d4");

            JsonAssertions.AssertSameObjectJson(profile, "ToolConsumerProfile");
        }
    }
}
