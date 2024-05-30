using System;
using UnityEngine;

namespace Script.UI.Component
{
    public class ProgressBarElement: MonoBehaviour
    {
        [SerializeField] private RectTransform fill;

        public void setFill(float percent)
        {
            if (percent is > 1 or < 0)
                throw new ArgumentException("hpbar percent should now out of 0 and 1 bound");

            var fillTransform = fill.transform;
            var scale = fillTransform.localScale;
            fillTransform.localScale = new Vector3(percent, scale.y, scale.z);
        }
    }
}