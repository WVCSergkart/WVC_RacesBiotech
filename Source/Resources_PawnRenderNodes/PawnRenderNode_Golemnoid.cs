using RimWorld;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class PawnRenderNode_Golemnoid : PawnRenderNode_AnimalPart
	{

		public CompSpawnOnDeath_GetColor colorComp;

		public PawnRenderNode_Golemnoid(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree)
			: base(pawn, props, tree)
		{
		}

		public override Graphic GraphicFor(Pawn pawn)
		{
			if (pawn.TryGetComp(out colorComp))
			{
				Graphic graphic = pawn.ageTracker.CurKindLifeStage.bodyGraphicData.Graphic;
				return GraphicDatabase.Get<Graphic_Multi>(graphic.path, ShaderDatabase.Cutout, graphic.drawSize, ColorFor(pawn));
			}
			return base.GraphicFor(pawn);
		}

		public override Color ColorFor(Pawn pawn)
		{
			ThingDef rockDef = colorComp?.StoneChunk;
			if (rockDef == null)
			{
				return Color.white;
			}
			return rockDef.graphic.data.color;
		}

	}

	//public class PawnRenderNode_FakePawn_Head : PawnRenderNode
	//{

	//	private Pawn pawn;

	//	private Pawn cachedOverseer;
	//	public Pawn Overseer
	//	{
	//		get
	//		{
	//			if (cachedOverseer == null)
	//			{
	//				cachedOverseer = pawn.GetOverseer();
	//			}
	//			return cachedOverseer;
	//		}
	//	}

	//	public PawnRenderNode_FakePawn_Head(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree)
	//		: base(pawn, props, tree)
	//	{
	//		this.pawn = pawn;
	//	}

	//	public override Graphic GraphicFor(Pawn pawn)
	//	{
	//		return Overseer.Drawer.renderer.HeadGraphic;
	//	}

	//}

	//public class PawnRenderNodeWorker_FakePawn_Head : PawnRenderNodeWorker_Head
	//{

	//	public override bool CanDrawNow(PawnRenderNode node, PawnDrawParms parms)
	//	{
	//		return true;
	//	}

	//}

	//public class PawnRenderNode_FakePawn_Body : PawnRenderNode
	//{

	//	private Pawn pawn;

	//	private Pawn cachedOverseer;
	//	public Pawn Overseer
	//	{
	//		get
	//		{
	//			if (cachedOverseer == null)
	//			{
	//				cachedOverseer = pawn.GetOverseer();
	//			}
	//			return cachedOverseer;
	//		}
	//	}

	//	public PawnRenderNode_FakePawn_Body(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree)
	//		: base(pawn, props, tree)
	//	{
	//		this.pawn = pawn;
	//	}

	//	public override Graphic GraphicFor(Pawn pawn)
	//	{
	//		return Overseer.Drawer.renderer.BodyGraphic;
	//	}

	//}

	//public class PawnRenderNode_FromTarget : PawnRenderNode_AnimalPart
	//{

	//	public CompSpawnOnDeath_PawnKind colorComp;

	//	public PawnRenderNode_FromTarget(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree)
	//		: base(pawn, props, tree)
	//	{
	//	}

	//	public override Graphic GraphicFor(Pawn pawn)
	//	{
	//		if (pawn.TryGetComp(out colorComp))
	//		{
	//			Graphic graphic = pawn.ageTracker.CurKindLifeStage.bodyGraphicData.Graphic;
	//			return GraphicDatabase.Get<Graphic_Multi>(graphic.path, ShaderDatabase.Cutout, graphic.drawSize, ColorFor(pawn));
	//		}
	//		return base.GraphicFor(pawn);
	//	}

	//	public override Color ColorFor(Pawn pawn)
	//	{
	//		PawnKindDef rockDef = colorComp?.PawnKind;
	//		if (rockDef == null)
	//		{
	//			return Color.white;
	//		}
	//		return rockDef.graphic.data.color;
	//	}

	//}


}
