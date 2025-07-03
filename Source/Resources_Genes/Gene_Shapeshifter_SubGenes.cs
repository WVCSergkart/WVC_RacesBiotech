using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_ShapeshifterDependant : Gene, IGeneShapeshift
	{

		public GeneExtension_Giver Giver => def?.GetModExtension<GeneExtension_Giver>();

		[Unsaved(false)]
		private Gene_Shapeshifter cachedShapeshifterGene;

		public Gene_Shapeshifter Shapeshifter
		{
			get
			{
				if (cachedShapeshifterGene == null || !cachedShapeshifterGene.Active)
				{
					cachedShapeshifterGene = pawn?.genes?.GetFirstGeneOfType<Gene_Shapeshifter>();
				}
				return cachedShapeshifterGene;
			}
		}

		public override void TickInterval(int delta)
		{

		}

        public virtual void Notify_PreShapeshift(Gene_Shapeshifter shapeshiftGene)
        {
            
        }

        public virtual void Notify_PostShapeshift(Gene_Shapeshifter newShapeshiftGene)
        {
            
        }

        // public override void PostRemove()
        // {
        // base.PostRemove();
        // HediffUtility.Notify_GeneRemoved(this, pawn);
        // }

    }

	public class Gene_PostShapeshift_Recovery : Gene_ShapeshifterDependant
	{

		private bool? savedBool;

		public override void PostAdd()
        {
            base.PostAdd();
			if (Shapeshifter != null)
			{
				savedBool = Shapeshifter.genesRegrowAfterShapeshift;
				Shapeshifter.genesRegrowAfterShapeshift = false;
			}
		}

		public override void PostRemove()
		{
			base.PostRemove();
			if (Shapeshifter != null && savedBool.HasValue)
			{
				Shapeshifter.genesRegrowAfterShapeshift = savedBool.Value;
			}
		}
		public override void Notify_PreShapeshift(Gene_Shapeshifter shapeshiftGene)
		{
			if (savedBool.HasValue)
			{
				shapeshiftGene.genesRegrowAfterShapeshift = savedBool.Value;
			}
		}

		public override void Notify_PostShapeshift(Gene_Shapeshifter newShapeshiftGene)
		{
			//shapeshiftGene.genesRegrowAfterShapeshift = savedBool;
			if (Giver == null)
			{
				return;
			}
			savedBool = newShapeshiftGene.genesRegrowAfterShapeshift;
			newShapeshiftGene.genesRegrowAfterShapeshift = false;
			HediffUtility.RemoveHediffsFromList(pawn, Giver.hediffDefs);
		}

        public override void ExposeData()
        {
            base.ExposeData();
			Scribe_Values.Look(ref savedBool, "savedGenesRegrowStatus");
		}

    }

	public class Gene_PostShapeshift_Regeneration : Gene_ShapeshifterDependant
	{

		public override void Notify_PreShapeshift(Gene_Shapeshifter shapeshiftGene)
		{
			RemoveAllRemovableBadHediffs(pawn);
		}

		public static void RemoveAllRemovableBadHediffs(Pawn pawn)
		{
			foreach (Hediff hediff in pawn.health.hediffSet.hediffs.ToList())
			{
				if (!hediff.def.isBad || !hediff.def.everCurableByItem)
				{
					continue;
				}
				pawn.health.RemoveHediff(hediff);
			}
		}

	}

	public class Gene_PostShapeshift_GiveHediff : Gene_ShapeshifterDependant
	{

		public override void Notify_PostShapeshift(Gene_Shapeshifter newShapeshiftGene)
		{
			Hediff hediff = HediffMaker.MakeHediff(Giver.hediffDefName, pawn);
			HediffComp_Disappears hediffComp_Disappears = hediff.TryGetComp<HediffComp_Disappears>();
			if (hediffComp_Disappears != null)
			{
				hediffComp_Disappears.ticksToDisappear = Giver.intervalRange.RandomInRange;
			}
			pawn.health.AddHediff(hediff);
		}

	}

	public class Gene_Shapeshift_TrueForm : Gene_ShapeshifterDependant
	{



	}

	public class Gene_PostShapeshift_Scarred : Gene_ShapeshifterDependant
	{

		public override void PostAdd()
		{
			base.PostAdd();
			if (Current.ProgramState != ProgramState.Playing)
			{
				Scarify();
			}
		}

		public void Scarify()
		{
			if (!ModLister.CheckIdeology("Scarification"))
			{
				return;
			}
			int currentTry = 0;
			while (pawn.health.hediffSet.GetHediffCount(HediffDefOf.Scarification) < 5)
			{
				Gene_Scarifier.Scarify(pawn);
				currentTry++;
				if (currentTry > 8)
				{
					break;
				}
			}
		}

		public override void Notify_PreShapeshift(Gene_Shapeshifter shapeshiftGene)
		{
			if (!ModLister.CheckIdeology("Scarification"))
			{
				return;
			}
			// List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
			// for (int num = 0; num < hediffs.Count; num++)
			// {
				// if (hediffs[num].def != HediffDefOf.Scarification)
				// {
					// continue;
				// }
				// pawn.health.RemoveHediff(hediffs[num]);
			// }
			foreach (Hediff hediff in pawn.health.hediffSet.hediffs.ToList())
			{
				if (hediff.def != HediffDefOf.Scarification)
				{
					continue;
				}
				pawn.health.RemoveHediff(hediff);
			}
		}

		public override void Notify_PostShapeshift(Gene_Shapeshifter newShapeshiftGene)
		{
			Scarify();
		}

	}

	public class Gene_Shapeshift_Remote : Gene_ShapeshifterDependant, IGeneRemoteControl, IGeneWithEffects
	{

		public virtual string RemoteActionName => "ERROR";

		public virtual TaggedString RemoteActionDesc => "ERROR";

		public virtual void RemoteControl_Action(Dialog_GenesSettings genesSettings)
		{

		}

		public virtual bool RemoteControl_Hide => !Active;

		public virtual bool RemoteControl_Enabled
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

		public virtual void RemoteControl_Recache()
		{
			XaG_UiUtility.RecacheRemoteController(pawn, ref remoteControllerCached, ref enabled);
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (enabled)
			{
				return XaG_UiUtility.GetRemoteControllerGizmo(pawn, remoteControllerCached, this);
			}
			return null;
		}

		public virtual void DoEffects()
		{
			if (pawn.Map == null)
			{
				return;
			}
			MiscUtility.DoShapeshiftEffects_OnPawn(pawn);
		}

		public void DoEffects(Pawn pawn)
		{
			DoEffects();
		}

	}

	public class Gene_Generemover : Gene_Shapeshift_Remote
	{

		public override string RemoteActionName => "WVC_XaG_GeneRemover_Label".Translate();

		public override TaggedString RemoteActionDesc => "WVC_XaG_GeneRemover_Desc".Translate();

		public override void RemoteControl_Action(Dialog_GenesSettings genesSettings)
		{
			if (!GeneResourceUtility.CanDo_ShifterGeneticStuff(pawn))
			{
				return;
			}
			Find.WindowStack.Add(new Dialog_Generemover(this));
			genesSettings.Close();
		}

	}

	public class Gene_Traitshifter : Gene_Shapeshift_Remote
	{

		public override string RemoteActionName => "WVC_XaG_Traitshifter_Label".Translate();

		public override TaggedString RemoteActionDesc => "WVC_XaG_Traitshifter_Desc".Translate();

		public override void RemoteControl_Action(Dialog_GenesSettings genesSettings)
		{
			if (!GeneResourceUtility.CanDo_ShifterGeneticStuff(pawn))
            {
				return;
			}
			Find.WindowStack.Add(new Dialog_Traitshifter(this));
			genesSettings.Close();
		}

	}

	public class Gene_Skillshifter : Gene_Shapeshift_Remote
	{

		public override string RemoteActionName => "WVC_XaG_Skillshifter_Label".Translate();

		public override TaggedString RemoteActionDesc => "WVC_XaG_Skillshifter_Desc".Translate();

		public override void RemoteControl_Action(Dialog_GenesSettings genesSettings)
		{
			if (!GeneResourceUtility.CanDo_ShifterGeneticStuff(pawn))
			{
				return;
			}
			Find.WindowStack.Add(new Dialog_Skillshifter(this));
			genesSettings.Close();
		}

	}

}
