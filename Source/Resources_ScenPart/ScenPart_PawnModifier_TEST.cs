// RimWorld.StatPart_Age
using RimWorld;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class ScenPart_PawnModifier_TEST : ScenPart_PawnModifier
	{

		protected override void ModifyNewPawn(Pawn p)
		{
			ReimplanterUtility.SetXenotype(p, DefDatabase<XenotypeDef>.AllDefsListForReading.Where(xenos => xenos.IsXenoGenesDef()).RandomElement());
			MiscUtility.Notify_DebugPawn(p);
		}

	}

}
