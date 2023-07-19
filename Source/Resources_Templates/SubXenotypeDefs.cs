using System;
using System.Xml;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Verse;
using UnityEngine;
using RimWorld;
using WVC;
using WVC_XenotypesAndGenes;


namespace WVC_XenotypesAndGenes
{

	// public class EndotypeDef : XenotypeDef
	// {
	// }

	public class RandomGenes
	{
		public bool inheritable = false;
		public List<GeneDef> genes = new();
	}

	public class SubXenotypeDef : Def
	{
		public List<GeneDef> genes = new();

		public List<GeneDef> removeGenes = null;

		public List<GeneDef> mainGenes = null;

		public bool inheritable = false;

		public bool overrideExistingGenes = false;

		public bool ignoreExistingGenes = false;

		// public bool useMainAdditionalGenes = false;

		public List<GeneDef> AllGenes => genes;

		public XenotypeIconDef xenotypeIconDef = null;

		public List<RandomGenes> randomGenes = new();

		// [Unsaved(false)]
		// private Texture2D cachedIcon;

		// public static readonly Color IconColor = new(0.75f, 0.75f, 0.75f);

		// public Texture2D Icon
		// {
			// get
			// {
				// if (cachedIcon == null)
				// {
					// if (xenotypeIconDef.texPath.NullOrEmpty())
					// {
						// cachedIcon = BaseContent.BadTex;
					// }
					// else
					// {
						// cachedIcon = ContentFinder<Texture2D>.Get(xenotypeIconDef.texPath);
					// }
				// }
				// return cachedIcon;
			// }
		// }

		public override void ResolveReferences()
		{
			if (genes.NullOrEmpty())
			{
				return;
			}
			if (descriptionHyperlinks == null)
			{
				descriptionHyperlinks = new List<DefHyperlink>();
			}
			if (!mainGenes.NullOrEmpty())
			{
				foreach (GeneDef gene in mainGenes)
				{
					descriptionHyperlinks.Add(new DefHyperlink(gene));
				}
			}
			foreach (GeneDef gene in genes)
			{
				descriptionHyperlinks.Add(new DefHyperlink(gene));
			}
		}

		public override IEnumerable<StatDrawEntry> SpecialDisplayStats(StatRequest req)
		{
			foreach (StatDrawEntry item in base.SpecialDisplayStats(req))
			{
				yield return item;
			}
			if (!mainGenes.NullOrEmpty())
			{
				yield return new StatDrawEntry(StatCategoryDefOf.Basics, "WVC_XaG_SubXeno_MainGenes".Translate().CapitalizeFirst(), mainGenes.Select((GeneDef x) => x.label).ToCommaList().CapitalizeFirst(), "GenesDesc".Translate() + "\n\n" + "WVC_XaG_SubXeno_MainGenes_Desc".Translate() + "\n\n" + mainGenes.Select((GeneDef x) => x.label).ToLineList("  - ", capitalizeItems: true), 1010);
			}
			yield return new StatDrawEntry(StatCategoryDefOf.Basics, "Genes".Translate().CapitalizeFirst(), genes.Select((GeneDef x) => x.label).ToCommaList().CapitalizeFirst(), "GenesDesc".Translate() + "\n\n" + genes.Select((GeneDef x) => x.label).ToLineList("  - ", capitalizeItems: true), 1000);
			if (!removeGenes.NullOrEmpty())
			{
				yield return new StatDrawEntry(StatCategoryDefOf.Basics, "WVC_XaG_SubXeno_RemovedGenes".Translate().CapitalizeFirst(), removeGenes.Select((GeneDef x) => x.label).ToCommaList().CapitalizeFirst(), "WVC_XaG_SubXeno_RemovedGenes_Desc".Translate() + "\n\n" + removeGenes.Select((GeneDef x) => x.label).ToLineList("  - ", capitalizeItems: true), 990);
			}
			if (!randomGenes.NullOrEmpty())
			{
				foreach (RandomGenes item in randomGenes)
				{
					yield return new StatDrawEntry(StatCategoryDefOf.Basics, "WVC_XaG_SubXeno_RandomGenes".Translate().CapitalizeFirst(), item.genes.Select((GeneDef x) => x.label).ToCommaList().CapitalizeFirst(), "WVC_XaG_SubXeno_RandomGenes_Desc".Translate() + "\n\n" + item.genes.Select((GeneDef x) => x.label).ToLineList("  - ", capitalizeItems: true), 980);
				}
			}
			if (!genes.NullOrEmpty())
			{
				yield return new StatDrawEntry(StatCategoryDefOf.Basics, "GenesAreInheritable".Translate(), inheritable.ToStringYesNo(), "GenesAreInheritableXenotypeDef".Translate(), 280);
				yield return new StatDrawEntry(StatCategoryDefOf.Basics, "WVC_XaG_SubXeno_OverrideExistingGenes".Translate(), overrideExistingGenes.ToStringYesNo(), "WVC_XaG_SubXeno_OverrideExistingGenes_Desc".Translate(), 270);
				yield return new StatDrawEntry(StatCategoryDefOf.Basics, "WVC_XaG_SubXeno_IgnoreExistingGenes".Translate(), ignoreExistingGenes.ToStringYesNo(), "WVC_XaG_SubXeno_IgnoreExistingGenes_Desc".Translate(), 260);
			}
		}
	}
}
