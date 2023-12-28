using RimWorld;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

    public class CompProperties_XenoTree : CompProperties
	{
		public IntRange ticksBetweenSpawn = new(480000, 720000);

		public float minMatchingGenes = 0.8f;

		// public GeneDef geneDef = null;

		// public HediffDef infectionHediffDef;

		public bool xenogerminationComa = true;

		public int xenotypeChangeCooldown = 420000;
		// public int rootsConnectionCooldown = 220000;

		// public int skillShareRefresh = 60000;

		// public ThingDef xenoBulbDef;

		// public GeneDef connectionGeneDef;

		// public IntRange geneticMaterial_Cpx = new(4, 12);
		// public IntRange geneticMaterial_Met = new(3, 7);
		// public IntRange geneticMaterial_Arc = new(0, 2);

		// public int limitPerBulb_Cpx = 32;
		// public int limitPerBulb_Met = 25;
		// public int limitPerBulb_Tox = 18;
		// public int limitPerBulb_Arc = 2;

		// public IntRange geneticMaterialProduction = new(1, 3);

		// public float minFertilityForMetabolism = 0.8f;

		// Bulb Only
		// public bool unlockXenos_Bloodfeeder = false;
		// public bool unlockXenos_Undead = false;
		// public bool unlockXenos_Toxic = false;
		// public bool unlockXenos_Archite = false;

		public string uniqueTag = "XenoTree";

		public string completeLetterLabel = "WVC_XaG_XenoTreeBirthLabel";
		public string completeLetterDesc = "WVC_XaG_XenoTreeBirthDesc";

		// public CompProperties_XenoTree()
		// {
			// compClass = typeof(CompXenoTree);
		// }
	}

	public class CompXenoTree : ThingComp
	{
		public int tickCounter = 0;
		// public int skillsRefreshTicks = 0;

		public int changeCooldown = 0;

		// private List<Pawn> connectedPawns = new();

		// public List<Pawn> AllConnectedPawns => connectedPawns.ToList();

		private bool spawnerIsActive = false;

		public XenotypeDef chosenXenotype = null;

		public CompProperties_XenoTree Props => (CompProperties_XenoTree)props;

		public CompSpawnSubplantDuration Subplant => parent.TryGetComp<CompSpawnSubplantDuration>();

		// public int geneticMaterial_Cpx = 0;
		// public int geneticMaterial_Met = 0;
		// public int geneticMaterial_Arc = 0;
		// public int geneticMaterial_Tox = 0;

		public override void Initialize(CompProperties props)
		{
			base.Initialize(props);
			ResetCounter();
		}

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			base.PostSpawnSetup(respawningAfterLoad);
			if (!respawningAfterLoad)
			{
				ResetCounter();
				// geneticMaterial_Arc += 1;
			}
		}

		public override void CompTick()
		{
			base.CompTick();
			Tick(1);
		}

		public override void CompTickRare()
		{
			base.CompTickRare();
			Tick(250);
		}

		public override void CompTickLong()
		{
			base.CompTickRare();
			Tick(2000);
		}

		public void Tick(int tick)
		{
			tickCounter -= tick;
			// if (Find.TickManager.TicksGame >= skillsRefreshTicks)
			// {
				// RefreshConnectedPawns();
			// }
			if (tickCounter > 0 && spawnerIsActive)
			{
				return;
			}
			TryDoSpawn();
			ResetCounter();
		}

		// private void RefreshConnectedPawns()
		// {
			// if (!AllConnectedPawns.NullOrEmpty())
			// {
				// foreach (Pawn pawn in AllConnectedPawns)
				// {
					// if (ConnectionCheck(pawn))
					// {
						// foreach (SkillRecord skill in pawn.skills.skills)
						// {
							// if (!skill.TotallyDisabled && skill.XpTotalEarned > 0f)
							// {
								// float num = skill.XpTotalEarned * 0.2f;
								// skill.Learn(0f - num, direct: true);
							// }
						// }
					// }
				// }
			// }
			// skillsRefreshTicks = Find.TickManager.TicksGame + Props.skillShareRefresh;
		// }

		public void TryDoSpawn()
		{
			if (spawnerIsActive && chosenXenotype != null)
			{
				GestationUtility.GenerateNewBornPawn_WithChosenXenotype(parent, chosenXenotype, Props.completeLetterLabel, Props.completeLetterDesc, Props.xenogerminationComa);
				if (Subplant != null)
				{
					Subplant.DoGrowSubplant();
				}
			}
		}

		// private int GetGenMatLimit(int baselimit)
		// {
			// return connectedBulbs.Count * baselimit;
		// }

		public override string CompInspectStringExtra()
		{
			StringBuilder stringBuilder = new(base.CompInspectStringExtra());
			// if (!connectedBulbs.NullOrEmpty())
			// {
				// stringBuilder.AppendLine(string.Format("{0}/n{1}/n{2}/n{3}", 
					// "WVC_XaG_XenoTreeXenoBulb_Cpx".Translate(geneticMaterial_Cpx.ToString(), GetGenMatLimit(Props.limitPerBulb_Cpx).ToString()), 
					// "WVC_XaG_XenoTreeXenoBulb_Tox".Translate(geneticMaterial_Tox.ToString(), GetGenMatLimit(Props.limitPerBulb_Tox).ToString()), 
					// "WVC_XaG_XenoTreeXenoBulb_Met".Translate(geneticMaterial_Met.ToString(), GetGenMatLimit(Props.limitPerBulb_Met).ToString()), 
					// "WVC_XaG_XenoTreeXenoBulb_Arc".Translate(geneticMaterial_Arc.ToString(), GetGenMatLimit(Props.limitPerBulb_Arc).ToString())
					// ));
			// }
			if (spawnerIsActive && chosenXenotype != null)
			{
				if (Find.TickManager.TicksGame < changeCooldown)
				{
					stringBuilder.AppendLine(string.Format("{0}", "WVC_XaG_XenoTreeXenotypeChangeCooldown".Translate(changeCooldown.ToStringTicksToPeriod())));
				}
				stringBuilder.AppendLine(string.Format("{0}: {1}", "WVC_XaG_CurrentXenotype".Translate(), chosenXenotype.label.CapitalizeFirst().Colorize(ColoredText.GeneColor)));
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
				// yield return new Command_Action
				// {
					// defaultLabel = "DEV: Create bulb",
					// action = delegate
					// {
						// CreateBulb();
					// }
				// };
				yield return new Command_Action
				{
					defaultLabel = "DEV: Reset cooldown",
					action = delegate
					{
						changeCooldown = 0;
					}
				};
			}
			// yield return new Command_Action
			// {
				// defaultLabel = "WVC_XaG_XenoTree_PawnConnectionLabel".Translate(),
				// defaultDesc = "WVC_XaG_XenoTree_PawnConnectionDesc".Translate(parent.LabelCap),
				// icon = parent.def.uiIcon,
				// action = delegate
				// {
					// List<Pawn> freeColonists = parent.Map.mapPawns.FreeColonists;
					// foreach (Pawn pawn in freeColonists)
					// {
						// Gene_InfectedMind infector = pawn?.genes?.GetFirstGeneOfType<Gene_InfectedMind>();
						// if (infector != null)
						// {
							// infector.xenoTree = parent;
						// }
						// else if (!pawn.health.hediffSet.HasHediff(Props.infectionHediffDef))
						// {
							// Hediff hediff = HediffMaker.MakeHediff(Props.infectionHediffDef, pawn);
							// HediffComp_XenoTreeInfection xenoTreeInfection = hediff.TryGetComp<HediffComp_XenoTreeInfection>();
							// if (xenoTreeInfection != null)
							// {
								// xenoTreeInfection.geneDef = Props.connectionGeneDef;
								// xenoTreeInfection.xenoTree = parent;
							// }
							// pawn.health.AddHediff(hediff);
							// if (PawnUtility.ShouldSendNotificationAbout(pawn))
							// {
								// Find.LetterStack.ReceiveLetter("WVC_XaG_XenoTree_PawnInfectedLetterLabel".Translate(pawn.LabelShort), "WVC_XaG_XenoTree_PawnInfectedLetterDesc".Translate(pawn.LabelShort, parent.LabelCap), LetterDefOf.NeutralEvent, new LookTargets(pawn));
							// }
						// }
					// }
				// }
			// };
			// if (Subplant != null && parent is Plant plant && plant.Growth < Subplant.Props.minGrowthForSpawn)
			// {
				// yield break;
			// }
			yield return new Command_Action
			{
				defaultLabel = "WVC_XaG_GeneBabyTree_label".Translate() + ": " + GeneUiUtility.OnOrOff(spawnerIsActive),
				defaultDesc = "WVC_XaG_GeneBabyTree_desc".Translate(),
				disabled = chosenXenotype == null,
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
				disabled = Subplant != null && parent is Plant plant && plant.Growth < Subplant.Props.minGrowthForSpawn,
				disabledReason = "WVC_XaG_XenoTreeXenotypeChooseDisabled_XenotypeMenu".Translate(parent.LabelCap, Subplant != null ? (Subplant.Props.minGrowthForSpawn * 100f).ToString() : (100).ToString()),
				icon = GetXenotypeIcon(),
				action = delegate
				{
					Find.WindowStack.Add(new Dialog_ChangeXenotype(parent));
				}
			};
			// if (Subplant != null && (geneticMaterial_Arc - 1) > 0)
			// {
				// yield return new Command_Action
				// {
					// defaultLabel = "WVC_XaG_XenoTreeCreateBulbLabel".Translate(),
					// defaultDesc = "WVC_XaG_XenoTreeCreateBulbDesc".Translate(),
					// icon = parent.def.uiIcon,
					// action = delegate
					// {
						// CreateBulb();
						// geneticMaterial_Arc -= 1;
					// }
				// };
			// }
		}

		// private void CreateBulb()
		// {
			// if (Subplant != null)
			// {
				// CompSpawnSubplantDuration.GrowSubplant(parent, Subplant.Props.maxRadius, Props.xenoBulbDef, Subplant.Props.plantsToNotOverwrite, Subplant.Props.initialGrowthRange, parent.Map, true);
			// }
		// }

		// private List<Thing> GetAllConnectedBulbs()
		// {
			// List<Thing> trees = new();
			// List<Thing> allThings = parent.Map.listerThings.AllThings;
			// foreach (Thing item in allThings)
			// {
				// if (item.def.plant != null && item.TryGetComp<CompXenoTree>() != null)
				// {
					// trees.Add(item);
				// }
			// }
			// return trees;
		// }

		private Texture2D GetXenotypeIcon()
		{
			if (chosenXenotype != null)
			{
				return ContentFinder<Texture2D>.Get(chosenXenotype.iconPath);
			}
			return parent.def.uiIcon;
		}

		// public string GetXenotypeLabel(XenotypeDef xenotype)
		// {
			// int allGenesCount = XenotypeUtility.GetAllGenesCount(xenotype);
			// string text = xenotype.label.CapitalizeFirst();
			// if (allGenesCount < 7)
			// {
				// text = xenotype.label.CapitalizeFirst().Colorize(ColoredText.SubtleGrayColor);
			// }
			// if (allGenesCount >= 7)
			// {
				// text = xenotype.label.CapitalizeFirst().Colorize(ColorLibrary.LightGreen);
			// }
			// if (allGenesCount >= 14)
			// {
				// text = xenotype.label.CapitalizeFirst().Colorize(ColorLibrary.LightBlue);
			// }
			// if (allGenesCount >= 21)
			// {
				// text = xenotype.label.CapitalizeFirst().Colorize(ColorLibrary.LightPurple);
			// }
			// if (allGenesCount >= 28)
			// {
				// text = xenotype.label.CapitalizeFirst().Colorize(ColorLibrary.LightOrange);
			// }
			// return text;
		// }

		// public bool ConnectionCheck(Pawn pawn)
		// {
			// if (pawn != null)
			// {
				// Gene_InfectedMind infector = pawn?.genes?.GetFirstGeneOfType<Gene_InfectedMind>();
				// if (infector != null)
				// {
					// return true;
				// }
			// }
			// RemoveFromConnectionList(pawn);
			// return false;
		// }

		// public void RemoveFromConnectionList(Pawn pawn)
		// {
			// if (pawn != null)
			// {
				// connectedPawns.Remove(pawn);
			// }
		// }

		// public void AddedToConnectionList(Pawn pawn)
		// {
			// if (pawn != null)
			// {
				// connectedPawns.Add(pawn);
			// }
		// }

		// public override void PostDrawExtraSelectionOverlays()
		// {
			// if (!AllConnectedPawns.NullOrEmpty())
			// {
				// foreach (Pawn item in AllConnectedPawns)
				// {
					// if (item != null && ConnectionCheck(item))
					// {
						// GenDraw.DrawLineBetween(parent.TrueCenter(), item.TrueCenter(), SimpleColor.Yellow);
					// }
				// }
			// }
		// }

		// public override void PostDestroy(DestroyMode mode, Map previousMap)
		// {=
			// if (!AllConnectedPawns.NullOrEmpty())
			// {
				// foreach (Pawn item in AllConnectedPawns)
				// {
					// Gene_InfectedMind infector = item?.genes?.GetFirstGeneOfType<Gene_InfectedMind>();
					// if (infector != null)
					// {
						// infector.xenoTree = null;
					// }
				// }
			// }
		// }

		public void ResetCounter()
		{
			tickCounter = Props.ticksBetweenSpawn.RandomInRange;
			// skillsRefreshTicks = Props.skillShareRefresh;
		}

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look(ref tickCounter, "tickCounterNextBabySpawn_" + Props.uniqueTag, 0);
			// Scribe_Values.Look(ref skillsRefreshTicks, "skillsRefreshTicks_" + Props.uniqueTag, 0);
			Scribe_Values.Look(ref changeCooldown, "changeCooldown_" + Props.uniqueTag, 0);
			Scribe_Defs.Look(ref chosenXenotype, "chosenXenotype_" + Props.uniqueTag);
			Scribe_Values.Look(ref spawnerIsActive, "spawnerIsActive", true);
			// Bulbs
			// Scribe_Collections.Look(ref connectedPawns, "connectedPawns_" + Props.uniqueTag, LookMode.Reference);
			// Bulb Materials
			// Scribe_Values.Look(ref geneticMaterial_Cpx, "geneticMaterial_Cpx_" + Props.uniqueTag, 0);
			// Scribe_Values.Look(ref geneticMaterial_Met, "geneticMaterial_Met_" + Props.uniqueTag, 0);
			// Scribe_Values.Look(ref geneticMaterial_Tox, "geneticMaterial_Tox_" + Props.uniqueTag, 0);
			// Scribe_Values.Look(ref geneticMaterial_Arc, "geneticMaterial_Arc_" + Props.uniqueTag, 0);
		}
	}

}
