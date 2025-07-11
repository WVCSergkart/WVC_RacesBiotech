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

		public override void TickInterval(int delta)
		{

		}

		public void AnomalyHeadsFix()
		{
			if (!Active)
			{
				return;
			}
			if (pawn.IsCreepJoiner)
			{
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
			}
			if (pawn.IsMutant)
			{
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

	}

	public class Gene_GauntSkin : Gene_Exoskin, IGeneLifeStageStarted, IGeneOverridden
	{

		public override void PostAdd()
		{
			base.PostAdd();
			ChangeBodyType();
		}

		public virtual void Notify_OverriddenBy(Gene overriddenBy)
		{
		}

		public virtual void Notify_Override()
		{
			ChangeBodyType();
		}

		public virtual void ChangeBodyType()
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

		public override void TickInterval(int delta)
		{
			if (!pawn.IsHashIntervalTick(8245, delta))
			{
				return;
			}
			List<Hediff> hediffs = pawn.health.hediffSet.hediffs.Where((hediff) => hediff is Hediff_MissingPart && hediff.Part.def.tags.Contains(BodyPartTagDefOf.SightSource)).ToList();
			foreach (Hediff hediff in hediffs)
            {
				HealingUtility.Regenerate(pawn, hediff);
            }
            //HealingUtility.Regeneration(pawn, 10, WVC_Biotech.settings.totalHealingIgnoreScarification, 3245);
        }
	}

	public class Gene_Mechaskin : Gene_Exoskin
	{

		//public bool Enabled => Giver?.mutantDef != null;

		//public override void PostAdd()
		//{
		//	base.PostAdd();
		//	if (!Enabled)
		//	{
		//		return;
		//	}
		//	Gene_Subhuman.ClearOrSetPawnAsMutantInstantly(pawn, Giver?.mutantDef);
		//}

		//public void Notify_OverriddenBy(Gene overriddenBy)
		//{
		//	if (!Enabled)
		//	{
		//		return;
		//	}
		//	if (overriddenBy != null)
		//	{
		//		Gene_Subhuman.ClearOrSetPawnAsMutantInstantly(pawn, null);
		//	}
		//}

		//public void Notify_Override()
		//{
		//	if (!Enabled)
		//	{
		//		return;
		//	}
		//	Gene_Subhuman.ClearOrSetPawnAsMutantInstantly(pawn, Giver?.mutantDef);
		//}

		//public override void PostRemove()
		//{
		//	base.PostRemove();
		//	if (!Enabled)
		//	{
		//		return;
		//	}
		//	Gene_Subhuman.ClearOrSetPawnAsMutantInstantly(pawn, null);
		//}

	}

	//[Obsolete]
	//public class Gene_FleshmassSkin : Gene_FleshEyesSkin
	//{

	//}

	public class Gene_ScarsSkin : Gene_Exoskin, IGeneScarifier
	{

		[Unsaved(false)]
		private Gene_Shapeshifter cachedShapeshifterGene;
		public Gene_Shapeshifter Shapeshifter
		{
			get
			{
				if (cachedShapeshifterGene == null || !cachedShapeshifterGene.Active)
				{
					cachedShapeshifterGene = pawn?.genes?.GetFirstGeneOfType<Gene_Shapeshifter>();
				}
				return cachedShapeshifterGene;
			}
		}

		public void Notify_Scarified()
        {
            if (Shapeshifter == null)
            {
                return;
            }
            if (!Shapeshifter.TryOffsetResource(4))
            {
                return;
            }
            if (PawnUtility.ShouldSendNotificationAbout(pawn))
            {
                Messages.Message("WVC_XaG_ScarsSkin_ResourceOffset".Translate(pawn.Named("PAWN"), 4), pawn, MessageTypeDefOf.NeutralEvent);
            }
        }

	}

	public class Gene_BoneSkin : Gene_GauntSkin
	{

		public override void ChangeBodyType()
		{
			if (!Active)
			{
				return;
			}
			if (pawn.story.bodyType == BodyTypeDefOf.Fat)
			{
				if (pawn?.gender == Gender.Female)
				{
					pawn.story.bodyType = BodyTypeDefOf.Female;
				}
				else if (pawn?.gender == Gender.Male)
				{
					pawn.story.bodyType = BodyTypeDefOf.Male;
				}
			}
		}

	}

}
