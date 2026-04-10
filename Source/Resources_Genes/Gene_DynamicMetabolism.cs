using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_DynamicMetabolism : XaG_Gene, IGeneOverriddenBy, IGeneMetabolism
	{

		public override void PostAdd()
		{
			base.PostAdd();
			UpdateMetabolism();
		}

		public override void PostRemove()
		{
			base.PostRemove();
			UpdateMetabolism();
		}

		public void Notify_OverriddenBy(Gene overriddenBy)
		{
			UpdateMetabolism();
		}

		public void Notify_Override()
		{
			UpdateMetabolism();
		}

		public void UpdateMetabolism()
		{
			HediffUtility.TryAddOrUpdMetabolism(pawn, this);
		}

	}

}
