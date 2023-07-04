using RimWorld;
using Verse;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{

	public class CompAbilityEffect_Spawn : CompAbilityEffect
	{
		public new CompProperties_AbilitySpawn Props => (CompProperties_AbilitySpawn)props;

		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			base.Apply(target, dest);
			GenSpawn.Spawn(Props.thingDef, target.Cell, parent.pawn.Map);
			// if (Props.sendSkipSignal)
			// {
				// CompAbilityEffect_Teleport.SendSkipUsedSignal(target, parent.pawn);
			// }
		}

		public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
		{
			if (target.Cell.Filled(parent.pawn.Map) || (!Props.allowOnBuildings && target.Cell.GetEdifice(parent.pawn.Map) != null))
			{
				if (throwMessages)
				{
					Messages.Message("CannotUseAbility".Translate(parent.def.label) + ": " + "AbilityOccupiedCells".Translate(), target.ToTargetInfo(parent.pawn.Map), MessageTypeDefOf.RejectInput, historical: false);
				}
				return false;
			}
			if ((!Props.allowOnNonSoil && !target.Cell.GetTerrain(parent.pawn.Map).IsSoil))
			{
				if (throwMessages)
				{
					Messages.Message("WVC_XaG_AbilitySpawn_WrongTarget".Translate() + " " + "WVC_XaG_AbilitySpawn_WrongTarget_NotSoil".Translate(), target.ToTargetInfo(parent.pawn.Map), MessageTypeDefOf.RejectInput, historical: false);
				}
				return false;
			}
			return true;
		}
	}

}
