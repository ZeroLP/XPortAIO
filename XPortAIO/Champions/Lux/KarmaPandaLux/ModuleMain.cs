using Oasys.Common.Enums.GameEnums;
using Oasys.Common.Extensions;
using Oasys.Common.GameObject.Clients;
using Oasys.Common.GameObject.Clients.ExtendedInstances.Spells;
using Oasys.Common.Menu.ItemComponents;
using Oasys.SDK;
using Oasys.SDK.Events;
using Oasys.SDK.Rendering;
using Oasys.SDK.SpellCasting;
using Oasys.SDK.Tools;
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
        private static SDKSpell Q = new SDKSpell(CastSlot.Q, SpellSlot.Q)
        {
            AllowCollision = (target, collisions) => collisions.Count() <= 1,
            PredictionMode = () => Prediction.MenuSelected.PredictionType.Line,
        };
        private static SDKSpell W = new SDKSpell(CastSlot.W, SpellSlot.W);
        private static SDKSpell E = new SDKSpell(CastSlot.E, SpellSlot.E);
        private static SDKSpell R = new SDKSpell(CastSlot.Q, SpellSlot.Q);

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
            if(Menu.KillStealMenu.GetItem<Switch>("Kill Steal using Q").IsOn
              || Menu.KillStealMenu.GetItem<Switch>("Kill Steal using E").IsOn
              || Menu.KillStealMenu.GetItem<Switch>("Kill Steal using R").IsOn)
            {
                KillSteal();
            }

            if(Menu.MiscMenu.GetItem<Switch>("Automatically Cast W").IsOn)
            {
                if(Menu.MiscMenu.GetItem<Switch>("Use W only on Modes").IsOn)
                {
                    if(Orbwalker.OrbwalkingMode == Orbwalker.OrbWalkingModeType.Combo ||
                        Orbwalker.OrbwalkingMode == Orbwalker.OrbWalkingModeType.LaneClear)
                    {
                        AutoW();
                    }
                }
                else if(Menu.MiscMenu.GetItem<Switch>("Use W only on Modes").IsOn)
                {
                    AutoW();
                }
            }

            if(Orbwalker.OrbwalkingMode == Orbwalker.OrbWalkingModeType.LaneClear)
            {
                LaneClear();
            }

            return Task.CompletedTask;
        }

        private Task GameEvents_OnCreateObject(List<AIBaseClient> callbackObjectList, AIBaseClient callbackObject, float callbackGameTime)
        {
            try
            {
                if (callbackObject == null)
                    return Task.CompletedTask;

                if (!callbackObject.IsObject(ObjectTypeFlag.AIMissileClient))
                    return Task.CompletedTask;

                var missile = callbackObject.As<AIMissileClient>();

                if (missile.Caster != null && missile.Caster.NetworkID != UnitManager.MyChampion.NetworkID /*|| isautoattack*/)
                    return Task.CompletedTask;

                if (missile.SpellData.SpellName.Contains("LuxLightStrikeKugel")
                  || missile.SpellData.SpellName.Contains("Lux_Base_E_mis.troy") || missile.SpellData.SpellName.Contains("LuxLightstrike_tar"))
                    LuxEObject = callbackObject;

                return Task.CompletedTask;
            }
            catch(Exception ex)
            {
                return Task.CompletedTask;
            }
        }

        private Task GameEvents_OnDeleteObject(List<AIBaseClient> callbackObjectList, AIBaseClient callbackObject, float callbackGameTime)
        {
            try
            {
                if (callbackObject == null)
                    return Task.CompletedTask;

                if (!callbackObject.IsObject(ObjectTypeFlag.AIMissileClient))
                    return Task.CompletedTask;

                var missile = callbackObject.As<AIMissileClient>();

                if (missile.Caster != null && missile.Caster.NetworkID != UnitManager.MyChampion.NetworkID)
                    return Task.CompletedTask;

                if (missile.SpellData.SpellName.Contains("LuxLightStrikeKugel")
                  || missile.SpellData.SpellName.Contains("Lux_Base_E_mis.troy") || missile.SpellData.SpellName.Contains("LuxLightstrike_tar"))
                    LuxEObject = null;

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                return Task.CompletedTask;
            }
        }

        private void CoreEvents_OnCoreRender()
        {
            if (Menu.DrawingMenu.GetItem<Oasys.Common.Menu.ItemComponents.Switch>("Draw Q Range").IsOn)
                RenderFactory.DrawNativeCircle(UnitManager.MyChampion.Position, 1300, !Q.SpellClass.IsSpellReady ? SharpDX.Color.Red : SharpDX.Color.LightGreen, 2.5f);

            if (Menu.DrawingMenu.GetItem<Oasys.Common.Menu.ItemComponents.Switch>("Draw W Range").IsOn)
                RenderFactory.DrawNativeCircle(UnitManager.MyChampion.Position, 1175, !W.SpellClass.IsSpellReady ? SharpDX.Color.Red : SharpDX.Color.LightGreen, 2.5f);

            if (Menu.DrawingMenu.GetItem<Oasys.Common.Menu.ItemComponents.Switch>("Draw E Range").IsOn)
                RenderFactory.DrawNativeCircle(UnitManager.MyChampion.Position, 1200, !E.SpellClass.IsSpellReady ? SharpDX.Color.Red : SharpDX.Color.LightGreen, 2.5f);

            if (Menu.DrawingMenu.GetItem<Oasys.Common.Menu.ItemComponents.Switch>("Draw R Range").IsOn)
                RenderFactory.DrawNativeCircle(UnitManager.MyChampion.Position, 3400, !R.SpellClass.IsSpellReady ? SharpDX.Color.Red : SharpDX.Color.LightGreen, 2.5f);
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

            if(useQ && Q.SpellClass.IsSpellReady && ManaManager(SpellSlot.Q))
            {
                var t = Oasys.Common.Logic.TargetSelector.GetBestHeroTarget(null, x => x.Distance <= 1300);

                if (t != null)
                {
                    var pred = Prediction.EB.GetPrediction(t, 
                        new Oasys.Common.Logic.EB.Prediction.Position.PredictionData(Oasys.Common.Logic.EB.Prediction.Position.PredictionData.PredictionType.Linear, 1300, 140, 0, 250, 70));

                    if (pred != null && t.IsAlive && pred.HitChancePercent >= Menu.ComboMenu.GetItem<Counter>("Cast Q if % HitChance").Value)
                        SpellCastProvider.CastSpell(CastSlot.Q, pred.CastPosition);
                }
            }

            if (useE && E.SpellClass.IsSpellReady && ManaManager(SpellSlot.E))
            {
                var t = Oasys.Common.Logic.TargetSelector.GetBestHeroTarget(null, x => x.Distance <= 1200);

                if (t != null)
                {
                    var pred = Prediction.EB.GetPrediction(t,
                        new Oasys.Common.Logic.EB.Prediction.Position.PredictionData(Oasys.Common.Logic.EB.Prediction.Position.PredictionData.PredictionType.Circular, 1200, 950, 0, 250, 275));

                    if (pred != null && t.IsAlive && pred.HitChancePercent >= Menu.ComboMenu.GetItem<Counter>("Cast E if % HitChance").Value)
                        SpellCastProvider.CastSpell(CastSlot.E, pred.CastPosition);
                }
            }

            if(LuxEObject != null && E.SpellClass.IsSpellReady && useE)
            {
                var target = UnitManager.EnemyChampions.Where(t => t.IsAlive && t.Distance(LuxEObject) <= LuxEObject.BoundingRadius);

                if(target.Any() && LuxEObject != null)
                    SpellCastProvider.CastSpell(CastSlot.E);
            }

            if (!useR || !R.SpellClass.IsSpellReady || !ManaManager(SpellSlot.R))
                return;

            var rTarget = Oasys.Common.Logic.TargetSelector.GetBestHeroTarget(null, x => x.Distance <= 3400);
            if (rTarget == null)
                return;

            var rPrediction = Prediction.EB.GetPrediction(rTarget, new Oasys.Common.Logic.EB.Prediction.Position.PredictionData(Oasys.Common.Logic.EB.Prediction.Position.PredictionData.PredictionType.Circular,
                                                                    3400, int.MaxValue, 0, 1000, 150));

            if (rPrediction != null && rTarget.IsAlive && rPrediction.HitChancePercent >= Menu.ComboMenu.GetItem<Counter>("Cast R if % HitChance").Value
                && (rPrediction.GetCollisionObjects<AIHeroClient>().Count() >= sliderR || rPrediction.GetCollisionObjects<AIHeroClient>().Count() == 0))
            {
                SpellCastProvider.CastSpell(CastSlot.R, rPrediction.CastPosition);
            }
        }

        private void KillSteal()
        {
            /*if (Menu.KillStealMenu.GetItem<Switch>("Kill Steal using Q").IsOn)
            {
                if (Q.SpellClass.IsSpellReady)
                {
                    var enemies = UnitManager.Enemies.Where(t => t.IsAlive && t.Distance <= 1300 && DamageLibrary.CalculateDamage(t, true, false, false, false) >= t.Health + (t.NeutralShield + t.MagicalShield + t.PhysicalShield));

                    foreach (var e in enemies)
                    {
                        var pred = Prediction.EB.GetPrediction(e,
                        new Oasys.Common.Logic.EB.Prediction.Position.PredictionData(Oasys.Common.Logic.EB.Prediction.Position.PredictionData.PredictionType.Linear, 1300, 140, 0, 250, 70));

                        if (pred != null && e.IsAlive && pred.HitChancePercent >= Menu.ComboMenu.GetItem<Counter>("Cast Q if % HitChance").Value)
                        {
                            SpellCastProvider.CastSpell(CastSlot.Q, pred.CastPosition);
                        }
                    }
                }
                else
                    return;
            }

            if (Menu.KillStealMenu.GetItem<Switch>("Kill Steal using E").IsOn)
            {
                if (E.SpellClass.IsSpellReady && LuxEObject == null)
                {
                    var enemies = UnitManager.Enemies.Where(t => t.IsAlive && t.Distance <= 1200 && DamageLibrary.CalculateDamage(t, false, false, true, false) >= t.Health + (t.NeutralShield + t.MagicalShield + t.PhysicalShield));

                    foreach (var e in enemies)
                    {
                        var pred = Prediction.EB.GetPrediction(e,
                        new Oasys.Common.Logic.EB.Prediction.Position.PredictionData(Oasys.Common.Logic.EB.Prediction.Position.PredictionData.PredictionType.Circular, 1200, 950, 0, 250, 275));

                        if (pred != null && e.IsAlive && pred.HitChancePercent >= Menu.ComboMenu.GetItem<Counter>("Cast E if % HitChance").Value)
                        {
                            SpellCastProvider.CastSpell(CastSlot.E, pred.CastPosition);
                        }
                    }
                }
                else if (!E.SpellClass.IsSpellReady && LuxEObject != null)
                {
                    var enemies = UnitManager.Enemies.Where(t => t.IsAlive && t.DistanceTo(LuxEObject.Position) <= LuxEObject.BoundingRadius && DamageLibrary.CalculateDamage(t, false, false, true, false) >= t.Health + (t.NeutralShield + t.MagicalShield + t.PhysicalShield));

                    if (enemies.Any() && LuxEObject != null)
                        SpellCastProvider.CastSpell(CastSlot.E);
                }
            }

            if (Menu.KillStealMenu.GetItem<Switch>("Kill Steal using R").IsOn)
            {
                if (R.SpellClass.IsSpellReady)
                {
                    var enemies = UnitManager.Enemies.Where(t => t.IsAlive && t.Distance <= 3400 && DamageLibrary.CalculateDamage(t, false, false, false, true) >= t.Health + (t.NeutralShield + t.MagicalShield + t.PhysicalShield));

                    foreach (var e in enemies)
                    {
                        var pred = Prediction.EB.GetPrediction(e, new Oasys.Common.Logic.EB.Prediction.Position.PredictionData(Oasys.Common.Logic.EB.Prediction.Position.PredictionData.PredictionType.Circular,
                                                                    3400, int.MaxValue, 0, 1000, 150));

                        if (pred != null && e.IsAlive && pred.HitChancePercent >= Menu.ComboMenu.GetItem<Counter>("Cast R if % HitChance").Value)
                        {
                            SpellCastProvider.CastSpell(CastSlot.R, pred.CastPosition);
                        }
                    }
                }
            }*/
        }

        private void AutoW()
        {
            if(W.SpellClass.IsSpellReady)
            {
                var allies = UnitManager.AllyChampions.Where(t => !t.IsMe && Menu.MiscMenu.GetItem<Switch>($"Auto W {t.UnitComponentInfo.SkinName}").IsOn
                                                             && Menu.MiscMenu.GetItem<Counter>("HP % before W").Value >= t.HealthPercent && t.IsAlive
                                                             && t.Distance <= 1175).ToArray();

                if(allies.Any())
                {
                    foreach(var a in allies)
                    {
                        SpellCastProvider.CastSpell(CastSlot.W, a.Position);
                    }
                }
            }
        }

        private void LaneClear()
        {

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
            public static float CalculateDamage(Oasys.Common.GameObject.GameObjectBase target, bool useQ, bool useW, bool useE, bool useR)
            {
                var totaldamage = 0f;

                if (useQ && Q.SpellClass.IsSpellReady)
                    totaldamage = totaldamage + QDamage(target);

                if(useW && W.SpellClass.IsSpellReady)
                    totaldamage = totaldamage + WDamage(target);

                if (useE && E.SpellClass.IsSpellReady)
                    totaldamage = totaldamage + EDamage(target);

                if (useR && R.SpellClass.IsSpellReady)
                    totaldamage = totaldamage + RDamage(target);

                return totaldamage;
            }

            /// <summary>
            /// Calculates the Damage done with useQ
            /// </summary>
            /// <param name="target">The Target</param>
            /// <returns>Returns the Damage done with useQ</returns>
            private static float QDamage(Oasys.Common.GameObject.GameObjectBase target)
            {
                return DamageCalculator.CalculateActualDamage(
                    UnitManager.MyChampion, target,
                    new[] { 0, 60, 110, 160, 210, 260 }[Q.SpellClass.Level] + (UnitManager.MyChampion.UnitStats.TotalMagicDamage * 0.7f));
            }

            /// <summary>
            /// Calculates the Damage done with useW
            /// </summary>
            /// <param name="target">The Target</param>
            /// <returns>Returns the Damage done with useW</returns>
            private static float WDamage(Oasys.Common.GameObject.GameObjectBase target)
            {
                return 0;
            }

            /// <summary>
            /// Calculates the Damage done with useE
            /// </summary>
            /// <param name="target">The Target</param>
            /// <returns>Returns the Damage done with useE</returns>
            private static float EDamage(Oasys.Common.GameObject.GameObjectBase target)
            {
                return DamageCalculator.CalculateActualDamage(
                    UnitManager.MyChampion, target,
                    new[] { 0, 60, 105, 150, 195, 240 }[E.SpellClass.Level] + (UnitManager.MyChampion.UnitStats.TotalMagicDamage * 0.6f));
            }

            /// <summary>
            /// Calculates the Damage done with useR
            /// </summary>
            /// <param name="target">The Target</param>
            /// <returns>Returns the Damage done with useR</returns>
            private static float RDamage(Oasys.Common.GameObject.GameObjectBase target)
            {
                return DamageCalculator.CalculateActualDamage(
                   UnitManager.MyChampion, target,
                   new[] { 0, 300, 400, 500 }[R.SpellClass.Level] + (UnitManager.MyChampion.UnitStats.TotalMagicDamage * 0.75f));
            }
        }
    }
}
