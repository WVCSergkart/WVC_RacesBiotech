using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	[Obsolete]
	public class Gene_Faceless : Gene
	{

		public GeneExtension_Giver Props => def?.GetModExtension<GeneExtension_Giver>();

		public bool drawGraphic = true;

		// public override void PostAdd()
		// {
		// base.PostAdd();
		// if (!Active)
		// {
		// return;
		// }
		// drawGraphic = CheckHead();
		// }

		// public override void Tick()
		// {
		// base.Tick();
		// if (!pawn.IsHashIntervalTick(19834))
		// {
		// return;
		// }
		// if (pawn.Map == null)
		// {
		// return;
		// }
		// bool active = CheckHead();
		// if (drawGraphic != active)
		// {
		// drawGraphic = active;
		// pawn.Drawer.renderer.SetAllGraphicsDirty();
		// }
		// }

		// public bool CheckHead()
		// {
		// return HediffUtility.HeadTypeIsCorrect(pawn, Props.headTypeDefs);
		// }

		// public override void ExposeData()
		// {
		// base.ExposeData();
		// if (Active)
		// {
		// drawGraphic = CheckHead();
		// }
		// }

	}
	public class Gene_Holoface : Gene
	{

		public GeneExtension_Giver Props => def?.GetModExtension<GeneExtension_Giver>();

		public Color color = Color.white;
		public bool visible = true;

		public override void PostAdd()
        {
            base.PostAdd();
			if (Props.holofaces.Where((XaG_CountWithChance x) => x.visible).ToList().TryRandomElement(out XaG_CountWithChance countWithChance))
			{
				color = countWithChance.color;
				color.a = 0.6f;
				visible = countWithChance.visible;
				pawn?.Drawer?.renderer?.SetAllGraphicsDirty();
			}
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (XaG_GeneUtility.SelectorDraftedActiveFactionMap(pawn, this))
			{
				yield break;
			}
			yield return new Command_Action
			{
				defaultLabel = "WVC_XaG_ColorableEyesLabel".Translate(),
				defaultDesc = "WVC_XaG_ColorableEyesDesc".Translate(),
				icon = ContentFinder<Texture2D>.Get(def.iconPath),
				defaultIconColor = color,
				action = delegate
				{
					List<FloatMenuOption> list = new();
					List<XaG_CountWithChance> list2 = Props.holofaces;
					for (int i = 0; i < list2.Count; i++)
					{
						XaG_CountWithChance face = list2[i];
						list.Add(new FloatMenuOption(face.label, delegate
						{
							color = face.color;
							color.a = 0.6f;
							visible = face.visible;
							pawn?.Drawer?.renderer?.SetAllGraphicsDirty();
						}, ContentFinder<Texture2D>.Get(def.iconPath), face.color));
					}
					if (!list.Any())
					{
						list.Add(new FloatMenuOption("WVC_XaG_NoSuitableTargets".Translate(), null));
					}
					Find.WindowStack.Add(new FloatMenu(list));
				}
			};
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref color, "color");
			Scribe_Values.Look(ref visible, "visible", defaultValue: true);
		}

	}

}
