// RimWorld.StatPart_Age
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class ScenPart_PawnModifier_Beholder : ScenPart_PawnModifier
	{

		public List<XenotypeDef> xenotypeDefs;

		//private List<XenotypeHolder> cachedHolder;
		//public List<XenotypeHolder> Xenotypes
		//{
		//	get
		//	{
		//		if (cachedHolder == null)
		//		{
		//			cachedHolder = xenotypeDefs.ConvertToHolder();
		//		}
		//		return cachedHolder;
		//	}
		//}

		private bool disabled = false;
		protected override void ModifyPawnPostGenerate(Pawn pawn, bool redressed)
		{
			if (disabled)
			{
				return;
			}
			try
			{
				if (Rand.Chance(0.05f) && XaG_GeneUtility.PawnIsBaseliner(pawn) && pawn.IsHuman())
				{
					ScenPart_PawnModifier_CustomWorld.TrySetupCustomWorldXenotype(pawn, xenotypeDefs.ConvertToHolder());
				}
			}
			catch (Exception arg)
			{
				disabled = true;
				Log.Error("Failed modify pawn xenotype. Reason: " + arg.Message);
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Collections.Look(ref xenotypeDefs, "xenotypeDefs", LookMode.Def);
		}

	}

}
