using RimWorld;
using System;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class CompAbilityEffect_ImplantGeneDef : CompAbilityEffect
	{

		public new CompProperties_AbilityReimplanter Props => (CompProperties_AbilityReimplanter)props;

		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			string phase = "";
			try
			{
				phase = "start";
				base.Apply(target, dest);
				Pawn targetPawn = target.Pawn;
				foreach (GeneDef geneDef in Props.geneDefs)
				{
					phase = "implant: " + geneDef.defName;
					if (!XaG_GeneUtility.HasGene(geneDef, targetPawn))
					{
						targetPawn.genes?.AddGene(geneDef, true);
					}
				}
				ReimplanterUtility.PostImplantDebug(targetPawn);
				Messages.Message("WVC_XaG_ImplantedGeneDef_Succes".Translate(), targetPawn, MessageTypeDefOf.NeutralEvent, historical: false);
			}
			catch (Exception arg)
			{
				Log.Error("Failed use implanter. On phase " + phase + ". Reason: " + arg.Message);
			}
		}

		public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
		{
			if (!base.Valid(target, throwMessages))
			{
				return false;
			}
			//if (target.HasThing && target.Thing is Corpse corpse)
			//{
			//	return ReimplanterUtility.ValidCorpseForImplant(target, throwMessages, corpse);
			//}
			if (target.Pawn != null && XaG_GeneUtility.HasAllGenes(Props.geneDefs, target.Pawn))
			{
				//if (throwMessages)
				//{
				//	Messages.Message("MessageCannotImplantInTempFactionMembers".Translate(), pawn, MessageTypeDefOf.RejectInput, historical: false);
				//}
				return false;
			}
			return ReimplanterUtility.ImplanterValidation(parent.def, parent.pawn, target, throwMessages, false);
		}

	}

}
