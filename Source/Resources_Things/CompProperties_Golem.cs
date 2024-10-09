using RimWorld;
using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_Golem : CompProperties
	{

		// public Type geneGizmoType = typeOf(GeneGizmo_Golems);

		// public GeneDef reqGeneDef;

		// public List<GeneDef> anyGeneDefs;

		public JobDef changeCasteJob;

		[Obsolete]
		public int golemIndex = -1;

		public int refreshHours = 2;

		[Obsolete]
		public float shutdownEnergyReplenish = 1.0f;

		[Obsolete]
		public IntRange checkOverseerInterval = new(45000, 85000);

		public string uniqueTag = "XaG_Golems";

		public CompProperties_Golem()
		{
			compClass = typeof(CompGolem);
		}

		public override void ResolveReferences(ThingDef parentDef)
		{
			if (!parentDef.description.NullOrEmpty())
			{
				parentDef.description = parentDef.description + "\n\n" + "WVC_XaG_GolemnoidsGeneralDesc".Translate();
			}
		}

	}

	public class CompGolem : ThingComp
	{

		public CompProperties_Golem Props => (CompProperties_Golem)props;

		private int nextEnergyTick = 1500;

		public override void CompTick()
		{
			nextEnergyTick--;
			if (nextEnergyTick <= 0f)
			{
				Pawn pawn = parent as Pawn;
				MechanoidsUtility.OffsetNeedEnergy(pawn, WVC_Biotech.settings.golemnoids_ShutdownRechargePerTick, Props.refreshHours);
				nextEnergyTick = Props.refreshHours * 1500;
			}
		}

		private static readonly CachedTexture ChangeModeIcon = new("WVC/UI/XaG_General/Ui_ChangeGolemMode_v0");

		// public override void PostSpawnSetup(bool respawningAfterLoad)
		// {
		// base.PostSpawnSetup(respawningAfterLoad);
		// if (!respawningAfterLoad)
		// {
		// Pawn pawn = parent as Pawn;
		// if (pawn?.needs?.energy != null)
		// {
		// pawn.needs.energy.CurLevel = pawn.needs.energy.MaxLevel;
		// }
		// }
		// }

		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			Pawn pawn = parent as Pawn;
			if (XaG_GeneUtility.SelectorFactionMap(pawn))
			{
				yield break;
			}
			if (Props.changeCasteJob == null)
			{
				yield break;
			}
			yield return new Command_Action
			{
				defaultLabel = "WVC_XaG_ChangeGolemMode".Translate(),
				defaultDesc = "WVC_XaG_ChangeGolemModeDesc".Translate(),
				icon = ChangeModeIcon.Texture,
				action = delegate
				{
					Thing chunk = Gene_Golemlink.GetBestStoneChunk(pawn, false);
					float limit = MechanoidsUtility.TotalGolembond(pawn.GetOverseer());
					float consumedGolembond = MechanoidsUtility.GetConsumedGolembond(pawn.GetOverseer()) - pawn.GetStatValue(WVC_GenesDefOf.WVC_GolemBondCost);
					if (chunk == null || consumedGolembond > limit)
					{
						Messages.Message("WVC_XaG_ChangeGolemCaste_NonChunk_NonGolembond".Translate(), null, MessageTypeDefOf.RejectInput, historical: false);
					}
					else
					{
						Find.WindowStack.Add(new Dialog_ChangeGolemCaste(pawn, consumedGolembond, limit, this, chunk));
					}
				}
			};
		}

		public override string CompInspectStringExtra()
		{
			if (parent.Faction != Faction.OfPlayer)
			{
				return null;
			}
			if (parent is Pawn pawn)
			{
				Need_MechEnergy energy = pawn?.needs?.energy;
				if (energy?.IsSelfShutdown == true)
				{
					return "WVC_XaG_GolemEnergyRecovery_Info".Translate((energy.CurLevelPercentage).ToStringPercent(), MechanoidsUtility.GolemsEnergyPerDayInPercent(energy.MaxLevel));
				}
			}
			return null;
		}

		public GolemModeDef targetMode;

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Defs.Look(ref targetMode, "targetMode_" + Props.uniqueTag);
		}

	}

}
