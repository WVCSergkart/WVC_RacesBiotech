// RimWorld.StatPart_Age
using RimWorld;

namespace WVC_XenotypesAndGenes
{
	public class GoodwillSituationWorker_HivemindHatred : GoodwillSituationWorker
	{

		public override int GetNaturalGoodwillOffset(Faction other)
		{
			if (Applies(other))
			{
				return def.naturalGoodwillOffset;
			}
			return 0;
		}

		private bool Applies(Faction a)
		{
			Ideo primaryIdeo = a.ideos.PrimaryIdeo;
			if (primaryIdeo == null)
			{
				return false;
			}
			Ideo primaryIdeo2 = Faction.OfPlayer.ideos.PrimaryIdeo;
			if (primaryIdeo2 == null)
			{
				return false;
			}
			if (primaryIdeo.memes.Contains(def.meme) || primaryIdeo2.memes.Contains(def.meme))
			{
				return true;
			}
			return false;
		}

	}

}
