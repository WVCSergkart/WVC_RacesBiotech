using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_CustomHair : Gene, IGeneCustomGraphic
	{

		public virtual Color CurrentColor => Color.white;
		public virtual Color? DefaultColor => Color.white;
		public virtual List<GeneralHolder> ColorHolder => new();

		public int currentTextID = 0;

		public virtual int CurrentTextID
		{
			get
			{
				return currentTextID;
			}
			set
			{
				currentTextID = value;
				int count = def.RenderNodeProperties.First((node) => node.nodeClass.SameOrSubclassOf<PawnRenderNode_CustomHair>()).texPaths.Count;
				if (currentTextID > count)
				{
					currentTextID = count;
				}
				//Log.Error(currentTextID.ToString());
				pawn?.Drawer?.renderer?.SetAllGraphicsDirty();
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref currentTextID, "currentTextID", 0);
		}

		public virtual void DoAction()
		{
			Find.WindowStack.Add(new Dialog_ChangeGraphic_Simple(this));
		}

		public virtual void SetColor(Color color, bool visible)
		{

		}

	}

	public class Gene_FungoidHair : Gene_CustomHair
	{

		public GeneExtension_Giver Props => def?.GetModExtension<GeneExtension_Giver>();

		public override Color CurrentColor => color;
		public override Color? DefaultColor => color;
		public override List<GeneralHolder> ColorHolder => Props.holofaces;

		public Color color;

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
			List<FloatMenuOption> list = new();
			list.Add(new FloatMenuOption("WVC_Color".Translate(), delegate
			{
				Find.WindowStack.Add(new Dialog_ChangeGeneColor(this, false));
			}));
			list.Add(new FloatMenuOption("Hair".Translate().CapitalizeFirst(), delegate
			{
				base.DoAction();
			}));
			Find.WindowStack.Add(new FloatMenu(list));
		}

	}

}
