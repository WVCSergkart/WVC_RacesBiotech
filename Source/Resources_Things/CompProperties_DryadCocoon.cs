// RimWorld.CompProperties_Toxifier
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

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
				if (DryadComp?.Gene_DryadQueen == null)
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
				DryadComp.Gene_DryadQueen.RemoveDryad(pawn);
				Pawn pawn2 = DryadComp.Gene_DryadQueen.GenerateNewDryad(dryadKind);
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
