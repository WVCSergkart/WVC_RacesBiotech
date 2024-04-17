using RimWorld;
using Verse;

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
			return MechanitorUtility.IsMechanitor(p);
		}
	}

}
