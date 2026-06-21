using System;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_ChimeraHediff : Gene_ChimeraDependant, IGeneOverriddenBy, IGeneAddOrRemoveHediff
	{

		public GeneExtension_Giver Props => def.GetModExtension<GeneExtension_Giver>();

		public virtual Hediff ChimeraHediff
		{
			get
			{
				if (Props?.hediffDefName == null)
				{
					return null;
				}
				return pawn.health?.hediffSet?.GetFirstHediffOfDef(Props.hediffDefName);
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
				Log.Error("Error in Gene_ChimeraHediff in def: " + def.defName + ". Pawn: " + pawn.Name + ". Reason: " + arg);
			}
		}

		public virtual void Notify_OverriddenBy(Gene overriddenBy)
		{
			Local_RemoveHediff();
		}

		public virtual void Notify_Override()
		{
			Local_AddOrRemoveHediff();
		}

		// The old update mechanics are used here. For the task here it is more reliable.
		public override void TickInterval(int delta)
		{
			if (!pawn.IsHashIntervalTick(67150, delta))
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

	}

}
