using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class TraitDefHolder
	{

		public TraitDef traitDef;

		public TraitDegreeData traitDegreeData;

		public int? traitDegree;

		public Pawn targetPawn;

		public bool locked;

		private List<ThoughtDef> GetPermaThoughts()
		{
			TraitDegreeData degree = traitDegreeData;
			List<ThoughtDef> list = new();
			List<ThoughtDef> allThoughts = DefDatabase<ThoughtDef>.AllDefsListForReading;
			for (int i = 0; i < allThoughts.Count; i++)
			{
				if (allThoughts[i].IsSituational && allThoughts[i].Worker is ThoughtWorker_AlwaysActive && allThoughts[i].requiredTraits != null && allThoughts[i].requiredTraits.Contains(traitDef) && (!allThoughts[i].RequiresSpecificTraitsDegree || allThoughts[i].requiredTraitsDegree == degree.degree))
				{
					list.Add(allThoughts[i]);
				}
			}
			return list;
		}

		[Unsaved(false)]
		private string cachedDescription;
		public string Description
		{
			get
			{
				if (cachedDescription == null)
				{
					StringBuilder stringBuilder = new();
					stringBuilder.AppendLine(LabelCap.Colorize(ColoredText.TipSectionTitleColor));
					stringBuilder.AppendLine();
					stringBuilder.AppendLine(traitDegreeData.description.Formatted(targetPawn.Named("PAWN")).AdjustedFor(targetPawn).Resolve());
					bool num = traitDegreeData.skillGains.Count > 0;
					bool flag = GetPermaThoughts().Any();
					bool flag2 = traitDegreeData.statOffsets != null;
					bool flag3 = traitDegreeData.statFactors != null;
					if (num || flag || flag2 || flag3 || traitDegreeData.painFactor != 1f || traitDegreeData.painOffset != 0f || !traitDegreeData.aptitudes.NullOrEmpty())
					{
						stringBuilder.AppendLine();
					}
					if (num)
					{
						foreach (SkillGain skillGain in traitDegreeData.skillGains)
						{
							if (skillGain.amount != 0)
							{
								stringBuilder.AppendLine("    " + skillGain.skill.skillLabel.CapitalizeFirst() + ":   " + skillGain.amount.ToString("+##;-##"));
							}
						}
					}
					if (flag)
					{
						foreach (ThoughtDef permaThought in GetPermaThoughts())
						{
							stringBuilder.AppendLine("    " + "PermanentMoodEffect".Translate() + " " + permaThought.stages[0].baseMoodEffect.ToStringByStyle(ToStringStyle.Integer, ToStringNumberSense.Offset));
						}
					}
					if (flag2)
					{
						for (int i = 0; i < traitDegreeData.statOffsets.Count; i++)
						{
							StatModifier statModifier = traitDegreeData.statOffsets[i];
							string valueToStringAsOffset = statModifier.ValueToStringAsOffset;
							string value = "    " + statModifier.stat.LabelCap + " " + valueToStringAsOffset;
							stringBuilder.AppendLine(value);
						}
					}
					if (traitDegreeData.painOffset != 0f)
					{
						stringBuilder.AppendLine("    " + "Pain".Translate() + " " + traitDegreeData.painOffset.ToStringByStyle(ToStringStyle.PercentOne, ToStringNumberSense.Offset));
					}
					if (flag3)
					{
						for (int j = 0; j < traitDegreeData.statFactors.Count; j++)
						{
							StatModifier statModifier2 = traitDegreeData.statFactors[j];
							string toStringAsFactor = statModifier2.ToStringAsFactor;
							string value2 = "    " + statModifier2.stat.LabelCap + " " + toStringAsFactor;
							stringBuilder.AppendLine(value2);
						}
					}
					if (traitDegreeData.hungerRateFactor != 1f)
					{
						string text = traitDegreeData.hungerRateFactor.ToStringByStyle(ToStringStyle.PercentOne, ToStringNumberSense.Factor);
						string value3 = "    " + "HungerRate".Translate() + " " + text;
						stringBuilder.AppendLine(value3);
					}
					if (traitDegreeData.painFactor != 1f)
					{
						stringBuilder.AppendLine("    " + "Pain".Translate() + " " + traitDegreeData.painFactor.ToStringByStyle(ToStringStyle.PercentOne, ToStringNumberSense.Factor));
					}
					if (!traitDegreeData.aptitudes.NullOrEmpty())
					{
						stringBuilder.AppendLine().AppendLineTagged(("Aptitudes".Translate().CapitalizeFirst() + ":").AsTipTitle());
						stringBuilder.AppendLine(traitDegreeData.aptitudes.Select((Aptitude x) => x.skill.LabelCap.ToString() + " " + x.level.ToStringWithSign()).ToLineList("  - ", capitalizeItems: true));
					}
					if (ModsConfig.RoyaltyActive)
					{
						List<MeditationFocusDef> allowedMeditationFocusTypes = traitDegreeData.allowedMeditationFocusTypes;
						if (!allowedMeditationFocusTypes.NullOrEmpty())
						{
							stringBuilder.AppendLine();
							stringBuilder.AppendLine("EnablesMeditationFocusType".Translate().Colorize(ColoredText.TipSectionTitleColor) + ":\n" + allowedMeditationFocusTypes.Select((MeditationFocusDef f) => f.LabelCap.Resolve()).ToLineList("  - "));
						}
					}
					if (ModsConfig.IdeologyActive)
					{
						List<IssueDef> affectedIssues = traitDegreeData.GetAffectedIssues(traitDef);
						if (affectedIssues.Count != 0)
						{
							stringBuilder.AppendLine();
							stringBuilder.AppendLine("OverridesSomePrecepts".Translate().Colorize(ColoredText.TipSectionTitleColor) + ":\n" + affectedIssues.Select((IssueDef x) => x.LabelCap.Resolve()).ToLineList("  - "));
						}
						List<MemeDef> affectedMemes = traitDegreeData.GetAffectedMemes(traitDef, agreeable: true);
						if (affectedMemes.Count > 0)
						{
							stringBuilder.AppendLine();
							stringBuilder.AppendLine("AgreeableMemes".Translate().Colorize(ColoredText.TipSectionTitleColor) + ":\n" + affectedMemes.Select((MemeDef x) => x.LabelCap.Resolve()).ToLineList("  - "));
						}
						List<MemeDef> affectedMemes2 = traitDegreeData.GetAffectedMemes(traitDef, agreeable: false);
						if (affectedMemes2.Count > 0)
						{
							stringBuilder.AppendLine();
							stringBuilder.AppendLine("DisagreeableMemes".Translate().Colorize(ColoredText.TipSectionTitleColor) + ":\n" + affectedMemes2.Select((MemeDef x) => x.LabelCap.Resolve()).ToLineList("  - "));
						}
					}
					if (stringBuilder.Length > 0 && stringBuilder[stringBuilder.Length - 1] == '\n')
					{
						if (stringBuilder.Length > 1 && stringBuilder[stringBuilder.Length - 2] == '\r')
						{
							stringBuilder.Remove(stringBuilder.Length - 2, 2);
						}
						else
						{
							stringBuilder.Remove(stringBuilder.Length - 1, 1);
						}
					}
					cachedDescription = stringBuilder.ToString();
				}
				return cachedDescription;
			}
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
					cachedLabel = traitDegreeData.GetLabelFor(targetPawn);
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

		public bool IsSame(TraitDef otherTrait)
		{
			if (otherTrait == traitDef)
			{
				return true;
			}
			return false;
		}

		public bool IsSame(Trait otherTrait, bool withDegree = true)
		{
			if (IsSame(otherTrait.def) && (!withDegree || otherTrait.CurrentData == traitDegreeData))
			{
				return true;
			}
			return false;
		}

		public bool IsSame(TraitDefHolder otherHolder, bool withDegree = true)
		{
			if (IsSame(otherHolder.traitDef) && (!withDegree || otherHolder.traitDegreeData == traitDegreeData))
			{
				return true;
			}
			return false;
		}

		public bool ConflictsWith(TraitDef otherTrait)
		{
			if (traitDef.ConflictsWith(otherTrait))
			{
				return true;
			}
			return false;
		}

		public bool ConflictsWith(Trait otherTrait)
		{
			if (ConflictsWith(otherTrait.def))
			{
				return true;
			}
			return false;
		}

		public bool ConflictsWith(TraitDefHolder otherHolder)
		{
			if (traitDef.ConflictsWith(otherHolder.traitDef))
			{
				return true;
			}
			return false;
		}

		public bool ConflictsWithAny(List<TraitDefHolder> holders)
		{
			foreach (TraitDefHolder holder in holders)
			{
				if (ConflictsWith(holder))
				{
					return true;
				}
			}
			return false;
		}

		public bool CanAdd()
		{
			if (locked)
			{
				return false;
			}
			foreach (Trait trait in targetPawn.story.traits.allTraits)
			{
				if (IsSame(trait))
				{
					return false;
				}
				if (ConflictsWith(trait))
				{
					return false;
				}
			}
			return true;
		}

		public bool IsSame(List<TraitDefHolder> list)
		{
			foreach (TraitDefHolder trait in list)
			{
				if (IsSame(trait))
				{
					return true;
				}
			}
			return false;
		}

	}

}
