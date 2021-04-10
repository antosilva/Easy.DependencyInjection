using System;
using System.Configuration;
using System.IO;
using System.Linq;
using Microsoft.Practices.Unity.Configuration;
using Unity;

namespace UnityInjection
{
    public class UnityConfiguration : IUnityConfiguration
    {
        public string ConfigFile { get; }
        public string ContainerName { get; }
        public IUnityContainer UnityContainer { get; }

        public UnityConfiguration(string unityConfigFile, string containerName = "")
        {
            if (!File.Exists(unityConfigFile))
            {
                throw new FileNotFoundException("Unity config file not found", unityConfigFile);
            }

            ConfigFile = unityConfigFile;
            ContainerName = containerName;

            UnityContainer = new UnityContainer();
            AliasElementCollection aliases = null;

            LoadContainer(UnityContainer, ConfigFile, containerName, ref aliases);
        }

        protected static void LoadContainer(IUnityContainer unityContainer,
                                            string configFile,
                                            string containerName,
                                            ref AliasElementCollection aliases)
        {
            var fileMap = new ExeConfigurationFileMap {ExeConfigFilename = configFile};
            Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

            // Base config files to load.
            KeyValueConfigurationElement baseFiles = configuration.AppSettings.Settings["base"];
            if (baseFiles != null)
            {
                string[] files = baseFiles.Value.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
                foreach (string file in files)
                {
                    string baseFile = Path.Combine(Path.GetDirectoryName(configFile), file.Trim());
                    if (!File.Exists(baseFile))
                    {
                        throw new FileNotFoundException(string.Format("Configuration file '{0}' not found", baseFile));
                    }

                    // Recursively load the base unity files.
                    LoadContainer(unityContainer, baseFile, containerName, ref aliases);
                }
            }

            // Get unity section from current config.
            UnityConfigurationSection unitySection = (UnityConfigurationSection) configuration.GetSection("unity");

            // Copy only base aliases to the current unity section.
            if (aliases != null)
            {
                foreach (AliasElement alias in aliases)
                {
                    if (unitySection.TypeAliases.Any(x => x.Alias == alias.Alias))
                    {
                        continue;
                    }

                    unitySection.TypeAliases[alias.Alias] = alias.TypeName;
                }
            }

            // Base aliases to apply.
            aliases = unitySection.TypeAliases;

            // Load the containers from the file unity section.
            if (unitySection.Containers.Any(x => x.Name == containerName))
            {
                unityContainer.LoadConfiguration(unitySection, containerName);
            }
        }
    }
}
