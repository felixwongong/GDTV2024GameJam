using System.Collections;
using Otter.MonsterChess.Core;
using Otter.MonsterChess.Skill;
using UnityEngine;

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
		public float castBackswingSecond = 0f;

		private DelayExecuteModifier[] delayModifiers;

		private void Awake()
		{
			delayModifiers = GetComponents<DelayExecuteModifier>();
		}

		public IEnumerator execute(SkillExecutionRequest req)
		{
			foreach (var delay in delayModifiers)
			{
				yield return delay.wait();
			}

			_executeArea(req);

			yield return new WaitForSeconds(getScaledDuration());
		}
		
		private void _executeArea(SkillExecutionRequest req)
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
	
		public void recycle() {
			Destroy(this.gameObject);
		}

		protected virtual float getScaledDuration()
		{
			return scaledDuration;
		}
	}