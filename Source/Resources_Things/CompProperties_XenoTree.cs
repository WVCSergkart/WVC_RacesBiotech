using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_XenoTree : CompProperties
	{

		public IntRange ticksBetweenSpawn = new(780000, 900000);

		public float minMatchingGenes = 0.8f;

		[Obsolete]
		public bool xenogerminationComa = true;

		public int xenotypeChangeCooldown = 900000;

		public string uniqueTag = "XenoTree";

		public string completeLetterLabel = "WVC_XaG_XenoTreeBirthLabel";
		public string completeLetterDesc = "WVC_XaG_XenoTreeBirthDesc";

		public CompProperties_XenoTree()
		{
			compClass = typeof(CompXenoTree);
		}

	}

	public class CompXenoTree : ThingComp
	{

		public int tickCounter = 0;

		public int changeCooldown = 0;

		private bool spawnerIsActive = false;

		public XenotypeDef chosenXenotype = null;

		public SaveableXenotypeHolder xenotypeHolder = null;

		public CompProperties_XenoTree Props => (CompProperties_XenoTree)props;

		public CompSpawnSubplantDuration Subplant => parent.TryGetComp<CompSpawnSubplantDuration>();

		public override void Initialize(CompProperties props)
		{
			base.Initialize(props);
			ResetCounter();
		}

		public void SetupHolder(XenotypeHolder holder)
		{
			SaveableXenotypeHolder newHolder = new();
			newHolder.xenotypeDef = holder.xenotypeDef;
			newHolder.name = holder.name;
			newHolder.iconDef = holder.iconDef;
			newHolder.genes = holder.genes;
			newHolder.inheritable = holder.inheritable;
			xenotypeHolder = newHolder;
			changeCooldown = Find.TickManager.TicksGame + Props.xenotypeChangeCooldown;
		}

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			base.PostSpawnSetup(respawningAfterLoad);
			if (!respawningAfterLoad)
			{
				ResetCounter();
			}
		}

		public override void CompTick()
		{
			//base.CompTick();
			Tick(1);
		}

		public override void CompTickRare()
		{
			//base.CompTickRare();
			Tick(250);
		}

		public override void CompTickLong()
		{
			//base.CompTickRare();
			Tick(2000);
		}

		public void Tick(int tick)
		{
			if (!spawnerIsActive)
			{
				return;
			}
			tickCounter -= tick;
			if (tickCounter > 0)
			{
				return;
			}
			TryDoSpawn();
			ResetCounter();
		}

		public void TryDoSpawn()
        {
            if (IsActive())
            {
				changeCooldown = 0;
				GestationUtility.GestateChild_WithXenotype(parent, chosenXenotype, xenotypeHolder, Props.completeLetterLabel, Props.completeLetterDesc);
                if (Subplant != null)
                {
                    Subplant.DoGrowSubplant();
                }
            }
        }

        private bool IsActive()
        {
            return spawnerIsActive && (chosenXenotype != null || xenotypeHolder != null);
        }

        public override string CompInspectStringExtra()
		{
			StringBuilder stringBuilder = new(base.CompInspectStringExtra());
			if (IsActive())
			{
				if (Find.TickManager.TicksGame < changeCooldown)
				{
					stringBuilder.AppendLine(string.Format("{0}", "WVC_XaG_XenoTreeXenotypeChangeCooldown".Translate(changeCooldown.ToStringTicksToPeriod())));
				}
				if (xenotypeHolder != null)
				{
					stringBuilder.AppendLine(string.Format("{0}: {1}", "WVC_XaG_CurrentXenotype".Translate(), xenotypeHolder.LabelCap.Colorize(ColoredText.GeneColor)));
				}
				else if (chosenXenotype != null)
				{
					stringBuilder.AppendLine(string.Format("{0}: {1}", "WVC_XaG_CurrentXenotype".Translate(), chosenXenotype.label.CapitalizeFirst().Colorize(ColoredText.GeneColor)));
				}
				stringBuilder.Append(string.Format("{0}", "WVC_XaG_Label_CompSpawnBabyPawnAndInheritGenes".Translate(tickCounter.ToStringTicksToPeriod())));
			}
			else if (Subplant != null && parent is Plant plant)
			{
				stringBuilder.Append(string.Format("{0}", "WVC_XaG_TreeGrowthInPercents".Translate(parent.LabelCap, ((int)(plant.Growth * 100)).ToString())));
			}
			return stringBuilder.ToString();
		}

		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: Spawn baby",
					action = delegate
					{
						ResetCounter();
						TryDoSpawn();
					}
				};
				yield return new Command_Action
				{
					defaultLabel = "DEV: Reset cooldown",
					action = delegate
					{
						changeCooldown = 0;
					}
				};
			}
			yield return new Command_Action
			{
				defaultLabel = "WVC_XaG_GeneBabyTree_label".Translate() + ": " + XaG_UiUtility.OnOrOff(spawnerIsActive),
				defaultDesc = "WVC_XaG_GeneBabyTree_desc".Translate(),
				Disabled = chosenXenotype == null && xenotypeHolder == null,
				disabledReason = "WVC_XaG_XenoTreeXenotypeChooseDisabled_OnOrOff".Translate(),
				icon = parent.def.uiIcon,
				action = delegate
				{
					spawnerIsActive = !spawnerIsActive;
					if (spawnerIsActive)
					{
						SoundDefOf.Tick_High.PlayOneShotOnCamera();
					}
					else
					{
						SoundDefOf.Tick_Low.PlayOneShotOnCamera();
					}
				}
			};
			yield return new Command_Action
			{
				defaultLabel = "WVC_XaG_XenoTreeXenotypeChooseLabel".Translate(),
				defaultDesc = "WVC_XaG_XenoTreeXenotypeChooseDesc".Translate(),
				Disabled = Subplant != null && parent is Plant plant && plant.Growth < Subplant.Props.minGrowthForSpawn,
				disabledReason = "WVC_XaG_XenoTreeXenotypeChooseDisabled_XenotypeMenu".Translate(parent.LabelCap, Subplant != null ? (Subplant.Props.minGrowthForSpawn * 100f).ToString() : (100).ToString()),
				icon = GetXenotypeIcon(),
				action = delegate
				{
					Find.WindowStack.Add(new Dialog_XenotypeTree(parent));
				}
			};
		}

		private Texture2D GetXenotypeIcon()
		{
			if (xenotypeHolder != null)
			{
				if (xenotypeHolder.iconDef != null)
				{
					return xenotypeHolder.iconDef.Icon;
				}
				else
				{
					return ContentFinder<Texture2D>.Get(xenotypeHolder.xenotypeDef.iconPath);
				}
			}
			else if (chosenXenotype != null)
			{
				return ContentFinder<Texture2D>.Get(chosenXenotype.iconPath);
			}
			return parent.def.uiIcon;
		}

		public void ResetCounter()
		{
			tickCounter = Props.ticksBetweenSpawn.RandomInRange;
		}

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look(ref tickCounter, "tickCounterNextBabySpawn_" + Props.uniqueTag, 0);
			Scribe_Values.Look(ref changeCooldown, "changeCooldown_" + Props.uniqueTag, 0);
			Scribe_Defs.Look(ref chosenXenotype, "chosenXenotype_" + Props.uniqueTag);
			Scribe_Deep.Look(ref xenotypeHolder, "xenotypeHolder_" + Props.uniqueTag);
			Scribe_Values.Look(ref spawnerIsActive, "spawnerIsActive", true);
		}

	}

}
