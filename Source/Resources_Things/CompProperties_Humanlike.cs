using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

		[NoTranslate]
		public string uniqueTag = "XaG_Undead";
		[NoTranslate]
		public string dupUniqueTag = "XaG_DupFix";

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

        //[Unsaved(false)]
        //private XaG_GameComponent cachedGameComponent;

        //public XaG_GameComponent GameComponent
        //{
        //    get
        //    {
        //        if (cachedGameComponent == null || Current.Game != cachedGameComponent.currentGame)
        //        {
        //            cachedGameComponent = Current.Game.GetComponent<XaG_GameComponent>();
        //        }
        //        return cachedGameComponent;
        //    }
        //}

        //      public override void PostSpawnSetup(bool respawningAfterLoad)
        //{
        //	if (parent is not Pawn pawn || pawn?.genes == null)
        //	{
        //		return;
        //	}
        //	StaticCollectionsClass.currentGameComponent?.TryUpdateKnownXenotype(pawn);
        //}

		//public override void Notify_Arrested(bool succeeded)
		//{
		//	if (parent?.Faction?.IsPlayer == true)
		//	{
		//		MiscUtility.CountAllPlayerControlledPawns_StaticCollection();
		//	}
		//}

		//public override void Notify_Released()
		//{
		//	if (parent?.Faction?.IsPlayer == true)
		//	{
		//		MiscUtility.CountAllPlayerControlledPawns_StaticCollection();
		//	}
		//}

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

		public Pawn Pawn => parent as Pawn;

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

		private static bool collapse = true;
		public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (!DebugSettings.ShowDevGizmos)
            {
                yield break;
            }
            yield return new Command_Action
            {
                defaultLabel = "DEV: ExpandDevTools",
				icon = ContentFinder<Texture2D>.Get("WVC/UI/XaG_General/DevMode_Setttings"),
				action = delegate
                {
                    collapse = !collapse;
                }
            };
            if (collapse)
			{
				yield break;
			}
            yield return new Command_Action
            {
                defaultLabel = "DEV: ResetXenotype",
				icon = ContentFinder<Texture2D>.Get("WVC/UI/XaG_General/ThrallMaker_XenoMenu_Gizmo_v0"),
				action = delegate
                {
                    Pawn pawn = parent as Pawn;
                    ReimplanterUtility.SetXenotype(pawn, pawn.genes.Xenotype);
                }
            };
            yield return new Command_Action
            {
                defaultLabel = "DEV: SetXenotype",
				icon = ContentFinder<Texture2D>.Get("WVC/UI/XaG_General/ThrallMaker_Implanter_Gizmo_v0"),
				action = delegate
                {
                    Pawn pawn = parent as Pawn;
                    List<FloatMenuOption> list = new();
                    List<XenotypeDef> xenotypeDefs = DefDatabase<XenotypeDef>.AllDefsListForReading;
                    for (int i = 0; i < xenotypeDefs.Count; i++)
                    {
                        XenotypeDef xenotypeDef = xenotypeDefs[i];
                        list.Add(new FloatMenuOption(xenotypeDef.LabelCap, delegate
                        {
                            ReimplanterUtility.SetXenotype_DoubleXenotype(pawn, xenotypeDef);
                        }, orderInPriority: 0 - i));
                    }
                    Find.WindowStack.Add(new FloatMenu(list));
                }
            };
            yield return new Command_Action
            {
                defaultLabel = "DEV: DebugGenes",
				icon = ContentFinder<Texture2D>.Get("WVC/UI/XaG_General/WorkSkillTex_v0"),
				action = delegate
                {
                    Pawn pawn = parent as Pawn;
                    ReimplanterUtility.PostImplantDebug(pawn);
					StaticCollectionsClass.ResetCollection();
					HivemindUtility.ResetCollection();
					HediffUtility.UpdatePawnGeneHediffs(Pawn);
				}
            };
            yield return new Command_Action
            {
                defaultLabel = "DEV: AddAllRemoteControllers",
				icon = ContentFinder<Texture2D>.Get("WVC/UI/XaG_General/UI_Golemlink_GizmoSummonSettings"),
				action = delegate
                {
                    XaG_GeneUtility.Debug_ImplantAllGenes(parent as Pawn, DefDatabase<GeneDef>.AllDefsListForReading.Where((GeneDef geneDef) => geneDef.IsGeneDefOfType<IGeneRemoteControl>()).ToList());
                }
            };
			yield return new Command_Action
			{
				defaultLabel = "DEV: GetGenesMatchList",
				icon = ContentFinder<Texture2D>.Get("WVC/UI/XaG_General/UI_ShapeshifterMode_Duplicate"),
				action = delegate
				{
					Pawn pawn = parent as Pawn;
					StringBuilder stringBuild = new();
					foreach (XenotypeDef xenos in DefDatabase<XenotypeDef>.AllDefsListForReading)
					{
						if (xenos.genes.NullOrEmpty())
                        {
							continue;
                        }
						float matchedGenes = XaG_GeneUtility.GetMatchingGenesList(pawn.genes.GenesListForReading, xenos.genes).Count;
						stringBuild.AppendLine(xenos.LabelCap + " genes match: " + (matchedGenes / xenos.genes.Count * 100) + "%");
					}
					Log.Error(stringBuild.ToString());
				}
			};
			yield return new Command_Action
			{
				defaultLabel = "DEV: SetAsDuplicateOf",
				icon = ContentFinder<Texture2D>.Get("WVC/UI/Genes/Gene_Duplicator_v0"),
				action = delegate
				{
					List<FloatMenuOption> list = new();
					List<Pawn> pawns = PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_Colonists;
					for (int i = 0; i < pawns.Count; i++)
					{
						Pawn pawn = pawns[i];
						list.Add(new FloatMenuOption(pawn.NameFullColored, delegate
						{
							SetDuplicate(pawn);
						}, orderInPriority: 0 - i));
					}
					Find.WindowStack.Add(new FloatMenu(list));
				}
			};
			//yield return new Command_Action
			//{
			//	defaultLabel = "DEV: IsSubGeneOfThis TEST",
			//	action = delegate
			//	{
			//		if (XaG_GeneUtility.IsSubGeneOfThisCycly(DefDatabase<GeneDef>.GetNamed("Hemogenic"), DefDatabase<GeneDef>.GetNamed("WVC_Psyfeeder")))
			//		{
			//			Log.Error("YES");
			//		}
			//		else
			//		{
			//			Log.Error("NO");
			//		}
			//	}
			//};
		}

		// ======================================

		public override void Notify_DuplicatedFrom(Pawn source)
        {
            SetDuplicate(source);
			//foreach (Gene gene in Pawn.genes.GenesListForReading)
			//{

			//}
			//HediffUtility.UpdatePawnGeneHediffs(Pawn);
        }

        public bool IsDuplicate
        {
            get
            {
                return pawnIsDuplicate || originalRef != null && originalRef != parent;
            }
        }

		public Pawn SourcePawn => originalRef ?? parent as Pawn;

		private bool pawnIsDuplicate = false;
		private Pawn originalRef;

		public void SetDuplicate(Pawn source)
        {
			if (source != null && source != parent)
            {
				originalRef = source;
			}
			pawnIsDuplicate = originalRef != null;
		}

		public override void PostExposeData()
		{
			Scribe_Values.Look(ref resurrectionDelay, "resurrectionDelay_" + Props.uniqueTag, 0);
			Scribe_Values.Look(ref shouldResurrect, "shouldResurrect_" + Props.uniqueTag, false);
			Scribe_Values.Look(ref resurrected, "resurrected_" + Props.uniqueTag, false);
			Scribe_Values.Look(ref pawnIsDuplicate, "pawnIsDuplicate_" + Props.dupUniqueTag, false);
			Scribe_References.Look(ref originalRef, "originalRef_" + Props.dupUniqueTag, true);
		}

		public bool Undead => resurrected;

		private bool resurrected = false;
		public virtual void Notify_Resurrected()
		{
			SetResurrected();
			if (ModLister.IdeologyInstalled && (Pawn.Map != null || Pawn.Corpse?.Map != null))
			{
				Find.HistoryEventsManager.RecordEvent(new HistoryEvent(HistoryEventDefOf.WVC_UndeadResurrection, Pawn.Named(HistoryEventArgsNames.Doer)));
			}
		}

		public void SetResurrected()
		{
			if (!resurrected)
			{
				GeneResourceUtility.UpdUndeads();
			}
			resurrected = true;
		}

	}

}
