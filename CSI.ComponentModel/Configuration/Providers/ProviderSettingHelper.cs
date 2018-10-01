using System;
using System.Configuration;

namespace CSI.Configuration.Providers
{
    public class ProviderSettingHelper
    {
        public static int IndexOf(ProviderSettingsCollection settings, ProviderSettings setting)
        {
            for (int i = 0; i < settings.Count; i++)
            {
                if (settings[i].Name == setting.Name)
                {
                    return i;
                }
            }
            return -1;
        }

        public static void Remove(ProviderSettingsCollection settings, ProviderSettings setting)
        {
            settings.Remove(setting.Name);
        }
    }
}

