using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.AI.Group;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{

	public class HediffCompProperties_OnlyOneOverseer : HediffCompProperties
	{

		public HediffCompProperties_OnlyOneOverseer()
		{
			compClass = typeof(HediffComp_OnlyOneOverseer);
		}
	}

	public class HediffComp_OnlyOneOverseer : HediffComp
	{

		public HediffCompProperties_OnlyOneOverseer Props => (HediffCompProperties_OnlyOneOverseer)props;

		private Pawn firstOverseer = null;

		public override string CompLabelInBracketsExtra => GetLabel();

		// public override void CompPostMake()
		// {
			// base.CompPostMake();
			// Pawn mechanoid = parent.pawn;
			// if (firstOverseer == null)
			// {
				// firstOverseer = mechanoid.GetOverseer();
			// }
		// }

		// public override void CompPostPostAdd(DamageInfo? dinfo)
		// {
			// base.CompPostPostAdd(dinfo);
			// Pawn mechanoid = parent.pawn;
			// if (firstOverseer == null)
			// {
				// firstOverseer = mechanoid.GetOverseer();
			// }
		// }

		public override void CompPostTick(ref float severityAdjustment)
		{
			base.CompPostTick(ref severityAdjustment);
			if (!Pawn.IsHashIntervalTick(1500))
			{
				return;
			}
			if (firstOverseer == null)
			{
				firstOverseer = parent.pawn.GetOverseer();
			}
			if (!Pawn.IsHashIntervalTick(60000))
			{
				return;
			}
			Pawn mechanoid = parent.pawn;
			if (mechanoid.Map == null)
			{
				return;
			}
			if (!Pawn.IsHashIntervalTick(300000))
			{
				return;
			}
			Pawn overseer = mechanoid.GetOverseer();
			if (overseer != firstOverseer)
			{
				base.Pawn.Kill(null, parent);
			}
		}

		public override void CompExposeData()
		{
			base.CompExposeData();
			Scribe_References.Look(ref firstOverseer, "firstOverseer");
		}

		public string GetLabel()
		{
			// Pawn mechanoid = parent.pawn;
			if (firstOverseer != null)
			{
				return firstOverseer.NameShortColored;
			}
			return "";
		}

	}

}
