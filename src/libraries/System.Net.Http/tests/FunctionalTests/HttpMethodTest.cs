// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;

using Xunit;

namespace System.Net.Http.Functional.Tests
{
    public class HttpMethodTest
    {
        public static IEnumerable<object[]> StaticHttpMethods { get;  }

        static HttpMethodTest()
        {
            List<object[]> staticHttpMethods = new List<object[]>
            {
                new object[] { HttpMethod.Get },
                new object[] { HttpMethod.Put },
                new object[] { HttpMethod.Post },
                new object[] { HttpMethod.Delete },
                new object[] { HttpMethod.Head },
                new object[] { HttpMethod.Options },
                new object[] { HttpMethod.Trace }
            };
            AddStaticHttpMethods(staticHttpMethods);
            StaticHttpMethods = staticHttpMethods;
        }

        [Fact]
        public void StaticProperties_VerifyValues_PropertyNameMatchesHttpMethodName()
        {
            Assert.Equal("GET", HttpMethod.Get.Method);
            Assert.Equal("PUT", HttpMethod.Put.Method);
            Assert.Equal("POST", HttpMethod.Post.Method);
            Assert.Equal("DELETE", HttpMethod.Delete.Method);
            Assert.Equal("HEAD", HttpMethod.Head.Method);
            Assert.Equal("OPTIONS", HttpMethod.Options.Method);
            Assert.Equal("TRACE", HttpMethod.Trace.Method);
            Assert.Equal("QUERY", HttpMethod.Query.Method);
        }

        [Fact]
        public void Ctor_ValidMethodToken_Success()
        {
            new HttpMethod("GET");
            new HttpMethod("custom");

            // Note that '!' is the first ASCII char after CTLs and '~' is the last character before DEL char.
            new HttpMethod("validtoken!#$%&'*+-.0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz^_`|~");
        }

        [Fact]
        public void Ctor_NullMethod_Exception()
        {
            AssertExtensions.Throws<ArgumentNullException>("method", () => { new HttpMethod(null); } );
        }

        [Theory]
        [InlineData('(')]
        [InlineData(')')]
        [InlineData('<')]
        [InlineData('>')]
        [InlineData('@')]
        [InlineData(',')]
        [InlineData(';')]
        [InlineData(':')]
        [InlineData('\\')]
        [InlineData('"')]
        [InlineData('/')]
        [InlineData('[')]
        [InlineData(']')]
        public void Ctor_SeparatorInMethod_Exception(char separator)
        {
            Assert.Throws<FormatException>(() => { new HttpMethod("Get" + separator); } );
        }

        [Fact]
        public void Equals_DifferentComparisonMethodsForSameMethods_MethodsConsideredEqual()
        {
            // Positive test cases
            Assert.True(new HttpMethod("GET") == HttpMethod.Get);
            Assert.False(new HttpMethod("GET") != HttpMethod.Get);
            Assert.True((new HttpMethod("GET")).Equals(HttpMethod.Get));

            Assert.True(new HttpMethod("get") == HttpMethod.Get);
            Assert.False(new HttpMethod("get") != HttpMethod.Get);
            Assert.True((new HttpMethod("get")).Equals(HttpMethod.Get));
        }

        [Fact]
        public void Equals_CompareWithMethodCastedToObject_ReturnsTrue()
        {
            object other = new HttpMethod("GET");
            Assert.True(HttpMethod.Get.Equals(other));
            Assert.False(HttpMethod.Get.Equals("GET"));
        }

        [Fact]
        public void Equals_NullComparand_ReturnsFalse()
        {
            Assert.False(null == HttpMethod.Options);
            Assert.False(HttpMethod.Trace == null);
        }

        [Theory]
        [InlineData("GET")]
        [InlineData("get")]
        [InlineData("Get")]
        [InlineData("CUSTOM")]
        [InlineData("cUsToM")]
        public void GetHashCode_CustomStringMethod_SameAsStringToUpperInvariantHashCode(string input)
        {
            HttpMethod method = new HttpMethod(input);
            Assert.Equal(input.ToUpperInvariant().GetHashCode(), method.GetHashCode());
        }

