using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
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


}
