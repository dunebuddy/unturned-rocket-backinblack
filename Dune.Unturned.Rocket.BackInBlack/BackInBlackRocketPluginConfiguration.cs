using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dune.Unturned.Rocket.BackInBlack
{
    public class BackInBlackRocketPluginConfiguration : IRocketPluginConfiguration
    {
        public bool Enabled { get; set; }
        public bool TimeLimitEnabled { get; set; }
        public int TimeLimit { get; set; }

        public IRocketPluginConfiguration DefaultConfiguration
        {
            get
            {
                return new BackInBlackRocketPluginConfiguration()
                    {
                        Enabled = true,
                        TimeLimitEnabled = true,
                        TimeLimit = 60
                    };
            }
        }
    }
}