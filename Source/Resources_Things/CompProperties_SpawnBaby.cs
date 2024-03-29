using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	[Obsolete]
	public class CompProperties_SpawnBabyPawnAndInheritGenes : CompProperties
	{
		public IntRange ticksBetweenSpawn = new(60000, 120000);

		public GeneDef geneDef = null;

		public string uniqueTag = "ResurgentChild";

		public string completeMessage = "WVC_RB_Gene_MechaGestator";

		public CompProperties_SpawnBabyPawnAndInheritGenes()
		{
			compClass = typeof(CompSpawnBabyPawnAndInheritGenes);
		}
	}

	[Obsolete]
	public class CompSpawnBabyPawnAndInheritGenes : ThingComp
	{
		public int tickCounter = 0;

		private Pawn childOwner = null;

		private bool spawnerIsActive = true;

		// private int ticksBetweenSpawn = 0;

		private CompProperties_SpawnBabyPawnAndInheritGenes Props => (CompProperties_SpawnBabyPawnAndInheritGenes)props;

		public override void Initialize(CompProperties props)
		{
			base.Initialize(props);
			ResetCounter();
			// GetChildOwner();
		}

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			base.PostSpawnSetup(respawningAfterLoad);
			if (!respawningAfterLoad)
			{
				ResetCounter();
			}
			GetChildOwner();
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
			if (tickCounter > 0)
			{
				return;
			}
			if (childOwner == null)
			{
				ForceChildOwner();
			}
			TryDoSpawn();
			ResetCounter();
			GetChildOwner();
		}

		public void TryDoSpawn()
		{
			if (childOwner != null && spawnerIsActive)
			{
				GestationUtility.GenerateNewBornPawn_Thing(childOwner, Props.completeMessage, true, true, parent);
			}
		}

		public override string CompInspectStringExtra()
		{
			if (childOwner != null && spawnerIsActive)
			{
				return "WVC_XaG_Label_CompSpawnBabyPawnAndInheritGenes".Translate((tickCounter).ToStringTicksToPeriod()) + "\n" + "WVC_XaG_Label_CompSpawnBabyPawnAndInheritGenes_ChildOwner".Translate() + ": " + childOwner.Name;
			}
			return null;
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
						GetChildOwner();
					}
				};
			}
			yield return new Command_Action
			{
				defaultLabel = "WVC_XaG_GeneBabyTree_label".Translate() + ": " + OnOff(),
				defaultDesc = "WVC_XaG_GeneBabyTree_desc".Translate(),
				// icon = ContentFinder<Texture2D>.Get(parent.def.uiIcon),
				icon = parent.def.uiIcon,
				action = delegate
				{
					spawnerIsActive = !spawnerIsActive;
				}
			};
		}

		private string OnOff()
		{
			if (spawnerIsActive)
			{
				return "WVC_XaG_Gene_DustMechlink_On".Translate();
			}
			return "WVC_XaG_Gene_DustMechlink_Off".Translate();
		}

		public void ResetCounter()
		{
			tickCounter = Props.ticksBetweenSpawn.RandomInRange;
		}

		public void GetChildOwner()
		{
			if (childOwner == null)
			{
				ForceChildOwner();
			}
		}

		public void ForceChildOwner()
		{
			List<Pawn> possibleParents = new();
			List<Pawn> list = parent.Map.mapPawns.FreeAdultColonistsSpawned;
			foreach (Pawn item in list)
			{
				// if (item.Faction == Faction.OfPlayer)
				// {
				// }
				if (XaG_GeneUtility.HasActiveGene(Props.geneDef, item) || Props.geneDef == null)
				{
					possibleParents.Add(item);
				}
			}
			if (possibleParents != null && possibleParents.Count > 0)
			{
				childOwner = possibleParents.RandomElement();
			}
		}

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look(ref tickCounter, "tickCounterNextBabySpawn_" + Props.uniqueTag, 0);
			Scribe_References.Look(ref childOwner, "childOwner");
			Scribe_Values.Look(ref spawnerIsActive, "babySpawn_OnOff", true);
		}
	}

}
