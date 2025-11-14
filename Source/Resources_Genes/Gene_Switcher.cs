using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.Noise;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Switcher : Gene, IGeneShapeshifter, IGeneRemoteControl
	{

		public string RemoteActionName
		{
			get
			{
				if (nextTick > 0)
				{
					return nextTick.ToStringTicksToPeriod().Colorize(ColoredText.DateTimeColor);
				}
				return LabelCap;
			}
		}

		public TaggedString RemoteActionDesc => "WVC_XaG_RemoteControl_Switcher".Translate();

		public void RemoteControl_Action(Dialog_GenesSettings genesSettings)
		{
			if (!Active || nextTick > 0)
			{
				SoundDefOf.ClickReject.PlayOneShotOnCamera();
				return;
			}
			List<FloatMenuOption> list = new();
			List<XenotypeHolder> xenotypes = Xenotypes.ToList();
			xenotypes.RemoveAll((holder) => XaG_GeneUtility.GenesIsMatch(pawn.genes.GenesListForReading, holder.genes, 1f));
			if (!xenotypes.NullOrEmpty())
			{
				for (int i = 0; i < xenotypes.Count; i++)
				{
					XenotypeHolder geneSet = xenotypes[i];
					list.Add(new FloatMenuOption(geneSet.LabelCap, delegate
					{
						Switch(pawn, geneSet);
						genesSettings.Close();
					}, orderInPriority: 0 - (int)geneSet.xenotypeDef.displayPriority));
				}
			}
			if (!list.Any())
			{
				list.Add(new FloatMenuOption("WVC_None".Translate(), delegate
				{

				}));
			}
			Find.WindowStack.Add(new FloatMenu(list));
		}

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

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (enabled)
			{
				yield return XaG_UiUtility.GetRemoteControllerGizmo(pawn, remoteControllerCached, this);
			}
		}

		// ====================

		public override void PostAdd()
		{
			base.PostAdd();
			if (MiscUtility.GameStarted())
			{
				nextTick = (pawn.Faction == Faction.OfPlayer ? 60000 : new IntRange(0, 60000).RandomInRange) * 15;
			}
		}

		private bool? cachedInheritable;
		public bool Inheritable
		{
			get
			{
				if (cachedInheritable == null)
				{
					cachedInheritable = !pawn.genes.IsXenogene(this);
				}
				return cachedInheritable.Value;
			}
		}

		public static List<XenotypeHolder> cachedHolders;
		public List<XenotypeHolder> Xenotypes
		{
			get
			{
				if (cachedHolders == null)
				{
					List<XenotypeHolder> newList = new();
					foreach (XenotypeHolder xenotypeHolder in ListsUtility.GetAllXenotypesHolders())
					{
						if (xenotypeHolder.inheritable != Inheritable)
						{
							continue;
						}
						//if (!XaG_GeneUtility.GenesIsMatch(pawn.genes.GenesListForReading, xenotypeHolder.genes, WVC_Biotech.settings.))
						//{
						//	continue;
						//}
						if (xenotypeHolder.genes.Any((geneDef) => geneDef.IsGeneDefOfType<Gene_Switcher>()))
						{
							newList.Add(xenotypeHolder);
						}
					}
					cachedHolders = newList;
				}
				return cachedHolders;
			}
		}

		public void Switch(Pawn pawn, XenotypeHolder newHolder)
		{
			//cachedHolders = null;
			try
			{
				if (!newHolder.inheritable || pawn.genes.Xenogenes.NullOrEmpty())
				{
					ReimplanterUtility.SetXenotypeDirect(pawn, newHolder);
				}
				if (Inheritable)
				{
					SetXenotype(newHolder, pawn.genes.Endogenes);
				}
				else
				{
					SetXenotype(newHolder, pawn.genes.Xenogenes);
				}
				ReimplanterUtility.XenogermReplicating_WithCustomDuration(pawn, new(nextTick, nextTick));
				ReimplanterUtility.TrySetSkinAndHairGenes(pawn);
				ReimplanterUtility.PostImplantDebug(pawn);
				if (ModLister.IdeologyInstalled)
				{
					Find.HistoryEventsManager.RecordEvent(new HistoryEvent(HistoryEventDefOf.WVC_Morph, pawn.Named(HistoryEventArgsNames.Doer)));
				}
				if (pawn.SpawnedOrAnyParentSpawned)
				{
					MiscUtility.DoSkipEffects(pawn.PositionHeld, pawn.MapHeld);
				}
			}
			catch (Exception arg)
			{
				Log.Error("Failed switch to xenotype: " + newHolder.LabelCap + ". Reason: " + arg.Message);
			}
		}

		private void SetXenotype(XenotypeHolder newHolder, List<Gene> genes)
		{
			foreach (Gene gene in genes.ToList())
			{
				if (newHolder.genes.Contains(gene.def))
				{
					continue;
				}
				RemoveGene(gene);
			}
			List<GeneDef> pawnGenes = genes.ConvertToDefs();
			foreach (GeneDef geneDef in newHolder.genes)
			{
				if (pawnGenes.Contains(geneDef))
				{
					continue;
				}
				AddGene(geneDef);
				nextTick += (int)(60000 * Find.Storyteller.difficulty.threatScale);
			}
		}

		private void AddGene(GeneDef geneDef)
		{
			if (!def.ConflictsWith(geneDef))
			{
				pawn.genes.AddGene(geneDef, !Inheritable);
			}
		}

		private void RemoveGene(Gene gene)
		{
			if (this != gene)
			{
				pawn.genes.RemoveGene(gene);
			}
		}

		private int nextTick = 0;
		public override void TickInterval(int delta)
		{
			if (nextTick > 0)
			{
				nextTick -= delta;
				MiscUtility.GeneAbilityReadyLetter(nextTick, this);
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref nextTick, "nextTick", 0);
		}

	}

}
