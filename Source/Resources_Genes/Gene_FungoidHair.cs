using System.Linq;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{
    public class Gene_FungoidHair : Gene
	{

		public GeneExtension_Giver Props => def?.GetModExtension<GeneExtension_Giver>();

		public Color color;

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
			this.color = color;
			pawn?.Drawer?.renderer?.SetAllGraphicsDirty();
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref color, "color");
		}

	}

}
