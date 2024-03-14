using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_SpawnOnDeath : CompProperties
	{

		public int filthCount = 5;
		public ThingDef filthDefToSpawn;
		public List<ThingDef> thingDefsToSpawn;

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

		public override void PostDestroy(DestroyMode mode, Map previousMap)
		{
			base.PostDestroy(mode, previousMap);
			Pawn pawn = parent as Pawn;
			for (int i = 0; i < Props.filthCount; i++)
			{
				IntVec3 intVec = pawn.Position + GenRadial.RadialPattern[i];
				FilthMaker.TryMakeFilth(intVec, previousMap, Props.filthDefToSpawn, 1);
			}
			// FilthMaker.TryMakeFilth(pawn.Position, previousMap, Props.filthDefToSpawn, filthCount);
			Thing thing = ThingMaker.MakeThing(Props.thingDefsToSpawn.RandomElement());
			GenPlace.TryPlaceThing(thing, pawn.Position, previousMap, ThingPlaceMode.Near, null, null, default);
		}

	}

	public class CompSpawnOnDeath_Subplants : ThingComp
	{

		private CompProperties_SpawnOnDeath Props => (CompProperties_SpawnOnDeath)props;

		public override void PostDestroy(DestroyMode mode, Map previousMap)
		{
			base.PostDestroy(mode, previousMap);
			Pawn pawn = parent as Pawn;
			for (int i = 0; i < Props.filthCount; i++)
			{
				IntVec3 intVec = pawn.Position + GenRadial.RadialPattern[i];
				FilthMaker.TryMakeFilth(intVec, previousMap, Props.filthDefToSpawn, 1);
			}
			for (int i = 0; i < Props.subplantCount; i++)
			{
				MiscUtility.GrowSubplant(pawn, Props.maxRadius, Props.subplant, Props.initialGrowthRange, previousMap, Props.canSpawnOverPlayerSownPlants);
			}
			// FilthMaker.TryMakeFilth(pawn.Position, previousMap, Props.filthDefToSpawn, filthCount);
			Thing thing = ThingMaker.MakeThing(Props.thingDefsToSpawn.RandomElement());
			GenPlace.TryPlaceThing(thing, pawn.Position, previousMap, ThingPlaceMode.Near, null, null, default);
		}

	}

	public class CompSpawnOnDeath_GetColor : ThingComp
	{

		public ThingDef rockDef;

		private CompProperties_SpawnOnDeath Props => (CompProperties_SpawnOnDeath)props;

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			base.PostSpawnSetup(respawningAfterLoad);
			if (rockDef == null)
			{
				Reset();
			}
		}

		private void Reset()
		{
			rockDef = Props.thingDefsToSpawn.RandomElement();
			// LongEventHandler.ExecuteWhenFinished(Apply);
		}

		public override void PostDestroy(DestroyMode mode, Map previousMap)
		{
			base.PostDestroy(mode, previousMap);
			Pawn pawn = parent as Pawn;
			for (int i = 0; i < Props.filthCount; i++)
			{
				IntVec3 intVec = pawn.Position + GenRadial.RadialPattern[i];
				FilthMaker.TryMakeFilth(intVec, previousMap, Props.filthDefToSpawn, 1);
			}
			Thing thing = ThingMaker.MakeThing(rockDef);
			GenPlace.TryPlaceThing(thing, pawn.Position, previousMap, ThingPlaceMode.Near, null, null, default);
		}

		public override List<PawnRenderNode> CompRenderNodes()
		{
			if (!Props.renderNodeProperties.NullOrEmpty() && parent is Pawn pawn)
			{
				List<PawnRenderNode> list = new();
				{
					foreach (PawnRenderNodeProperties renderNodeProperty in Props.renderNodeProperties)
					{
						PawnRenderNode_ColorFromGetColorComp pawnRenderNode_ColorFromGetColorComp = (PawnRenderNode_ColorFromGetColorComp)Activator.CreateInstance(renderNodeProperty.nodeClass, pawn, renderNodeProperty, pawn.Drawer.renderer.renderTree);
						pawnRenderNode_ColorFromGetColorComp.colorComp = this;
						list.Add(pawnRenderNode_ColorFromGetColorComp);
					}
					return list;
				}
			}
			return base.CompRenderNodes();
		}

		// private void Apply()
		// {
			// if (parent is Pawn pawn)
			// {
				// Graphic graphic = pawn.Drawer.renderer.BodyGraphic;
				// PawnRenderer renderer = pawn.Drawer.renderer;
				// Color color = rockDef.graphic.data.color;
				// GraphicData graphicData = new();
				// graphicData.CopyFrom(pawn.ageTracker.CurKindLifeStage.bodyGraphicData);
				// graphic.color = color;
				// graphic.colorTwo = color;
				// if (!renderer.graphics.AllResolved)
				// {
					// renderer.graphics.ResolveAllGraphics();
				// }
				// renderer.graphics.nakedGraphic = graphicData.Graphic;
				// renderer.graphics.ClearCache();
				// pawn.Drawer.renderer.BodyGraphic = GraphicDatabase.Get<Graphic_Multi>(graphic.path, ShaderDatabase.Cutout, graphic.drawSize, rockDef.graphic.data.color);
			// }
		// }

		// public void SetStoneColour(ThingDef thingDef)
		// {
			// rockDef = thingDef;
			// LongEventHandler.ExecuteWhenFinished(Apply);
		// }

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Defs.Look(ref rockDef, "rockDef_" + Props.uniqueTag);
		}

	}

}
