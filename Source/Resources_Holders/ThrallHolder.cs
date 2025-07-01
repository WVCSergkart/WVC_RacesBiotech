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
					stringBuilder.Append(thrallDef.Description);
					cachedDescription = stringBuilder.ToString();
				}
				return cachedDescription;
			}
		}

	}

}
