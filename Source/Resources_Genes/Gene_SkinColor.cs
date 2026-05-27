using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_SkinColor : Gene_RemoteController, IGeneCustomGraphic, IGeneOverriddenBy
	{

		public override string RemoteActionName => "WVC_Color".Translate();
		public override TaggedString RemoteActionDesc => "WVC_XaG_SkinColorDesc".Translate();

		public override void RemoteControl_Action(Dialog_GenesSettings genesSettings)
		{
			Find.WindowStack.Add(new Dialog_StylingExtra(pawn, this, true, false, null));
			genesSettings.Close();
		}

		public override void PostAdd()
		{
			base.PostAdd();
			//defaultSkinColor = pawn.story?.SkinColorBase;
			if (Active)
			{
				CurrentColor = AllColors.RandomElement();
			}
		}

		private static List<Color> cachedSkinColors;
		public List<Color> AllColors
		{
			get
			{
				if (cachedSkinColors == null)
				{
					cachedSkinColors = new List<Color>();
					foreach (GeneDef allDef in DefDatabase<GeneDef>.AllDefsListForReading)
					{
						Color? color = allDef.skinColorOverride ?? allDef.skinColorBase;
						if (color == null)
						{
							continue;
						}
						cachedSkinColors.Add(color.Value);
					}
					cachedSkinColors.SortByColor((Color x) => x);
				}
				return cachedSkinColors;
			}
		}

		public int StyleId => -1;

		public StyleGeneDef StyleGeneDef
		{
			get
			{
				return null;
			}
			set
			{

			}
		}

		public void Notify_OverriddenBy(Gene overriddenBy)
		{
			ResetBaseSkinColor();
		}

		private void ResetBaseSkinColor()
		{
			//pawn.story.SkinColorBase = defaultSkinColor ?? MainDefOf.Skin_SheerWhite.skinColorOverride.Value;
			EnsureCorrectSkinColorOverride();
			pawn?.Drawer?.renderer?.SetAllGraphicsDirty();
		}

		private void EnsureCorrectSkinColorOverride()
		{
			bool flag = false;
			Color? skinColorOverride = null;
			List<Gene> genesListForReading = pawn.genes.GenesListForReading;
			for (int i = 0; i < genesListForReading.Count; i++)
			{
				Gene gene = genesListForReading[i];
				if (gene.overriddenByGene == this)
				{
					if (gene.def.skinIsHairColor)
					{
						flag = true;
					}
					else if (gene.def.skinColorOverride.HasValue)
					{
						skinColorOverride = gene.def.skinColorOverride;
					}
					break;
				}
			}
			if (skinColorOverride == null)
			{
				return;
			}
			if (flag)
			{
				pawn.story.skinColorOverride = pawn.story.HairColor;
			}
			else
			{
				pawn.story.skinColorOverride = skinColorOverride;
			}
		}

		//private Color? defaultSkinColor;

		public void Notify_Override()
		{
			if (color.HasValue)
			{
				CurrentColor = color.Value;
			}
		}

		private Color? color;
		public Color CurrentColor
		{
			get
			{
				if (color == null)
				{
					color = pawn.story.SkinColor;
				}
				return color.Value;
			}
			set
			{
				color = value;
				//pawn.story.SkinColorBase = value;
				//if (pawn.story.SkinColorOverriden)
				//{
				//	foreach (Gene gene in pawn.genes.GenesListForReading)
				//	{
				//		if (gene.def.skinColorOverride != null)
				//		{
				//			gene.OverrideBy(this);
				//		}
				//	}
				//	pawn.story.skinColorOverride = null;
				//}
				pawn.story.skinColorOverride = value;
				pawn?.Drawer?.renderer?.SetAllGraphicsDirty();
			}
		}

		public override void PostRemove()
		{
			base.PostRemove();
			ResetBaseSkinColor();
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref color, "color");
			//Scribe_Values.Look(ref defaultSkinColor, "defaultSkinColor");
			//Scribe_Defs.Look(ref styleGeneDef, "styleGeneDef");
		}

	}

}
