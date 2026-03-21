using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_CustomHair : Gene, IGeneCustomGraphic, IGeneOverriddenBy
	{

		public virtual Color CurrentColor
		{
			get
			{
				return Color.white;
			}
			set
			{

			}
		}

		public virtual int StyleId => -1;

		public virtual Color? DefaultColor => Color.white;
		public virtual List<GeneralHolder> ColorHolder => new();

		public StyleGeneDef currentTextID;

		//public virtual bool IsStylable => true;

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

		public virtual List<Color> AllColors => new();

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Defs.Look(ref currentTextID, "currentTextID");
		}

		public virtual void DoAction()
		{
			//Find.WindowStack.Add(new Dialog_ChangeGraphic_Simple(this));
			Find.WindowStack.Add(new Dialog_StylingExtra(pawn, this, true, false));
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

		public override Color CurrentColor
		{
			get
			{
				return color;
			}
			set
			{
				color = value;
			}
		}

		public override Color? DefaultColor => color;
		public override List<GeneralHolder> ColorHolder => Props.holofaces;

		public Color color;

		public override int StyleId => 1000001;

		public override List<Color> AllColors
		{
			get
			{
				List<Color>  allHairColors = new();
				foreach (GeneralHolder colorHolder in ColorHolder)
				{
					if (allHairColors.Contains(colorHolder.color))
					{
						continue;
					}
					allHairColors.Add(colorHolder.color);
				}
				foreach (ColorDef allDef in DefDatabase<ColorDef>.AllDefsListForReading)
				{
					Color color = allDef.color;
					if (allDef.displayInStylingStationUI && !allHairColors.Any((Color x) => x.WithinDiffThresholdFrom(color, 0.15f)))
					{
						allHairColors.Add(color);
					}
				}
				foreach (GeneDef allDef in DefDatabase<GeneDef>.AllDefsListForReading)
				{
					if (!allDef.hairColorOverride.HasValue)
					{
						continue;
					}
					Color color = allDef.hairColorOverride.Value;
					if (!allHairColors.Any((Color x) => x.WithinDiffThresholdFrom(color, 0.15f)))
					{
						allHairColors.Add(color);
					}
				}
				allHairColors.SortByColor((Color x) => x);
				return allHairColors;
			}
		}

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

		//public override void DoAction()
		//{
		//	//List<FloatMenuOption> list = new();
		//	//list.Add(new FloatMenuOption("WVC_Color".Translate(), delegate
		//	//{
		//	//	Find.WindowStack.Add(new Dialog_ChangeGeneColor(this, false));
		//	//}));
		//	//list.Add(new FloatMenuOption("Hair".Translate().CapitalizeFirst(), delegate
		//	//{
		//	//	base.DoAction();
		//	//}));
		//	//Find.WindowStack.Add(new FloatMenu(list));
		//	Find.WindowStack.Add(new Dialog_StylingFungiHairGene(pawn, this, true));
		//}

	}

}
