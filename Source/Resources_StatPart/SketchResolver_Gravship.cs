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
		private static readonly int[][] BaseStructure = new int[23][]
		{
		new int[32] { 0, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
		new int[32] { 2, 2, 1, 1, 1, 1, 1, 1, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
		new int[32] { 3, 1, 1, 1, 1, 1, 1, 1, 1, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
		new int[32] { 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 2, 0, 0, 0, 2, 3, 2, 0, 0, 0, 0, 2, 3, 2, 0, 0, 0, 0, 0, 0, 0, 0 },
		new int[32] { 0, 0, 2, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 1, 2, 2, 0, 0, 0, 0, 0, 0, 0 },
		new int[32] { 0, 0, 0, 2, 2, 2, 2, 2, 2, 3, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 0, 0, 0, 0, 0 },
		new int[32] { 0, 0, 4, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 0, 0, 0 },
		new int[32] { 0, 0, 0, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 0 },
		new int[32] { 0, 0, 4, 1, 1, 5, 1, 5, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2 },
		new int[32] { 0, 0, 0, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 8, 1, 1, 1, 2, 8, 1, 1, 1, 1, 1, 1, 1, 1, 1, 3 },
		new int[32] { 0, 0, 0, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 8, 1, 1, 1, 2, 8, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2 },
		new int[32] { 0, 0, 0, 3, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 7, 1, 8, 1, 1, 1, 2, 8, 1, 1, 1, 1, 1, 1, 6, 1, 2, 0 },
		new int[32] { 0, 0, 0, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 8, 1, 1, 1, 2, 8, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2 },
		new int[32] { 0, 0, 0, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 8, 1, 1, 1, 2, 8, 1, 1, 1, 1, 1, 1, 1, 1, 1, 3 },
		new int[32] { 0, 0, 4, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2 },
		new int[32] { 0, 0, 0, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 0 },
		new int[32] { 0, 0, 4, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 0, 0, 0 },
		new int[32] { 0, 0, 0, 2, 2, 2, 2, 2, 2, 3, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 0, 0, 0, 0, 0 },
		new int[32] { 0, 0, 2, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 1, 2, 2, 0, 0, 0, 0, 0, 0, 0 },
		new int[32] { 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 2, 0, 0, 0, 2, 3, 2, 0, 0, 0, 0, 2, 3, 2, 0, 0, 0, 0, 0, 0, 0, 0 },
		new int[32] { 3, 1, 1, 1, 1, 1, 1, 1, 1, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
		new int[32] { 2, 2, 1, 1, 1, 1, 1, 1, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
		new int[32] { 0, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
		};

		protected override bool CanResolveInt(SketchResolveParams parms)
		{
			return parms.sketch != null;
		}

		protected override void ResolveInt(SketchResolveParams parms)
		{
			if (ModLister.CheckOdyssey("Ancient launch pad"))
			{
				Sketch sketch = new();
				GenerateBaseStructure(sketch);
				//sketch.AddThing(ThingDefOf.GravEngine, new IntVec3(10, 0, 8), Rot4.North, null, 1, null, null, wipeIfCollides: true, 0.5f);
				//sketch.AddThing(ThingDefOf.ChemfuelTank, new IntVec3(6, 0, 1), Rot4.North);
				//sketch.AddThing(ThingDefOf.ChemfuelTank, new IntVec3(6, 0, 7), Rot4.North);
				//sketch.AddThing(ThingDefOf.SmallThruster, new IntVec3(0, 0, 2), Rot4.East);
				//sketch.AddThing(ThingDefOf.SmallThruster, new IntVec3(0, 0, 9), Rot4.East);
				//sketch.AddThing(ThingDefOf.PilotConsole, new IntVec3(4, 0, 4), Rot4.East);
				//sketch.AddThing(ThingDefOf.Stool, new IntVec3(3, 0, 4), Rot4.East, ThingDefOf.WoodLog);
				//sketch.AddThing(ThingDefOf.Shelf, new IntVec3(1, 0, 3), Rot4.East, ThingDefOf.Steel);
				//sketch.AddThing(ThingDefOf.Shelf, new IntVec3(1, 0, 6), Rot4.East, ThingDefOf.Steel);
				//sketch.AddThing(ThingDefOf.ShelfSmall, new IntVec3(1, 0, 4), Rot4.East, ThingDefOf.Steel);
				//sketch.AddThing(ThingDefOf.Shelf, new IntVec3(2, 0, 1), Rot4.North, ThingDefOf.Steel);
				//sketch.AddThing(ThingDefOf.Shelf, new IntVec3(3, 0, 7), Rot4.South, ThingDefOf.Steel);
				//sketch.AddThing(ThingDefOf.Shelf, new IntVec3(5, 0, 1), Rot4.West, ThingDefOf.Steel);
				//sketch.AddThing(ThingDefOf.Shelf, new IntVec3(5, 0, 6), Rot4.West, ThingDefOf.Steel);
                parms.sketch.Merge(sketch);
			}
		}

		private void GenerateBaseStructure(Sketch sketch)
		{
			for (int i = 0; i < BaseStructure.Length; i++)
			{
				int[] array = BaseStructure[i];
				for (int j = 0; j < array.Length; j++)
				{
					IntVec3 pos = new(j, 0, i);
					if (array[j] == 7)
					{
						sketch.AddThing(ThingDefOf.GravEngine, pos, Rot4.North, null, 1, null, null, wipeIfCollides: true, 0.5f);
					}
				}
			}
            for (int i = 0; i < BaseStructure.Length; i++)
            {
                int[] array = BaseStructure[i];
                for (int j = 0; j < array.Length; j++)
                {
                    IntVec3 pos = new(j, 0, i);
					if (array[j] != 0)
					{
						sketch.AddTerrain(TerrainDefOf.Substructure, pos);
					}
				}
            }
            for (int i = 0; i < BaseStructure.Length; i++)
			{
				int[] array = BaseStructure[i];
				for (int j = 0; j < array.Length; j++)
				{
					IntVec3 pos = new(j, 0, i);
					switch (array[j])
					{
						case 8:
							//sketch.AddTerrain(TerrainDefOf.FlagstoneSandstone, pos, false);
							sketch.AddThing(ThingDefOf.ShelfSmall, pos, Rot4.East, ThingDefOf.Steel);
							break;
						case 2:
							//sketch.AddTerrain(TerrainDefOf.Substructure, pos);
							sketch.AddThing(ThingDefOf.GravshipHull, pos, Rot4.North, ThingDefOf.Steel);
							break;
						case 3:
							//sketch.AddTerrain(TerrainDefOf.Substructure, pos);
							sketch.AddThing(ThingDefOf.Door, pos, Rot4.North, ThingDefOf.Steel);
							break;
						case 4:
							//sketch.AddTerrain(TerrainDefOf.Substructure, pos);
							sketch.AddThing(ThingDefOf.SmallThruster, pos, Rot4.East);
							break;
						case 5:
							//sketch.AddTerrain(TerrainDefOf.Substructure, pos);
							sketch.AddThing(ThingDefOf.ChemfuelTank, pos, Rot4.North);
							break;
						case 6:
							//sketch.AddTerrain(TerrainDefOf.Substructure, pos);
							sketch.AddThing(ThingDefOf.PilotConsole, pos, Rot4.East);
							break;
					}
				}
			}
		}
	}

}
