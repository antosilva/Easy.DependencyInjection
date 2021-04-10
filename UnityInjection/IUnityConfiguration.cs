using Unity;

namespace UnityInjection
{
    public interface IUnityConfiguration
    {
        string ConfigFile { get; }
        string ContainerName { get; }
        IUnityContainer UnityContainer { get; }
    }
}
