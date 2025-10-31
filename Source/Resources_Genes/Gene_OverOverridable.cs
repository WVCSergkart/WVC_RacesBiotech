using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_OverOverridable : Gene, IGeneOverridden, IGeneOverOverridable
	{

		public override void TickInterval(int delta)
		{

		}

		private int overrideTries = 0;

		public virtual void Notify_OverriddenBy(Gene overriddenBy)
		{
			if (!WVC_Biotech.settings.enable_OverOverridableGenesMechanic)
			{
				return;
			}
			if (overriddenBy is not IGeneOverOverridable && overrideTries < 100 && overriddenBy.def.ConflictsWith(def))
			{
				this.OverrideBy(null);
				overriddenBy.OverrideBy(this);
			}
			overrideTries++;
		}

		public virtual void Notify_Override()
		{

		}

	}

}
