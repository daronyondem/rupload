using System;
namespace rupload.Services
{
    public interface IJsonConfigService<T>
    {
        System.Threading.Tasks.Task<T> GetSettings();
        System.Threading.Tasks.Task SaveSetting(T configObject);
    }
}
