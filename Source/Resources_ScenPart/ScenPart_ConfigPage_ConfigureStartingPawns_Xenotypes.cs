using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	//public class ScenPart_ConfigPage_ConfigureStartingPawns_Xenotypes : ScenPart_ConfigPage_ConfigureStartingPawnsBase
	//{

	//	public Gender forcedGender = Gender.None;
	//	public List<XenotypeCount> xenotypeCounts = new List<XenotypeCount>();
	//	public List<XenotypePawnKind> overrideKinds = new List<XenotypePawnKind>();
	//	//public List<XenotypeDef> allowIfAny = new();

	//	[MustTranslate]
	//	public string customSummary;

	//	protected override int TotalPawnCount => xenotypeCounts.Sum((XenotypeCount x) => x.count);

	//	public override string Summary(Scenario scen)
	//	{
	//		return customSummary;
	//	}

	//	protected override void GenerateStartingPawns()
	//	{
	//		int num = 0;
	//		do
	//		{
	//			StartingPawnUtility.ClearAllStartingPawns();
	//			int num2 = 0;
	//			foreach (XenotypeCount xenotypeCount in xenotypeCounts)
	//			{
	//				for (int i = 0; i < xenotypeCount.count; i++)
	//				{
	//					PawnGenerationRequest generationRequest = StartingPawnUtility.GetGenerationRequest(num2);
	//					generationRequest.ForcedXenotype = xenotypeCount.xenotype;
	//					if (forcedGender != Gender.None)
	//					{
	//						generationRequest.FixedGender = forcedGender;
	//					}
	//					if (xenotypeCount.xenotype != null)
	//					{
	//						generationRequest.PawnKindDefGetter = delegate (XenotypeDef t)
	//						{
	//							foreach (XenotypePawnKind overrideKind in overrideKinds)
	//							{
	//								if (t == overrideKind.xenotype)
	//								{
	//									return overrideKind.pawnKind;
	//								}
	//							}
	//							return Find.GameInitData.startingPawnKind ?? Faction.OfPlayer.def.basicMemberKind;
	//						};
	//					}
	//					else
	//					{
	//						generationRequest.PawnKindDefGetter = null;
	//					}
	//					StartingPawnUtility.SetGenerationRequest(num2, generationRequest);
	//					StartingPawnUtility.AddNewPawn(num2);
	//					num2++;
	//				}
	//			}
	//			num++;
	//		}
	//		while (num <= 20 && !StartingPawnUtility.WorkTypeRequirementsSatisfied());
	//	}

	//	public override void PostIdeoChosen()
	//	{
	//		Find.GameInitData.startingPawnsRequired = null;
	//		Find.GameInitData.startingPawnKind = null;
	//		Find.GameInitData.startingXenotypesRequired = null;
	//		base.PostIdeoChosen();
	//	}

	//	public override bool HasNullDefs()
	//	{
	//		if (base.HasNullDefs())
	//		{
	//			return true;
	//		}
	//		foreach (XenotypeCount xenotypeCount in xenotypeCounts)
	//		{
	//			if (xenotypeCount.xenotype == null)
	//			{
	//				return true;
	//			}
	//		}
	//		return false;
	//	}

	//	public override void ExposeData()
	//	{
	//		base.ExposeData();
	//		Scribe_Collections.Look(ref xenotypeCounts, "xenotypeCounts", LookMode.Deep);
	//	}

	//	public override int GetHashCode()
	//	{
	//		int num = base.GetHashCode();
	//		foreach (XenotypeCount xenotypeCount in xenotypeCounts)
	//		{
	//			num ^= xenotypeCount.GetHashCode();
	//		}
	//		return num;
	//	}

	//}


}
