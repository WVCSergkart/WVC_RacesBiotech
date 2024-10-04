using RimWorld;
using System.Collections.Generic;
using Verse;


namespace WVC_XenotypesAndGenes
{

	public class Hediff_GolemChunk : Hediff
	{

		public CompSpawnOnDeath_GetColor comp;

		public override bool ShouldRemove => false;

		public override bool Visible => false;

		private HediffStage curStage;

		public override HediffStage CurStage
		{
			get
			{
				if (curStage == null)
				{
					return SetStoneChunk();
				}
				return curStage;
			}
		}

		public HediffStage SetStoneChunk()
		{
			curStage = new();
			if (pawn.TryGetComp(out comp))
			{
				StatModifier armorRating_Blunt = new()
				{
					stat = StatDefOf.ArmorRating_Blunt,
					value = comp.StoneChunk.GetStatValueAbstract(StatDefOf.BluntDamageMultiplier)
				};
				StatModifier armorRating_Sharp = new()
				{
					stat = StatDefOf.ArmorRating_Sharp,
					value = comp.StoneChunk.GetStatValueAbstract(StatDefOf.SharpDamageMultiplier)
				};
				curStage = new HediffStage
				{
					statFactors = new List<StatModifier> { armorRating_Blunt, armorRating_Sharp }
				};
			}
			return curStage;
		}

	}

}
