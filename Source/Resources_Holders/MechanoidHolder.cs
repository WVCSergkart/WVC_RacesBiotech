using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace WVC_XenotypesAndGenes
{
    public class MechanoidHolder
    {

        public PawnKindDef pawnKindDef = null;

        public List<StatDef> displayedStats = new() { StatDefOf.WorkSpeedGlobal, StatDefOf.BandwidthCost, StatDefOf.ArmorRating_Blunt, StatDefOf.ArmorRating_Sharp };


		private float? voidEnergyCost;

		public float VoidEnergyCost
		{
			get
			{
				if (!voidEnergyCost.HasValue)
                {
                    voidEnergyCost = GetVoidMechCost(pawnKindDef, 99f);
                }
                return voidEnergyCost.Value;
			}
		}

        public static float GetVoidMechCost(PawnKindDef pawnKindDef, float limit = 99f)
		{
			float voidCost = (float)Math.Round((pawnKindDef.race.race.baseBodySize + pawnKindDef.race.race.baseHealthScale + (!pawnKindDef.race.race.mechEnabledWorkTypes.NullOrEmpty() ? pawnKindDef.race.race.mechEnabledWorkTypes.Count : 3)) * WVC_Biotech.settings.voidlink_mechCostFactor * pawnKindDef.race.race.mechWeightClass.ToFloatFactor(), 0, MidpointRounding.AwayFromZero);
			if (voidCost > limit)
			{
				return limit;
			}
			return voidCost;
		}

		private bool? isWorkGolemnoid;

        public bool Worker
        {
            get
            {
                if (isWorkGolemnoid == null)
                {
                    isWorkGolemnoid = pawnKindDef?.race?.race?.mechEnabledWorkTypes?.NullOrEmpty() == false;
                }
                return isWorkGolemnoid.Value;
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
					string phase = "start";
                    try
					{
						StringBuilder stringBuilder = new();
						stringBuilder.AppendLine(pawnKindDef.race.description);
						stringBuilder.AppendLine();
						stringBuilder.AppendLine("WVC_XaG_DialogVoidlink_DescEnergyCost".Translate(GetVoidMechCost(pawnKindDef, WVC_Biotech.settings.voidlink_mechCostLimit)).ToString());
						stringBuilder.AppendLine();
						phase = "get stats";
						stringBuilder.AppendLine("WVC_XaG_Dialog_Golemlink_Stats".Translate().Colorize(ColoredText.TipSectionTitleColor) + ":");
						if (!displayedStats.NullOrEmpty())
						{
							for (int j = 0; j < displayedStats.Count; j++)
							{
								StatDef statDef = displayedStats[j];
								stringBuilder.AppendLine(" - " + statDef.LabelCap + ": " + statDef.ValueToString(pawnKindDef.race.GetStatValueAbstract(statDef), statDef.toStringNumberSense));
							}
						}
						phase = "get mechEnabledWorkTypes";
						if (Worker)
						{
							stringBuilder.AppendLine();
							stringBuilder.AppendLine("MechWorkActivities".Translate().Colorize(ColoredText.TipSectionTitleColor) + ":");
							stringBuilder.AppendLine(" - " + pawnKindDef.race.race.mechEnabledWorkTypes.Select((WorkTypeDef w) => w.gerundLabel).ToCommaList(useAnd: true).CapitalizeFirst());
						}
						phase = "get weaponTags";
						if (!pawnKindDef.weaponTags.NullOrEmpty())
						{
							stringBuilder.AppendLine();
							stringBuilder.AppendLine("WVC_XaG_Dialog_Golemlink_Weapon".Translate().Colorize(ColoredText.TipSectionTitleColor) + ":");
							stringBuilder.AppendLine(" - " + DefDatabase<ThingDef>.AllDefsListForReading.Where((ThingDef thing) => !thing.weaponTags.NullOrEmpty() && thing.weaponTags.Contains(pawnKindDef.weaponTags.FirstOrDefault()))?.FirstOrDefault()?.label.CapitalizeFirst());
						}
						stringBuilder.AppendLine();
						phase = "get MechWeightClass";
						stringBuilder.Append("WVC_MechWeightClass".Translate().Colorize(ColoredText.TipSectionTitleColor) + ": " + pawnKindDef.race.race.mechWeightClass.LabelCap);
						cachedDescription = stringBuilder.ToString();
					}
                    catch (Exception arg)
                    {
						Log.Error("Failed get description for mechanoid. On phase: " + phase + " Reason: " + arg);
						cachedDescription = "WVC_XaG_NoDescError".Translate() + " Cause failed get description for mechanoid. On phase: " + phase;
					}
                }
                return cachedDescription;
            }
        }

	}

}
