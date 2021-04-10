using System;
using NUnit.Framework;
using Unity;

namespace UnityInjection.Tests
{
    public class UnityConfigurationTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase(".\\Unity\\unity.main.config", typeof(Test1))]
        [TestCase(".\\Unity\\unity.dev.config", typeof(Test2))]
        public void Given_unity_config_should_resolve_to_the_right_type(string unityConfig, Type expectedType)
        {
            UnityConfiguration unity = new(unityConfig, "ContTest");

            ITest test = unity.UnityContainer.Resolve<ITest>("Test");
            Type actualType = test.GetType();

            Assert.That(actualType == expectedType);
        }
    }
}