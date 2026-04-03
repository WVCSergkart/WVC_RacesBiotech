// RimWorld.StatPart_Age
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class ScenPart_PawnModifier_DynamicXenotypes : ScenPart_PawnModifier
	{

		public float dynamicXenotypeChance = 0.05f;

		public virtual float DynamicChance => dynamicXenotypeChance;

		private bool disabled = false;
		protected override void ModifyPawnPostGenerate(Pawn pawn, bool redressed)
		{
			if (disabled)
			{
				return;
			}
			//if (MiscUtility.GameNotStarted())
			//{
			//	return;
			//}
			try
			{
				GetXenotype(pawn);
			}
			catch (Exception arg)
			{
				disabled = true;
				Log.Error("Failed modify pawn xenotype. Reason: " + arg.Message);
			}
		}

		private void GetXenotype(Pawn pawn)
		{
			if (pawn.genes == null || !XaG_GeneUtility.PawnIsBaseliner(pawn))
			{
				return;
			}
			XenotypeDef xenotypeDef = null;
			if (Rand.Chance(DynamicChance))
			{
				//List<XenotypeGetterDef> xenotypeGetterDefs = ListsUtility.XenotypeGetterDefs;
				//xenotypeGetterDefs.Shuffle();
				//foreach (XenotypeGetterDef xenotypeGetterDef in xenotypeGetterDefs)
				//{
				//	if (xenotypeDef != null)
				//	{
				//		break;
				//	}
				//	if (Rand.Chance(xenotypeGetterDef.Worker.Chance() / xenotypeGetterDefs.Count) && xenotypeGetterDef.Worker.CanFire(pawn))
				//	{
				//		xenotypeDef = xenotypeGetterDef.Worker.GetXenotype();
				//	}
				//}
				xenotypeDef = ListsUtility.XenotypeGetterDefs?.Where(getter => getter.Worker.CanFire(pawn))?.RandomElementByWeight(getter => getter.Worker.Chance())?.Worker?.GetXenotype();
			}
			if (xenotypeDef != null)
			{
				SetXenotype(pawn, xenotypeDef);
			}
		}

		//private static bool LogError()
		//{
		//	Log.Error("0");
		//	return true;
		//}

		public void SetXenotype(Pawn pawn, XenotypeDef xenotypeDef)
		{
			ScenPart_PawnModifier_CustomWorld.TrySetupCustomWorldXenotype(pawn, [new(xenotypeDef)]);
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref dynamicXenotypeChance, "dynamicXenotypeChance", defaultValue: 0.05f);
		}

	}

}