        [Theory]
        [MemberData(nameof(StaticHttpMethods))]
        public void GetHashCode_StaticMethods_SameAsStringToUpperInvariantHashCode(HttpMethod method)
        {
            Assert.Equal(method.ToString().ToUpperInvariant().GetHashCode(), method.GetHashCode());
        }

        [Fact]
        public void GetHashCode_DifferentlyCasedMethod_SameHashCode()
        {
            string input = "GeT";
            HttpMethod method = new HttpMethod(input);
            Assert.Equal(HttpMethod.Get.GetHashCode(), method.GetHashCode());
        }

        [Fact]
        public void ToString_UseCustomStringMethod_SameAsString()
        {
            string custom = "custom";
            HttpMethod method = new HttpMethod(custom);
            Assert.Equal(custom, method.ToString());
        }

        [Fact]
        public void Method_AccessProperty_MatchesCtorString()
        {
            HttpMethod method = new HttpMethod("custom");
            Assert.Equal("custom", method.Method);
        }

        [Fact]
        public void Patch_VerifyValue_PropertyNameMatchesHttpMethodName()
        {
            Assert.Equal("PATCH", HttpMethod.Patch.Method);
        }

        public static IEnumerable<object[]> Parse_UsesKnownInstances_MemberData()
        {
            yield return new object[] { HttpMethod.Connect, nameof(HttpMethod.Connect) };
            yield return new object[] { HttpMethod.Delete, nameof(HttpMethod.Delete) };
            yield return new object[] { HttpMethod.Get, nameof(HttpMethod.Get) };
            yield return new object[] { HttpMethod.Head, nameof(HttpMethod.Head) };
            yield return new object[] { HttpMethod.Options, nameof(HttpMethod.Options) };
            yield return new object[] { HttpMethod.Patch, nameof(HttpMethod.Patch) };
            yield return new object[] { HttpMethod.Post, nameof(HttpMethod.Post) };
            yield return new object[] { HttpMethod.Put, nameof(HttpMethod.Put) };
            yield return new object[] { HttpMethod.Query, nameof(HttpMethod.Query) };
            yield return new object[] { HttpMethod.Trace, nameof(HttpMethod.Trace) };
        }

        [Theory]
        [MemberData(nameof(Parse_UsesKnownInstances_MemberData))]
        public void Parse_KnownMethod_UsesKnownInstances(HttpMethod method, string methodName)
        {
            Assert.Same(method, HttpMethod.Parse(methodName));
            Assert.Same(method, HttpMethod.Parse(methodName.ToUpperInvariant()));
            Assert.Same(method, HttpMethod.Parse(methodName.ToLowerInvariant()));
        }

        [Theory]
        [InlineData("Unknown")]
        [InlineData("custom")]
        public void Parse_UnknownMethod_UsesNewInstances(string method)
        {
            var h = HttpMethod.Parse(method);
            Assert.NotNull(h);
            Assert.NotSame(h, HttpMethod.Parse(method));
        }

        [Theory]
        [InlineData("")]
        [InlineData("    ")]
        public void Parse_Whitespace_ThrowsArgumentException(string method)
        {
            AssertExtensions.Throws<ArgumentException>("method", () => HttpMethod.Parse(method));
        }

        [Theory]
        [InlineData("  GET  ")]
        [InlineData(" Post")]
        [InlineData("Put ")]
        [InlineData("multiple things")]
        public void Parse_InvalidToken_Throws(string method)
        {
            Assert.Throws<FormatException>(() => HttpMethod.Parse(method));
        }

        private static void AddStaticHttpMethods(List<object[]> staticHttpMethods)
        {
            staticHttpMethods.Add(new object[] { HttpMethod.Patch });
        }
    }
}
