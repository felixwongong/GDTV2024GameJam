using System;
using UnityEngine;

namespace Otter.MonsterChess.Skill
{
    [CreateAssetMenu(fileName = "Area_SingleTarget", menuName = "Area/Single Target", order = 0)]
    public class SingleTargetArea : AxialArea
    {
        public override AxialCoord[] getArea(AxialCoord target)
        {
            return new[] { target };
        }

        public override AxialCoord[] getArea(AxialCoord target, AxialCoord direction)
        {
            return getArea(target);
        }
    }
}