using System;
using System.Collections.Generic;
using UnityEngine;

namespace Script.Core
{
    [CreateAssetMenu(fileName = "New Team Configuration", menuName = "Team/New Team Config", order = 0)]
    public class TeamProperties : ScriptableObject
    {
        public List<TeamProperty> properties;
    }

    [Serializable]
    public class TeamProperty
    {
        public UnitTeam team;
        public Color color;
    }
}