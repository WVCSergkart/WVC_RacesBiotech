using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class CompProperties_Corpse : CompProperties
	{

		public CompProperties_Corpse()
		{
			compClass = typeof(CompCorpse);
		}

	}

	public class CompCorpse : ThingComp
	{

		public Corpse Corpse => parent is Corpse corpse ? corpse : null;
		public Pawn InnerPawn => Corpse?.InnerPawn;

		private bool? shouldResurrect;
		public bool ShouldResurrect
		{
			get
			{
				if (shouldResurrect == null)
				{
					CompHumanlike compHumanlike = InnerPawn?.HumanComponent();
					if (compHumanlike == null)
					{
						shouldResurrect = false;
					}
					else
					{
						shouldResurrect = compHumanlike.ShouldResurrect;
					}
					if (shouldResurrect.Value)
					{
						resurrectionDelay = compHumanlike.ResurrectionDelay;
					}
				}
				return shouldResurrect.Value;
			}
		}

		private int resurrectionDelay = 0;

		public override string CompInspectStringExtra()
		{
			if (ShouldResurrect)
			{
				return "WVC_XaG_Gene_UndeadResurrectionCorpse_Info".Translate().Resolve() + ": " + (resurrectionDelay - Find.TickManager.TicksGame).ToStringTicksToPeriod().Colorize(ColoredText.DateTimeColor);
			}
			return null;
		}


		//public bool initialized;
		//public override void CompTickRare()
		//{
		//	if (initialized)
		//	{
		//		return;
		//	}
		//	initialized = true;
		//	CompRottable compRottable = Corpse.GetComp<CompRottable>();
		//	if (compRottable != null &&)
		//	{
		//		compRottable.disabled = true;
		//	}
		//}

	}

}
