using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	[Obsolete]
	public class Gene_Faceless : XaG_Gene
	{

	}

	//InDev
	public class Gene_Eyeless : XaG_Gene
	{

		//public void Notify_OverriddenBy(Gene overriddenBy)
		//{
		//	foreach (Gene gene in pawn.genes.GenesListForReading)
		//	{
		//		if (gene.def.prerequisite == null || gene.def == def)
		//		{
		//			continue;
		//		}
		//		if (XaG_GeneUtility.GeneDefIsSubGeneOf(gene.def.prerequisite, def))
		//              {
		//			gene.OverrideBy(overriddenBy);
		//		}
		//	}
		//}

		//public void Notify_Override()
		//{
		//	foreach (Gene gene in pawn.genes.GenesListForReading)
		//	{
		//		if (gene.def.prerequisite == null || gene.def == def)
		//		{
		//			continue;
		//		}
		//		if (XaG_GeneUtility.GeneDefIsSubGeneOf(gene.def.prerequisite, def))
		//		{
		//			gene.OverrideBy(null);
		//		}
		//	}
		//}

	}

	public class Gene_Eyes : XaG_Gene, IGeneCustomGraphic
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
		public GeneExtension_Graphic Graphic => def?.GetModExtension<GeneExtension_Graphic>();

		private Color color = Color.white;
		//public bool visible = true;

		public Color CurrentColor
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

		public Color? DefaultColor => pawn.genes?.Xenotype?.GetModExtension<GeneExtension_Giver>()?.defaultColor;

		public List<GeneralHolder> ColorHolder => Props.holofaces;

		public virtual float Alpha => 1f;

		private StyleGeneDef styleGeneDef;
		public StyleGeneDef StyleGeneDef
		{
			get
			{
				return styleGeneDef;
			}
			set
			{
				styleGeneDef = value;
			}
		}

		//public bool IsStylable => false;

		private int? cachedStyleId;
		public int StyleId
		{
			get
			{
				if (cachedStyleId == null)
				{
					cachedStyleId = Graphic != null ? Graphic.styleId : -1;
				}
				return cachedStyleId.Value;
				//if (Graphic != null)
				//{
				//	return Graphic.styleId;
				//}
				//return -1;
			}
		}

		public List<Color> AllColors
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
					if (allHairColors.Contains(allDef.color))
					{
						continue;
					}
					allHairColors.Add(allDef.color);
				}
				allHairColors.SortByColor((Color x) => x);
				return allHairColors;
			}
		}

		public override void PostAdd()
		{
			base.PostAdd();
			if (pawn.genes?.Xenotype?.GetModExtension<GeneExtension_Giver>()?.defaultColor != null)
			{
				SetColor(pawn.genes.Xenotype.GetModExtension<GeneExtension_Giver>().defaultColor);
			}
			else if (Props != null && Props.holofaces.Where((GeneralHolder x) => x.visible).ToList().TryRandomElement(out GeneralHolder countWithChance))
			{
				SetColor(countWithChance.color);
			}
		}

		public void SetColor(Color color)
		{
			this.color = color;
			//color.a = alpha;
			//this.styleGeneDef = visible ? null :;
			pawn?.Drawer?.renderer?.SetAllGraphicsDirty();
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: EyesColor",
					action = delegate
					{
						ChangeColor();
					}
				};
			}
		}

		public virtual void ChangeColor()
		{
			//Find.WindowStack.Add(new Dialog_ChangeGeneColor(this, closeOnAccept));
			//Find.WindowStack.Add(new Dialog_StylingEyesGene(pawn, this, true));
			Find.WindowStack.Add(new Dialog_StylingExtra(pawn, this, true, false, false));
			//        List<FloatMenuOption> list = new();
			//        List<XaG_CountWithChance> list2 = Props.holofaces;
			//        for (int i = 0; i < list2.Count; i++)
			//        {
			//            XaG_CountWithChance face = list2[i];
			//            list.Add(new FloatMenuOption(face.label, delegate
			//            {
			//	SetColor(face.color, face.visible);
			//}, ContentFinder<Texture2D>.Get(def.iconPath), face.color));
			//        }
			//        if (!list.Any())
			//        {
			//            list.Add(new FloatMenuOption("WVC_XaG_NoSuitableTargets".Translate(), null));
			//        }
			//        Find.WindowStack.Add(new FloatMenu(list));
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref color, "color");
			Scribe_Defs.Look(ref styleGeneDef, "styleGeneDef");
		}

		public void DoAction()
		{
			ChangeColor();
		}

	}

	public class Gene_Holoface : Gene_Eyes, IGeneRemoteControl
	{
		public string RemoteActionName => "WVC_Color".Translate();

		public TaggedString RemoteActionDesc => "WVC_XaG_RemoteControlBasicDesc".Translate();

		public void RemoteControl_Action(Dialog_GenesSettings genesSettings)
		{
			ChangeColor();
			genesSettings.Close();
		}

		public override float Alpha => 0.8f;

		public bool RemoteControl_Hide => !Active;

		public bool RemoteControl_Enabled
		{
			get
			{
				return enabled;
			}
			set
			{
				enabled = value;
				remoteControllerCached = false;
			}
		}

		public override void PostRemove()
		{
			base.PostRemove();
			XaG_UiUtility.SetAllRemoteControllersTo(pawn);
		}

		public bool enabled = true;
		public bool remoteControllerCached = false;

		public void RemoteControl_Recache()
		{
			XaG_UiUtility.RecacheRemoteController(pawn, ref remoteControllerCached, ref enabled);
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (enabled)
			{
				yield return XaG_UiUtility.GetRemoteControllerGizmo(pawn, remoteControllerCached, this);
			}
		}

	}

}
