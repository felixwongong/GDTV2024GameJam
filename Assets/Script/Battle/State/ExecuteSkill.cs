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
            var unit = playerSM.attachedUnit;

            var skill = Instantiate(playerSM.skill, transform.position, Quaternion.identity);
            skill.gameObject.SetActive(false);
            
            UnityTimeScheduler.instance.AddDelay(skill.castBackswingSecond * 1000, () =>
            {
                skill.gameObject.SetActive(true);
                stateMachine.GoToState(PlayerStateId.Movement);
                CoroutineExecutor.instance.execute(
                    skill.execute(new SkillExecutionRequest()
                    {
                        casterId = playerSM.attachedUnit.id,
                        direction = AxialCoord.Top,
                        triggerPosition = TileManager.instance.tilemap.WorldToCell(skill.transform.position).xy()
                            .axial()
                    }),
                    () => { skill.recycle(); });
            }, percent =>
            {
                unit.progressBar.setFill(1 - percent);
            });
        }
    }
}