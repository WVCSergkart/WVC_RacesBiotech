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
			List<XenotypeGetterDef> xenotypeGetterDefs = ListsUtility.XenotypeGetterDefs;
			xenotypeGetterDefs.Shuffle();
			foreach (XenotypeGetterDef xenotypeGetterDef in xenotypeGetterDefs)
			{
				if (xenotypeDef != null)
				{
					break;
				}
				if (Rand.Chance(xenotypeGetterDef.Worker.Chance() / xenotypeGetterDefs.Count) && xenotypeGetterDef.Worker.CanFire())
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
