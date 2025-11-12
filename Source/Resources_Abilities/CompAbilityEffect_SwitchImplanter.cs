using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class CompAbilityEffect_SwitchImplanter : CompAbilityEffect_NewImplanter, IAbilityFloatMenu
	{

		//private new CompProperties_AbilityReimplanter Props => (CompProperties_AbilityReimplanter)props;

		[Unsaved(false)]
		private Gene_Switcher cachedGene;
		public Gene_Switcher SwitcherGene
		{
			get
			{
				if (cachedGene == null || !cachedGene.Active)
				{
					cachedGene = parent?.pawn?.genes?.GetFirstGeneOfType<Gene_Switcher>();
				}
				return cachedGene;
			}
		}

		public IEnumerable<FloatMenuOption> FloatMenuOptions
		{
			get
			{
				if (SwitcherGene == null)
				{
					return new List<FloatMenuOption>();
				}
				List<FloatMenuOption> list = new();
				List<XenotypeHolder> xenotypes = SwitcherGene.Xenotypes.ToList();
				//xenotypes.RemoveAll((holder) => XaG_GeneUtility.GenesIsMatch(parent.pawn.genes.GenesListForReading, holder.genes, 1f));
				if (!xenotypes.NullOrEmpty())
				{
					for (int i = 0; i < xenotypes.Count; i++)
					{
						XenotypeHolder geneSet = xenotypes[i];
						list.Add(new FloatMenuOption(geneSet.LabelCap, delegate
						{
							xenotypeHolder = new(geneSet);
						}, orderInPriority: 0 - (int)geneSet.xenotypeDef.displayPriority));
					}
				}
				if (!list.Any())
				{
					list.Add(new FloatMenuOption("WVC_None".Translate(), delegate
					{

					}));
				}
				return list;
			}
		}

		public SaveableXenotypeHolder xenotypeHolder;

		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			string phase = "";
			try
			{
				phase = "start";
				if (SwitcherGene == null || SwitcherGene.Xenotypes.NullOrEmpty())
				{
					return;
				}
				phase = "check holder";
				if (xenotypeHolder == null)
				{
					xenotypeHolder = new(SwitcherGene.Xenotypes.RandomElement());
				}
				//RecacheGizmos();
				//phase = "base apply";
				//base.Apply(target, dest);
				Pawn caster = parent.pawn;
				phase = "try corpse";
				if (target.Thing is Corpse corpse && corpse.InnerPawn != null)
				{
					Pawn innerPawn = corpse.InnerPawn;
					phase = "ResurrectWithSickness";
					GeneResourceUtility.ResurrectWithSickness(innerPawn);
					Messages.Message("MessagePawnResurrected".Translate(innerPawn), innerPawn, MessageTypeDefOf.PositiveEvent);
					phase = "MakeAttachedOverlay";
					MoteMaker.MakeAttachedOverlay(innerPawn, ThingDefOf.Mote_ResurrectFlash, Vector3.zero);
					//CompAbilityEffect_PostImplanter.ApplyOnCorpse(innerPawn, parent.pawn, Props.reimplantEndogenes, Props.reimplantXenogenes);
					phase = "SetXenotype";
					SetXenotype(innerPawn, caster);
					return;
				}
				phase = "try target";
				Pawn targetPawn = target.Pawn;
				if (targetPawn != null)
				{
					SetXenotype(targetPawn, caster);
					return;
				}
			}
			catch (Exception arg)
			{
				Log.Error("Failed use switch implanter. On phase " + phase + ". Reason: " + arg.Message);
			}
		}

		private void SetXenotype(Pawn target, Pawn caster)
		{
			ReimplanterUtility.SetXenotype_Safe(target, xenotypeHolder);
			ReimplanterUtility.UpdateXenogermReplication_WithComa(target);
			ReimplanterUtility.ExtractXenogerm(caster);
			CompAbilityEffect_NewImplanter.Notify_Reimplanted(target, caster);
			ReimplanterUtility.FleckAndLetter(caster, target);
		}

		public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
		{
			if (SwitcherGene == null)
			{
				return false;
			}
			return base.Valid(target, throwMessages);
		}

		//public void RecacheGizmos()
		//{
		//	foreach (Gizmo gizmo in parent.GetGizmos())
		//	{
		//		if (gizmo is Command_Ability_FloatMenu ability_FloatMenu)
		//		{
		//			ability_FloatMenu.Recache();
		//		}
		//	}
		//}

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Deep.Look(ref xenotypeHolder, "xenotypeHolder");
		}

	}

}
