// ReSharper disable All
namespace TomsToolbox.Desktop.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class PropertyDependencyAttributeTests
    {
        class TestType
        {
            public string Property1 { get; private set; }

            [PropertyDependency("Property1")]
            public string Property2 { get; private set; }

            [PropertyDependency("Property1", "Property2")]
            public string Property3 { get; private set; }

            [PropertyDependency("Property3")]
            public string Property4 { get; private set; }

            [PropertyDependency("Property1")]
            public string Property5 { get; private set; }

            public string Property6 { get; private set; }
        }

        class TestTypeWithRecursion
        {
            [PropertyDependency("Property3")]
            public string Property1 { get; private set; }

            [PropertyDependency("Property1", "Property2")]
            public string Property2 { get; private set; }

            [PropertyDependency("Property2")]
            public string Property3 { get; private set; }
        }

        class TestTypeWithErrors
        {
            public string Property1 { get; private set; }

            [PropertyDependency("Property1")]
            public string Property2 { get; private set; }

            [PropertyDependency("Property4")] // Should raise an error: Property4 does not exist!
            public string Property3 { get; private set; }
        }

        /// <summary>
        ///A test for PropertyDependencyAttribute Constructor
        ///</summary>
        [TestMethod]
        public void PropertyDependencyAttribute_ConstructorTest()
        {
            new PropertyDependencyAttribute("Test");
        }

        /// <summary>
        ///A test for CreateDependencyMapping
        ///</summary>
        [TestMethod]
        public void PropertyDependencyAttribute_CreateDependencyMappingTest()
        {
            var actual = PropertyDependencyAttribute.CreateDependencyMapping(typeof(TestType));

            var expectedKeys = GetPropertyNames(1, 2, 3);
            var actualKeys = actual.Keys.OrderBy(name => name);
            Assert.IsTrue(expectedKeys.SequenceEqual(actualKeys));

            var expectedProperty1Dependencies = GetPropertyNames(2, 3, 4, 5);
            var actualProperty1Dependencies = actual["Property1"].OrderBy(name => name);
            Assert.IsTrue(expectedProperty1Dependencies.SequenceEqual(actualProperty1Dependencies));

            var expectedProperty2Dependencies = GetPropertyNames(3, 4);
            var actualProperty2Dependencies = actual["Property2"].OrderBy(name => name);
            Assert.IsTrue(expectedProperty2Dependencies.SequenceEqual(actualProperty2Dependencies));

            var expectedProperty3Dependencies = GetPropertyNames(4);
            var actualProperty3Dependencies = actual["Property3"].OrderBy(name => name);
            Assert.IsTrue(expectedProperty3Dependencies.SequenceEqual(actualProperty3Dependencies));
        }

        /// <summary>
        ///A test for CreateDependencyMapping; recursion in attribute definition is OK.
        ///</summary>
        [TestMethod]
        public void PropertyDependencyAttribute_CreateDependencyMappingWithRecursionTest()
        {
            var actual = PropertyDependencyAttribute.CreateDependencyMapping(typeof(TestTypeWithRecursion));

            var expectedKeys = GetPropertyNames(1, 2, 3);
            var actualKeys = actual.Keys.OrderBy(name => name);
            Assert.IsTrue(expectedKeys.SequenceEqual(actualKeys));

            var expectedProperty1Dependencies = GetPropertyNames(2, 3);
            var actualProperty1Dependencies = actual["Property1"].OrderBy(name => name);
            Assert.IsTrue(expectedProperty1Dependencies.SequenceEqual(actualProperty1Dependencies));

            var expectedProperty2Dependencies = GetPropertyNames(1, 3);
            var actualProperty2Dependencies = actual["Property2"].OrderBy(name => name);
            Assert.IsTrue(expectedProperty2Dependencies.SequenceEqual(actualProperty2Dependencies));

            var expectedProperty3Dependencies = GetPropertyNames(1, 2);
            var actualProperty3Dependencies = actual["Property3"].OrderBy(name => name);
            Assert.IsTrue(expectedProperty3Dependencies.SequenceEqual(actualProperty3Dependencies));
        }

        /// <summary>
        ///A test for CreateDependencyMapping;
        ///</summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void PropertyDependencyAttribute_CreateDependencyMappingThrowsOnInvalidDefinitionTest()
        {
            var actual = PropertyDependencyAttribute.CreateDependencyMapping(typeof(TestTypeWithErrors));
        }

        [TestMethod]
        public void PropertyDependencyAttribute_GetInvalidDependenciesTest()
        {
            var invalidDependencies = PropertyDependencyAttribute.GetInvalidDependencies(typeof(TestTypeWithErrors)).ToList();
            Assert.AreEqual(1, invalidDependencies.Count);

            var invalidDependency = invalidDependencies.Single();

            Assert.IsTrue(invalidDependency.StartsWith(typeof(TestTypeWithErrors).FullName + ".Property3 "));
            Assert.IsTrue(invalidDependency.EndsWith(" Property4"));
        }

        static IEnumerable<string> GetPropertyNames(params int[] indexes)
        {
            return indexes.Select(i => "Property" + i);
        }
    }
}
