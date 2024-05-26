public class AttackSkill : BaseSkill
{
    private void Start()
    {
        execute(new SkillExecutionRequest
        {
            triggerPosition = TileManager.instance.tilemap.WorldToCell(transform.position).xy().axial(),
            mapData = TileManager.instance._tilemapData,
            direction = AxialCoord.Top
        });
    }

    protected override void _execute(SkillExecutionRequest req)
    {
    }
}