// RimWorld.StatPart_Age
using RimWorld;
using System;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class GoodwillSituationWorker_HivemindHatred : GoodwillSituationWorker
	{

		private static bool silentCatcherStatus = false;

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
			if (silentCatcherStatus)
			{
				return false;
			}
			try
			{
				Ideo primaryIdeo = a.ideos?.PrimaryIdeo;
				if (primaryIdeo == null)
				{
					return false;
				}
				Ideo primaryIdeo2 = Faction.OfPlayerSilentFail?.ideos?.PrimaryIdeo;
				if (primaryIdeo2 == null)
				{
					return false;
				}
				if (primaryIdeo.memes.Contains(def.meme) || primaryIdeo2.memes.Contains(def.meme))
				{
					return true;
				}
			}
			catch (Exception arg)
			{
				Log.Error("Error in hivemind goodwill. Hatred disabled. Reason: " + arg.Message);
				silentCatcherStatus = true;
			}
			return false;
		}

	}

}
