using System;
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class BloodeaterModeDef : Def
	{

		public AbilityDef abilityDef;

		public Type workerClass = typeof(BloodeaterMode);

		public int displayOrder = 0;

		[Unsaved(false)]
		private BloodeaterMode workerInt;

		public BloodeaterMode Worker
		{
			get
			{
				if (workerInt == null)
				{
					workerInt = (BloodeaterMode)Activator.CreateInstance(workerClass);
					workerInt.def = this;
				}
				return workerInt;
			}
		}

		public bool CanUse(Pawn pawn)
		{
			if (abilityDef == null)
			{
				return false;
			}
			return Worker.CanUseAbility(pawn);
		}

	}

}
