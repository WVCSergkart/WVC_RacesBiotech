using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class PawnRenderNode_ColorableEyes : PawnRenderNode_AttachmentHead
	{

		//public Gene_Holoface holoface;

		public PawnRenderNode_ColorableEyes(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree)
			: base(pawn, props, tree)
		{
			//if (gene is Gene_Holoface holoface)
			//{
			//	this.holoface = holoface;
			//}
		}

		public override Graphic GraphicFor(Pawn pawn)
		{
			if (gene is not Gene_Eyes holoface || holoface.visible != true)
			{
				return null;
			}
			return base.GraphicFor(pawn);
		}

		public override Color ColorFor(Pawn pawn)
		{
			if (gene is not Gene_Eyes holoface || holoface?.color == null)
			{
				return Color.white;
			}
			Color newColor = holoface.color;
			newColor.a = holoface.Alpha;
			return newColor;
		}

	}

	//public class PawnRenderNode_ColorableBody : PawnRenderNode
	//{

	//	public PawnRenderNode_ColorableBody(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree)
	//		: base(pawn, props, tree)
	//	{
	//	}

	//	public override Graphic GraphicFor(Pawn pawn)
	//	{
	//		if (gene is not Gene_Eyes holoface || holoface.visible != true)
	//		{
	//			return null;
	//		}
	//		return base.GraphicFor(pawn);
	//	}

	//	public override Color ColorFor(Pawn pawn)
	//	{
	//		if (gene is not Gene_Eyes holoface || holoface?.color == null)
	//		{
	//			return Color.white;
	//		}
	//		Color newColor = holoface.color;
	//		newColor.a = holoface.Alpha;
	//		return newColor;
	//	}

	//}

}
