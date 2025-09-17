using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;

namespace WVC_XenotypesAndGenes
{

    public class Gene_XenotypeForcer : Gene
	{

		public XenotypeDef Xenotype => def.GetModExtension<GeneExtension_Giver>()?.xenotypeForcerDef;

		public override void PostAdd()
		{
			base.PostAdd();
			RandomXenotype(pawn, this, Xenotype);
		}

		public static void RandomXenotype(Pawn pawn, Gene gene, XenotypeDef xenotype)
		{
			if (pawn.genes.IsXenogene(gene))
			{
				pawn.genes.RemoveGene(gene);
				pawn.genes.AddGene(gene.def, false);
				return;
			}
			// bool geneIsXenogene = true;
			// List<Gene> endogenes = pawn.genes.Endogenes;
			// if (endogenes.Contains(gene))
			// {
			// geneIsXenogene = false;
			// }
			if (xenotype == null)
			{
				List<XenotypeDef> xenotypeDef = ListsUtility.GetWhiteListedXenotypes(true, true);
				if (gene.def.GetModExtension<GeneExtension_Giver>() != null && gene.def.GetModExtension<GeneExtension_Giver>().xenotypeIsInheritable)
				{
					xenotype = xenotypeDef.Where((XenotypeDef randomXenotypeDef) => randomXenotypeDef.inheritable).RandomElement();
				}
				else
				{
					xenotype = xenotypeDef.Where((XenotypeDef randomXenotypeDef) => !randomXenotypeDef.inheritable).RandomElement();
				}
				if (xenotype == null)
				{
					Log.Error("Generated gene with null xenotype. Choose random.");
					xenotype = xenotypeDef.RandomElement();
				}
			}
			if (xenotype == null)
			{
				pawn.genes.RemoveGene(gene);
				Log.Error("Xenotype is null. Do not report this to the developer, you yourself created this creepy world filled with bugs. To fix the situation, reset the filter in the " + "WVC_BiotechSettings".Translate() + " mod settings and restart the game.");
				return;
			}
			XenotypeForcer_SimpleVersion(pawn, xenotype);
			if (gene != null)
			{
				pawn.genes.RemoveGene(gene);
			}
		}

		public static void XenotypeForcer_SimpleVersion(Pawn pawn, XenotypeDef xenotype)
		{
			Pawn_GeneTracker genes = pawn.genes;
            if (xenotype.inheritable)
            {
                for (int numEndogenes = genes.Endogenes.Count - 1; numEndogenes >= 0; numEndogenes--)
                {
                    pawn.genes?.RemoveGene(genes.Endogenes[numEndogenes]);
                }
            }
            else
            {
                for (int numXenogenes = genes.Xenogenes.Count - 1; numXenogenes >= 0; numXenogenes--)
                {
                    pawn.genes?.RemoveGene(genes.Xenogenes[numXenogenes]);
                }
            }
			//if (pawn.genes.Xenogenes.NullOrEmpty() && xenotype.inheritable || !xenotype.inheritable)
			//{
			//	pawn.genes?.SetXenotypeDirect(xenotype);
			//	pawn.genes.iconDef = null;
			//}
			ReimplanterUtility.SetXenotypeDirect(null, pawn, xenotype, pawn.genes.Xenogenes.NullOrEmpty() && xenotype.inheritable || !xenotype.inheritable);
			for (int i = 0; i < xenotype.genes.Count; i++)
			{
				pawn.genes?.AddGene(xenotype.genes[i], !xenotype.inheritable);
			}
            ReimplanterUtility.TrySetSkinAndHairGenes(pawn);
			ReimplanterUtility.PostImplantDebug(pawn);
        }

    }

	public class Gene_Implanter : Gene, IGeneFloatMenuOptions
	{

		private CompAbilityEffect_Reimplanter cachedReimplanterComp;

