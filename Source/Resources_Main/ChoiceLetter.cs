using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{

	//public class ChoiceLetter_SacrificerMode : ChoiceLetter
	//{
	//	public override bool CanDismissWithRightClick => TimeoutPassed;
	//	//private Pawn pawn;
	//	private ScenPart_PawnModifier_Deathworld scenario;
	//	public void Start(ScenPart_PawnModifier_Deathworld scenPart_PawnModifier_Deathworld)
	//	{
	//		scenario = scenPart_PawnModifier_Deathworld;
	//		StartTimeout(60000);
	//	}

	//	public override void Removed()
	//	{
	//		base.Removed();
	//		if (!scenario.ModeSetted)
	//		{
	//			scenario.SetMode(Gene_Sacrificer.ModeDefs.Where(def => def.canBeScenarioOption).RandomElement());
	//		}
	//	}

	//	public override bool CanShowInLetterStack
	//	{
	//		get
	//		{
	//			return base.CanShowInLetterStack;
	//		}
	//	}

	//	public override bool ShouldAutomaticallyOpenLetter
	//	{
	//		get
	//		{
	//			return base.ShouldAutomaticallyOpenLetter;
	//		}
	//	}

	//	public override IEnumerable<DiaOption> Choices
	//	{
	//		get
	//		{
	//			if (!base.ArchivedOnly)
	//			{
	//				foreach (SacrificerModeDef sacrificerModeDef in Gene_Sacrificer.ModeDefs.Where(def => def.canBeScenarioOption))
	//				{
	//					yield return new DiaOption("WVC_DeathworldScenario_ChoosePenalty".Translate(sacrificerModeDef.LabelCap))
	//					{
	//						resolveTree = true,
	//						action = delegate
	//						{
	//							scenario.SetMode(sacrificerModeDef);
	//						}
	//					};
	//				}
	//			}
	//			else
	//			{
	//				yield return base.Option_Close;
	//			}
	//			//if (base.ArchivedOnly)
	//			//{
	//			//	yield return base.Option_JumpToLocation;
	//			//}
	//			//if (base.ArchivedOnly)
	//			//{
	//			//	yield return base.Option_Close;
	//			//}
	//		}
	//	}

	//}

}
