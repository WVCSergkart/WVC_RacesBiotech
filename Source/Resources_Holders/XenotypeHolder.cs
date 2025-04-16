using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace WVC_XenotypesAndGenes
{
    public class XenotypeHolder
	{

		public string name = null;

		public XenotypeIconDef iconDef = null;

		public XenotypeDef xenotypeDef = null;

		public List<GeneDef> genes = new();

		public bool inheritable;

		public float displayPriority;

		public bool shouldSkip = false;

		public bool isTrueShiftForm;

		public bool isOverriden;

		public float? matchPercent;

		//public string customEffectsDesc = null;

		public bool Baseliner => xenotypeDef == XenotypeDefOf.Baseliner && genes.NullOrEmpty();

		public bool CustomXenotype => xenotypeDef == XenotypeDefOf.Baseliner && !genes.NullOrEmpty();

		public XenotypeHolder()
		{

		}

		public XenotypeHolder(XenotypeDef xenotypeDef)
		{
			this.xenotypeDef = xenotypeDef;
			genes = xenotypeDef.genes;
			inheritable = xenotypeDef.inheritable;
		}

		[Unsaved(false)]
		private TaggedString cachedLabelCap = null;

		[Unsaved(false)]
		private TaggedString cachedLabel = null;

		public virtual TaggedString Label
		{
			get
			{
				if (cachedLabel == null)
				{
					if (name.NullOrEmpty())
					{
						cachedLabel = xenotypeDef.label;
					}
					else
					{
						cachedLabel = name;
					}
				}
				return cachedLabel;
			}
		}

		public virtual TaggedString LabelCap
		{
			get
			{
				if (cachedLabelCap == null)
				{
					cachedLabelCap = Label.CapitalizeFirst();
				}
				return cachedLabelCap;
			}
		}

		[Unsaved(false)]
		private string cachedDescription;

		public virtual string Description
		{
			get
			{
				if (cachedDescription == null)
				{
					StringBuilder stringBuilder = new();
					stringBuilder.AppendLine(LabelCap.Colorize(ColoredText.TipSectionTitleColor));
					stringBuilder.AppendLine();
					if (xenotypeDef != XenotypeDefOf.Baseliner)
					{
						stringBuilder.AppendLine(!xenotypeDef.descriptionShort.NullOrEmpty() ? xenotypeDef.descriptionShort : xenotypeDef.description);
						if (!xenotypeDef.doubleXenotypeChances.NullOrEmpty())
						{
							stringBuilder.AppendLine();
							stringBuilder.AppendLine(("WVC_DoubleXenotypes".Translate() + ":").Colorize(ColoredText.TipSectionTitleColor) + "\n" + xenotypeDef.doubleXenotypeChances.Select((XenotypeChance x) => "WVC_XaG_DoubleXenotypeWithChanceText".Translate(x.xenotype.LabelCap, (x.chance * 100f).ToString()).ToString()).ToLineList(" - "));
						}
					}
					else
					{
						stringBuilder.AppendLine("UniqueXenotypeDesc".Translate());
					}
					stringBuilder.AppendLine();
					//if (customEffectsDesc != null)
					//{
					//	stringBuilder.AppendLine(customEffectsDesc);
					//	stringBuilder.AppendLine();
					//}
					XaG_GeneUtility.GetBiostatsFromList(genes, out int biostatCpx, out int biostatMet, out int biostatArc);
					bool flag2 = false;
					if (biostatCpx != 0)
					{
						stringBuilder.AppendLineTagged("Complexity".Translate().Colorize(GeneUtility.GCXColor) + ": " + biostatCpx.ToStringWithSign());
						flag2 = true;
					}
					if (biostatMet != 0)
					{
						stringBuilder.AppendLineTagged("Metabolism".Translate().CapitalizeFirst().Colorize(GeneUtility.METColor) + ": " + biostatMet.ToStringWithSign());
						flag2 = true;
					}
					if (biostatArc != 0)
					{
						stringBuilder.AppendLineTagged("ArchitesRequired".Translate().Colorize(GeneUtility.ARCColor) + ": " + biostatArc.ToStringWithSign());
						flag2 = true;
					}
					if (flag2)
					{
						stringBuilder.AppendLine();
					}
					if (matchPercent.HasValue)
					{
						stringBuilder.AppendLine(("WVC_XaG_XenoHolder_GenesMatch".Translate() + ":").Colorize(ColoredText.TipSectionTitleColor) + " " + (matchPercent.Value * 100 + "%").ToString());
						stringBuilder.AppendLine();
					}
					stringBuilder.Append(("WVC_Inheritable".Translate() + ":").Colorize(ColoredText.TipSectionTitleColor) + " " + inheritable.ToStringYesNo());
					cachedDescription = stringBuilder.ToString();
				}
				return cachedDescription;
			}
		}

	}

}
