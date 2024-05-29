using Otter.MonsterChess.Core;
using Otter.MonsterChess.Skill;
using UnityEngine;

	public struct SkillStatus
	{
		public float duration;
	}

	public struct SkillExecutionRequest
	{
		public int casterId;
		public AxialCoord triggerPosition;
		public AxialCoord direction;
	}
	
	public abstract class BaseSkill : MonoBehaviour {
		[Tooltip("VFX play on all cell in area")]
		[SerializeField] private ParticleSystem _emitVfxPrefab;
		[Tooltip("VFX play on all unit with targetUnitType in area")]
		[SerializeField] private ParticleSystem _hitVfxPrefab;
		[SerializeField] private AxialArea effectArea;
		[SerializeField] private float scaledDuration;
		
		public SkillStatus execute(SkillExecutionRequest req)
		{
			var mapData = TileManager.instance._tilemapData;
			
			AxialCoord[] area;
            area = effectArea.getArea(req.triggerPosition, req.direction);
			
			for (var i = 0; i < area.Length; i++)
			{
				if (!mapData.tryGetTile(area[i], out var tile)) continue;

				if (_emitVfxPrefab != null) playVfxOn(getVfx(_emitVfxPrefab), area[i], mapData);

				var singleReq = new SkillExecutionRequest()
				{
					casterId = req.casterId,
					direction = req.direction,
					triggerPosition = area[i],
				};
				
				_executeCellInArea(singleReq);
				if (_hitVfxPrefab != null) playVfxOn(getVfx(_hitVfxPrefab), area[i], mapData);
			}

			return new SkillStatus()
			{
				duration = getScaledDuration()
			};
		}
        
		protected abstract void _executeCellInArea(SkillExecutionRequest req);

		private void playVfxOn(ParticleSystem vfx, AxialCoord coord, AxialTilemapData mapData) {
			if(!mapData.tryGetTile(coord, out var tile)) return;

			FLog.Log("Playing vfx");
			vfx.transform.position = tile.transform.position;
			vfx.Play();
		}
	
		ParticleSystem getVfx(ParticleSystem prefab) {
			if (!prefab) FLog.LogError($"No trigger VFX prefab on {gameObject.name}");
			return Instantiate(prefab);
		}
	
		void recycle(ParticleSystem ps) {
			Destroy(ps);
		}

		protected virtual float getScaledDuration()
		{
			return scaledDuration;
		}
	}