using RimWorld;
using Verse;
using Verse.AI;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{

	public class RoleRequirement_Mechanitor : RoleRequirement
	{
		// public static readonly RoleRequirement_NotChild Requirement = new RoleRequirement_NotChild();

		public override string GetLabel(Precept_Role role)
		{
			return "WVC_RoleRequirementShouldBeMechanitor".Translate();
		}

		public override bool Met(Pawn p, Precept_Role role)
		{
			return p.health.hediffSet.HasHediff(HediffDefOf.MechlinkImplant);
		}
	}

}
