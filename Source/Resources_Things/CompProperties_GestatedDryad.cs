// RimWorld.CompProperties_Toxifier
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

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

		public Pawn Master
		{
			get
			{
				return dryadMaster;
			}
		}

		[Unsaved(false)]
		private Gene_DryadQueen cachedGauranlenConnectionGene;

		public Gene_DryadQueen Gene_DryadQueen
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

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			if (parent is not Pawn dryad)
			{
				return;
			}
			if (!respawningAfterLoad)
			{
				if (dryad?.needs?.rest != null)
				{
					dryad.needs.rest.CurLevel = dryad.needs.rest.MaxLevel;
				}
			}
			if (!Gestated)
			{
				return;
			}
			if (dryad.mutant == null)
			{
				MutantDef mutantDef = Gene_DryadQueen?.Props?.mutantDef;
				if (mutantDef != null)
				{
					Gene_Subhuman.ClearOrSetPawnAsMutantInstantly(dryad, mutantDef);
				}
			}
		}

		public override void PostDestroy(DestroyMode mode, Map previousMap)
		{
			if (!Gestated)
			{
				return;
			}
			Pawn pawn = parent as Pawn;
			Gene_DryadQueen?.RemoveDryad(pawn);
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
			Gene_DryadQueen?.Notify_DryadKilled(pawn, gainMemory);
		}

		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			if (!Gestated || Gene_DryadQueen == null || parent is not Pawn pawn || pawn.Faction != Faction.OfPlayer)
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
					Find.WindowStack.Add(new Dialog_ChangeDryadCaste(Gene_DryadQueen, this));
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

}
