using Oasys.SDK;
using Oasys.SDK.Events;
using Oasys.SDK.Tools;

namespace XPortAIO
{
    public class Main
    {
        [OasysModuleEntryPoint]
        public static void Execute()
        {
            GameEvents.OnGameLoadComplete += GameEvents_OnGameLoadComplete;
        }

        private static Task GameEvents_OnGameLoadComplete()
        {
            XPModuleManager.LoadModules();
            XPModuleManager.InitializeModules();

            return Task.CompletedTask;
        }
    }
}