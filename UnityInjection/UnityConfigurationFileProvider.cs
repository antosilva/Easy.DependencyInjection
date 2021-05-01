using System;
using System.IO;
using System.Linq;
using System.Net;

namespace UnityInjection
{
    public class UnityConfigurationFileProvider : IUnityConfigurationFileProvider
    {
        public string GetConfigFile(string configFolder = @".\Config")
        {
            string[] candidateConfigs =
            {
                Path.Combine(configFolder, $"unity.user.{Environment.UserName.ToLower()}.config"),
                Path.Combine(configFolder, $"unity.host.{Dns.GetHostName().ToLower()}.config"),
                Path.Combine(configFolder, "unity.main.config"),
            };

            return candidateConfigs.FirstOrDefault(File.Exists);
        }
    }
}
