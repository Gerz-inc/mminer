using System.Collections.Generic;

namespace smartcash
{
    public class Call
    {
        private Plugin plugin;

        public Call()
        {
            plugin = new Plugin();
        }

        public void ShowSettings()
        {
            plugin.ShowSettings();
        }

        public string GetPluginName()
        {
            return "SmartCash (SMART)";
        }

        public KeyValuePair<double, double> GetDifficulty()
        {
            return plugin.GetDifficulty();
        }
    }
}
