public class AttackSkill : BaseSkill
{
    protected override void _executeCellInArea(SkillExecutionRequest req)
    {
        TileManager.instance._tilemapData.tryGetTile(req.triggerPosition, out var tile);
        
        tile.occupyBy(req.casterId);
    }
}