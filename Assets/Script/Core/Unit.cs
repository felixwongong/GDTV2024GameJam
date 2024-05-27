using UnityEngine;

public enum UnitTeam
{
    None,
    Ocean,
    Forest,
    Volcano
}

public class Unit : MonoBehaviour
{
    public string id;
    public UnitTeam team;
}