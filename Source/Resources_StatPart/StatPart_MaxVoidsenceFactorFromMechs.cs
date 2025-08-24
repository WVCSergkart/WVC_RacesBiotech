// RimWorld.StatPart_Age
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{
    public class StatPart_MaxVoidsenceFactorFromMechs : StatPart
	{

		//public string label = "WVC_XaG_StatPart_OffsetFromGenes";

		public override void TransformValue(StatRequest req, ref float val)
		{
			if (TryGetFactor(req, out var offset))
			{
				val *= offset;
			}
		}

		public override string ExplanationPart(StatRequest req)
		{
			return null;
		}

		public virtual bool TryGetFactor(StatRequest req, out float offset)
		{
			//Log.Error("0");
			offset = 0f;
			if (!WVC_Biotech.settings.voidlink_dynamicResourceLimit)
            {
				return false;
            }
			if (!req.HasThing || req.Thing is not Pawn pawn || pawn.genes == null)
			{
				return false;
			}
			if (pawn.mechanitor == null)
            {
				return false;
            }
            Gene_Voidlink voidlink = pawn?.genes?.GetFirstGeneOfType<Gene_Voidlink>();
			if (voidlink == null)
            {
				return false;
            }
			//if (voidlink.MaxMechs > voidlink.AllMechsCount)
			//{
			//}
			offset = 1f + (0.1f * (voidlink.MaxMechs - voidlink.AllMechsCount));
			return true;
		}

	}

}
