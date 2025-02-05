using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Exoskin : Gene
	{

		public GeneExtension_Graphic Graphic => def?.GetModExtension<GeneExtension_Graphic>();

		public GeneExtension_Giver Giver => def?.GetModExtension<GeneExtension_Giver>();

		public override void PostAdd()
		{
			base.PostAdd();
			AnomalyHeadsFix();
		}

		public void AnomalyHeadsFix()
		{
			if (!Active)
			{
				return;
			}
			if (!ModsConfig.AnomalyActive)
			{
				return;
			}
			CreepJoinerFormKindDef formKindDef = pawn?.creepjoiner?.form;
			if (formKindDef != null)
			{
				if (formKindDef.forcedHeadTypes.NullOrEmpty())
				{
					return;
				}
				if (formKindDef.forcedHeadTypes.Contains(pawn.story.headType))
				{
					return;
				}
				pawn.story.TryGetRandomHeadFromSet(formKindDef.forcedHeadTypes);
			}
			MutantDef mutantDef = pawn?.mutant?.Def;
			if (mutantDef != null)
			{
				if (mutantDef.forcedHeadTypes.NullOrEmpty())
				{
					return;
				}
				if (mutantDef.forcedHeadTypes.Contains(pawn.story.headType))
				{
					return;
				}
				pawn.story.TryGetRandomHeadFromSet(mutantDef.forcedHeadTypes);
			}
		}

	}

	public class Gene_GauntSkin : Gene_Exoskin, IGeneLifeStageStarted, IGeneOverridden
	{

		public override void PostAdd()
		{
			base.PostAdd();
			ChangeBodyType();
		}

		public void Notify_OverriddenBy(Gene overriddenBy)
		{
		}

		public void Notify_Override()
		{
			ChangeBodyType();
		}

		public void ChangeBodyType()
		{
			if (!Active)
			{
				return;
			}
			if (pawn?.gender == Gender.Female)
			{
				if (pawn?.story?.bodyType == BodyTypeDefOf.Hulk || pawn?.story?.bodyType == BodyTypeDefOf.Fat)
				{
					pawn.story.bodyType = BodyTypeDefOf.Female;
				}
			}
			else if (pawn?.gender == Gender.Male)
			{
				if (pawn?.story?.bodyType == BodyTypeDefOf.Hulk || pawn?.story?.bodyType == BodyTypeDefOf.Fat)
				{
					pawn.story.bodyType = BodyTypeDefOf.Male;
				}
			}
		}

		public void Notify_LifeStageStarted()
		{
			ChangeBodyType();
		}

	}

	// WIP
	// [Obsolete]
	//public class Gene_BodySize : Gene
	//{

	//	public GeneExtension_Graphic Graphic => def?.GetModExtension<GeneExtension_Graphic>();

	//}

	public class Gene_FleshEyesSkin : Gene_Exoskin
	{

		public override void Tick()
		{
			if (!pawn.IsHashIntervalTick(8245))
			{
				return;
			}
			List<Hediff> hediffs = pawn.health.hediffSet.hediffs.Where((hediff) => hediff.def == HediffDefOf.MissingBodyPart && hediff.Part.def.tags.Contains(BodyPartTagDefOf.SightSource)).ToList();
			foreach (Hediff hediff in hediffs)
			{
				BodyPartRecord part = hediff.Part;
				pawn.health.RemoveHediff(hediff);
				Hediff hediff2 = pawn.health.AddHediff(HediffDefOf.Misc, part);
				float partHealth = pawn.health.hediffSet.GetPartHealth(part);
				hediff2.Severity = Mathf.Max(partHealth - 1f, partHealth * 0.9f);
				pawn.health.hediffSet.Notify_Regenerated(partHealth - hediff2.Severity);
			}
			//HealingUtility.Regeneration(pawn, 10, WVC_Biotech.settings.totalHealingIgnoreScarification, 3245);
		}

	}

	[Obsolete]
	public class Gene_FleshmassSkin : Gene_FleshEyesSkin
	{

	}

}
