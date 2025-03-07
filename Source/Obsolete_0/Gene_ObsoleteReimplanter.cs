using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	[Obsolete]
	public class Gene_XenotypeImplanter : Gene
	{

		public XenotypeDef xenotypeDef = null;

		public override void PostAdd()
		{
			base.PostAdd();
			if (xenotypeDef == null)
			{
				xenotypeDef = pawn.genes.Xenotype;
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
				defaultLabel = def.LabelCap + ": " + (xenotypeDef != null ? xenotypeDef.LabelCap.ToString() : "ERR"),
				defaultDesc = "WVC_XaG_GeneShapeshifter_Desc".Translate(),
				icon = ContentFinder<Texture2D>.Get(def.iconPath),
				action = delegate
				{
					Find.WindowStack.Add(new Dialog_ReimplanterXenotype(this));
				}
			};
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Defs.Look(ref xenotypeDef, "xenotypeDef");
		}

	}

	[Obsolete]
	public class Gene_Reimplanter : Gene_XenotypeImplanter
	{



	}

}
