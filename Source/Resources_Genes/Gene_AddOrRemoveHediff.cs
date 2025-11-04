using System;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_AddOrRemoveHediff : Gene, IGeneOverridden, IGeneAddOrRemoveHediff
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
			Local_RemoveHediff();
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
			Local_RemoveHediff();
		}

		public virtual void Local_RemoveHediff()
		{
			HediffUtility.TryRemoveHediff(Props.hediffDefName, pawn);
		}

		//public override IEnumerable<Gizmo> GetGizmos()
		//{
		//	if (DebugSettings.ShowDevGizmos)
		//	{
		//		yield return new Command_Action
		//		{
		//			defaultLabel = "DEV: Add Or Remove Hediff",
		//			action = delegate
		//			{
		//				if (Active)
		//				{
		//					Local_AddOrRemoveHediff();
		//				}
		//			}
		//		};
		//	}
		//}

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

	public class Gene_SeverityHediff : Gene, IGeneOverridden, IGeneAddOrRemoveHediff
	{

		private HediffDef cachedHediffDef;
		public HediffDef HediffDef
		{
			get
			{
				if (cachedHediffDef == null)
				{
					cachedHediffDef = def?.GetModExtension<GeneExtension_Giver>()?.hediffDefName;
				}
				return cachedHediffDef;
			}
		}

		private float? savedSeverity;
		public float GeneSeverity
		{
			get
			{
				if (savedSeverity == null)
				{
					UpdSeverity();
				}
				return savedSeverity.Value;
			}
		}

		private void UpdSeverity()
		{
			if (savedSeverity.HasValue)
			{
				return;
			}
			Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDef);
			if (hediff != null)
			{
				savedSeverity = hediff.Severity;
			}
			else
			{
				savedSeverity = new FloatRange(HediffDef.minSeverity, HediffDef.maxSeverity).RandomInRange;
			}
		}

		public override void PostAdd()
		{
			base.PostAdd();
			Local_AddOrRemoveHediff();
		}

		public void Notify_OverriddenBy(Gene overriddenBy)
		{
			Local_AddOrRemoveHediff();
		}

		public void Notify_Override()
		{
			Local_AddOrRemoveHediff();
		}

		public override void TickInterval(int delta)
		{
			if (!pawn.IsHashIntervalTick(67200, delta))
			{
				return;
			}
			Local_AddOrRemoveHediff();
		}

		public void Local_AddOrRemoveHediff()
		{
			try
			{
				UpdSeverity();
				HediffUtility.TryAddOrRemoveHediff(HediffDef, pawn, this, severity: GeneSeverity);
			}
			catch (Exception arg)
			{
				Log.Error("Error in Gene_SeverityHediff in def: " + def.defName + ". Pawn: " + pawn.Name + ". Reason: " + arg);
			}
		}

		public void Local_RemoveHediff()
		{
			UpdSeverity();
			HediffUtility.TryRemoveHediff(HediffDef, pawn);
		}

		public override void PostRemove()
		{
			base.PostRemove();
			Local_RemoveHediff();
		}

		//public override IEnumerable<Gizmo> GetGizmos()
		//{
		//	if (DebugSettings.ShowDevGizmos)
		//	{
		//		yield return new Command_Action
		//		{
		//			defaultLabel = "DEV: SeverityHediff",
		//			action = delegate
		//			{
		//				Local_AddOrRemoveHediff();
		//			}
		//		};
		//	}
		//}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref savedSeverity, "savedSeverity", null);
		}

	}

	[Obsolete]
	public class Gene_GenerateHediffWithRandomSeverity : Gene_SeverityHediff
	{

	}

	public class Gene_ResurgentHediff : Gene_ResurgentDependent, IGeneOverridden, IGeneAddOrRemoveHediff
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
			Local_AddOrRemoveHediff();
		}

		public void Notify_OverriddenBy(Gene overriddenBy)
		{
			Local_RemoveHediff();
		}

		public void Notify_Override()
		{
			Local_AddOrRemoveHediff();
		}

		public override void TickInterval(int delta)
		{
			//base.TickInterval(delta);
			if (!pawn.IsHashIntervalTick(67230, delta))
			{
				return;
			}
			Local_AddOrRemoveHediff();
		}

		public override void PostRemove()
		{
			base.PostRemove();
			Local_RemoveHediff();
		}

		public void Local_AddOrRemoveHediff()
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
						Local_RemoveHediff();
					}
				}
			}
			else
			{
				Local_RemoveHediff();
			}
		}

		public void Local_RemoveHediff()
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

		//public override IEnumerable<Gizmo> GetGizmos()
		//{
		//	if (DebugSettings.ShowDevGizmos)
		//	{
		//		yield return new Command_Action
		//		{
		//			defaultLabel = "DEV: Add Or Remove Hediff",
		//			action = delegate
		//			{
		//				AddOrRemoveHediff();
		//			}
		//		};
		//	}
		//}

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
