using System;
using System.IO;
using System.Net;
using NUnit.Framework;

namespace UnityInjection.Tests
{
    [TestFixture]
    public class UnityConfigurationFileProviderTests
    {
        [TestCase(@".\Config")]
        public void Given_folder_should_return_main_unity_config_file_name(string configFolder)
        {
            UnityConfigurationFileProvider unityConfigurationFileProvider = new();
            string mainConfigFile = unityConfigurationFileProvider.GetConfigFile(configFolder);
            Assert.That(mainConfigFile == Path.Combine(configFolder, "unity.main.config"));
        }

        [TestCase(@".\Config")]
        public void Given_folder_should_return_host_or_user_unity_config_file_name(string configFolder)
        {
            string userConfig = Path.Combine(configFolder, $"unity.user.{Environment.UserName.ToLower()}.config");
            CheckConfigFile(userConfig);

            string hostConfig = Path.Combine(configFolder, $"unity.host.{Dns.GetHostName().ToLower()}.config");
            CheckConfigFile(hostConfig);

            void CheckConfigFile(string configFile)
            {
                using (File.Create(configFile)) // Create a dummy file to simulate an existing config.
                {
                    UnityConfigurationFileProvider unityConfigurationFileProvider = new();
                    string mainConfigFile = unityConfigurationFileProvider.GetConfigFile(configFolder);
                    Assert.That(mainConfigFile == configFile);
                }

                File.Delete(configFile);
            }
        }
    }
}
