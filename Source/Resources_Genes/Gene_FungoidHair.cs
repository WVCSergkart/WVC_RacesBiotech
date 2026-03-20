using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_CustomHair : Gene, IGeneCustomGraphic, IGeneOverriddenBy
	{

		public virtual Color CurrentColor => Color.white;
		public virtual Color? DefaultColor => Color.white;
		public virtual List<GeneralHolder> ColorHolder => new();

		public StyleGeneDef currentTextID;

		public virtual StyleGeneDef CurrentTextID
		{
			get
			{
				return currentTextID;
			}
			set
			{
				currentTextID = value;
				//int count = def.RenderNodeProperties.First((node) => node.nodeClass.SameOrSubclassOf<PawnRenderNode_CustomHair>()).texPaths.Count;
				//if (currentTextID > count)
				//{
				//	currentTextID = count;
				//}
				//Log.Error(currentTextID.ToString());
				pawn?.Drawer?.renderer?.SetAllGraphicsDirty();
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Defs.Look(ref currentTextID, "currentTextID");
		}

		public virtual void DoAction()
		{
			//Find.WindowStack.Add(new Dialog_ChangeGraphic_Simple(this));
		}

		public virtual void SetColor(Color color, bool visible)
		{

		}

		//================RECACHE===================
		//================RECACHE===================
		//================RECACHE===================

		public override void PostAdd()
		{
			base.PostAdd();
			ResetCache();
		}

		public void Notify_OverriddenBy(Gene overriddenBy)
		{
			ResetCache();
		}

		public void Notify_Override()
		{
			ResetCache();
		}

		private static void ResetCache()
		{
			CompStylingStation.cachedPawns = null;
		}

		public override void PostRemove()
		{
			base.PostRemove();
			ResetCache();
		}

		//================RECACHE===================
		//================RECACHE===================
		//================RECACHE===================

	}

	public class Gene_FungoidHair : Gene_CustomHair
	{

		public GeneExtension_Giver Props => def?.GetModExtension<GeneExtension_Giver>();

		public override Color CurrentColor => color;
		public override Color? DefaultColor => color;
		public override List<GeneralHolder> ColorHolder => Props.holofaces;

		public Color color;

		//private static List<FungiHairDef> cachedFungiHairs;
		//public List<FungiHairDef> FungiHairs
		//{
		//	get
		//	{
		//		if (cachedFungiHairs == null)
		//		{
		//			List<FungiHairDef> list = new();
		//			List<string> textPaths = def.RenderNodeProperties.First((node) => node.nodeClass.SameOrSubclassOf<PawnRenderNode_CustomHair>()).texPaths;
		//			for (int i = 0; i < textPaths.Count; i++)
		//			{
		//				FungiHairDef newDef = new FungiHairDef();
		//				newDef.defName = "FungiHair_" + i.ToString();
		//				newDef.textId = i;
		//				list.Add(newDef);
		//			}
		//			//foreach (string textPath in def.RenderNodeProperties.First((node) => node.nodeClass.SameOrSubclassOf<PawnRenderNode_CustomHair>()).texPaths)
		//			//{
		//			//	FungiHairDef newDef = new FungiHairDef();
		//			//	newDef.defName = currentId.ToString();
		//			//	currentId++;
		//			//}
		//			cachedFungiHairs = list;
		//		}
		//		return cachedFungiHairs;
		//	}
		//}

		public override void PostAdd()
		{
			base.PostAdd();
			if (Props != null && Props.holofaces.Where((GeneralHolder x) => x.visible).ToList().TryRandomElement(out GeneralHolder countWithChance))
			{
				SetColor(countWithChance.color, true);
			}
		}

		public override void SetColor(Color color, bool visible)
		{
			this.color = color;
			pawn?.Drawer?.renderer?.SetAllGraphicsDirty();
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref color, "color");
		}

		public override void DoAction()
		{
			//List<FloatMenuOption> list = new();
			//list.Add(new FloatMenuOption("WVC_Color".Translate(), delegate
			//{
			//	Find.WindowStack.Add(new Dialog_ChangeGeneColor(this, false));
			//}));
			//list.Add(new FloatMenuOption("Hair".Translate().CapitalizeFirst(), delegate
			//{
			//	base.DoAction();
			//}));
			//Find.WindowStack.Add(new FloatMenu(list));
			Find.WindowStack.Add(new Dialog_StylingFungiHairGene(pawn, this, true));
		}

	}

}
