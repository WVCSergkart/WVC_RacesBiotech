using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class Gene_SelfOverrider : Gene, IGeneRemoteControl, IGeneOverridden, IGeneMetabolism
	{

		private int lastTick = -1;

		private bool? metHediifDisabled;
		private HediffDef metHediffDef;
        public HediffDef MetHediffDef
        {
            get
            {
				if (!metHediifDisabled.HasValue)
                {
					metHediifDisabled = (metHediffDef = def?.GetModExtension<GeneExtension_Giver>()?.metHediffDef) != null;
				}
				if (!metHediifDisabled.Value)
				{
					return null;
				}
				return metHediffDef;
            }
        }

        public virtual string RemoteActionName
        {
            get
			{
                int tick = lastTick - Find.TickManager.TicksGame;
                if (tick > 0)
				{
					return tick.ToStringTicksToPeriod().Colorize(ColoredText.DateTimeColor);
				}
				return XaG_UiUtility.OnOrOff(!Overridden);
            }
        }

        public virtual string RemoteActionDesc => "WVC_XaG_SelfOverrideDesc".Translate();

		public virtual void RemoteControl_Action(Dialog_GenesSettings genesSettings)
		{
			if (base.overriddenByGene != null && base.overriddenByGene != this || lastTick > Find.TickManager.TicksGame)
			{
				SoundDefOf.ClickReject.PlayOneShotOnCamera();
				return;
			}
			cycleTry = 0;
			lastTick = Find.TickManager.TicksGame + 120;
			if (Overridden)
			{
				overrided = false;
				OverrideBy(null);
			}
			else
			{
				overrided = true;
				OverrideBy(this);
				UpdateMetabolism();
			}
			MiscUtility.Notify_DebugPawn(pawn);
		}

		public virtual bool RemoteControl_Hide => false;

		public bool overrided = false;
		//public bool OverridedBeforeSave => overrided;

		public virtual void Notify_OverriddenBy(Gene overriddenBy)
		{
			cycleTry++;
			if (cycleTry > 1000)
			{
				return;
			}
			if (overriddenBy != this)
			{
				overrided = false;
			}
			else
			{
				XaG_GeneUtility.Notify_GenesConflicts(pawn, def);
			}
		}

		private int cycleTry = 0;

		public virtual void Notify_Override()
		{
			cycleTry++;
			if (cycleTry > 1000)
			{
				return;
			}
			if (overrided)
			{
				OverrideBy(this);
			}
			else
			{
				XaG_GeneUtility.Notify_GenesConflicts(pawn, def, this);
			}
			UpdateMetabolism();
		}

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

		public override void PostAdd()
		{
			base.PostAdd();
			UpdateMetabolism();
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

		public override void Tick()
		{

		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref overrided, "overrided", defaultValue: false);
		}

		public void UpdateMetabolism()
		{
			HediffUtility.TryAddOrUpdMetabolism(MetHediffDef, pawn, this);
		}

	}

    public class Gene_SelfOverrider_Deathrest : Gene_SelfOverrider
	{

		private int deathrestCapacity = 0;
		private float deathrestNeed = 1f;
		private bool deathrestAdded = false;
		private bool firtsAdded = false;

		public override void RemoteControl_Action(Dialog_GenesSettings genesSettings)
		{
			firtsAdded = false;
			base.RemoteControl_Action(genesSettings);
		}

		public void Notify_GeneDeathrest()
		{
			//if (!deathrestAdded)
			//{
			//	return;
			//}
			foreach (Gene gene in pawn.genes.GenesListForReading)
            {
				if (gene is Gene_Deathrest deathrest)
				{
					//Log.Error("Saved capacity: " + deathrestCapacity);
					//Log.Error("Target capacity: " + (deathrestCapacity - deathrest.CurrentCapacity));
					deathrest.OffsetCapacity(Mathf.Clamp(deathrestCapacity - deathrest.DeathrestCapacity, 0, 999), false);
					Need_Deathrest deathrestNeed1 = deathrest.DeathrestNeed;
					if (deathrestNeed1 != null)
					{
                        deathrestNeed1.CurLevel = deathrestNeed;
					}
				}
            }
		}

		private bool? cachedIsXenogene;
        public bool IsXenogene
        {
            get
            {
				if (!cachedIsXenogene.HasValue)
                {
					cachedIsXenogene = pawn.genes.IsXenogene(this);
				}
                return cachedIsXenogene.Value;
            }
        }

		//private Gene_Deathrest gene_Deathrest;

		public void UpdGeneDeathrest()
		{
			Gene_Deathrest gene_Deathrest = pawn.genes.GetFirstGeneOfType<Gene_Deathrest>();
			if (Overridden)
			{
				if (gene_Deathrest != null)
				{
					//Log.Error("Capacity: " + gene_Deathrest.DeathrestCapacity);
					deathrestCapacity = gene_Deathrest.DeathrestCapacity;
					deathrestNeed = gene_Deathrest.DeathrestNeed.CurLevel;
					//Log.Error("Saved capacity: " + deathrestCapacity);
				}
				RemoveDeathrestGene();
			}
            else
			{
				if (gene_Deathrest == null)
				{
					pawn.genes.AddGene(WVC_GenesDefOf.Deathrest, IsXenogene);
					deathrestAdded = true;
				}
				Notify_GeneDeathrest();
			}
        }


		public override void PostMake()
		{
			base.PostMake();
			firtsAdded = true;
		}

		public override void PostAdd()
		{
			base.PostAdd();
            if (!firtsAdded)
            {
                UpdGeneDeathrest();
            }
			else
            {
				overrided = true;
				OverrideBy(this);
            }
        }

        public override void Notify_OverriddenBy(Gene overriddenBy)
		{
			base.Notify_OverriddenBy(overriddenBy);
			if (!firtsAdded)
			{
				UpdGeneDeathrest();
			}
		}

		public override void Notify_Override()
		{
			base.Notify_Override();
			if (firtsAdded)
			{
				OverrideBy(this);
			}
			else if(!overrided)
			{
				UpdGeneDeathrest();
			}
		}

		public override void PostRemove()
        {
            base.PostRemove();
			if (!firtsAdded)
			{
				RemoveDeathrestGene();
			}
        }

        private void RemoveDeathrestGene()
        {
			if (!deathrestAdded)
            {
				return;
            }
            foreach (Gene gene in pawn.genes.GenesListForReading.ToList())
            {
                if (gene is Gene_Deathrest deathrest)
                {
                    pawn.genes.RemoveGene(deathrest);
                }
            }
        }

        public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref deathrestCapacity, "deathrestCapacity", defaultValue: 0);
			Scribe_Values.Look(ref deathrestAdded, "deathrestAdded", defaultValue: false);
			Scribe_Values.Look(ref deathrestNeed, "deathrestNeed", defaultValue: 1);
		}

	}

    public class Gene_SelfOverrider_Healing : Gene_SelfOverrider
	{

		private bool? regenerateEyes;
		public bool RegenerateEyes
		{
			get
			{
				if (!regenerateEyes.HasValue)
				{
					regenerateEyes = HealingUtility.ShouldRegenerateEyes(pawn);
				}
				return regenerateEyes.Value;
			}
		}

		public override void Tick()
        {
            base.Tick();
			if (!pawn.IsHashIntervalTick(713))
			{
				return;
			}
			HealingUtility.Regeneration(pawn, 100, WVC_Biotech.settings.totalHealingIgnoreScarification, 676, RegenerateEyes);
		}

	}

	public class Gene_SelfOverrider_Stomach : Gene_SelfOverrider, IGeneNotifyGenesChanged
	{

		private float? cachedOffset;
		public float Offset
		{
			get
			{
				if (!cachedOffset.HasValue)
				{
					cachedOffset = Gene_HungerlessStomach.GetFoodOffset(pawn);
				}
				return cachedOffset.Value;
			}
		}

		public void Notify_GenesChanged(Gene changedGene)
		{
			cachedOffset = null;
		}

		public override void Tick()
		{
			base.Tick();
			if (!pawn.IsHashIntervalTick(3101))
			{
				return;
			}
			GeneResourceUtility.OffsetNeedFood(pawn, Offset);
		}

		public override void Notify_IngestedThing(Thing thing, int numTaken)
		{
			if (!Active)
            {
				return;
            }
			MiscUtility.TryAddFoodPoisoningHediff(pawn, thing);
		}

	}

}
