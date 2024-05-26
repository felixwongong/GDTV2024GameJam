using UnityEngine;

namespace Otter.MonsterChess.Skill
{
    public abstract class AxialArea : ScriptableObject
    {
        public abstract AxialCoord[] getArea(AxialCoord target);
        public abstract AxialCoord[] getArea(AxialCoord target, AxialCoord direction);
    }
}