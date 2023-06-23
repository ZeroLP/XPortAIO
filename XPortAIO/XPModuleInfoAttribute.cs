using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPortAIO
{
    /// <summary>
    /// Cross platform module info attribute for module classes.
    /// </summary>
    internal class XPModuleInfoAttribute : Attribute
    {
        public string ChampionName { get; set; }
        public bool IsChampionModule => !string.IsNullOrEmpty(ChampionName);
        public string ModuleName { get; set; }
        public string ModuleAuthor { get; set; }
        public PortedPlatformName PortedPlatform { get; set; }

        public XPModuleInfoAttribute(string championName, string moduleName, string author, PortedPlatformName portedPlatform)
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
