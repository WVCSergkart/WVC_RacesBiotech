using RimWorld;
using System;
using System.Collections.Generic;
using Verse;
using Verse.Sound;
using static UnityEngine.GraphicsBuffer;

namespace WVC_XenotypesAndGenes
{
	public class CompChimeraLimit : ThingComp
	{

		public CompProperties_CustomFloatMenu Props => (CompProperties_CustomFloatMenu)props;

		public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
		{
			if (Props.jobDef == null)
			{
				yield break;
			}
			if (!selPawn.IsChimerkin())
			{
				yield break;
			}
			//Log.Error("01");
			yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("WVC_XaG_GeneChimera_ArchiteDevour".Translate(), delegate
			{
				MiscUtility.MakeCustomJob(selPawn, parent, Props.jobDef, factor: Props.factor);
			}), selPawn, parent);
			if (parent.stackCount <= 1)
			{
				yield break;
			}
			yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("WVC_XaG_GeneChimera_ArchiteDevour".Translate() + " x" + parent.stackCount, delegate
			{
				MiscUtility.MakeCustomJob(selPawn, parent, Props.jobDef, null, true, factor: Props.factor);
			}), selPawn, parent);
		}

		public virtual void Action(Pawn pawn, Thing target, bool consumeStack, float factor = 1f)
		{

		}

	}

	public class CompChimeraComplexityLimit : CompChimeraLimit
	{

		public override void Action(Pawn pawn, Thing target, bool consumeStack, float factor = 1f)
		{
			Gene_Chimera chimera = pawn.genes?.GetFirstGeneOfType<Gene_Chimera>();
			if (chimera == null)
			{
				return;
			}
			if (consumeStack)
			{
				chimera.AddComplexityLimit((int)(target.stackCount * factor));
				target.Destroy();
			}
			else
			{
				chimera.AddComplexityLimit((int)(1 * factor));
				target.ReduceStack();
			}
			if (!chimera.Extension_Undead.soundDefOnImplant.NullOrUndefined())
			{
				chimera.Extension_Undead.soundDefOnImplant.PlayOneShot(SoundInfo.InMap(pawn));
			}
			Messages.Message("WVC_XaG_ChimeraCpxLimitMessage".Translate(pawn.NameShortColored), pawn, MessageTypeDefOf.PositiveEvent, historical: false);
		}

	}

	public class CompChimeraArchiteLimit : CompChimeraLimit
	{

		public override void Action(Pawn pawn, Thing target, bool consumeStack, float factor = 1f)
		{
			Gene_Chimera chimera = pawn.genes?.GetFirstGeneOfType<Gene_Chimera>();
			if (chimera == null)
			{
				return;
			}
			if (consumeStack)
			{
				chimera.AddArchiteLimit((int)(target.stackCount * factor));
				target.Destroy();
			}
			else
			{
				chimera.AddArchiteLimit((int)(1 * factor));
				target.ReduceStack();
			}
			if (!chimera.Extension_Undead.soundDefOnImplant.NullOrUndefined())
			{
				chimera.Extension_Undead.soundDefOnImplant.PlayOneShot(SoundInfo.InMap(pawn));
			}
			Messages.Message("WVC_XaG_ChimeraArcLimitMessage".Translate(pawn.NameShortColored), pawn, MessageTypeDefOf.PositiveEvent, historical: false);
		}

	}

}
