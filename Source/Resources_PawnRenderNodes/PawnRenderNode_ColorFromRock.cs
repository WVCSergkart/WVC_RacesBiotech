using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class PawnRenderNode_ColorFromGetColorComp : PawnRenderNode
	{

		public CompSpawnOnDeath_GetColor colorComp;

		public PawnRenderNode_ColorFromGetColorComp(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree)
			: base(pawn, props, tree)
		{
		}

		public override Graphic GraphicFor(Pawn pawn)
		{
			string body = TexPathFor(pawn);
			if (body.NullOrEmpty())
			{
				return null;
			}
			return GraphicDatabase.Get<Graphic_Multi>(body, DefaultShader, Vector2.one, ColorFor(pawn));
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

		protected override string TexPathFor(Pawn pawn)
		{
			return pawn?.ageTracker?.CurKindLifeStage?.bodyGraphicData?.Graphic?.path;
		}

	}

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
