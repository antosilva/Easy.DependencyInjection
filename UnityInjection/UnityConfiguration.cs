using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using Microsoft.Practices.Unity.Configuration;
using Unity;

namespace UnityInjection
{
    public class UnityConfiguration : IUnityConfiguration
    {
        protected class RecursionData
        {
            public AliasElementCollection Aliases;
            public AssemblyElementCollection Assemblies;
            public NamespaceElementCollection Namespaces;
        }

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
            RecursionData data = new();

            LoadContainer(UnityContainer, ConfigFile, containerName, ref data);
        }

        protected static void LoadContainer(IUnityContainer unityContainer,
                                            string configFile,
                                            string containerName,
                                            ref RecursionData data)
        {
            var fileMap = new ExeConfigurationFileMap {ExeConfigFilename = configFile};
            Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

            // Parent configs to load.
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

                    // Recursively load parent files.
                    LoadContainer(unityContainer, baseFile, containerName, ref data);
                }
            }

            UnityConfigurationSection unitySection = (UnityConfigurationSection) configuration.GetSection("unity");

            // Copy aliases.
            IList<AliasElement> aliasElements = data.Aliases?.Where(x => unitySection.TypeAliases.All(y => y.Alias != x.Alias)).ToList();
            if (aliasElements != null)
            {
                foreach (AliasElement aliasElement in aliasElements)
                {
                    unitySection.TypeAliases.Add(new AliasElement {Alias = aliasElement.Alias, TypeName = aliasElement.TypeName});
                }
            }

            // Copy namespaces.
            IList<NamespaceElement> namespaceElements = data.Namespaces?.Where(x => unitySection.Namespaces.All(y => y.Name != x.Name)).ToList();
            if (namespaceElements != null)
            {
                foreach (NamespaceElement namespaceElement in namespaceElements)
                {
                    unitySection.Namespaces.Add(new NamespaceElement {Name = namespaceElement.Name});
                }
            }

            // Copy assemblies.
            IList<AssemblyElement> assembliesElements = data.Assemblies?.Where(x => unitySection.Assemblies.All(y => y.Name != x.Name)).ToList();
            if (assembliesElements != null)
            {
                foreach (AssemblyElement assemblyElement in assembliesElements)
                {
                    unitySection.Assemblies.Add(new AssemblyElement {Name = assemblyElement.Name});
                }
            }

            data.Aliases = unitySection.TypeAliases;
            data.Assemblies = unitySection.Assemblies;
            data.Namespaces = unitySection.Namespaces;

            if (unitySection.Containers.Any(x => x.Name == containerName))
            {
                unityContainer.LoadConfiguration(unitySection, containerName);
            }
        }
    }
}
