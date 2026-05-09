using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.Noise;

namespace WVC_XenotypesAndGenes
{

	public class CompAbilityEffect_Replacer : CompAbilityEffect_NewImplanter
	{

		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			try
			{
				Pawn caster = parent.pawn;
				Pawn victim = target.Pawn;
				if (victim != null)
				{
					DuplicateUtility.CopyPawn(caster, victim);
					MiscUtility.DoSkipEffects(caster.PositionHeld, caster.MapHeld);
					MiscUtility.DoSkipEffects(victim.PositionHeld, victim.MapHeld);
				}
			}
			catch (Exception arg)
			{
				Log.Error($"Failed replace pawn. Reason: {arg.Message}");
			}
		}

		public override Window ConfirmationDialog(LocalTargetInfo target, Action confirmAction)
		{
			return null;
		}

		public override IEnumerable<Mote> CustomWarmupMotes(LocalTargetInfo target)
		{
			return Enumerable.Empty<Mote>();
		}

	}

}
