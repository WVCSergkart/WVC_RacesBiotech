// RimWorld.CompProperties_Toxifier
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_GestatedDryad : CompProperties
	{

		public PawnKindDef defaultDryadPawnKindDef;

		public string uniqueTag = "XaG_Dryads";

		public CompProperties_GestatedDryad()
		{
			compClass = typeof(CompGestatedDryad);
		}

	}

	public class CompGestatedDryad : ThingComp
	{

		public CompProperties_GestatedDryad Props => (CompProperties_GestatedDryad)props;

		public GauranlenGeneModeDef currentMode = null;

		public PawnKindDef DryadKind => currentMode?.pawnKindDef ?? Props?.defaultDryadPawnKindDef;

		private Pawn dryadMaster;

		public bool Gestated => dryadMaster != null;

		// [Unsaved(false)]
		// private Pawn cachedConnectedPawn;

		// public Pawn Master
		// {
			// get
			// {
				// if (parent is Pawn pawn && (cachedConnectedPawn == null || pawn.Faction != cachedConnectedPawn.Faction))
				// {
					// cachedConnectedPawn = pawn?.connections?.ConnectedThings.FirstOrDefault() is Pawn master ? master : null;
				// }
				// return cachedConnectedPawn;
			// }
		// }

		public Pawn Master
		{
			get
			{
				//if (dryadMaster == null && parent is Pawn dryad)
				//{
				//	dryadMaster = dryad?.connections?.ConnectedThings.FirstOrDefault() is Pawn master ? master : null;
				//}
				return dryadMaster;
			}
		}

		[Unsaved(false)]
		private Gene_DryadQueen cachedGauranlenConnectionGene;

		public Gene_DryadQueen Gene_GauranlenConnection
		{
			get
			{
				if (cachedGauranlenConnectionGene == null || !cachedGauranlenConnectionGene.Active)
				{
					cachedGauranlenConnectionGene = Master?.genes?.GetFirstGeneOfType<Gene_DryadQueen>();
				}
				return cachedGauranlenConnectionGene;
			}
		}

		// public override string TransformLabel(string label)
		// {
			// if (Gestated)
			// {
				// return "WVC_XaG_GestatedDryad".Translate() + " " + label;
			// }
			// return label;
		// }

		// public override string GetDescriptionPart()
		// {
			// if (Gestated)
			// {
				// return "WVC_XaG_GestatedDryadDescription".Translate().Resolve();
			// }
			// return null;
		// }

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			base.PostSpawnSetup(respawningAfterLoad);
			if (!respawningAfterLoad)
			{
				Pawn pawn = parent as Pawn;
				if (pawn?.needs?.rest != null)
				{
					pawn.needs.rest.CurLevel = pawn.needs.rest.MaxLevel;
				}
				// ResetOverseerTick();
			}
		}

		public override void PostDestroy(DestroyMode mode, Map previousMap)
		{
			if (!Gestated)
			{
				return;
			}
			Pawn pawn = parent as Pawn;
			Gene_GauranlenConnection?.RemoveDryad(pawn);
		}

		public override void Notify_Killed(Map prevMap, DamageInfo? dinfo = null)
		{
			if (!Gestated)
			{
				return;
			}
			RemoveThisDryad(true);
			SetMaster(null);
		}

		public void RemoveThisDryad(bool gainMemory = false)
		{
			Pawn pawn = parent as Pawn;
			Gene_GauranlenConnection?.Notify_DryadKilled(pawn, gainMemory);
		}

		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			Pawn pawn = parent as Pawn;
			if (!Gestated || Gene_GauranlenConnection == null || pawn.Faction != Faction.OfPlayer)
			{
				yield break;
			}
			yield return new Command_Action
			{
				defaultLabel = "ChangeMode".Translate(),
				defaultDesc = "WVC_XaG_CompGauranlenDryad_ChangeMode".Translate(),
				icon = currentMode?.pawnKindDef != null ? Widgets.GetIconFor(currentMode.pawnKindDef.race) : Widgets.GetIconFor(pawn.kindDef.race),
				action = delegate
				{
					Find.WindowStack.Add(new Dialog_ChangeDryadCaste(Gene_GauranlenConnection, this));
				}
			};
			yield return new Command_Action
			{
				defaultLabel = "WVC_XaG_GeneDryadsSelectDryadQueen".Translate(),
				defaultDesc = "WVC_XaG_GeneDryadsSelectDryadQueen_Desc".Translate(),
				icon = ContentFinder<Texture2D>.Get("WVC/UI/XaG_General/UI_SelectDryadQueen"),
				Order = -87f,
				action = delegate
				{
					Find.Selector.ClearSelection();
					Find.Selector.Select(dryadMaster);
				}
			};
		}

		public void SetMaster(Pawn master)
		{
			dryadMaster = master;
		}

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Defs.Look(ref currentMode, "currentMode_" + Props.uniqueTag);
			Scribe_References.Look(ref dryadMaster, "dryadMaster_" + Props.uniqueTag);
		}

	}

	// ============================Cocoon=Cocoon============================

	public class CompProperties_InheritableDryadCocoon : CompProperties_DryadCocoon
	{

		public ThingDef inheritFromDef;

		public override void ResolveReferences(ThingDef parentDef)
		{
			CompProperties_DryadCocoon cocoon = inheritFromDef?.GetCompProperties<CompProperties_DryadCocoon>();
			if (cocoon != null)
			{
				daysToComplete = cocoon.daysToComplete;
				drawContents = cocoon.drawContents;
			}
		}

	}

	public class CompDryadHolder_WithGene : CompDryadHolder
	{

		[Unsaved(false)]
		private Pawn cachedDryadPawn;

		public Pawn Dryad
		{
			get
			{
				if (cachedDryadPawn == null)
				{
					cachedDryadPawn = GetDirectlyHeldThings().FirstOrDefault() is Pawn pawn ? pawn : null;
				}
				return cachedDryadPawn;
			}
		}

		[Unsaved(false)]
		private CompGestatedDryad cachedDryadComp;

		public CompGestatedDryad DryadComp
		{
			get
			{
				if (cachedDryadComp == null)
				{
					cachedDryadComp = Dryad?.TryGetComp<CompGestatedDryad>();
				}
				return cachedDryadComp;
			}
		}

		public override void CompTick()
		{
			//innerContainer.ThingOwnerTick();
			if (tickComplete >= 0)
			{
				if (DryadComp?.Gene_GauranlenConnection == null)
				{
					parent.Destroy();
				}
				else if (Dryad.Dead)
				{
					parent.Destroy();
				}
				else if (Find.TickManager.TicksGame >= tickComplete)
				{
					Complete();
				}
			}
		}

		public override void TryAcceptPawn(Pawn p)
		{
			bool num = p.DeSpawnOrDeselect();
			innerContainer.TryAddOrTransfer(p, 1);
			if (num)
			{
				Find.Selector.Select(p, playSound: false, forceDesignatorDeselect: false);
			}
			SoundDefOf.Pawn_EnterDryadPod.PlayOneShot(SoundInfo.InMap(parent));
		}

		protected override void Complete()
		{
		}

	}

	// Cocoon

	public class CompDryadCocoon_WithGene : CompDryadHolder_WithGene
	{
		private int tickExpire = -1;

		private PawnKindDef dryadKind;

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			if (!respawningAfterLoad)
			{
				innerContainer = new ThingOwner<Thing>(this, oneStackOnly: false);
				tickExpire = Find.TickManager.TicksGame + 600;
			}
		}

		public override void TryAcceptPawn(Pawn p)
		{
			base.TryAcceptPawn(p);
			p.Rotation = Rot4.South;
			tickComplete = Find.TickManager.TicksGame + (int)(60000f * base.Props.daysToComplete);
			tickExpire = -1;
			dryadKind = DryadComp.DryadKind;
		}

		protected override void Complete()
		{
			tickComplete = Find.TickManager.TicksGame;
			// CompTreeConnection treeComp = base.TreeComp;
			if (DryadComp != null && innerContainer.Count > 0)
			{
				Pawn pawn = (Pawn)innerContainer[0];
				long ageBiologicalTicks = pawn.ageTracker.AgeBiologicalTicks;
				DryadComp.Gene_GauranlenConnection.RemoveDryad(pawn);
				Pawn pawn2 = DryadComp.Gene_GauranlenConnection.GenerateNewDryad(dryadKind);
				pawn2.ageTracker.AgeBiologicalTicks = ageBiologicalTicks;
				if (!pawn.Name.Numerical)
				{
					pawn2.Name = pawn.Name;
				}
				CompGestatedDryad newPawnComp = pawn2.TryGetComp<CompGestatedDryad>();
				newPawnComp.currentMode = DryadComp.currentMode;
				pawn.Destroy();
				innerContainer.TryAddOrTransfer(pawn2, 1);
				EffecterDefOf.DryadEmergeFromCocoon.Spawn(parent.Position, parent.Map).Cleanup();
			}
			parent.Destroy();
		}

		public override void PostDeSpawn(Map map, DestroyMode mode = DestroyMode.Vanish)
		{
			innerContainer.TryDropAll(parent.Position, map, ThingPlaceMode.Near, delegate(Thing t, int c)
			{
				t.Rotation = Rot4.South;
				SoundDefOf.Pawn_Dryad_Spawn.PlayOneShot(parent);
			}, null, playDropSound: false);
		}

		public override void CompTick()
		{
			base.CompTick();
			if (!parent.Destroyed)
			{
				if (dryadKind != null && dryadKind != DryadComp?.DryadKind)
				{
					parent.Destroy();
				}
				else if (innerContainer.Count > 0 && DryadComp == null)
				{
					parent.Destroy();
				}
				else if (tickExpire >= 0 && Find.TickManager.TicksGame >= tickExpire)
				{
					tickExpire = -1;
					parent.Destroy();
				}
			}
		}

		public override string CompInspectStringExtra()
		{
			string text = base.CompInspectStringExtra();
			if (!innerContainer.NullOrEmpty() && dryadKind != null)
			{
				if (!text.NullOrEmpty())
				{
					text += "\n";
				}
				text += "ChangingDryadIntoType".Translate(innerContainer[0].Named("DRYAD"), NamedArgumentUtility.Named(dryadKind, "TYPE")).Resolve();
			}
			return text;
		}

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look(ref tickExpire, "tickExpire", -1);
			Scribe_Defs.Look(ref dryadKind, "dryadKind");
		}
	}

	public class CompDryadHealingPod_WithGene : CompDryadHolder_WithGene
	{
		private int tickExpire = -1;

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			if (!respawningAfterLoad)
			{
				innerContainer = new ThingOwner<Thing>(this, oneStackOnly: false);
				tickExpire = Find.TickManager.TicksGame + 600;
			}
		}

		public override void PostDeSpawn(Map map, DestroyMode mode = DestroyMode.Vanish)
		{
			innerContainer.TryDropAll(parent.Position, map, ThingPlaceMode.Near, delegate(Thing t, int c)
			{
				if (t is Pawn pawn && pawn.mindState != null)
				{
					pawn.mindState.returnToHealingPod = false;
				}
				t.Rotation = Rot4.South;
				SoundDefOf.Pawn_Dryad_Spawn.PlayOneShot(parent);
			}, null, playDropSound: false);
		}

		public override void CompTick()
		{
			base.CompTick();
			if (!parent.Destroyed)
			{
				if (innerContainer.Count > 0 && DryadComp == null)
				{
					parent.Destroy();
				}
				else if (tickExpire >= 0 && Find.TickManager.TicksGame >= tickExpire)
				{
					tickExpire = -1;
					parent.Destroy();
				}
			}
		}

		public override void TryAcceptPawn(Pawn p)
		{
			base.TryAcceptPawn(p);
			p.Rotation = Rot4.South;
			tickComplete = Find.TickManager.TicksGame + (int)(60000f * base.Props.daysToComplete);
			tickExpire = -1;
		}

		protected override void Complete()
		{
			tickComplete = Find.TickManager.TicksGame;
			EffecterDefOf.DryadEmergeFromCocoon.Spawn(parent.Position, parent.Map).Cleanup();
			foreach (Thing item in (IEnumerable<Thing>)innerContainer)
			{
				if (item is not Pawn pawn)
				{
					continue;
				}
				pawn.mindState.returnToHealingPod = false;
				int num = 1000;
				while (num > 0 && HealthUtility.TryGetWorstHealthCondition(pawn, out Hediff hediff, out BodyPartRecord part))
				{
					if (hediff != null)
					{
						pawn.health.RemoveHediff(hediff);
					}
					else if (part != null)
					{
						pawn.health.RestorePart(part);
					}
					num--;
				}
			}
			parent.Destroy();
		}
	}

}
