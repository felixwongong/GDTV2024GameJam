public class AttackSkill : BaseSkill
{
    protected override void _executeCellInArea(SkillExecutionRequest req)
    {
        req.mapData.tryGetTile(req.triggerPosition, out var tile);
        
        tile.occupyBy(req.caster);
    }
}