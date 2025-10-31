using System.Collections.Generic;
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_SpawnOnDeath : CompProperties
	{

		public int filthCount = 5;
		public ThingDef filthDefToSpawn;
		public List<ThingDef> thingDefsToSpawn;

		public List<HediffDef> hediffDefs;

		public int subplantCount = 3;
		public ThingDef subplant;
		public float maxRadius;
		public FloatRange? initialGrowthRange;
		public bool canSpawnOverPlayerSownPlants = true;
		// public List<ThingDef> plantsToNotOverwrite;

		public string uniqueTag = "XaG_Golems";

		public List<PawnRenderNodeProperties> renderNodeProperties;

		// public CompProperties_SpawnOnDeath()
		// {
		// compClass = typeof(CompSpawnOnDeath);
		// }
	}

	public class CompSpawnOnDeath : ThingComp
	{

		private CompProperties_SpawnOnDeath Props => (CompProperties_SpawnOnDeath)props;

		public override void Notify_Killed(Map prevMap, DamageInfo? dinfo = null)
		{
			base.Notify_Killed(prevMap, dinfo);
			Pawn pawn = parent as Pawn;
			for (int i = 0; i < Props.filthCount; i++)
			{
				IntVec3 intVec = pawn.Position + GenRadial.RadialPattern[i];
				FilthMaker.TryMakeFilth(intVec, prevMap, Props.filthDefToSpawn, 1);
			}
			Thing thing = ThingMaker.MakeThing(Props.thingDefsToSpawn.RandomElement());
			GenPlace.TryPlaceThing(thing, pawn.Position, prevMap, ThingPlaceMode.Near, null, null, default);
		}

	}

	public class CompSpawnOnDeath_Subplants : ThingComp
	{

		private CompProperties_SpawnOnDeath Props => (CompProperties_SpawnOnDeath)props;

		public override void Notify_Killed(Map prevMap, DamageInfo? dinfo = null)
		{
			base.Notify_Killed(prevMap, dinfo);
			Pawn pawn = parent as Pawn;
			for (int i = 0; i < Props.filthCount; i++)
			{
				IntVec3 intVec = pawn.Position + GenRadial.RadialPattern[i];
				FilthMaker.TryMakeFilth(intVec, prevMap, Props.filthDefToSpawn, 1);
			}
			for (int i = 0; i < Props.subplantCount; i++)
			{
				MiscUtility.GrowSubplant(pawn, Props.maxRadius, Props.subplant, Props.initialGrowthRange, prevMap, Props.canSpawnOverPlayerSownPlants);
			}
			Thing thing = ThingMaker.MakeThing(Props.thingDefsToSpawn.RandomElement());
			GenPlace.TryPlaceThing(thing, pawn.Position, prevMap, ThingPlaceMode.Near, null, null, default);
		}

	}

	public class CompSpawnOnDeath_GetColor : ThingComp
	{

		private ThingDef rockDef;

		private CompProperties_SpawnOnDeath Props => (CompProperties_SpawnOnDeath)props;

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			base.PostSpawnSetup(respawningAfterLoad);
			if (rockDef == null)
			{
				SetStoneChunk();
			}
		}

		public ThingDef StoneChunk
		{
			get
			{
				if (rockDef == null)
				{
					SetStoneChunk();
				}
				return rockDef;
			}
		}

		public void SetStoneChunk(ThingDef chunkDef = null)
		{
			rockDef = chunkDef ?? Props.thingDefsToSpawn.RandomElement();
			// if (Props.hediffDefs != null)
			// {
			// Pawn pawn = parent as Pawn;
			// foreach (HediffDef hediff in Props.hediffDefs)
			// {
			// pawn.health.AddHediff(hediff);
			// }
			// }
		}

		public override void Notify_Killed(Map prevMap, DamageInfo? dinfo = null)
		{
			base.Notify_Killed(prevMap, dinfo);
			Pawn pawn = parent as Pawn;
			for (int i = 0; i < Props.filthCount; i++)
			{
				IntVec3 intVec = pawn.Position + GenRadial.RadialPattern[i];
				FilthMaker.TryMakeFilth(intVec, prevMap, Props.filthDefToSpawn, 1);
			}
			Thing thing = ThingMaker.MakeThing(rockDef);
			GenPlace.TryPlaceThing(thing, pawn.Position, prevMap, ThingPlaceMode.Near, null, null, default);
			//prevMap.effecterMaintainer.AddEffecterToMaintain(EffecterDefOf.ControlMech.SpawnMaintained(thing, prevMap), thing.Position, 120);
		}

		// public override List<PawnRenderNode> CompRenderNodes()
		// {
		// if (!Props.renderNodeProperties.NullOrEmpty() && parent is Pawn pawn)
		// {
		// List<PawnRenderNode> list = new();
		// {
		// foreach (PawnRenderNodeProperties renderNodeProperty in Props.renderNodeProperties)
		// {
		// PawnRenderNode_ColorFromGetColorComp pawnRenderNode_ColorFromGetColorComp = (PawnRenderNode_ColorFromGetColorComp)Activator.CreateInstance(renderNodeProperty.nodeClass, pawn, renderNodeProperty, pawn.Drawer.renderer.renderTree);
		// pawnRenderNode_ColorFromGetColorComp.colorComp = this;
		// list.Add(pawnRenderNode_ColorFromGetColorComp);
		// }
		// return list;
		// }
		// }
		// return null;
		// }

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Defs.Look(ref rockDef, "rockDef_" + Props.uniqueTag);
		}

	}

	//public class CompSpawnOnDeath_PawnKind : ThingComp
	//{

	//	private PawnKindDef pawnKindDef;

	//	private CompProperties_SpawnOnDeath Props => (CompProperties_SpawnOnDeath)props;

	//	public PawnKindDef PawnKind
	//	{
	//		get
	//		{
	//			if (pawnKindDef == null)
	//			{
	//				SetPawnKindDef();
	//			}
	//			return pawnKindDef;
	//		}
	//	}

	//	public void SetPawnKindDef(PawnKindDef pawnKindDef = null)
	//	{
	//		this.pawnKindDef = pawnKindDef;
	//	}

	//	public override void PostExposeData()
	//	{
	//		base.PostExposeData();
	//		Scribe_Defs.Look(ref pawnKindDef, "pawnKindDef_" + Props.uniqueTag);
	//	}

	//}

}
