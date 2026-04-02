// RimWorld.StatPart_Age
using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class ScenPart_PawnModifier_Undead : ScenPart_PawnModifier
	{

		public List<XenotypeDef> undeadXenotypeDefs;
		public List<XenotypeDef> rareXenotypeDefs;

		private bool disabled = false;
		protected override void ModifyPawnPostGenerate(Pawn pawn, bool redressed)
		{
			if (disabled)
			{
				return;
			}
			try
			{
				SetXenotype(pawn);
			}
			catch (Exception arg)
			{
				disabled = true;
				Log.Error("Failed modify pawn xenotype. Reason: " + arg.Message);
			}
		}

		private void SetXenotype(Pawn pawn)
		{
			if (Rand.Chance(0.20f) && XaG_GeneUtility.PawnIsBaseliner(pawn) && pawn.IsHuman())
			{
				if (Rand.Chance(0.80f) || pawn.Faction != null)
				{
					ScenPart_PawnModifier_CustomWorld.TrySetupCustomWorldXenotype(pawn, undeadXenotypeDefs.ConvertToHolder());
					pawn.HumanComponent()?.SetResurrected();
				}
				else
				{
					ScenPart_PawnModifier_CustomWorld.TrySetupCustomWorldXenotype(pawn, rareXenotypeDefs.ConvertToHolder());
				}
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Collections.Look(ref undeadXenotypeDefs, "xenotypeDefs", LookMode.Def);
			Scribe_Collections.Look(ref rareXenotypeDefs, "rareXenotypeDefs", LookMode.Def);
		}

	}

}
