using System;
using System.Collections;
using CofyEngine;
using UnityEngine;

namespace Otter.MonsterChess.Core.State
{
    public class ExecuteSkill : PlayerState
    {
        public override PlayerStateId id => PlayerStateId.ExecuteSkill;

        protected override void StartContext()
        {
            var playerSM = (PlayerStateMachine)stateMachine;

            var skill = Instantiate(playerSM.skill, transform.position, Quaternion.identity);


            stateMachine.GoToState(PlayerStateId.Movement);
            UnityTimeScheduler.instance.AddDelay(skill.castBackswingSecond * 1000, () =>
            {
                CoroutineExecutor.instance.execute(
                    skill.execute(new SkillExecutionRequest()
                    {
                        casterId = playerSM.attachedUnit.id,
                        direction = AxialCoord.Top,
                        triggerPosition = TileManager.instance.tilemap.WorldToCell(skill.transform.position).xy()
                            .axial()
                    }),
                    () => { skill.recycle(); });
            });
        }
    }
}