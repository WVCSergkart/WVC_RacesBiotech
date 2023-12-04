// RimWorld.CompProperties_Toxifier
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_UseEffectEatSeed : CompProperties_Usable
	{
		public HediffDef hediffDef;

		public int bandwidthGain = 1;

		public CompProperties_UseEffectEatSeed()
		{
			compClass = typeof(CompUseEffect_EatSeed);
		}
	}

	public class CompUseEffect_EatSeed : CompUseEffect
	{
		public CompProperties_UseEffectEatSeed Props => (CompProperties_UseEffectEatSeed)props;

		private int cachedSporesCount = 0;

		public override void DoEffect(Pawn user)
		{
			RemoveHediff(Props.hediffDef, user);
			AddHediff(Props.hediffDef, user, cachedSporesCount + Props.bandwidthGain);
		}

		public void AddHediff(HediffDef hediffDef, Pawn pawn, int amount)
		{
			if (!pawn.health.hediffSet.HasHediff(hediffDef))
			{
				Hediff hediff = HediffMaker.MakeHediff(Props.hediffDef, pawn);
				HediffWithComps_Spores hediff_Spores = (HediffWithComps_Spores)hediff;
				if (hediff_Spores != null)
				{
					hediff_Spores.cachedSporesCount = amount;
				}
				pawn.health.AddHediff(hediff);
			}
		}

		public void RemoveHediff(HediffDef hediffDef, Pawn pawn)
		{
			if (pawn.health.hediffSet.HasHediff(hediffDef))
			{
				Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(Props.hediffDef);
				HediffWithComps_Spores hediff_Spores = (HediffWithComps_Spores)firstHediffOfDef;
				if (hediff_Spores != null)
				{
					cachedSporesCount = hediff_Spores.cachedSporesCount;
				}
				pawn.health.RemoveHediff(firstHediffOfDef);
			}
		}

		public override bool CanBeUsedBy(Pawn p, out string failReason)
		{
			if (!WalkingUtility.MechanitorIsLich(p))
			{
				failReason = "WVC_XaG_MechanitorShouldBeLich".Translate();
				return false;
			}
			if (!p.IsFreeColonist || p.HasExtraHomeFaction())
			{
				failReason = "InstallImplantNotAllowedForNonColonists".Translate();
				return false;
			}
			failReason = null;
			return true;
		}

	}

}
