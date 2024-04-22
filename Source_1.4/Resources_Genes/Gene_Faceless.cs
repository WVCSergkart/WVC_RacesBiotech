using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Faceless : Gene
	{

		public GeneExtension_Giver Props => def?.GetModExtension<GeneExtension_Giver>();

		public bool drawGraphic = true;

		public override void PostAdd()
		{
			base.PostAdd();
			if (!Active)
			{
				return;
			}
			// drawGraphic = CheckHead();
		}

		public override void Tick()
		{
			base.Tick();
			if (!pawn.IsHashIntervalTick(12000))
			{
				return;
			}
			if (pawn.Map == null)
			{
				return;
			}
			bool active = CheckHead();
			if (drawGraphic != active)
			{
				drawGraphic = active;
				pawn.Drawer.renderer.graphics.SetAllGraphicsDirty();
			}
		}

		public bool CheckHead()
		{
			return HediffUtility.HeadTypeIsCorrect(pawn, Props.headTypeDefs);
		}

		// public override void ExposeData()
		// {
			// base.ExposeData();
			// if (Active)
			// {
				// drawGraphic = CheckHead();
			// }
		// }

	}
}
