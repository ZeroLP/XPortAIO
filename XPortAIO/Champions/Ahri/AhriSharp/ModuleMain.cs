using Oasys.Common.Menu;
using Oasys.SDK;
using Oasys.SDK.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oasys.Common.Menu.ItemComponents;

namespace XPortAIO.Champions.Ahri.AhriSharp
{

    [XPModuleInfo("Ahri", moduleName: "AhriSharp", author: "Beaving", PortedPlatformName.LeagueSharp)]
    internal class ModuleMain : XPModuleBase
    {
        private static Helper Helper;

        private SDKSpell _spellQ, _spellW, _spellE, _spellR;

        const float _spellQSpeed = 2600;
        const float _spellQSpeedMin = 400;
        const float _spellQFarmSpeed = 1600;
        const float _spellQAcceleration = -3200;

        public override void Initialize()
        {
            Helper = new Helper();

            if (UnitManager.MyChampion.ModelName != "Ahri")
                return;

            Tab menu = new Tab("AhriSharp");

            //var targetSelectorMenu = new Group("TargetSelector");
            //menu.AddGroup(targetSelectorMenu);

            var comboMenu = new Group("Combo");
            comboMenu.AddItem(new Switch("Use Q", true));
            comboMenu.AddItem(new Switch("Use W", true));
            comboMenu.AddItem(new Switch("Use E", true));
            comboMenu.AddItem(new Switch("Use R", true));
            comboMenu.AddItem(new Switch("Use R only if user initiated", false));
            menu.AddGroup(comboMenu);


            MenuManager.AddTab(menu);
            base.Initialize();
        }

        private Task CoreEvents_OnCoreMainInputAsync()
        {
            return Task.CompletedTask;
        }

        public override void Terminate()
        {
            base.Terminate();
        }
    }
}
