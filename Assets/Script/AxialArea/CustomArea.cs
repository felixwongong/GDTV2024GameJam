using System;
using System.Collections.Generic;
using UnityEngine;

namespace Otter.MonsterChess.Skill
{
    [CreateAssetMenu(fileName = "Area_Custom", menuName = "Area/Custom", order = 1)]
    public class CustomArea: AxialArea
    {
        [SerializeField] public List<AxialCoord> offsets = new();
        
        public override AxialCoord[] getArea(AxialCoord target)
        {
            var targets = new AxialCoord[offsets.Count];
            for (var i = 0; i < offsets.Count; i++)
            {
                targets[i] = target + offsets[i];
            }

            return targets;
        }

        public override AxialCoord[] getArea(AxialCoord target, AxialCoord direction)
        {
            var targets = new AxialCoord[offsets.Count];
            for (var i = 0; i < offsets.Count; i++)
            {
                targets[i] = target + offsets[i].rotateFromTop(direction);
            }

            return targets;
        }
    }
}