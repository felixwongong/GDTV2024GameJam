using CofyEngine;
using CofyEngine.Network;
using UnityEngine;

namespace Otter.MonsterChess.Core.State
{
    public class ExecuteSkill: NetworkState<PlayerState>
    {
        public override PlayerState id => PlayerState.ExecuteSkill;
        protected override void StartContext()
        {
            var playerSM = (PlayerStateMachine)stateMachine;

            var skill = Instantiate(playerSM.skill, transform.position, Quaternion.identity);

            FLog.Log("executing skill");
            
            UnityTimeScheduler.instance.AddDelay(3000, () =>
            {
                skill.execute(new SkillExecutionRequest()
                {
                    caster = playerSM.attachedUnit,
                    direction = AxialCoord.Top,
                    mapData = TileManager.instance._tilemapData,
                    triggerPosition = TileManager.instance.tilemap.WorldToCell(skill.transform.position).xy().axial()
                });
                Destroy(skill.gameObject);
            });

            stateMachine.GoToStateServerRpc(PlayerState.Movement);
        }
    }
}