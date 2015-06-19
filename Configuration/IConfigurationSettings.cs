namespace Wimt.CachingFramework.Configuration
{
    public interface IConfigurationSettings
    {
        bool IsCachingEnabled();

        bool IsEventLoggingEnabled();
    }
}