		public CompAbilityEffect_Reimplanter ReimplanterComp
		{
			get
			{
				if (cachedReimplanterComp == null && def.abilities != null)
				{
					cachedReimplanterComp = pawn?.abilities?.GetAbility(def.abilities.FirstOrDefault()).CompOfType<CompAbilityEffect_Reimplanter>();
				}
				return cachedReimplanterComp;
			}
		}

		public IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
		{
			if (XaG_GeneUtility.ActiveDowned(pawn, this))
			{
				yield break;
			}
			if (ReimplanterComp?.Props?.absorberJob == null)
			{
				yield break;
			}
			if (!selPawn.CanReach(pawn, PathEndMode.ClosestTouch, Danger.Deadly) || !selPawn.IsHuman())
			{
				yield break;
			}
			if (pawn.IsQuestLodger())
			{
				yield return new FloatMenuOption("TemporaryFactionMember".Translate(pawn.Named("PAWN")), null);
				yield break;
			}
			if (pawn.health.hediffSet.HasHediff(HediffDefOf.XenogermLossShock))
			{
				yield return new FloatMenuOption("XenogermLossShockPresent".Translate(pawn.Named("PAWN")), null);
				yield break;
			}
			yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("WVC_ForceXenogermImplantation".Translate() + " (" + base.LabelCap + ")", delegate
			{
				GiveJob(selPawn);
			}), selPawn, pawn);
		}

