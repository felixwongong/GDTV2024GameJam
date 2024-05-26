using CofyEngine;
using UnityEngine;

namespace Otter.MonsterChess.Core.State
{
    public class ExecuteSkill: MonoState<PlayerState>
    {
        public override PlayerState id => PlayerState.ExecuteSkill;
        protected internal override void StartContext(MonoStateMachine<PlayerState> sm, object param)
        {
            var playerSM = (PlayerStateMachine)sm;

            var skill = Instantiate(playerSM.skill, transform.position, Quaternion.identity);

            FLog.Log("executing skill");
            
            UnityTimeScheduler.instance.AddDelay(3000, () =>
            {
                skill.execute(new SkillExecutionRequest()
                {
                    caster = null,
                    direction = AxialCoord.Top,
                    mapData = TileManager.instance._tilemapData,
                    triggerPosition = TileManager.instance.tilemap.WorldToCell(skill.transform.position).xy().axial()
                });
                Destroy(skill.gameObject);
            });

            sm.GoToState(PlayerState.Movement);
        }
    }
}