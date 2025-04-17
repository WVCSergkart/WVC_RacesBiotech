using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_Humanlike : CompProperties
	{

		public bool shouldResurrect = true;

		public int recacheFrequency = 60;

		public IntRange resurrectionDelay = new(6000, 9000);

		public string uniqueTag = "XaG_Undead";

		public CompProperties_Humanlike()
		{
			compClass = typeof(CompHumanlike);
		}

		// public override void ResolveReferences(ThingDef parentDef)
		// {
			// if (shouldResurrect && parentDef.race?.corpseDef != null)
			// {
				// if (parentDef.race.corpseDef.GetCompProperties<CompProperties_UndeadCorpse>() != null)
				// {
					// return;
				// }
				// CompProperties_UndeadCorpse undead_comp = new();
				// undead_comp.resurrectionDelay = resurrectionDelay;
				// undead_comp.uniqueTag = uniqueTag;
				// parentDef.race.corpseDef.comps.Add(undead_comp);
			// }
		// }

	}

	public class CompHumanlike : ThingComp
	{

		public CompProperties_Humanlike Props => (CompProperties_Humanlike)props;

		// =================

		[Unsaved(false)]
		private List<IGeneFloatMenuOptions> cachedFloatMenuOptionsGenes;
		[Unsaved(false)]
		private List<IGeneInspectInfo> cachedInfoGenes;
        //[Unsaved(false)]
        //private List<IGeneRemoteControl> cachedRemoteControlGenes;

        public List<IGeneInspectInfo> InfoGenes
		{
			get
			{
				if (cachedInfoGenes == null)
				{
					RecacheGenes();
				}
				return cachedInfoGenes;
			}
		}

		public List<IGeneFloatMenuOptions> FloatMenuOptions
		{
			get
			{
				if (cachedFloatMenuOptionsGenes == null)
				{
					RecacheGenes();
				}
				return cachedFloatMenuOptionsGenes;
			}
		}

        //public List<IGeneRemoteControl> RemoteControl
        //{
        //    get
        //    {
        //        if (cachedRemoteControlGenes == null)
        //        {
        //            RecacheGenes();
        //        }
        //        return cachedRemoteControlGenes;
        //    }
        //}

        public void RecacheGenes()
		{
			cachedInfoGenes = new();
			cachedFloatMenuOptionsGenes = new();
			//cachedRemoteControlGenes = new();
			if (parent is Pawn pawn)
			{
				foreach (Gene gene in pawn.genes.GenesListForReading)
				{
					if (gene is IGeneInspectInfo geneInspectInfo && gene.Active)
					{
						cachedInfoGenes.Add(geneInspectInfo);
					}
					if (gene is IGeneFloatMenuOptions geneFloatMenu && gene.Active)
					{
						cachedFloatMenuOptionsGenes.Add(geneFloatMenu);
					}
                    //if (gene is IGeneRemoteControl geneRemoteControl)
                    //{
                    //    cachedRemoteControlGenes.Add(geneRemoteControl);
                    //}
                }
			}
		}

		public void ResetInspectString()
		{
			cachedInfoGenes = null;
			cachedFloatMenuOptionsGenes = null;
			//cachedRemoteControlGenes = null;
			// isBloodeater = null;
		}

		public override string CompInspectStringExtra()
		{
			if (parent.Faction != Faction.OfPlayer)
			{
				return null;
			}
			if (parent is Pawn pawn)
			{
				return Info(pawn);
			}
			return null;
		}

		private string info = null;

		private int nextRecache = -1;

		public string Info(Pawn pawn)
		{
			nextRecache--;
			if (nextRecache <= 0)
			{
				if (pawn?.genes == null)
				{
					return null;
				}
				info = null;
				int count = 0;
				foreach (IGeneInspectInfo gene in InfoGenes)
				{
					string geneText = gene.GetInspectInfo;
					if (geneText.NullOrEmpty())
					{
						continue;
					}
					if (count > 0)
					{
						info += "\n";
					}
					info += geneText;
					count++;
				}
				nextRecache = Props.recacheFrequency;
			}
			return info;
		}

		// =============

		public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
		{
			foreach (IGeneFloatMenuOptions gene in FloatMenuOptions)
			{
				foreach (FloatMenuOption gizmo in gene.CompFloatMenuOptions(selPawn))
				{
					yield return gizmo;
				}
			}
		}

		// =====================

		public override void Notify_Killed(Map prevMap, DamageInfo? dinfo = null)
		{
			if (parent is not Pawn pawn || pawn?.genes == null)
			{
				return;
			}
			foreach (Gene gene in pawn.genes.GenesListForReading)
			{
				if (gene is IGeneNotifyOnKilled igene && gene.Active)
				{
					try
					{
						igene.Notify_PawnKilled();
					}
					catch
					{
						Log.Error("Failed trigger Notify_PawnKilled for gene " + gene.def.defName);
					}
				}
			}
		}

		private int resurrectionDelay = 0;
		private bool shouldResurrect = false;

		public void SetUndead(bool resurrect, int delay, Pawn pawn)
		{
			shouldResurrect = resurrect && (pawn.IsColonist || WVC_Biotech.settings.canNonPlayerPawnResurrect);
			resurrectionDelay = delay;
			if (delay > 0)
			{
				resurrectionDelay += Find.TickManager.TicksGame + Props.resurrectionDelay.RandomInRange;
			}
		}

		public override void CompTickRare()
		{
			UndeadTick();
		}

		public void UndeadTick()
		{
			if (!shouldResurrect)
			{
				return;
			}
			if (Find.TickManager.TicksGame < resurrectionDelay)
			{
				return;
			}
			TryResurrect();
		}

		public void TryResurrect()
		{
			if (parent is not Pawn pawn)
			{
				return;
			}
			if (pawn.Corpse?.Map == null)
			{
				return;
			}
			pawn.GetUndeadGene(out Gene_Undead gene);
			if (gene != null)
			{
				if (pawn.Corpse?.CurRotDrawMode != RotDrawMode.Fresh)
				{
					if (ModLister.CheckAnomaly("Shambler"))
					{
						MutantUtility.ResurrectAsShambler(pawn, new IntRange(145563, 888855).RandomInRange, pawn.Faction);
					}
				}
				else
				{
					GeneResourceUtility.GeneUndeadResurrection(pawn, gene);
				}
			}
			resurrectionDelay = 0;
			shouldResurrect = false;
		}

		public override void PostExposeData()
		{
			Scribe_Values.Look(ref resurrectionDelay, "resurrectionDelay_" + Props.uniqueTag, 0);
			Scribe_Values.Look(ref shouldResurrect, "shouldResurrect_" + Props.uniqueTag, false);
		}

		public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (DebugSettings.ShowDevGizmos)
            {
                yield return new Command_Action
                {
                    defaultLabel = "DEV: ResetXenotype",
                    action = delegate
                    {
                        Pawn pawn = parent as Pawn;
                        ReimplanterUtility.SetXenotype(pawn, pawn.genes.Xenotype);
                    }
                };
				yield return new Command_Action
				{
					defaultLabel = "DEV: NotifyGenesChanged",
					action = delegate
					{
						Pawn pawn = parent as Pawn;
						ReimplanterUtility.NotifyGenesChanged(pawn);
					}
				};
                yield return new Command_Action
                {
                    defaultLabel = "DEV: AddAllRemoteControllers",
                    action = delegate
                    {
                        XaG_GeneUtility.Debug_ImplantAllGenes(parent as Pawn, DefDatabase<GeneDef>.AllDefsListForReading.Where((GeneDef geneDef) => geneDef.IsGeneDefOfType<IGeneRemoteControl>()).ToList());
                    }
                };
            }
		}

	}

}
