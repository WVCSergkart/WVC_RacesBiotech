using System;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_XenogenesEditor_Mainframe : Gene_XenogenesEditor_General
	{

		public XaG_GameComponent GameComponent => StaticCollectionsClass.GameComponent;

		public override bool Active
		{
			get
			{
				if (MechanoidsUtility.CerebrexCoreDefeated || pawn?.mechanitor == null)
				{
					return false;
				}
				return base.Active;
			}
		}

		public override List<GeneDef> CollectedGenes => GameComponent.UnlockedGeneDefs;

		public override int ArchiteLimit => GameComponent.UnlcokedXenotypes.Count;
		public override int ComplexityLimit => pawn.mechanitor.TotalBandwidth - pawn.mechanitor.UsedBandwidth;
		public override IntRange ReqMetRange => new(-5, 5);
		public override bool ReqCooldown => true;

		private Pawn mainframeTarget;
		public override Pawn Pawn
		{
			get
			{
				if (mainframeTarget != null)
				{
					return mainframeTarget;
				}
				return base.Pawn;
			}
		}

		public void SetTarget(Pawn newTarget)
		{
			mainframeTarget = newTarget != pawn ? newTarget : null;
		}

		//public override void ExposeData()
		//{
		//	base.ExposeData();
		//	Scribe_References.Look(ref mainframeTarget, "target");
		//}

	}

}
