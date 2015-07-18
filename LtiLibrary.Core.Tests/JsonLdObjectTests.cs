using System;
using LtiLibrary.Core.Common;
using LtiLibrary.Core.Tests.SimpleHelpers;
using Xunit;

namespace LtiLibrary.Core.Tests
{
    public class JsonLdObjectTests
    {
        [Fact]
        public void ExternalAndInternalContexts_MatchesReferenceJson()
        {
            var parent = new Parent
            {
                Id = new Uri("http://localhost/test/1"),
                Name = "MyParent",
                Child = new Child
                {
                    Name = "MyChild",
                    GrandChild = new Child
                    {
                        Name = "MyGrandChild"
                    }
                }
            };
            JsonAssertions.AssertSameObjectJson(parent, "ExternalAndInternalContexts");
        }

        [Fact]
        public void ExternalContextOnly_MatchesReferenceJson()
        {
            var parent = new Parent
            {
                Id = new Uri("http://localhost/test/1"),
                Name = "MyParent",
            };
            JsonAssertions.AssertSameObjectJson(parent, "ExternalContextOnly");
        }

        [Fact]
        public void InternalContextOnly_MatchesReferenceJson()
        {
            var child = new Child
            {
                Name = "MyChild",
                GrandChild = new Child
                {
                    Name = "MyGrandChild"
                }
            };
            JsonAssertions.AssertSameObjectJson(child, "InternalContextOnly");
        }
    }

    public class Parent : JsonLdObject
    {
        public Parent() : base()
        {
            ExternalContextId = new Uri("http://localhost/ParentContext");
            Type = "Parent";
        }

        public Child Child { get; set; }

        public string Name { get; set; }
    }

    public class Child : JsonLdObject
    {
        public Child()
        {
            Terms.Add("Child", "ChildTerms");
            Type = "Child";
        }

        public string Name { get; set; }

        public Child GrandChild { get; set; }
    }
}
