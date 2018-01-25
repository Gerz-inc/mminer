using System.Collections.Generic;

namespace fromjson
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
            return "Statistic from JSON";
        }

        public KeyValuePair<double, double> GetDifficultyByName(string coin)
        {
            return plugin.GetCoinDifficultyByName(coin);
        }
    }
}
