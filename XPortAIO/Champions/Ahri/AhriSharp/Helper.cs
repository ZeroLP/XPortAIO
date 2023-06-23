using Oasys.Common.Evade;
using Oasys.Common.GameObject.Clients;
using Oasys.SDK;
using Oasys.SDK.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Oasys.SDK.Menu;
using Oasys.Common.Menu;

namespace XPortAIO.Champions.Ahri.AhriSharp
{
    internal class EnemyInfo
    {
        public AIHeroClient Player;
        public int LastSeen;

        public EnemyInfo(AIHeroClient player)
        {
            this.Player = player;
        }
    }

    internal class Helper
    {
        public IEnumerable<AIHeroClient> EnemyTeam;
        public IEnumerable<AIHeroClient> OwnTeam;
        public List<EnemyInfo> EnemyInfo = new List<EnemyInfo>();

        public Helper()
        {
            OwnTeam = UnitManager.AllyChampions;
            EnemyTeam = UnitManager.EnemyChampions;

            EnemyInfo = EnemyTeam.Select(x => new EnemyInfo(x)).ToList();

            CoreEvents.OnCoreMainTick += CoreEvents_OnCoreMainTick;
        }

        private Task CoreEvents_OnCoreMainTick()
        {
            var time = Utils.TickCount;

            foreach (EnemyInfo enemyInfo in EnemyInfo.Where(x => x.Player.IsVisible))
                enemyInfo.LastSeen = time;

            return Task.CompletedTask;
        }

        public EnemyInfo GetPlayerInfo(AIHeroClient enemy)
        {
            return EnemyInfo.Find(x => x.Player.NetworkID == enemy.NetworkID);
        }

        public float GetTargetHealth(EnemyInfo playerInfo, int additionalTime)
        {
            if(playerInfo.Player.IsVisible)
                return playerInfo.Player.Health;

            var predictedHealth = playerInfo.Player.PredictHealth((int)((Utils.TickCount - playerInfo.LastSeen + additionalTime) / 1000f));

            return predictedHealth > playerInfo.Player.MaxHealth ? playerInfo.Player.MaxHealth : predictedHealth;
        }
    }
}
