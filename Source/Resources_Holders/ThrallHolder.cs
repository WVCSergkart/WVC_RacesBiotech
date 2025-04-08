using RimWorld;
using System.Linq;
using System.Text;
using Verse;

namespace WVC_XenotypesAndGenes
{
    public class ThrallHolder : XenotypeHolder
	{

		public ThrallDef thrallDef;

		[Unsaved(false)]
		private string cachedDescription;

		public override string Description
		{
			get
			{
				if (cachedDescription == null)
				{
					StringBuilder stringBuilder = new();
					stringBuilder.AppendLine(LabelCap.Colorize(ColoredText.TipSectionTitleColor));
					stringBuilder.AppendLine();
					stringBuilder.AppendLine(thrallDef.description);
					if (thrallDef.xenotypeDef != null && !thrallDef.xenotypeDef.descriptionShort.NullOrEmpty())
					{
						stringBuilder.AppendLine();
						stringBuilder.AppendLine(thrallDef.xenotypeDef.descriptionShort);
					}
					if (thrallDef.reqGeneDef != null)
					{
						stringBuilder.AppendLine();
						stringBuilder.AppendLine("Requires".Translate() + ": " + thrallDef.reqGeneDef.LabelCap);
					}
					stringBuilder.AppendLine();
					stringBuilder.Append("WVC_XaG_AcceptableRotStages".Translate().Colorize(ColoredText.TipSectionTitleColor) + ":\n" + thrallDef.acceptableRotStages.Select((RotStage x) => x.ToStringHuman()).ToLineList(" - "));
					cachedDescription = stringBuilder.ToString();
				}
				return cachedDescription;
			}
		}

	}

}
