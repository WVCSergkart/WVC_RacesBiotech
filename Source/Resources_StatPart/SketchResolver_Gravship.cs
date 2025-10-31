using RimWorld;
using RimWorld.SketchGen;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class SketchResolver_Gravship : SketchResolver
	{

		public PrefabDef prefabDef;
		public IntVec3 engineVector = new(-1, 0, 0);

		protected override bool CanResolveInt(SketchResolveParams parms)
		{
			return parms.sketch != null;
		}

		protected override void ResolveInt(SketchResolveParams parms)
		{
			Sketch sketch = new();
			sketch.AddThing(ThingDefOf.GravEngine, engineVector, Rot4.North, null, 1, null, null, wipeIfCollides: true, 0.5f);
			if (prefabDef != null)
			{
				sketch.AddPrefab(prefabDef, new IntVec3(0, 0, 0), Rot4.North);
			}
			//ThingDefOf.GravEngine.GetCompProperties<CompProperties_SubstructureFootprint>().radius = 18.9f;
			parms.sketch.Merge(sketch);
		}

	}

}
