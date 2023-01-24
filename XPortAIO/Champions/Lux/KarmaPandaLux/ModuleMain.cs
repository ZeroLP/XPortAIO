using Oasys.Common.Enums.GameEnums;
using Oasys.Common.Extensions;
using Oasys.Common.GameObject.Clients;
using Oasys.Common.GameObject.Clients.ExtendedInstances.Spells;
using Oasys.Common.Menu.ItemComponents;
using Oasys.SDK;
using Oasys.SDK.Events;
using Oasys.SDK.Rendering;
using Oasys.SDK.SpellCasting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPortAIO.Champions.Lux.KarmaPanda
{
    //https://github.com/DamnedN00b/EloBuddy-1/blob/master/Lux/Program.cs
    [PortedModuleInfo("Lux", moduleName:"StarBuddy - Lux", author: "KarmaPanda", PortedPlatformName.Elobuddy)]
    internal class ModuleMain : ChampionModuleBase
    {
        private static SpellClass Q = UnitManager.MyChampion.GetSpellBook().GetSpellClass(SpellSlot.Q),
                           W = UnitManager.MyChampion.GetSpellBook().GetSpellClass(SpellSlot.W),
                           E = UnitManager.MyChampion.GetSpellBook().GetSpellClass(SpellSlot.E),
                           R = UnitManager.MyChampion.GetSpellBook().GetSpellClass(SpellSlot.R);

        private AIBaseClient LuxEObject;

        public override void Initialize()
        {
            Menu.LoadMenu();

            CoreEvents.OnCoreMainTick += CoreEvents_OnCoreMainTick;
            GameEvents.OnCreateObject += GameEvents_OnCreateObject;
            GameEvents.OnDeleteObject += GameEvents_OnDeleteObject;
            CoreEvents.OnCoreRender += CoreEvents_OnCoreRender;
            CoreEvents.OnCoreMainInputAsync += CoreEvents_OnCoreMainInputAsync;

            base.Initialize();
        }

        private Task CoreEvents_OnCoreMainInputAsync()
        {
            if (Orbwalker.OrbwalkingMode == Orbwalker.OrbWalkingModeType.Combo)
                Combo();

            return Task.CompletedTask;
        }

        public override void Terminate()
        {
            base.Terminate();
        }

        private Task CoreEvents_OnCoreMainTick()
        {


            return Task.CompletedTask;
        }

        private Task GameEvents_OnCreateObject(List<AIBaseClient> callbackObjectList, AIBaseClient callbackObject, float callbackGameTime)
        {
            if(!callbackObject.IsObject(ObjectTypeFlag.AIMissileClient))
                return Task.CompletedTask;

            var missile = callbackObject.As<AIMissileClient>();

            if (missile.Caster != UnitManager.MyChampion /*|| isautoattack*/)
                return Task.CompletedTask;

            if (missile.SpellData.SpellName.Contains("LuxLightStrikeKugel")
              || missile.SpellData.SpellName.Contains("Lux_Base_E_mis.troy") || missile.SpellData.SpellName.Contains("LuxLightstrike_tar"))
                LuxEObject = callbackObject;

            return Task.CompletedTask;
        }

        private Task GameEvents_OnDeleteObject(List<AIBaseClient> callbackObjectList, AIBaseClient callbackObject, float callbackGameTime)
        {
            if (!callbackObject.IsObject(ObjectTypeFlag.AIMissileClient))
                return Task.CompletedTask;

            var missile = callbackObject.As<AIMissileClient>();

            if (missile.Caster != UnitManager.MyChampion)
                return Task.CompletedTask;

            if (missile.SpellData.SpellName.Contains("LuxLightStrikeKugel")
              || missile.SpellData.SpellName.Contains("Lux_Base_E_mis.troy") || missile.SpellData.SpellName.Contains("LuxLightstrike_tar"))
                LuxEObject = null;

            return Task.CompletedTask;
        }

        private void CoreEvents_OnCoreRender()
        {
            if (Menu.DrawingMenu.GetItem<Oasys.Common.Menu.ItemComponents.Switch>("Draw Q Range").IsOn)
                RenderFactory.DrawNativeCircle(UnitManager.MyChampion.Position, Q.SpellData.CastRange, !Q.IsSpellReady ? SharpDX.Color.Red : SharpDX.Color.LightGreen, 2.5f);

            if (Menu.DrawingMenu.GetItem<Oasys.Common.Menu.ItemComponents.Switch>("Draw W Range").IsOn)
                RenderFactory.DrawNativeCircle(UnitManager.MyChampion.Position, W.SpellData.CastRange, !W.IsSpellReady ? SharpDX.Color.Red : SharpDX.Color.LightGreen, 2.5f);

            if (Menu.DrawingMenu.GetItem<Oasys.Common.Menu.ItemComponents.Switch>("Draw E Range").IsOn)
                RenderFactory.DrawNativeCircle(UnitManager.MyChampion.Position, E.SpellData.CastRange, !E.IsSpellReady ? SharpDX.Color.Red : SharpDX.Color.LightGreen, 2.5f);

            if (Menu.DrawingMenu.GetItem<Oasys.Common.Menu.ItemComponents.Switch>("Draw R Range").IsOn)
                RenderFactory.DrawNativeCircle(UnitManager.MyChampion.Position, R.SpellData.CastRange, !R.IsSpellReady ? SharpDX.Color.Red : SharpDX.Color.LightGreen, 2.5f);
        }

        /// <summary>
        /// Does Combo
        /// </summary>
        private void Combo()
        {
            var useQ = Menu.ComboMenu.GetItem<Switch>("Use Q").IsOn;
            var useE = Menu.ComboMenu.GetItem<Switch>("Use E").IsOn;
            var useR = Menu.ComboMenu.GetItem<Switch>("Use R").IsOn;
            var sliderR = Menu.ComboMenu.GetItem<Counter>("Amount of Enemies before casting R").Value;

            if(useQ && Q.IsSpellReady && ManaManager(SpellSlot.Q))
            {
                var t = TargetSelector.GetBestChampionTarget();

                if(t != null)
                {
                    //Properly port prediction data
                    var pred = Prediction.EB.GetPrediction(t, 
                        new Oasys.Common.Logic.EB.Prediction.Position.PredictionData(Oasys.Common.Logic.EB.Prediction.Position.PredictionData.PredictionType.Linear, 1175, 1300, 0, 250, 70));

                    if (pred != null && t.IsAlive && pred.HitChancePercent >= Menu.ComboMenu.GetItem<Counter>("Cast Q if % HitChance").Value)
                        SpellCastProvider.CastSpell(CastSlot.Q, pred.CastPosition);
                }
            }

            if (useE && E.IsSpellReady && ManaManager(SpellSlot.E))
            {
                var t = TargetSelector.GetBestChampionTarget();

                if (t != null)
                {
                    //Properly port prediction data
                    var pred = Prediction.EB.GetPrediction(t,
                        new Oasys.Common.Logic.EB.Prediction.Position.PredictionData(Oasys.Common.Logic.EB.Prediction.Position.PredictionData.PredictionType.Circular, 1200, 950, 0, 250, 275));

                    if (pred != null && t.IsAlive && pred.HitChancePercent >= Menu.ComboMenu.GetItem<Counter>("Cast E if % HitChance").Value)
                        SpellCastProvider.CastSpell(CastSlot.E, pred.CastPosition);
                }
            }

            if(LuxEObject != null && E.IsSpellReady && useE)
            {
                var target = UnitManager.EnemyChampions.Where(t => t.IsAlive && t.Distance(LuxEObject) <= LuxEObject.BoundingRadius);

                if(target.Any() && LuxEObject != null)
                    SpellCastProvider.CastSpell(CastSlot.E);
            }

            if (!useR || !R.IsSpellReady || !ManaManager(SpellSlot.R))
                return;

            var rTarget = TargetSelector.GetBestChampionTarget();
            if (rTarget == null)
                return;

            //Properly port prediction data
            var rPrediction = Prediction.EB.GetPrediction(rTarget, new Oasys.Common.Logic.EB.Prediction.Position.PredictionData(Oasys.Common.Logic.EB.Prediction.Position.PredictionData.PredictionType.Circular,
                                                                    3300, int.MaxValue, 0, 1000, 150));

            if(rPrediction != null && rTarget.IsAlive && rPrediction.HitChancePercent >= Menu.ComboMenu.GetItem<Counter>("Cast R if % HitChance").Value
                && rPrediction.GetCollisionObjects<AIHeroClient>().Count() >= sliderR)
            {
                SpellCastProvider.CastSpell(CastSlot.R, rPrediction.CastPosition);
            }
        }

        /// <summary>
        /// Gets Lux Passive Damage
        /// </summary>
        /// <param name="target">The Target</param>
        /// <returns>The damage that the passive will do.</returns>
        private float LuxPassiveDamage(AIBaseClient target)
        {
            if(target.BuffManager.HasBuff("luxilluminatingfraulein"))
                return DamageCalculator.CalculateActualDamage(UnitManager.MyChampion, target, (float)(10 + (8 * UnitManager.MyChampion.Level) * (UnitManager.MyChampion.UnitStats.FlatMagicDamageMod * 0.2)));

            return 0;
        }

        /// <summary>
        /// Gets if the Spell should be casted by calculating current mana to ManaManager Limit
        /// </summary>
        /// <param name="spellSlot">The Spell Being calculated</param>
        /// <returns>If the spell should be casted.</returns>
        private bool ManaManager(SpellSlot spellSlot)
        {
            if (Menu.MiscMenu.GetItem<Switch>("Disable Manager Manager in Combo").IsOn
               && Orbwalker.OrbwalkingMode == Orbwalker.OrbWalkingModeType.Combo)
                return true;

            var playerManaPercent = UnitManager.MyChampion.ManaPercent;

            if (spellSlot == SpellSlot.Q)
                return Menu.MiscMenu.GetItem<Counter>("Mana Manager Q").Value < playerManaPercent;

            if (spellSlot == SpellSlot.W)
                return Menu.MiscMenu.GetItem<Counter>("Mana Manager W").Value < playerManaPercent;

            if (spellSlot == SpellSlot.E)
                return Menu.MiscMenu.GetItem<Counter>("Mana Manager E").Value < playerManaPercent;

            if (spellSlot == SpellSlot.R)
                return Menu.MiscMenu.GetItem<Counter>("Mana Manager R").Value < playerManaPercent;

            return false;
        }

        /// <summary>
        /// DamageLibrary Class for Lux Spells.
        /// </summary>
        private static class DamageLibrary
        {
            /// <summary>
            /// Calculates and returns damage totally done to the target
            /// </summary>
            /// <param name="target">The Target</param>
            /// <param name="useQ">Include useQ in Calculations?</param>
            /// <param name="useW">Include useW in Calculations?</param>
            /// <param name="useE">Include useE in Calculations?</param>
            /// <param name="useR">Include useR in Calculations?</param>
            /// <returns>The total damage done to target.</returns>
            public static float CalculateDamage(AIBaseClient target, bool useQ, bool useW, bool useE, bool useR)
            {
                var totaldamage = 0f;

                if (useQ && Q.IsSpellReady)
                    totaldamage = totaldamage + QDamage(target);

                if(useW && W.IsSpellReady)
                    totaldamage = totaldamage + WDamage(target);

                if (useE && E.IsSpellReady)
                    totaldamage = totaldamage + EDamage(target);

                if (useR && R.IsSpellReady)
                    totaldamage = totaldamage + RDamage(target);

                return totaldamage;
            }

            /// <summary>
            /// Calculates the Damage done with useQ
            /// </summary>
            /// <param name="target">The Target</param>
            /// <returns>Returns the Damage done with useQ</returns>
            private static float QDamage(AIBaseClient target)
            {
                return DamageCalculator.CalculateActualDamage(
                    UnitManager.MyChampion, target,
                    new[] { 0, 60, 110, 160, 210, 260 }[Q.Level] + (UnitManager.MyChampion.UnitStats.TotalMagicDamage * 0.7f));
            }

            /// <summary>
            /// Calculates the Damage done with useW
            /// </summary>
            /// <param name="target">The Target</param>
            /// <returns>Returns the Damage done with useW</returns>
            private static float WDamage(AIBaseClient target)
            {
                return 0;
            }

            /// <summary>
            /// Calculates the Damage done with useE
            /// </summary>
            /// <param name="target">The Target</param>
            /// <returns>Returns the Damage done with useE</returns>
            private static float EDamage(AIBaseClient target)
            {
                return DamageCalculator.CalculateActualDamage(
                    UnitManager.MyChampion, target,
                    new[] { 0, 60, 105, 150, 195, 240 }[E.Level] + (UnitManager.MyChampion.UnitStats.TotalMagicDamage * 0.6f));
            }

            /// <summary>
            /// Calculates the Damage done with useR
            /// </summary>
            /// <param name="target">The Target</param>
            /// <returns>Returns the Damage done with useR</returns>
            private static float RDamage(AIBaseClient target)
            {
                return DamageCalculator.CalculateActualDamage(
                   UnitManager.MyChampion, target,
                   new[] { 0, 300, 400, 500 }[R.Level] + (UnitManager.MyChampion.UnitStats.TotalMagicDamage * 0.75f));
            }
        }
    }
}
