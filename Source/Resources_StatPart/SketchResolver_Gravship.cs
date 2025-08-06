using RimWorld;
using RimWorld.SketchGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class SketchResolver_Gravship : SketchResolver
	{

		public PrefabDef prefabDef;

		protected override bool CanResolveInt(SketchResolveParams parms)
		{
			return parms.sketch != null;
		}

		protected override void ResolveInt(SketchResolveParams parms)
		{
			Sketch sketch = new();
			sketch.AddThing(ThingDefOf.GravEngine, new IntVec3(-1, 0, 0), Rot4.North, null, 1, null, null, wipeIfCollides: true, 0.5f);
			if (prefabDef != null)
			{
				sketch.AddPrefab(prefabDef, new IntVec3(0, 0, 0), Rot4.North);
			}
			parms.sketch.Merge(sketch);
		}

    }

}
