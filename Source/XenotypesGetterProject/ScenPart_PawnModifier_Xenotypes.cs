// RimWorld.StatPart_Age
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class ScenPart_PawnModifier_Xenotypes : ScenPart_PawnModifier
	{

		private bool disabled = false;
		protected override void ModifyPawnPostGenerate(Pawn pawn, bool redressed)
		{
			if (disabled)
			{
				return;
			}
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
			if (!XaG_GeneUtility.PawnIsBaseliner(pawn) || !pawn.IsHuman())
			{
				return;
			}
			XenotypeDef xenotypeDef = null;
			List<XenotypeGetterDef> allDefsListForReading = DefDatabase<XenotypeGetterDef>.AllDefsListForReading;
			allDefsListForReading.Shuffle();
			foreach (XenotypeGetterDef xenotypeGetterDef in allDefsListForReading)
			{
				if (xenotypeDef != null)
				{
					break;
				}
				if (Rand.Chance(xenotypeGetterDef.chance) && xenotypeGetterDef.Worker.CanFire())
				{
					xenotypeDef = xenotypeGetterDef.Worker.GetXenotype();
				}
			}
			if (xenotypeDef != null)
			{
				SetXenotype(pawn, xenotypeDef);
			}
		}

		private void SetXenotype(Pawn pawn, XenotypeDef xenotypeDef)
		{
			ScenPart_PawnModifier_CustomWorld.TrySetupCustomWorldXenotype(pawn, [new(xenotypeDef)]);
		}

	}

}
