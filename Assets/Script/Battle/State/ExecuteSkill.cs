using CofyEngine;
using UnityEngine;

namespace Otter.MonsterChess.Core.State
{
    public class ExecuteSkill: PlayerState
    {
        public override PlayerStateId id => PlayerStateId.ExecuteSkill;
        protected override void StartContext()
        {
            var playerSM = (PlayerStateMachine)stateMachine;

            var skill = Instantiate(playerSM.skill, transform.position, Quaternion.identity);

            FLog.Log("executing skill");
            
            UnityTimeScheduler.instance.AddDelay(3000, () =>
            {
                skill.execute(new SkillExecutionRequest()
                {
                    casterId = playerSM.attachedUnit.id,
                    direction = AxialCoord.Top,
                    triggerPosition = TileManager.instance.tilemap.WorldToCell(skill.transform.position).xy().axial()
                });
                Destroy(skill.gameObject);
            });

            stateMachine.GoToState(PlayerStateId.Movement);
        }
    }
}