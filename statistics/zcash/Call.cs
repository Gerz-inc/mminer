using System.Collections.Generic;

namespace zcash
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
            return "ZCash (ZEC)";
        }

        public KeyValuePair<double, double> GetDifficulty()
        {
            return plugin.GetDifficulty();
        }
    }
}
