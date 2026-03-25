using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_CustomHair : XaG_Gene, IGeneCustomGraphic, IGeneOverriddenBy
	{

		public virtual Color CurrentColor
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

		public virtual int StyleId => -1;

		public virtual Color? DefaultColor => Color.white;
		public virtual List<GeneralHolder> ColorHolder => new();

		private StyleGeneDef styleGeneDef;
		private Color color;

		//public virtual bool IsStylable => true;

		public virtual StyleGeneDef StyleGeneDef
		{
			get
			{
				return styleGeneDef;
			}
			set
			{
				styleGeneDef = value;
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
			Scribe_Values.Look(ref color, "color");
			Scribe_Defs.Look(ref styleGeneDef, "currentTextID");
		}

		public virtual void DoAction()
		{
			//Find.WindowStack.Add(new Dialog_ChangeGraphic_Simple(this));
			Find.WindowStack.Add(new Dialog_StylingExtra(pawn, this, true, false, false));
		}

		//public virtual void SetColor(Color color, bool visible)
		//{

		//}

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

		private GeneExtension_Giver cachedGeneExtension;
		public GeneExtension_Giver Props
		{
			get
			{
				if (cachedGeneExtension == null)
				{
					cachedGeneExtension = def.GetModExtension<GeneExtension_Giver>();
				}
				return cachedGeneExtension;
			}
		}

		public override Color? DefaultColor
		{
			get
			{
				if (Props.holofaces.Where((GeneralHolder x) => x.visible).ToList().TryRandomElement(out GeneralHolder countWithChance))
				{
					return countWithChance.color;
				}
				return null;
			}
		}

		public override List<GeneralHolder> ColorHolder => Props.holofaces;

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

		public override void PostAdd()
		{
			base.PostAdd();
			if (Props != null && Props.holofaces.Where((GeneralHolder x) => x.visible).ToList().TryRandomElement(out GeneralHolder countWithChance))
			{
				SetColor(countWithChance.color);
			}
		}

		public void SetColor(Color color)
		{
			this.CurrentColor = color;
			pawn?.Drawer?.renderer?.SetAllGraphicsDirty();
		}

	}

	public class Gene_CenterFrill : Gene_CustomHair
	{

		public override Color CurrentColor
		{
			get
			{
				return pawn.story.SkinColor;
			}
			set
			{

			}
		}

		public override int StyleId => 1000011;

	}

}
