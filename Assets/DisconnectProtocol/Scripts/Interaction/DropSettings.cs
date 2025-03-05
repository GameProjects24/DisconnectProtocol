using System.Collections.Generic;
using UnityEngine;
using DS = DisconnectProtocol.DropSettings;

namespace DisconnectProtocol
{
	// maybe ScriptableObject?
	public class DropSettings : MonoBehaviour {
		public enum Mode {
			Infinite,
			RandomAmongAll,
			DetermAfterLE,
		}
		[System.Serializable]
		public class DropObject {
			public string tag;
			public GameObject prefab;
			public ushort maxCount;
			public Mode mode;
		}

		[Tooltip("Only one of them will drop at once")]
		public List<DropObject> incompatible = new List<DropObject>();
		[Tooltip("All of them can be dropped simultaneously")]
		public List<DropObject> other = new List<DropObject>();
	}

	public class DropController {
		public class DropObject {
			public GameObject prefab;
			public ushort maxCount;
			public DS.Mode mode;

			public static implicit operator DropObject(DS.DropObject ds) {
				return new DropObject {
					prefab = ds.prefab,
					maxCount = ds.maxCount,
					mode = ds.mode
				};
			}
		}

		public class DmgStat {
			public ushort countDmg = 0;
			public List<DropObject> drops = new List<DropObject>();

			public DmgStat(DS.DropObject drop) {
				drops.Add(drop);
			}
		}

		private static Dictionary<string, DmgStat> m_incompatible = new Dictionary<string, DmgStat>();
		private static Dictionary<string, DmgStat> m_other = new Dictionary<string, DmgStat>();

        public static void Init(List<DS.DropObject> incomp, List<DS.DropObject> other) {
			m_incompatible.Clear();
			m_other.Clear();
			DmgStat stat;
			foreach (var inc in incomp) {
				if (m_incompatible.TryGetValue(inc.tag, out stat)) {
					stat.drops.Add(inc);
				} else {
					m_incompatible.Add(inc.tag, new DmgStat(inc));
				}
			}
			foreach (var oth in other) {
				if (m_other.TryGetValue(oth.tag, out stat)) {
					stat.drops.Add(oth);
				} else {
					m_other.Add(oth.tag, new DmgStat(oth));
				}
			}
			foreach (var dmg in GameObject.FindObjectsByType<Damageable>(FindObjectsSortMode.None)) {
				dmg.OnDieKnow += TryDrop;
				if (m_incompatible.TryGetValue(dmg.tag, out stat)) {
					++stat.countDmg;
				}
				if (m_other.TryGetValue(dmg.tag, out stat)) {
					++stat.countDmg;
				}
			}
		}

		private static void TryDrop(Damageable corpse) {
			DmgStat stat;
			if (m_incompatible.TryGetValue(corpse.tag, out stat)) {
				if (stat.drops.Count > 0) {
					int idxInc = Random.Range(0, stat.drops.Count);
					if (TryDrop(corpse, stat.drops[idxInc], stat.countDmg)) {
						stat.drops.RemoveAt(idxInc);
					}
				}
				--stat.countDmg;
			}
			if (m_other.TryGetValue(corpse.tag, out stat)) {
				foreach (var dr in stat.drops) {
					TryDrop(corpse, dr, stat.countDmg);
				}
				--stat.countDmg;
			}
		}

		// bool is for "can delete" not for "is actually dropped"
		// maybe reorganize;
		private static bool TryDrop(Damageable corpse, DropObject drop, ushort dmgCount) {
			if (drop.mode != DS.Mode.Infinite && drop.maxCount == 0) {
				return true;
			}

			float chance = drop.mode switch {
				DS.Mode.Infinite => Random.Range(0f, 1f),
				DS.Mode.RandomAmongAll => drop.maxCount / dmgCount,
				DS.Mode.DetermAfterLE => dmgCount <= drop.maxCount ? 1f : -1f,
				_ => throw new System.ArgumentException("Unknown DropSettings Mode"),
			};
			
			if (chance >= Random.Range(0f, 1f)) {
				--drop.maxCount;
				Drop(drop.prefab, corpse.transform);
			}
			return drop.mode != DS.Mode.Infinite && drop.maxCount == 0;
		}

		// should be using pools
		private static void Drop(GameObject obj, Transform parent = null) {
			var drop = GameObject.Instantiate(obj, parent);
			drop.transform.SetParent(null);
		}
	}
}