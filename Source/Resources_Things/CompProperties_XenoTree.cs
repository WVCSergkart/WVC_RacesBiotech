using RimWorld;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_XenoTree : CompProperties
	{
		public IntRange ticksBetweenSpawn = new(480000, 720000);

		// public GeneDef geneDef = null;

		public float minGrowthForSpawn = 1.0f;

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

		private bool spawnerIsActive = false;

		public XenotypeDef chosenXenotype = null;

		private CompProperties_XenoTree Props => (CompProperties_XenoTree)props;

		public CompSpawnSubplantDuration Subplant => parent.TryGetComp<CompSpawnSubplantDuration>();

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
			if (tickCounter > 0 && spawnerIsActive)
			{
				return;
			}
			TryDoSpawn();
			ResetCounter();
		}

		public void TryDoSpawn()
		{
			if (spawnerIsActive && chosenXenotype != null)
			{
				GestationUtility.GenerateNewBornPawn_WithChosenXenotype(parent, chosenXenotype, Props.completeLetterLabel, Props.completeLetterDesc);
				if (Subplant != null)
				{
					Subplant.DoGrowSubplant();
				}
			}
		}

		public override string CompInspectStringExtra()
		{
			StringBuilder stringBuilder = new(base.CompInspectStringExtra());
			if (spawnerIsActive && chosenXenotype != null)
			{
				stringBuilder.AppendLine(string.Format("{0}", "WVC_XaG_Label_CompSpawnBabyPawnAndInheritGenes".Translate(tickCounter.ToStringTicksToPeriod())));
				stringBuilder.Append(string.Format("{0}: {1}", "WVC_XaG_CurrentXenotype".Translate(), chosenXenotype.label.CapitalizeFirst().Colorize(ColoredText.GeneColor)));
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
			}
			if (parent is Plant plant && plant.Growth < Props.minGrowthForSpawn)
			{
				yield break;
			}
			yield return new Command_Action
			{
				defaultLabel = "WVC_XaG_GeneBabyTree_label".Translate() + ": " + MechanoidizationUtility.OnOrOff(spawnerIsActive),
				defaultDesc = "WVC_XaG_GeneBabyTree_desc".Translate(),
				icon = parent.def.uiIcon,
				action = delegate
				{
					spawnerIsActive = !spawnerIsActive;
				}
			};
			yield return new Command_Action
			{
				defaultLabel = "WVC_XaG_XenoTreeXenotypeChooseLabel".Translate(),
				defaultDesc = "WVC_XaG_XenoTreeXenotypeChooseDesc".Translate(),
				icon = GetXenotypeIcon(),
				action = delegate
				{
					List<FloatMenuOption> list = new();
					List<XenotypeDef> xenotypes = XenotypeFilterUtility.AllXenotypesExceptAndroids();
					for (int i = 0; i < xenotypes.Count; i++)
					{
						XenotypeDef xenotype = xenotypes[i];
						if (XenotypeUtility.XenoTree_CanSpawn(xenotype, parent) || DebugSettings.ShowDevGizmos)
						{
							list.Add(new FloatMenuOption(xenotype.label.CapitalizeFirst(), delegate
							{
								chosenXenotype = xenotype;
								Messages.Message("WVC_XaG_XenoTreeXenotypeChooseAssigned".Translate(xenotype.label.CapitalizeFirst()), null, MessageTypeDefOf.NeutralEvent, historical: false);
							}));
						}
					}
					if (list.Any())
					{
						Find.WindowStack.Add(new FloatMenu(list));
					}
				}
			};
		}

		private Texture2D GetXenotypeIcon()
		{
			if (chosenXenotype != null)
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
			Scribe_Defs.Look(ref chosenXenotype, "chosenXenotype");
			Scribe_Values.Look(ref spawnerIsActive, "babySpawn_OnOff", true);
		}
	}

}
