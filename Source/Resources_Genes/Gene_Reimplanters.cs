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
			if (!xenotype.inheritable)
			{
				for (int numXenogenes = genes.Xenogenes.Count - 1; numXenogenes >= 0; numXenogenes--)
				{
					pawn.genes?.RemoveGene(genes.Xenogenes[numXenogenes]);
				}
			}
			if (xenotype.inheritable)
			{
				for (int numEndogenes = genes.Endogenes.Count - 1; numEndogenes >= 0; numEndogenes--)
				{
					pawn.genes?.RemoveGene(genes.Endogenes[numEndogenes]);
				}
			}
			if (pawn.genes.Xenogenes.NullOrEmpty() && xenotype.inheritable || !xenotype.inheritable)
			{
				pawn.genes?.SetXenotypeDirect(xenotype);
				// pawn.genes.xenotypeName = xenotype.label;
				pawn.genes.iconDef = null;
			}
			bool xenotypeHasSkinColor = false;
			bool xenotypeHasHairColor = false;
			for (int i = 0; i < xenotype.genes.Count; i++)
			{
				pawn.genes?.AddGene(xenotype.genes[i], !xenotype.inheritable);
				if (xenotype.genes[i].skinColorBase != null || xenotype.genes[i].skinColorOverride != null)
				{
					xenotypeHasSkinColor = true;
				}
				if (xenotype.genes[i].hairColorOverride != null)
				{
					xenotypeHasHairColor = true;
				}
			}
			if (xenotype.inheritable && !xenotypeHasSkinColor || xenotype == XenotypeDefOf.Baseliner)
			{
				pawn.genes?.AddGene(WVC_GenesDefOf.Skin_SheerWhite, xenogene: false);
			}
			if (xenotype.inheritable && !xenotypeHasHairColor || xenotype == XenotypeDefOf.Baseliner)
			{
				pawn.genes?.AddGene(WVC_GenesDefOf.Hair_SnowWhite, xenogene: false);
			}
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
			if (ReimplanterComp?.Props?.absorberJob == null || !ReimplanterUtility.CanAbsorbGenogerm(pawn))
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
			if (ReimplanterComp?.Props?.absorberJob == null || !ReimplanterUtility.CanAbsorbGenogerm(pawn))
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
							if (!CompAbilityEffect_Reimplanter.PawnIdeoCanAcceptReimplant(myPawn, absorber))
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

		private CompAbilityEffect_RiseFromTheDead cachedReimplanterComp;

		public CompAbilityEffect_RiseFromTheDead ReimplanterComp
		{
			get
			{
				if (cachedReimplanterComp == null && def.abilities != null)
				{
					cachedReimplanterComp = pawn?.abilities?.GetAbility(def.abilities.FirstOrDefault()).CompOfType<CompAbilityEffect_RiseFromTheDead>();
				}
				return cachedReimplanterComp;
			}
		}

	}

    public class Gene_XenotypeImplanter : Gene
	{

		public XenotypeDef xenotypeDef = null;

		public override void PostAdd()
		{
			base.PostAdd();
			if (xenotypeDef == null)
			{
				xenotypeDef = pawn.genes.Xenotype;
			}
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (XaG_GeneUtility.SelectorDraftedActiveFactionMap(pawn, this))
			{
				yield break;
			}
			yield return new Command_Action
			{
				defaultLabel = def.LabelCap + ": " + (xenotypeDef != null ? xenotypeDef.LabelCap.ToString() : "ERR"),
				defaultDesc = "WVC_XaG_GeneShapeshifter_Desc".Translate(),
				icon = ContentFinder<Texture2D>.Get(def.iconPath),
				action = delegate
				{
					Find.WindowStack.Add(new Dialog_ReimplanterXenotype(this));
				}
			};
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Defs.Look(ref xenotypeDef, "xenotypeDef");
		}

	}

	[Obsolete]
	public class Gene_Reimplanter : Gene_XenotypeImplanter
	{



	}

    public class Gene_PostImplanterDependant : Gene
    {

        [Unsaved(false)]
        private Gene_PostImplanter cachedImplanterGene;

        public Gene_PostImplanter PostImplanter
        {
            get
            {
                if (cachedImplanterGene == null || !cachedImplanterGene.Active)
                {
                    cachedImplanterGene = pawn?.genes?.GetFirstGeneOfType<Gene_PostImplanter>();
                }
                return cachedImplanterGene;
            }
        }

        public virtual void Notify_TargetResurrected(Pawn target)
        {

        }

    }

    public class Gene_PostImplanter_Brainwash : Gene_PostImplanterDependant
	{

		public override void Notify_TargetResurrected(Pawn target)
		{
			if ((target.Faction == null || target.Faction != Faction.OfPlayer) && target.guest.Recruitable)
			{
				RecruitUtility.Recruit(target, Faction.OfPlayer, pawn);
				Messages.Message("WVC_XaG_ReimplantResurrectionRecruiting".Translate(target), target, MessageTypeDefOf.PositiveEvent);
			}
		}

	}

}
