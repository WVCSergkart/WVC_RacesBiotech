using RimWorld;
using System.Collections;
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

		public XenotypeDef XenotypeDef_Safe
		{
			get
			{
				if (xenotypeDef == null)
				{
					xenotypeDef = ReimplanterUtility.UnknownXenotypeDef(genes);
					if (xenotypeDef == null)
					{
						xenotypeDef = XenotypeDefOf.Baseliner;
					}
					inheritable = xenotypeDef.inheritable;
				}
				return xenotypeDef;
			}
		}

		public bool Baseliner => XenotypeDef_Safe == XenotypeDefOf.Baseliner && genes.NullOrEmpty();
		public bool CustomXenotype => XenotypeDef_Safe == XenotypeDefOf.Baseliner && !genes.NullOrEmpty();

		public XenotypeHolder()
		{

		}

		//public XenotypeHolder(XenotypeDef xenotypeDef)
		//{
		//	this.xenotypeDef = xenotypeDef;
		//	genes = xenotypeDef.genes;
		//	inheritable = xenotypeDef.inheritable;
		//}

		public XenotypeHolder(XenotypeDef xenotypeDef)
		{
			if (xenotypeDef == XenotypeDefOf.Baseliner || xenotypeDef.genes.NullOrEmpty())
			{
				this.shouldSkip = true;
			}
			this.xenotypeDef = xenotypeDef;
			this.genes = xenotypeDef.genes;
			this.displayPriority = xenotypeDef.displayPriority;
			this.inheritable = xenotypeDef.inheritable;
		}

		public XenotypeHolder(CustomXenotype customXenotype)
		{
			SetupCustomXenotype(customXenotype);
		}

		public XenotypeHolder(CustomXenotype customXenotype, int index)
		{
			SetupCustomXenotype(customXenotype);
			this.displayPriority = -1 * (10000 + index);
		}

		private void SetupCustomXenotype(CustomXenotype customXenotype)
		{
			if (customXenotype.genes.NullOrEmpty())
			{
				this.shouldSkip = true;
			}
			this.name = customXenotype.fileName;
			this.iconDef = customXenotype.iconDef;
			this.genes = customXenotype.genes;
			this.xenotypeDef = XenotypeDefOf.Baseliner;
			this.inheritable = customXenotype.inheritable;
		}

		//public bool PawnIsSameXenotype(Pawn pawn)
		//{
		//	if (pawn.genes.Xenotype == xenotypeDef && (pawn.genes.CustomXenotype == null || pawn.genes.CustomXenotype.name == name))
		//          {
		//		return true;
		//          }
		//	return false;
		//}

		//public bool XenotypeIsSameXenotype(XenotypeDef newXenotype)
		//{
		//	return newXenotype == xenotypeDef;
		//}

		//public bool XenotypeIsSameXenotype(CustomXenotype newXenotype)
		//{
		//	return newXenotype.name == name;
		//}

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
						cachedLabel = XenotypeDef_Safe.label;
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
						stringBuilder.AppendLine(("WVC_XaG_XenoHolder_GenesMatch".Translate() + ":").Colorize(ColoredText.TipSectionTitleColor) + " " + matchPercent.Value.ToStringPercent());
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
