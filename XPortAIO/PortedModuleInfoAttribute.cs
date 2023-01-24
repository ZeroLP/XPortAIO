using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPortAIO
{
    /// <summary>
    /// Champion module info attribute for a module class.
    /// </summary>
    internal class PortedModuleInfoAttribute : Attribute
    {
        public string ChampionName { get; set; }
        public string ModuleName { get; set; }
        public string ModuleAuthor { get; set; }
        public PortedPlatformName PortedPlatform { get; set; }

        public PortedModuleInfoAttribute(string championName, string moduleName, string author, PortedPlatformName portedPlatform)
        {
            ChampionName = championName;
            ModuleName = moduleName;
            ModuleAuthor = author;
            PortedPlatform = portedPlatform;
        }
    }

    internal enum PortedPlatformName
    {
        Elobuddy,
        LeagueSharp
    }
}
