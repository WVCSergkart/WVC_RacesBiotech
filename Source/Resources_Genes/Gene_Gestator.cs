using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

    public class Gene_SimpleGestator : Gene, IGeneOverridden, IGeneRemoteControl
	{
		public virtual string RemoteActionName => "WVC_Start".Translate();

		public TaggedString RemoteActionDesc => Desc;

		public virtual void RemoteControl_Action(Dialog_GenesSettings genesSettings)
		{
			if (!Active)
            {
				SoundDefOf.ClickReject.PlayOneShotOnCamera();
				return;
            }
			if (MiscUtility.CanStartPregnancy(pawn, Giver))
			{
				if (UseDialogWarning)
				{
					Dialog_MessageBox window = Dialog_MessageBox.CreateConfirmation(Warning, delegate
					{
						StartPregnancy();
					});
					Find.WindowStack.Add(window);
				}
				else
				{
					StartPregnancy();
				}
			}
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

		public bool enabled = true;
		public bool remoteControllerCached = false;

		public void RemoteControl_Recache()
		{
			XaG_UiUtility.RecacheRemoteController(pawn, ref remoteControllerCached, ref enabled);
		}

		//===========

		public GeneExtension_Spawner Spawner => def?.GetModExtension<GeneExtension_Spawner>();

		public GeneExtension_Giver Giver => def?.GetModExtension<GeneExtension_Giver>();

		public virtual string Desc => "WVC_XaG_Gene_SimpleGestatorDesc".Translate();
		public virtual string Warning => "WVC_XaG_Gene_SimpleGestatorWarning".Translate();
		public virtual bool UseDialogWarning => true;


		public override IEnumerable<Gizmo> GetGizmos()
		{
			//if (XaG_GeneUtility.SelectorDraftedActiveFaction(pawn, this))
			//{
			//	yield break;
			//}
			//if (DebugSettings.ShowDevGizmos)
			//{
			//	yield return new Command_Action
			//	{
			//		defaultLabel = "DEV: Spawn NewBornGenes",
			//		action = delegate
			//		{
			//			GestationUtility.GestateChild_WithGenes(pawn);
			//		}
			//	};
			//	yield return new Command_Action
			//	{
			//		defaultLabel = "DEV: Spawn NewBornXenotype",
			//		action = delegate
			//		{
			//			GestationUtility.GestateChild_WithXenotype(pawn, ListsUtility.GetAllXenotypesExceptAndroids().RandomElement(), null, "WVC_XaG_XenoTreeBirthLabel", "WVC_XaG_XenoTreeBirthDesc");
			//		}
			//	};
			//}
			//yield return new Command_Action
			//{
			//	defaultLabel = LabelCap,
			//	defaultDesc = Desc,
			//	icon = ContentFinder<Texture2D>.Get(def.iconPath),
			//	action = delegate
			//	{
			//		if (MiscUtility.CanStartPregnancy_Gestator(pawn, Giver))
			//		{
			//			if (UseDialogWarning)
			//			{
			//				Dialog_MessageBox window = Dialog_MessageBox.CreateConfirmation(Warning, delegate
			//				{
			//					StartPregnancy();
			//				});
			//				Find.WindowStack.Add(window);
			//			}
			//			else
			//			{
			//				StartPregnancy();
			//			}
			//		}
			//	}
			//};
			if (enabled)
			{
				yield return XaG_UiUtility.GetRemoteControllerGizmo(pawn, remoteControllerCached, this);
			}
		}

		public virtual void StartPregnancy()
        {
            Hediff hediff = HediffMaker.MakeHediff(Spawner.gestationHediffDef, pawn);
            HediffComp_GeneHediff hediff_GeneCheck = hediff.TryGetComp<HediffComp_GeneHediff>();
            if (hediff_GeneCheck != null)
            {
                hediff_GeneCheck.geneDef = def;
            }
            pawn.health.AddHediff(hediff);
            if (Spawner.cooldownHediffDef != null)
            {
                Hediff cooldownHediff = HediffMaker.MakeHediff(Spawner.cooldownHediffDef, pawn);
                HediffComp_Disappears hediffComp_Disappears = cooldownHediff.TryGetComp<HediffComp_Disappears>();
                if (hediffComp_Disappears != null)
                {
                    hediffComp_Disappears.ticksToDisappear = 60000 * 15;
                }
                HediffComp_GeneHediff cooldownHediff_GeneCheck = cooldownHediff.TryGetComp<HediffComp_GeneHediff>();
                if (cooldownHediff_GeneCheck != null)
                {
                    cooldownHediff_GeneCheck.geneDef = def;
                }
                pawn.health.AddHediff(cooldownHediff);
            }
        }

        public void Notify_OverriddenBy(Gene overriddenBy)
		{
			RemoveHediffs();
		}

		public void RemoveHediffs()
		{
			HediffUtility.RemoveHediffsFromList(pawn, Giver?.hediffDefs);
		}

		public void Notify_Override()
		{
		}

		public override void PostRemove()
		{
			base.PostRemove();
			RemoveHediffs();
			XaG_UiUtility.SetAllRemoteControllersTo(pawn);
		}

	}

	[Obsolete]
	public class Gene_Gestator_TestTool : Gene_SimpleGestator
	{


	}

	//[Obsolete]
	//public class Gene_DustGestator_TestTool : Gene_FoodEfficiency
	//{

	//	public override IEnumerable<Gizmo> GetGizmos()
	//	{
	//		// DEV
	//		if (DebugSettings.ShowDevGizmos)
	//		{
	//			yield return new Command_Action
	//			{
	//				defaultLabel = "DEV: Spawn pawn",
	//				action = delegate
	//				{
	//					GestationUtility.GestateChild_WithGenes(pawn);
	//				}
	//			};
	//		}
	//	}

	//}

	public class Gene_Parthenogenesis : Gene_SimpleGestator, IGenePregnantHuman
	{

		public override string Warning => "WVC_XaG_Gene_ParthenogenesisWarning".Translate();

		public override string Desc => "WVC_XaG_Gene_ParthenogenesisDesc".Translate();

        public override bool Active => pawn?.gender == Gender.Female && base.Active;

        public override void StartPregnancy()
		{
			MiscUtility.Impregnate(pawn);
		}

		public void Notify_PregnancyStarted(Hediff_Pregnant pregnancy)
        {
            AddParentGenes(pawn, pregnancy);
        }

        public static void AddParentGenes(Pawn pawn, Hediff_Pregnant pregnancy)
        {
            GeneSet geneSet = pregnancy.geneSet;
            if (geneSet != null)
            {
                HediffUtility.AddParentGenes(pawn, geneSet);
            }
            else
            {
                GeneSet newGeneSet = new();
                HediffUtility.AddParentGenes(pawn, newGeneSet);
                geneSet = newGeneSet;
            }
            geneSet.SortGenes();
        }

        public bool Notify_CustomPregnancy(Hediff_Pregnant pregnancy)
		{
			// Debug fire
			//Notify_PregnancyStarted(pregnancy);
			return false;
		}

	}

	// Gene-Gestator
	public class Gene_XenotypeGestator : Gene_SimpleGestator
	{
		public override bool UseDialogWarning => false;

		public override string Desc => "WVC_XaG_GeneXenoGestator_Desc".Translate();

		public override void StartPregnancy()
		{
			Find.WindowStack.Add(new Dialog_XenotypeGestator(this));
		}

	}

	// Storage-Gestator
	public class Gene_StorageGestator : Gene_SimpleGestator
	{

		[Unsaved(false)]
		private Gene_StorageImplanter cachedGene;
		public Gene_StorageImplanter Gene
		{
			get
			{
				if (cachedGene == null || !cachedGene.Active)
				{
					cachedGene = pawn.genes.GetFirstGeneOfType<Gene_StorageImplanter>();
				}
				return cachedGene;
			}
		}

		public override void RemoteControl_Action(Dialog_GenesSettings genesSettings)
		{
			if (Gene?.XenotypeHolder == null)
			{
				SoundDefOf.ClickReject.PlayOneShotOnCamera();
				return;
			}
			base.RemoteControl_Action(genesSettings);
		}

		public override string Warning => "WVC_XaG_GeneStorageGestator_Desc".Translate() + "\n\n" + "WVC_XaG_Gene_SimpleGestatorWarning".Translate();

		public override string Desc => "WVC_XaG_GeneStorageGestator_Desc".Translate();

		public override void StartPregnancy()
        {
			if (Gene?.XenotypeHolder == null)
            {
				return;
            }
            base.StartPregnancy();
			HediffComp_XenotypeGestator hediff = pawn.health.hediffSet?.GetHediffComps<HediffComp_XenotypeGestator>()?.FirstOrDefault();
			if (hediff != null)
            {
				hediff.SetupHolder(Gene.XenotypeHolder);
				//hediff.xenotypeDef = null;
				hediff.gestationIntervalDays = pawn.RaceProps.gestationPeriodDays;
				Gene.ResetHolder();
			}
		}

    }

}
