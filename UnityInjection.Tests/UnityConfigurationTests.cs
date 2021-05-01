using System;
using NUnit.Framework;
using Unity;

namespace UnityInjection.Tests
{
    [TestFixture]
    public class UnityConfigurationTests
    {
        [TestCase(@".\Config\unity.dev.config")]
        public void Given_dev_unity_config_should_resolve_to_dev_values(string unityConfig)
        {
            UnityConfiguration unity = new(unityConfig, "ContTest");

            // Check the dev type (defined in "unity.dev.config")
            ITest test = unity.UnityContainer.Resolve<ITest>("Test");
            Type actualType = test.GetType();
            Assert.That(actualType == typeof(Test2));

            // Check we get the instance values passed to the constructor in the container.
            string value = test.GetValue();
            Assert.That(value == "Test2=john/doe");

            // Check the dev timestamp format (defined and inherited from "unity.base.config")
            string timestamp = unity.UnityContainer.Resolve<string>("TimeStampFormat");
            Assert.That(timestamp == "yyyy-MM-dd HH:mm:ss");
        }

        [TestCase(@".\Config\unity.main.config")]
        public void Given_main_unity_config_should_resolve_to_main_values(string unityConfig)
        {
            UnityConfiguration unity = new(unityConfig, "ContTest");

            // Check the main type (defined in "unity.dev.config" but overriden in "unity.main.config")
            ITest test = unity.UnityContainer.Resolve<ITest>("Test");
            Type actualType = test.GetType();
            Assert.That(actualType == typeof(Test1));

            // Check we get the instance value passed to the constructor in the container.
            string value = test.GetValue();
            Assert.That(value == "Test1=john");

            // Check the main timestamp format (defined in "unity.base.config" but overriden in "unity.main.config")
            string timestamp = unity.UnityContainer.Resolve<string>("TimeStampFormat");
            Assert.That(timestamp == "MAIN-yyyy-MM-dd HH:mm:ss");
        }
    }
}