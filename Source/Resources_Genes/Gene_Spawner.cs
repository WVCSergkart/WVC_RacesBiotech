using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Spawner : Gene
	{
		public ThingDef ThingDefToSpawn => def.GetModExtension<GeneExtension_Spawner>().thingDefToSpawn;
		public int StackCount => def.GetModExtension<GeneExtension_Spawner>().stackCount;
		// public string CustomLabel => def.GetModExtension<GeneExtension_Spawner>().customLabel;

		public int ticksUntilSpawn;

		// private static readonly IntRange HealingIntervalTicksRange = new IntRange(120000, 300000);

		// [Unsaved(false)]
		// private GeneGizmo_Spawner gizmo;

		public override void PostAdd()
		{
			base.PostAdd();
			ResetInterval();
		}

		public override void Tick()
		{
			base.Tick();
			ticksUntilSpawn--;
			if (ticksUntilSpawn <= 0)
			{
				if (pawn.Map != null && Active)
				{
					SpawnItems(pawn);
				}
				ResetInterval();
			}
		}

		private void SpawnItems(Pawn pawn)
		{
			Thing thing = ThingMaker.MakeThing(ThingDefToSpawn);
			// thing.stackCount = SpawnCount(pawn);
			thing.stackCount = StackCount;
			GenPlace.TryPlaceThing(thing, pawn.Position, pawn.Map, ThingPlaceMode.Near, null, null, default);
		}

		private void ResetInterval()
		{
			ticksUntilSpawn = def.GetModExtension<GeneExtension_Spawner>().spawnIntervalRange.RandomInRange;
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: Spawn " + ThingDefToSpawn.label,
					defaultDesc = "NextSpawnedResourceIn".Translate() + ": " + ticksUntilSpawn.ToStringTicksToPeriod().Colorize(ColoredText.DateTimeColor),
					action = delegate
					{
						if (pawn.Map != null && Active)
						{
							SpawnItems(pawn);
						}
						ResetInterval();
					}
				};
			}
			// if (gizmo == null)
			// {
				// gizmo = new GeneGizmo_Spawner(this);
			// }
			// if (Find.Selector.SelectedPawns.Count == 1 && !pawn.Drafted && WVC_Biotech.settings.enableGeneSpawnerGizmo == true && Active)
			// {
				// yield return gizmo;
			// }
			// if (Active)
			// {
				// yield return new Command_Settle
				// {
					// defaultLabel = ThingDefToSpawn.label,
					// defaultDesc = "NextSpawnedResourceIn".Translate() + ": " + ticksUntilSpawn.ToStringTicksToPeriod().Colorize(ColoredText.DateTimeColor),
					// icon = TexCommand.DesirePower
				// };
			// }
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref ticksUntilSpawn, "ticksToSpawnThing", 0);
		}

		// public string CompInspectStringExtra()
		// {
			// if (def.GetModExtension<GeneExtension_Spawner>().writeTimeLeftToSpawn == true)
			// {
				// return "NextSpawnedResourceIn".Translate() + ": " + ticksUntilSpawn.ToStringTicksToPeriod().Colorize(ColoredText.DateTimeColor);
			// }
			// return null;
		// }
	}

	// public class GeneGizmo_Spawner : Gizmo
	// {
		// protected Gene_Spawner gene;

		// public override float GetWidth(float maxWidth)
		// {
			// return 140f;
		// }

		// public GeneGizmo_Spawner(Gene_Spawner gene)
		// {
			// this.gene = gene;
			// Order = -500f;
		// }

		// public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
		// {
			// Rect rect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
			// Rect position = rect.ContractedBy(6f);
			// float num = position.height / 3f;
			// Widgets.DrawWindowBackground(rect);
			// GUI.BeginGroup(position);
			// Widgets.Label(new Rect(0f, 0f, position.width, num), "WVC_GeneSpawner_GizmoTitle".Translate() + ":");
			// Text.Anchor = TextAnchor.UpperCenter;
			// Widgets.Label(new Rect(0f, num * 1.2f, position.width, Text.LineHeight), "WVC_GeneSpawner_GizmoSpawn".Translate() + " " + gene.ThingDefToSpawn.label);
			// Widgets.Label(new Rect(0f, num * 2f, position.width, Text.LineHeight), "WVC_GeneSpawner_GizmoNextIn".Translate() + " " + gene.ticksUntilSpawn.ToStringTicksToPeriod().Colorize(ColoredText.DateTimeColor));
			// Rect rect2 = new Rect(0f, num * 1f, position.width, 75f);
			// Text.Anchor = TextAnchor.UpperLeft;
			// if (Mouse.IsOver(rect2))
			// {
				// Widgets.DrawHighlight(rect2);
				// TooltipHandler.TipRegion(rect2, "WVC_GeneSpawner_GizmoSpawn".Translate() + " " + gene.ThingDefToSpawn.label + "\n" + "WVC_GeneSpawner_GizmoNextIn".Translate() + " " + gene.ticksUntilSpawn.ToStringTicksToPeriod().Colorize(ColoredText.DateTimeColor) + " " + "WVC_GeneSpawner_GizmoCountSpawn".Translate() + " " + gene.StackCount + "\n\n(" + "WVC_GeneSpawner_GizmoTooltip".Translate() + ")");
			// }
			// GUI.EndGroup();
			// return new GizmoResult(GizmoState.Clear);
		// }
	// }
}
