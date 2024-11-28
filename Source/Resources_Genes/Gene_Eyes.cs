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

	}

	//InDev
	public class Gene_Eyeless : Gene
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

	public class Gene_Eyes : Gene
	{

		public GeneExtension_Giver Props => def?.GetModExtension<GeneExtension_Giver>();

		public Color color = Color.white;
		public bool visible = true;

		public Color? DefaultEyesColor => pawn.genes?.Xenotype?.GetModExtension<GeneExtension_Giver>()?.defaultColor;

		public virtual float Alpha => 1f;

		public override void PostAdd()
		{
			base.PostAdd();
			if (pawn.genes?.Xenotype?.GetModExtension<GeneExtension_Giver>()?.defaultColor != null)
            {
                SetColor(pawn.genes.Xenotype.GetModExtension<GeneExtension_Giver>().defaultColor, true);
            }
            else if (Props.holofaces.Where((XaG_CountWithChance x) => x.visible).ToList().TryRandomElement(out XaG_CountWithChance countWithChance))
			{
				SetColor(countWithChance.color, countWithChance.visible);
			}
		}

        public void SetColor(Color color, bool visible)
        {
            this.color = color;
            //color.a = alpha;
			this.visible = visible;
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
                        ChangeEyesColor();
                    }
                };
			}
		}

        public void ChangeEyesColor()
		{
			Find.WindowStack.Add(new Dialog_ChangeEyesColor(this));
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
			Scribe_Values.Look(ref visible, "visible", defaultValue: true);
		}

	}

	public class Gene_Holoface : Gene_Eyes, IGeneRemoteControl
	{
        public string RemoteActionName => "WVC_Color".Translate();

        public string RemoteActionDesc => "WVC_XaG_RemoteControlBasicDesc".Translate();

        public void RemoteControl()
		{
			ChangeEyesColor();
		}

        public override float Alpha => 0.8f;

		public bool Enabled
		{
			get
			{
				return enabled;
			}
			set
			{
				enabled = value;
			}
		}

		public override void PostRemove()
		{
			base.PostRemove();
			if (cachedRemoteControlGenes != null)
			{
				foreach (IGeneRemoteControl gene in cachedRemoteControlGenes)
				{
					gene.Enabled = true;
				}
			}
		}

		public void RecacheGenes()
		{
			if (cachedRemoteControlGenes != null)
            {
				foreach (IGeneRemoteControl gene in cachedRemoteControlGenes)
				{
					gene.Enabled = true;
				}
			}
			cachedRemoteControlGenes = new();
			foreach (Gene gene in pawn.genes.GenesListForReading)
			{
                if (gene is IGeneRemoteControl geneRemoteControl)
                {
                    cachedRemoteControlGenes.Add(geneRemoteControl);
					geneRemoteControl.Enabled = false;
				}
            }
			enabled = true;
		}

		public bool enabled = true;

		public void RemoteControl_Recache()
		{
			RecacheGenes();
		}

		[Unsaved(false)]
		private List<IGeneRemoteControl> cachedRemoteControlGenes;

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (!enabled)
			{
				yield break;
			}
			if (cachedRemoteControlGenes == null)
			{
				RecacheGenes();
			}
			if (XaG_GeneUtility.SelectorDraftedFactionMap(pawn))
			{
				yield break;
			}
			yield return new Command_Action
			{
				defaultLabel = "WVC_XaG_GenesSettings".Translate(),
				defaultDesc = "WVC_XaG_GenesSettingsDesc".Translate(),
				icon = XaG_UiUtility.GenesSettingsGizmo.Texture,
				action = delegate
				{
					Find.WindowStack.Add(new Dialog_GenesSettings(this, cachedRemoteControlGenes));
				}
			};
		}

	}

}
