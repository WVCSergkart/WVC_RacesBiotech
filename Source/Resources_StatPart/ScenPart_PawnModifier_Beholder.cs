// RimWorld.StatPart_Age
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class ScenPart_PawnModifier_Beholder : ScenPart_PawnModifier
	{

		public List<XenotypeDef> xenotypeDefs;

		private List<XenotypeHolder> cachedHolder;
		public List<XenotypeHolder> Xenotypes
		{
			get
			{
				if (cachedHolder == null)
				{
					cachedHolder = xenotypeDefs.ConvertToHolder();
				}
				return cachedHolder;
			}
		}

		protected override void ModifyPawnPostGenerate(Pawn pawn, bool redressed)
		{
			if (Rand.Chance(0.05f) && pawn.IsBaseliner() && pawn.IsHuman())
			{
				ScenPart_PawnModifier_CustomWorld.TrySetupCustomWorldXenotype(pawn, Xenotypes);
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Collections.Look(ref xenotypeDefs, "xenotypeDefs", LookMode.Def);
		}

	}

}
