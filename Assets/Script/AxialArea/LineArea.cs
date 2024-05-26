using System;
using UnityEngine;
using UnityEngine.Pool;

namespace Otter.MonsterChess.Skill
{
    [CreateAssetMenu(fileName = "Area_Line", menuName = "Area/Line", order = 0)]
    public class LineArea : AxialArea
    {
        public Direction direction;
        public int length;
        [Range(1, 10)] public int radius;

        public override AxialCoord[] getArea(AxialCoord target)
        {
            var targets = ListPool<AxialCoord>.Get();

            forEachCoordInArea(target, coord => targets.Add(coord));

            return targets.ToArray();
        }

        public override AxialCoord[] getArea(AxialCoord target, AxialCoord direction)
        {
            var targets = ListPool<AxialCoord>.Get();

            forEachCoordInArea(target, coord => targets.Add(coord.rotateFromTop(direction)));

            return targets.ToArray();
        }

        private void forEachCoordInArea(AxialCoord coord, Action<AxialCoord> action)
        {
            var directionCoord = direction.coord();
            for (int i = 0; i <= length; i++) //ignore origin
            {
                var target = coord + directionCoord * i;
                action(target);

                var clockwiseCoord = target + directionCoord.rotate120();
                var counterClockwiseCoord = target + directionCoord.rotate120Counter();
                for (int j = 0; j < radius - 1; j++)
                {
                    action(clockwiseCoord);
                    action(counterClockwiseCoord);
                    clockwiseCoord += directionCoord.rotate120();
                    counterClockwiseCoord += directionCoord.rotate120Counter();
                }
            }
        }
    }
}