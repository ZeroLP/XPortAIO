using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Oasys.SDK;
using Oasys.SDK.Tools;

namespace XPortAIO.Champions
{
    internal class ChampionModuleManager
    {
        private static readonly List<ChampionModuleBase> LoadedChampionModules = new List<ChampionModuleBase>();

        internal static void LoadModules()
        {
            var currentPlayingChampName = UnitManager.MyChampion.ModelName;

            foreach(Type netType in Assembly.GetExecutingAssembly().GetTypes())
            {
                foreach(var customAttribute in netType.CustomAttributes)
                {
                    if (customAttribute.AttributeType.Name == nameof(PortedModuleInfoAttribute))
                    {
                        var moduleChampName = customAttribute.ConstructorArguments[0].Value;

                        if(moduleChampName.Equals(currentPlayingChampName))
                        {
                            var moduleName = customAttribute.ConstructorArguments[1].Value != string.Empty ? customAttribute.ConstructorArguments[1].Value : moduleChampName;
                            var moduleAuthor = customAttribute.ConstructorArguments[2].Value;
                            var portedPlatform = customAttribute.ConstructorArguments[3].Value;

                            LoadedChampionModules.Add((ChampionModuleBase)Activator.CreateInstance(netType));

                            Logger.Log($"Loaded {moduleName} module by {moduleAuthor} from {(PortedPlatformName)portedPlatform}.");
                        }
                    }
                }
            }
        }

        internal static void InitializeModules()
        {
            foreach(var mod in LoadedChampionModules)
                mod.Initialize();
        }

        internal static void TerminateModules()
        {
            foreach(var mod in LoadedChampionModules)
                mod.Terminate();
        }
    }
}
