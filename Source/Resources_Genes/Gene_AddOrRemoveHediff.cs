using System;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_AddOrRemoveHediff : Gene, IGeneOverridden
	{

		// public HediffDef HediffDefName => def.GetModExtension<GeneExtension_Giver>().hediffDefName;

		//public GeneExtension_Giver Props => def.GetModExtension<GeneExtension_Giver>();

		private GeneExtension_Giver cachedGeneExtension;
		public GeneExtension_Giver Props
		{
			get
			{
				if (cachedGeneExtension == null)
				{
					cachedGeneExtension = def.GetModExtension<GeneExtension_Giver>();
				}
				return cachedGeneExtension;
			}
		}

		public override void PostAdd()
		{
			base.PostAdd();
			Local_AddOrRemoveHediff();
		}

		public virtual void Local_AddOrRemoveHediff()
		{
			try
			{
				HediffUtility.TryAddOrRemoveHediff(Props?.hediffDefName, pawn, this, Props?.bodyparts);
			}
			catch (Exception arg)
			{
				Log.Error("Error in Gene_AddOrRemoveHediff in def: " + def.defName + ". Pawn: " + pawn.Name + ". Reason: " + arg);
			}
		}

		public virtual void Notify_OverriddenBy(Gene overriddenBy)
		{
			//if (overriddenBy != null)
			//{
			//	Log.Error("Override gene: " + overriddenBy.def.defName);
			//}
			RemoveHediff();
		}

		public virtual void Notify_Override()
        {
			//Log.Error("Override with null gene");
			Local_AddOrRemoveHediff();
		}

		public override void TickInterval(int delta)
		{
			//base.Tick();
			if (!pawn.IsHashIntervalTick(67200, delta))
			{
				return;
			}
			Local_AddOrRemoveHediff();
		}

		public override void PostRemove()
        {
            base.PostRemove();
            RemoveHediff();
        }

        public virtual void RemoveHediff()
        {
            HediffUtility.TryRemoveHediff(Props.hediffDefName, pawn);
        }

        public override IEnumerable<Gizmo> GetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: Add Or Remove Hediff",
					action = delegate
					{
						if (Active)
						{
							Local_AddOrRemoveHediff();
						}
					}
				};
			}
		}

	}

	// public class Gene_HediffGiver : Gene
	// {

		// public List<HediffDef> HediffDefs => def.GetModExtension<GeneExtension_Giver>().hediffDefs;
		// public List<BodyPartDef> Bodyparts => def.GetModExtension<GeneExtension_Giver>().bodyparts;

		// public override void PostAdd()
		// {
			// base.PostAdd();
			// foreach (HediffDef hediffDef in HediffDefs)
			// {
				// if (!pawn.health.hediffSet.HasHediff(hediffDef))
				// {
					// Gene_PermanentHediff.BodyPartsGiver(Bodyparts, pawn, hediffDef);
				// }
			// }
		// }

	// }

	//[Obsolete]
	//public class Gene_PermanentHediff : Gene
	//{

	//	public GeneExtension_Giver Props => def.GetModExtension<GeneExtension_Giver>();

	//	public override void PostAdd()
	//	{
	//		base.PostAdd();
	//		if (!pawn.health.hediffSet.HasHediff(Props.hediffDefName))
	//		{
	//			HediffUtility.BodyPartsGiver(Props.bodyparts, pawn, Props.hediffDefName, def);
	//		}
	//	}

	//}

	public class Gene_GenerateHediffWithRandomSeverity : Gene, IGeneOverridden
	{

		private HediffDef cachedHediffDef;
        public HediffDef HediffDef
        {
            get
            {
				if (cachedHediffDef == null)
                {
					cachedHediffDef = def.GetModExtension<GeneExtension_Giver>().hediffDefName;
				}
                return cachedHediffDef;
            }
        }

        public override void PostAdd()
		{
			base.PostAdd();
			AddOrRemoveHediff(HediffDef, pawn, this);
		}

		public void Notify_OverriddenBy(Gene overriddenBy)
		{
			AddOrRemoveHediff(HediffDef, pawn, this);
		}

		public void Notify_Override()
		{
			AddOrRemoveHediff(HediffDef, pawn, this);
		}

		public override void TickInterval(int delta)
		{
			if (!pawn.IsHashIntervalTick(67200, delta))
			{
				return;
			}
			AddOrRemoveHediff(HediffDef, pawn, this);
		}

		public void AddOrRemoveHediff(HediffDef hediffDef, Pawn pawn, Gene gene)
		{
			HediffUtility.TryAddOrRemoveHediff(hediffDef, pawn, gene, randomizeSeverity: true);
		}

		public override void PostRemove()
		{
			base.PostRemove();
			HediffUtility.TryRemoveHediff(HediffDef, pawn);
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: Add Or Remove Hediff",
					action = delegate
					{
						AddOrRemoveHediff(HediffDef, pawn, this);
					}
				};
			}
		}

	}

	public class Gene_ResurgentHediff : Gene_ResurgentDependent, IGeneOverridden
	{

		//public HediffDef HediffDef => def.GetModExtension<GeneExtension_Giver>().hediffDefName;

		private HediffDef cachedHediffDef;
		public HediffDef HediffDef
		{
			get
			{
				if (cachedHediffDef == null)
				{
					cachedHediffDef = def.GetModExtension<GeneExtension_Giver>().hediffDefName;
				}
				return cachedHediffDef;
			}
		}

		public override void PostAdd()
		{
			base.PostAdd();
			AddOrRemoveHediff();
		}

		public void Notify_OverriddenBy(Gene overriddenBy)
		{
			RemoveHediff();
		}

		public void Notify_Override()
		{
			AddOrRemoveHediff();
		}

		public override void TickInterval(int delta)
		{
			//base.TickInterval(delta);
			if (!pawn.IsHashIntervalTick(67200, delta))
			{
				return;
			}
			AddOrRemoveHediff();
		}

		public override void PostRemove()
		{
			base.PostRemove();
			RemoveHediff();
		}

		public void AddOrRemoveHediff()
		{
			if (Active)
			{
				Gene_Resurgent gene_Resurgent = pawn.genes?.GetFirstGeneOfType<Gene_Resurgent>();
				if (gene_Resurgent != null)
				{
					if (gene_Resurgent.Value >= def.resourceLossPerDay)
					{
						if (!pawn.health.hediffSet.HasHediff(HediffDef))
						{
							// pawn.health.AddHediff(HediffDefName);
							Hediff hediff = HediffMaker.MakeHediff(HediffDef, pawn);
							HediffComp_GeneHediff hediff_GeneCheck = hediff.TryGetComp<HediffComp_GeneHediff>();
							if (hediff_GeneCheck != null)
							{
								hediff_GeneCheck.geneDef = this.def;
							}
						}
					}
					else
					{
						RemoveHediff();
					}
				}
			}
			else
			{
				RemoveHediff();
			}
		}

		public void RemoveHediff()
		{
			if (pawn.health.hediffSet.HasHediff(HediffDef))
			{
				Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDef);
				if (firstHediffOfDef != null)
				{
					pawn.health.RemoveHediff(firstHediffOfDef);
				}
			}
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: Add Or Remove Hediff",
					action = delegate
					{
						AddOrRemoveHediff();
					}
				};
			}
		}

	}

	public class Gene_BloodfeedHediffGiver : Gene, IGeneBloodfeeder
	{

		public GeneExtension_Giver Props => def?.GetModExtension<GeneExtension_Giver>();

		public void Notify_Bloodfeed(Pawn victim)
		{
			if (Props == null || victim == null)
			{
				return;
			}
			Hediff hediff = HediffMaker.MakeHediff(Props.hediffDefName, pawn);
			HediffComp_Disappears hediffComp_Disappears = hediff.TryGetComp<HediffComp_Disappears>();
			if (hediffComp_Disappears != null)
			{
				hediffComp_Disappears.ticksToDisappear = Props.ticksToDisappear;
			}
			pawn.health.AddHediff(hediff);
		}

	}

}