		private void GiveJob(Pawn selPawn)
		{
			if (GeneUtility.PawnWouldDieFromReimplanting(pawn))
			{
				Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("WarningPawnWillDieFromReimplanting".Translate(pawn.Named("PAWN")), delegate
				{
					ReimplanterUtility.GiveReimplantJob(selPawn, pawn, ReimplanterComp.Props.absorberJob);
				}, destructive: true));
			}
			else
			{
				ReimplanterUtility.GiveReimplantJob(selPawn, pawn, ReimplanterComp.Props.absorberJob);
			}
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (XaG_GeneUtility.ActiveDowned(pawn, this))
			{
				yield break;
			}
			if (ReimplanterComp?.Props?.absorberJob == null)
			{
				yield break;
			}
			Pawn myPawn = pawn;
			Command_Action command_Action = new()
			{
				defaultLabel = "WVC_ForceXenogermImplantation".Translate(),
				defaultDesc = "WVC_ForceXenogermImplantationDesc".Translate(),
				icon = ContentFinder<Texture2D>.Get(def.iconPath),
				action = delegate
				{
					List<FloatMenuOption> list = new();
					List<Pawn> list2 = myPawn.MapHeld.mapPawns.SpawnedPawnsInFaction(Faction.OfPlayer);
					for (int i = 0; i < list2.Count; i++)
					{
						Pawn absorber = list2[i];
						if (absorber != pawn && absorber.IsColonistPlayerControlled && absorber.CanReach(pawn, PathEndMode.ClosestTouch, Danger.Deadly) && absorber.IsHuman())
						{
							if (!ReimplanterUtility.PawnIdeoCanAcceptReimplant(myPawn, absorber))
							{
								list.Add(new FloatMenuOption(absorber.LabelCap + ": " + "IdeoligionForbids".Translate(), null, absorber, Color.white));
							}
							else
							{
								list.Add(new FloatMenuOption(absorber.LabelShort, delegate
								{
									GiveJob(absorber);
								}, absorber, Color.white));
							}
						}
					}
					if (list.Any())
					{
						Find.WindowStack.Add(new FloatMenu(list));
					}
				}
			};
			if (myPawn.IsQuestLodger())
			{
				command_Action.Disable("TemporaryFactionMember".Translate(myPawn.Named("PAWN")));
			}
			else if (myPawn.health.hediffSet.HasHediff(HediffDefOf.XenogermLossShock))
			{
				command_Action.Disable("XenogermLossShockPresent".Translate(myPawn.Named("PAWN")));
			}
			else if (myPawn.IsPrisonerOfColony && !myPawn.Downed)
			{
				command_Action.Disable("MessageTargetMustBeDownedToForceReimplant".Translate(myPawn.Named("PAWN")));
			}
			yield return command_Action;
		}

	}

    // InDev
    public class Gene_PostImplanter : Gene
	{

		private CompAbilityEffect_PostImplanter cachedReimplanterComp;

		public CompAbilityEffect_PostImplanter ReimplanterComp
		{
			get
			{
				if (cachedReimplanterComp == null && def.abilities != null)
				{
					cachedReimplanterComp = pawn?.abilities?.GetAbility(def.abilities.FirstOrDefault()).CompOfType<CompAbilityEffect_PostImplanter>();
				}
				return cachedReimplanterComp;
			}
		}

	}

	public class Gene_ImplanterDependant : Gene, IGeneRemoteControl
	{
		public string RemoteActionName => XaG_UiUtility.OnOrOff(activateSubGeneEffect);

		public TaggedString RemoteActionDesc => "WVC_XaG_PostImplanterSubGeneDesc".Translate();

		public void RemoteControl_Action(Dialog_GenesSettings genesSettings)
		{
			activateSubGeneEffect = !activateSubGeneEffect;
		}

		public bool RemoteControl_Hide => !Active;

		public bool RemoteControl_Enabled
		{
			get
			{
				return enabled;
			}
			set
			{
				enabled = value;
				remoteControllerCached = false;
			}
		}

		public override void PostRemove()
		{
			base.PostRemove();
			XaG_UiUtility.SetAllRemoteControllersTo(pawn);
		}

		public bool enabled = true;
		public bool remoteControllerCached = false;

		public void RemoteControl_Recache()
		{
			XaG_UiUtility.RecacheRemoteController(pawn, ref remoteControllerCached, ref enabled);
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (enabled)
			{
				return XaG_UiUtility.GetRemoteControllerGizmo(pawn, remoteControllerCached, this);
			}
			return null;
		}

		public bool activateSubGeneEffect = true;

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref activateSubGeneEffect, "activateSubGeneEffect", defaultValue: true);
		}

		//[Unsaved(false)]
		//private Gene_PostImplanter cachedImplanterGene;

		//public Gene_PostImplanter PostImplanter
		//{
		//	get
		//	{
		//		if (cachedImplanterGene == null || !cachedImplanterGene.Active)
		//		{
		//			cachedImplanterGene = pawn?.genes?.GetFirstGeneOfType<Gene_PostImplanter>();
		//		}
		//		return cachedImplanterGene;
		//	}
		//}

		public void Notify_PostReimplanted(Pawn target)
		{
			if (activateSubGeneEffect)
			{
				DoEffects(target);
			}
		}

		public virtual void DoEffects(Pawn target)
		{

		}

	}

	public class Gene_Implanter_Recruit : Gene_ImplanterDependant
	{

		public override void DoEffects(Pawn target)
		{
			if ((target.Faction == null || target.Faction != Faction.OfPlayer) && target.guest.Recruitable)
			{
				RecruitUtility.Recruit(target, Faction.OfPlayer, pawn);
				Messages.Message("WVC_XaG_ReimplantResurrectionRecruiting".Translate(target), target, MessageTypeDefOf.PositiveEvent);
				target.ideo?.SetIdeo(pawn.ideo.Ideo);
			}
		}

	}

	[Obsolete]
	public class Gene_PostImplanter_Recruit : Gene_Implanter_Recruit
	{

	}

	[Obsolete]
	public class Gene_PostImplanter_Brainwash : Gene_PostImplanter_Recruit
	{

	}

	public class Gene_Implanter_Convert : Gene_ImplanterDependant
	{

		public override void DoEffects(Pawn target)
		{
			if (target.ideo?.Ideo != null && pawn.ideo?.Ideo != null && target.ideo.Ideo != pawn.ideo.Ideo)
			{
				target.ideo.SetIdeo(pawn.ideo.Ideo);
			}
		}

	}

	[Obsolete]
	public class Gene_PostImplanter_Convert : Gene_Implanter_Convert
	{

	}

}
