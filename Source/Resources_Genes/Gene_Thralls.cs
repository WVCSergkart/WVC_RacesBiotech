using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class Gene_ThrallMaker : Gene, IGeneRemoteControl
	{
		public string RemoteActionName => "WVC_HideShow".Translate();

		public string RemoteActionDesc => "WVC_XaG_HideShowDesc".Translate();

		public void RemoteControl()
		{
			shouldDrawGizmo = !shouldDrawGizmo;
		}

		public bool Enabled
		{
			get
			{
				return enabled;
			}
			set
			{
				enabled = value;
			}
		}

		public override void PostRemove()
		{
			base.PostRemove();
			XaG_UiUtility.ResetAllRemoteControllers(ref cachedRemoteControlGenes);
		}

		public void RecacheGenes()
		{
			XaG_UiUtility.RecacheRemoteController(pawn, ref cachedRemoteControlGenes, ref enabled);
		}

		public bool enabled = true;

		public void RemoteControl_Recache()
		{
			RecacheGenes();
		}

		private List<IGeneRemoteControl> cachedRemoteControlGenes;


		//===========

		public GeneExtension_Giver Giver => def?.GetModExtension<GeneExtension_Giver>();

		public ThrallDef thrallDef = null;

		private Gizmo gizmo;

        public override void PostAdd()
        {
            base.PostAdd();
			shouldDrawGizmo = pawn?.genes?.GetFirstGeneOfType<Gene_ResurgentCells>() != null;
		}

        private bool shouldDrawGizmo;

		//public void Notify_GenesChanged(Gene changedGene)
		//{
		//	cachedShouldDraw = pawn?.genes?.GetFirstGeneOfType<Gene_ResurgentCells>() != null;
		//}

		public void ThrallMakerDialog()
		{
			Find.WindowStack.Add(new Dialog_ThrallMaker(this));
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (enabled)
			{
				foreach (Gizmo gizmo in XaG_UiUtility.GetRemoteControllerGizmo(pawn, this, cachedRemoteControlGenes))
				{
					yield return gizmo;
				}
			}
			if (XaG_GeneUtility.SelectorDraftedActiveFactionMap(pawn, this))
			{
				yield break;
			}
			yield return new Command_Action
			{
				defaultLabel = thrallDef != null ? thrallDef.LabelCap.ToString() : "WVC_XaG_XenoTreeXenotypeChooseLabel".Translate(),
				defaultDesc = "WVC_XaG_GeneThrallMaker_ButtonDesc".Translate(),
				icon = ContentFinder<Texture2D>.Get(def.iconPath),
				action = delegate
				{
					ThrallMakerDialog();
				}
			};
			if (shouldDrawGizmo)
			{
				if (gizmo == null)
				{
					gizmo = (Gizmo)Activator.CreateInstance(def.resourceGizmoType, this);
				}
				yield return gizmo;
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Defs.Look(ref thrallDef, "thrallDef");
			Scribe_Values.Look(ref shouldDrawGizmo, "shouldDrawGizmo", defaultValue: true);
		}

    }

	public class Gene_GeneticThrall : Gene_GeneticInstability
	{

		public override void DelayJob()
		{
			if (TryHuntForCells(pawn))
			{
				return;
			}
			base.DelayJob();
		}

		public static bool TryHuntForCells(Pawn pawn)
		{
            //if (Gene_Rechargeable.PawnHaveThisJob(pawn, WVC_GenesDefOf.WVC_XaG_CastCellsfeedOnPawnMelee))
            //{
            //	return false;
            //}
            List<Pawn> targets = MiscUtility.GetAllPlayerControlledMapPawns_ForBloodfeed(pawn);
            foreach (Pawn colonist in targets)
            {
                if (!GeneFeaturesUtility.CanCellsFeedNowWith(pawn, colonist))
                {
                    continue;
				}
				ConsumeCellsFromTarget(pawn, colonist);
				return true;
            }
            return false;
		}

		public static List<Pawn> GetAllThrallsFromList(List<Pawn> pawns)
		{
			List<Pawn> hunters = new();
			foreach (Pawn item in pawns)
			{
				if (item?.genes?.GetFirstGeneOfType<Gene_GeneticThrall>() == null)
				{
					continue;
				}
				hunters.Add(item);
			}
			return hunters;
		}

		public override void InCaravan()
		{
			Caravan caravan = pawn.GetCaravan();
			if (caravan == null)
			{
				return;
			}
			List<Pawn> pawns = caravan.PawnsListForReading;
			if (pawns.NullOrEmpty())
			{
				return;
			}
			pawns.Shuffle();
			for (int j = 0; j < pawns.Count; j++)
			{
				if (!GeneFeaturesUtility.CanCellsFeedNowWith(pawn, pawns[j]))
				{
					continue;
				}
				//CompProperties_AbilityCellsfeederBite cellsfeederComponent = GetAbilityCompProperties_CellsFeeder(WVC_GenesDefOf.WVC_XaG_Cellsfeed);
				ConsumeCellsFromTarget(pawn, pawns[j]);
				break;
			}
			base.InCaravan();
		}

		public static void ConsumeCellsFromTarget(Pawn consumer, Pawn target)
		{
			GeneFeaturesUtility.DoCellsBite(consumer, target, 3, 1f);
            if (consumer.Map != null)
			{
				FleckMaker.AttachedOverlay(consumer, DefDatabase<FleckDef>.GetNamed("PsycastPsychicEffect"), Vector3.zero);
			}
		}

		public override void GeneticStuff()
		{
			if (ModsConfig.AnomalyActive && MutantDefOf.Ghoul.allowedDevelopmentalStages == pawn.DevelopmentalStage)
			{
				MutantUtility.SetPawnAsMutantInstantly(pawn, MutantDefOf.Ghoul);
				if (pawn.Map != null)
				{
					WVC_GenesDefOf.CocoonDestroyed.SpawnAttached(pawn, pawn.Map).Trigger(pawn, null);
				}
				Find.LetterStack.ReceiveLetter("WVC_XaG_LetterLabelThrallTransformedIntoGhoul".Translate(), "WVC_XaG_LetterDescThrallTransformedIntoGhoul".Translate(pawn), LetterDefOf.NegativeEvent, new LookTargets(pawn));
			}
			else
			{
				base.GeneticStuff();
			}
		}

		//public static CompProperties_AbilityCellsfeederBite GetAbilityCompProperties_CellsFeeder(AbilityDef abilityDef)
		//{
		//	if (abilityDef?.comps != null)
		//	{
		//		foreach (AbilityCompProperties comp in abilityDef.comps)
		//		{
		//			if (comp is CompProperties_AbilityCellsfeederBite cellsFeeder)
		//			{
		//				return cellsFeeder;
		//			}
		//		}
		//	}
		//	return null;
		//}

		public override string RemoteActionDesc => "WVC_XaG_Gene_GeneticThrallDesc".Translate();

		//public override IEnumerable<Gizmo> GetGizmos()
		//{
		//	if (XaG_GeneUtility.SelectorActiveFaction(pawn, this))
		//	{
		//		yield break;
		//	}
		//	if (DebugSettings.ShowDevGizmos)
		//	{
		//		yield return new Command_Action
		//		{
		//			defaultLabel = "DEV: GeneticInstability",
		//			action = delegate
		//			{
		//				GeneticStuff();
		//			}
		//		};
		//		yield return new Command_Action
		//		{
		//			defaultLabel = "DEV: Reduce instability ticker",
		//			action = delegate
		//			{
		//				nextTick -= 30000;
		//			}
		//		};
		//	}
		//	yield return new Command_Action
		//	{
		//		defaultLabel = "WVC_XaG_Gene_GeneticThrallLabel".Translate() + ": " + XaG_UiUtility.OnOrOff(useStabilizerAuto),
		//		defaultDesc = "WVC_XaG_Gene_GeneticThrallDesc".Translate(),
		//		icon = ContentFinder<Texture2D>.Get(def.iconPath),
		//		action = delegate
		//		{
		//			useStabilizerAuto = !useStabilizerAuto;
		//			if (useStabilizerAuto)
		//			{
		//				SoundDefOf.Tick_High.PlayOneShotOnCamera();
		//			}
		//			else
		//			{
		//				SoundDefOf.Tick_Low.PlayOneShotOnCamera();
		//			}
		//		}
		//	};
		//}

		// public override void PostRemove()
		// {
		// base.PostRemove();
		// pawn.genes.AddGene(this.def, false);
		// }

	}

}
