using UnityEngine;
using UnityEngine.Pool;

namespace Otter.MonsterChess.Skill
{
    [CreateAssetMenu(fileName = "Area_Circular", menuName = "Area/Circular", order = 1)]
    public class CircularArea : AxialArea
    {
        [Range(1, 10)]
        [SerializeField] 
        private int radius;

        public override AxialCoord[] getArea(AxialCoord target)
        {
            var targets = ListPool<AxialCoord>.Get();
            
            for (int q = -radius; q <= radius; q++)
            {
                for (int r = Mathf.Max(-radius, -q - radius); r <= Mathf.Min(radius, -q + radius); r++)
                {
                    targets.Add(target + new AxialCoord(q, r));
                }
            }
            
            return targets.ToArray();
        }

        public override AxialCoord[] getArea(AxialCoord target, AxialCoord direction)
        {
            return getArea(target);
        }
    }
}