using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Oasys.SDK;
using Oasys.SDK.Tools;

namespace XPortAIO
{
    internal class XPModuleManager
    {
        private static readonly List<XPModuleBase> AllLoadedModules = new List<XPModuleBase>();

        private static List<XPModuleBase> LoadedChampionModules => AllLoadedModules.Where(m => m.IsChampionModule).ToList();
        private static List<XPModuleBase> LoadedUtilityModules => AllLoadedModules.Where(m => !m.IsChampionModule).ToList();

        internal static void LoadModules()
        {
            var currentPlayingChampName = UnitManager.MyChampion.ModelName;

            foreach (Type netType in Assembly.GetExecutingAssembly().GetTypes())
            {
                foreach (var customAttribute in netType.CustomAttributes)
                {
                    if (customAttribute.AttributeType.Name == nameof(XPModuleInfoAttribute))
                    {
                        //Check for champion module
                        var moduleChampName = customAttribute.ConstructorArguments[0].Value;
                        if(!string.IsNullOrEmpty((string)moduleChampName))
                        {
                            if(moduleChampName.Equals(currentPlayingChampName))
                            {
                                var initializedModuleObject = (XPModuleBase)Activator.CreateInstance(netType);
                                initializedModuleObject.IsChampionModule = true;
                                AllLoadedModules.Add(initializedModuleObject);
                            }
                        }
                        //Check for utility module
                        else
                        {
                            AllLoadedModules.Add((XPModuleBase)Activator.CreateInstance(netType));
                        }

                        var moduleName =  !string.IsNullOrEmpty((string)customAttribute.ConstructorArguments[1].Value) ? customAttribute.ConstructorArguments[1].Value : moduleChampName;
                        var moduleAuthor = customAttribute.ConstructorArguments[2].Value;
                        var portedPlatform = customAttribute.ConstructorArguments[3].Value;

                        Logger.Log($"Loaded {moduleName} module by {moduleAuthor} from {(PortedPlatformName)portedPlatform}.");
                    }
                }
            }
        }

        internal static void InitializeModules()
        {
            foreach (var mod in AllLoadedModules)
                mod.Initialize();
        }

        internal static void TerminateModules()
        {
            foreach (var mod in AllLoadedModules)
                mod.Terminate();
        }
    }
}
