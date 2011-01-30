using System;

using Microsoft.Xna.Framework.Input;

using EasyConfig;

namespace NePlus.Components.EngineComponents
{
    public class Configuration
    {
        ConfigFile configFile;

        public Configuration()
        {
            configFile = new ConfigFile(@"Content\Config.ini");
        }

        public bool GetBooleanConfig(string settingsGroup, string settingName)
        {
            return configFile.SettingGroups[settingsGroup].Settings[settingName].GetValueAsBool();
        }

        public Buttons GetButtonConfig(string settingsGroup, string settingName)
        {
            string button = configFile.SettingGroups[settingsGroup].Settings[settingName].GetValueAsString();

            return (Buttons)Enum.Parse(typeof(Buttons), button, true);
        }

        public float GetFloatConfig(string settingsGroup, string settingName)
        {
            return configFile.SettingGroups[settingsGroup].Settings[settingName].GetValueAsFloat();
        }
        
        public int GetIntConfig(string settingsGroup, string settingName)
        {
            return configFile.SettingGroups[settingsGroup].Settings[settingName].GetValueAsInt();
        }        

        public Keys GetKeyConfig(string settingsGroup, string settingName)
        {
            string key = configFile.SettingGroups[settingsGroup].Settings[settingName].GetValueAsString();

            return (Keys)Enum.Parse(typeof(Keys), key, true);
        }
    }
}