using System;
using Xamarin.Forms;

namespace MyFormsLibrary.Services
{
    public interface IDeviceServiceEx:Prism.Services.IDeviceService
    {
        new T OnPlatform<T>(T iOS, T android, T defaultValue=default(T));
    }

    public class DeviceServiceEx:Prism.Services.DeviceService,IDeviceServiceEx
    {
        public new T OnPlatform<T>(T iOS, T android, T defaultValue = default(T))
        {
            switch(Device.RuntimePlatform){
                case Device.iOS:
                    return iOS;
                case Device.Android:
                    return android;
                default:
                    return defaultValue;
            }
        }
    }
}
