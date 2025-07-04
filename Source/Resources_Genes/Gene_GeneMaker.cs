using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Genemaker : Gene, IGeneInspectInfo, IGeneRemoteControl
	{
		public string RemoteActionName => XaG_UiUtility.OnOrOff(spawnGenepack);

		public TaggedString RemoteActionDesc => "WVC_XaG_RemoteControlGeneMakerDesc".Translate();

		public void RemoteControl_Action(Dialog_GenesSettings genesSettings)
		{
			spawnGenepack = !spawnGenepack;
		}

		public bool spawnGenepack = false;

		public bool RemoteControl_Hide => !Active;

		public bool RemoteControl_Enabled
		{
			get
			{
				return enabled;
			}
			set
			{
				enabled = value;
				remoteControllerCached = false;
			}
		}

		public override void PostRemove()
		{
			base.PostRemove();
			XaG_UiUtility.SetAllRemoteControllersTo(pawn);
		}

		public bool enabled = true;
		public bool remoteControllerCached = false;

		public void RemoteControl_Recache()
		{
			XaG_UiUtility.RecacheRemoteController(pawn, ref remoteControllerCached, ref enabled);
		}


		//===========

		public GeneExtension_Spawner Props => def.GetModExtension<GeneExtension_Spawner>();

		public int ticksUntilSpawn;

        public override void PostAdd()
		{
			base.PostAdd();
			ResetInterval();
		}

		public override void TickInterval(int delta)
		{
			//base.Tick();
			if (!spawnGenepack)
			{
				return;
			}
			ticksUntilSpawn -= delta;
			if (ticksUntilSpawn > 0)
			{
				return;
			}
			if (!XaG_GeneUtility.ActiveFactionMap(pawn, this) && Props != null)
			{
				SpawnItems();
			}
			ResetInterval();
		}

		private void SpawnItems()
		{
			SpawnItems(pawn, Props.showMessageIfOwned, Props.spawnMessage);
		}

		private void ResetInterval()
		{
			ticksUntilSpawn = Props.spawnIntervalRange.RandomInRange;
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: SpawnGenepack",
					defaultDesc = "NextSpawnedResourceIn".Translate() + ": " + ticksUntilSpawn.ToStringTicksToPeriod().Colorize(ColoredText.DateTimeColor),
					action = delegate
					{
						if (pawn.Map != null && Active)
						{
							SpawnItems();
						}
						ResetInterval();
					}
				};
			}
			if (enabled)
			{
				foreach (Gizmo gizmo in XaG_UiUtility.GetRemoteControllerGizmo(pawn, remoteControllerCached, this))
				{
					yield return gizmo;
				}
			}
		}

		public void SpawnItems(Pawn pawn, bool showMessage = false, string message = "MessageCompSpawnerSpawnedItem")
		{
			string catchString = "";
			try
			{
				catchString = "genepack";
				Thing thing = ThingMaker.MakeThing(ThingDefOf.Genepack);
				if (thing is Genepack genepack)
				{
					GeneSet geneSet = genepack.GeneSet;
					foreach (GeneDef geneDef in geneSet.GenesListForReading.ToList())
					{
						geneSet.Debug_RemoveGene(geneDef);
					}
					catchString = "getting gene for genepack";
					GeneDef choosenGene = GetGene(pawn);
					geneSet.AddGene(choosenGene);
					geneSet.GenerateName();
				}
				thing.stackCount = 1;
				catchString = "style";
				if (Props.styleDef != null)
				{
					thing.SetStyleDef(Props.styleDef);
				}
				catchString = "spawn";
				GenPlace.TryPlaceThing(thing, pawn.Position, pawn.Map, ThingPlaceMode.Near, null, null, default);
				SoundDefOf.Execute_Cut.PlayOneShot(pawn);
				catchString = "blood";
				GeneFeaturesUtility.TrySpawnBloodFilth(pawn, new(0,1));
				if (showMessage)
				{
					Messages.Message(message.Translate(thing.LabelCap), thing, MessageTypeDefOf.PositiveEvent);
				}
				catchString = "xenogermination";
				ReimplanterUtility.XenogermReplicating_WithCustomDuration(pawn, Props.durationIntervalRange, pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.XenogermReplicating));
			}
			catch (Exception arg)
			{
				Log.Error($"{def.defName} failed spawn genepack during phase {catchString}: {arg}");
			}
		}

		private GeneDef GetGene(Pawn pawn)
		{
			List<XenotypeDef> allMatchedXenotypes = XaG_GeneUtility.GetAllMatchedXenotypes(pawn, ListsUtility.GetAllXenotypesExceptAndroids(), Props.matchPercent);
			List<GeneDef> allGenes = XaG_GeneUtility.ConvertToDefs(pawn.genes.GenesListForReading);
			foreach (XenotypeDef xenoDef in allMatchedXenotypes)
			{
				foreach (GeneDef geneDef in xenoDef.genes)
				{
					if (!allGenes.Contains(geneDef))
					{
						allGenes.Add(geneDef);
					}
				}
			}
			List<CustomXenotype> allMatchedCustomXenotypes = XaG_GeneUtility.GetAllMatchedCustomXenotypes(pawn, ListsUtility.GetCustomXenotypesList(), Props.matchPercent);
			if (!allMatchedCustomXenotypes.NullOrEmpty())
			{
				foreach (CustomXenotype xenoDef in allMatchedCustomXenotypes)
				{
					foreach (GeneDef geneDef in xenoDef.genes)
					{
						if (!allGenes.Contains(geneDef))
						{
							allGenes.Add(geneDef);
						}
					}
				}
			}
			// Log.Error("All matched xenotypes:" + "\n" + allMatchedXenotypes.Select((XenotypeDef x) => x.defName).ToLineList(" - "));
			// Log.Error("All matched custom xenotypes:" + "\n" + allMatchedCustomXenotypes.Select((CustomXenotype x) => x.name).ToLineList(" - "));
			return allGenes.Where((GeneDef x) => x.canGenerateInGeneSet).RandomElement();
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref spawnGenepack, "spawnGenepack", defaultValue: false);
			Scribe_Values.Look(ref ticksUntilSpawn, "ticksToSpawnThing", 0);
		}

		public string GetInspectInfo
		{
			get
			{
				if (!spawnGenepack)
				{
					return null;
				}
				if (pawn.Drafted)
				{
					return null;
				}
				return "NextSpawnedItemIn".Translate(GenLabel.ThingLabel(ThingDefOf.Genepack, null, 1)).Resolve() + ": " + ticksUntilSpawn.ToStringTicksToPeriod().Colorize(ColoredText.DateTimeColor);
			}
		}
    }

}
