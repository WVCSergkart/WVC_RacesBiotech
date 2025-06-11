using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace WVC_XenotypesAndGenes
{
    //public class ChimeraDef : Def
    //{

    //	public List<GeneDef> addedGenes = new();

    //	public List<GeneDef> removedGenes = new();

    //}

    public class GolemModeDef : Def
	{

		public List<StatDef> displayedStats;

		public List<DefHyperlink> hyperlinks = new();

		public PawnKindDef pawnKindDef;

		public bool canBeSummoned = false;
		public bool CanBeSummoned
		{
			get
			{
				if (GolembondCost > 1)
				{
					return false;
				}
				return canBeSummoned;
			}
		}

		public bool canBeAnimated = true;

		public bool changeable = true;

		public float iconSize = 1f;

		public float order = 0f;

		private bool? isWorkGolemnoid;
		public bool Worker
		{
			get
			{
				if (isWorkGolemnoid == null)
				{
					isWorkGolemnoid = !pawnKindDef.race.race.mechEnabledWorkTypes.NullOrEmpty();
				}
				return isWorkGolemnoid.Value;
			}
		}

		private float? cachedGolembondCost;
		public float GolembondCost
		{
			get
			{
				if (cachedGolembondCost == null)
				{
					cachedGolembondCost = pawnKindDef.race.GetStatValueAbstract(WVC_GenesDefOf.WVC_GolemBondCost);
				}
				return cachedGolembondCost.Value;
			}
		}

		private string cachedDescription;
		public string Description
		{
			get
			{
				if (cachedDescription == null)
				{
					StringBuilder stringBuilder = new();
					stringBuilder.AppendLine(pawnKindDef.race.description);
					stringBuilder.AppendLine();
					stringBuilder.AppendLine("WVC_XaG_Dialog_Golemlink_Stats".Translate().Colorize(ColoredText.TipSectionTitleColor) + ":");
					if (!displayedStats.NullOrEmpty())
					{
						for (int j = 0; j < displayedStats.Count; j++)
						{
							StatDef statDef = displayedStats[j];
							stringBuilder.AppendLine(" - " + statDef.LabelCap + ": " + statDef.ValueToString(pawnKindDef.race.GetStatValueAbstract(statDef), statDef.toStringNumberSense));
						}
					}
					//stringBuilder.AppendLine(" - " + StatDefOf.ArmorRating_Blunt.LabelCap + ": " + pawnKindDef.race.GetStatValueAbstract(StatDefOf.ArmorRating_Blunt).ToStringPercent());
					//stringBuilder.AppendLine(" - " + StatDefOf.ArmorRating_Sharp.LabelCap + ": " + pawnKindDef.race.GetStatValueAbstract(StatDefOf.ArmorRating_Sharp).ToStringPercent());
					//stringBuilder.AppendLine(" - " + StatDefOf.MoveSpeed.LabelCap + ": " + StatDefOf.MoveSpeed.ValueToString(pawnKindDef.race.GetStatValueAbstract(StatDefOf.MoveSpeed), ToStringNumberSense.Absolute, !StatDefOf.MoveSpeed.formatString.NullOrEmpty()));
					//stringBuilder.AppendLine(" - " + WVC_GenesDefOf.WVC_GolemBondCost.LabelCap + ": " + pawnKindDef.race.GetStatValueAbstract(WVC_GenesDefOf.WVC_GolemBondCost));
					//stringBuilder.AppendLine(" - " + StatDefOf.BandwidthCost.LabelCap + ": " + pawnKindDef.race.GetStatValueAbstract(StatDefOf.BandwidthCost));
					if (Worker)
					{
						stringBuilder.AppendLine();
						stringBuilder.AppendLine("MechWorkActivities".Translate().Colorize(ColoredText.TipSectionTitleColor) + ":");
						stringBuilder.AppendLine(" - " + pawnKindDef.race.race.mechEnabledWorkTypes.Select((WorkTypeDef w) => w.gerundLabel).ToCommaList(useAnd: true).CapitalizeFirst());
					}
					if (!pawnKindDef.weaponTags.NullOrEmpty())
					{
						stringBuilder.AppendLine();
						stringBuilder.AppendLine("WVC_XaG_Dialog_Golemlink_Weapon".Translate().Colorize(ColoredText.TipSectionTitleColor) + ":");
						stringBuilder.AppendLine(" - " + DefDatabase<ThingDef>.AllDefsListForReading.Where((ThingDef thing) => !thing.weaponTags.NullOrEmpty() && thing.weaponTags.Contains(pawnKindDef.weaponTags.FirstOrDefault())).FirstOrDefault().label.CapitalizeFirst());
					}
					stringBuilder.AppendLine();
					stringBuilder.AppendLine("WVC_XaG_OrbitalSummon".Translate().Colorize(ColoredText.TipSectionTitleColor) + ": " + CanBeSummoned.ToStringYesNo());
					stringBuilder.AppendLine();
					stringBuilder.Append("WVC_MechWeightClass".Translate().Colorize(ColoredText.TipSectionTitleColor) + ": " + pawnKindDef.race.race.mechWeightClass.LabelCap);
					cachedDescription = stringBuilder.ToString();
				}
				return cachedDescription;
			}
		}

		public override void ResolveReferences()
		{
			base.ResolveReferences();
			label = pawnKindDef.label;
			description = pawnKindDef.description;
			hyperlinks.Add(new DefHyperlink(pawnKindDef.race));
			CompProperties_Spawner compProperties_Spawner = pawnKindDef?.race?.GetCompProperties<CompProperties_Spawner>();
			if (compProperties_Spawner != null)
			{
				hyperlinks.Add(new DefHyperlink(compProperties_Spawner.thingToSpawn));
			}
		}


	}

}
