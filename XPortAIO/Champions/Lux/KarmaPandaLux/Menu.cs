using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Oasys.SDK.Menu;
using Oasys.Common.Menu.ItemComponents;
using Oasys.SDK;

namespace XPortAIO.Champions.Lux.KarmaPanda
{
    internal class Menu
    {
        internal static Oasys.Common.Menu.Group ComboMenu, HarassMenu, LaneClearMenu, KillStealMenu, MiscMenu, DrawingMenu;

        internal static void LoadMenu()
        {
            var mainTab = new Oasys.Common.Menu.Tab("StarBuddy - Lux");

            ComboMenu = new Oasys.Common.Menu.Group("Combo");
            ComboMenu.AddItem(new InfoDisplay() { Information = "Combo Settings" });
            ComboMenu.AddItem(new Switch("Use Q", true));
            ComboMenu.AddItem(new Switch("Use E", true));
            ComboMenu.AddItem(new Switch("Use R", true));
            ComboMenu.AddItem(new Counter("Amount of Enemies before casting R", 3, 1, 5));

            ComboMenu.AddItem(new InfoDisplay() { Information = "Prediction Settings" });
            ComboMenu.AddItem(new Counter("Cast Q if % HitChance", 75, 0, 100));
            ComboMenu.AddItem(new Counter("Cast E if % HitChance", 75, 0, 100));
            ComboMenu.AddItem(new Counter("Cast R if % HitChance", 75, 0, 100));
            mainTab.AddGroup(ComboMenu);

            HarassMenu = new Oasys.Common.Menu.Group("Harass");
            HarassMenu.AddItem(new InfoDisplay() { Information = "Harass Settings" });
            HarassMenu.AddItem(new Switch("Harass using Q", true));
            HarassMenu.AddItem(new Switch("Harass using E", true));
            HarassMenu.AddItem(new InfoDisplay() { Information = "Prediction Settings" });
            HarassMenu.AddItem(new Counter("Cast Q if % HitChance", 75, 0, 100));
            HarassMenu.AddItem(new Counter("Cast E if % HitChance", 75, 0, 100));
            HarassMenu.AddItem(new Counter("Cast R if % HitChance", 75, 0, 100));
            mainTab.AddGroup(HarassMenu);

            LaneClearMenu = new Oasys.Common.Menu.Group("Lane Clear");
            LaneClearMenu.AddItem(new InfoDisplay() { Information = "Lane Clear Settings" });
            LaneClearMenu.AddItem(new Switch("Lane Clear using Q", false));
            LaneClearMenu.AddItem(new Switch("Lane Clear using E", true));
            LaneClearMenu.AddItem(new Switch("Mentally Retarded Mode (Use R)", false));
            LaneClearMenu.AddItem(new Counter("Minions before Q", 1, 1, 2));
            LaneClearMenu.AddItem(new Counter("Minions before E", 3, 1, 6));
            LaneClearMenu.AddItem(new Counter("Minions before R", 4, 1, 10));
            mainTab.AddGroup(LaneClearMenu);

            KillStealMenu = new Oasys.Common.Menu.Group("Kill Steal");
            KillStealMenu.AddItem(new InfoDisplay() { Information = "Kill Steal Settings" });
            KillStealMenu.AddItem(new Switch("Kill Steal using Q", true));
            KillStealMenu.AddItem(new Switch("Kill Steal using E", true));
            KillStealMenu.AddItem(new Switch("Kill Steal using R", true));
            KillStealMenu.AddItem(new InfoDisplay() { Information = "Prediction Settings" });
            KillStealMenu.AddItem(new Counter("Cast Q if % HitChance", 75, 0, 100));
            KillStealMenu.AddItem(new Counter("Cast E if % HitChance", 75, 0, 100));
            KillStealMenu.AddItem(new Counter("Cast R if % HitChance", 75, 0, 100));
            mainTab.AddGroup(KillStealMenu);

            MiscMenu = new Oasys.Common.Menu.Group("Misc");
            MiscMenu.AddItem(new InfoDisplay() { Information = "Mana Manager Settings" });
            MiscMenu.AddItem(new Counter("Mana Manager Q", 25, 0, 100));
            MiscMenu.AddItem(new Counter("Mana Manager W", 25, 0, 100));
            MiscMenu.AddItem(new Counter("Mana Manager E", 25, 0, 100));
            MiscMenu.AddItem(new Counter("Mana Manager R", 25, 0, 100));
            MiscMenu.AddItem(new Switch("Disable Manager Manager in Combo", true));
            MiscMenu.AddItem(new InfoDisplay() { Information = "Misc Settings" });
            MiscMenu.AddItem(new Switch("Automatically Cast W", true));
            MiscMenu.AddItem(new Switch("Use W only on Modes", true));
            MiscMenu.AddItem(new Counter("HP % before W", 25, 0, 100));
            MiscMenu.AddItem(new InfoDisplay() { Information = "Who to use W on?" });
            var allies = UnitManager.AllyChampions.Where(a => !a.IsMe).OrderBy(a => a.UnitComponentInfo.SkinName);
            foreach (var a in allies)
            {
                MiscMenu.AddItem(new Switch($"Auto W {a.UnitComponentInfo.SkinName}", true));
            }
            MiscMenu.AddItem(new Switch("Auto W Self", true));
            mainTab.AddGroup(MiscMenu);

            DrawingMenu = new Oasys.Common.Menu.Group("Drawing");
            DrawingMenu.AddItem(new InfoDisplay() { Information = "Drawing Settings" });
            DrawingMenu.AddItem(new Switch("Draw Q Range", true));
            DrawingMenu.AddItem(new Switch("Draw W Range", true));
            DrawingMenu.AddItem(new Switch("Draw E Range", true));
            DrawingMenu.AddItem(new Switch("Draw R Range", true));
            DrawingMenu.AddItem(new InfoDisplay() { Information = "DamageIndicator" });
            DrawingMenu.AddItem(new Switch("Use Damage Indicator", true));
            DrawingMenu.AddItem(new Switch("Draw Q Damage", true));
            DrawingMenu.AddItem(new Switch("Draw W Damage", false));
            DrawingMenu.AddItem(new Switch("Draw E Damage", true));
            DrawingMenu.AddItem(new Switch("Draw R Damage", true));
            mainTab.AddGroup(DrawingMenu);

            MenuManager.AddTab(mainTab);
        }
    }
}
