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
            else if (Props != null && Props.holofaces.Where((GeneralHolder x) => x.visible).ToList().TryRandomElement(out GeneralHolder countWithChance))
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
                        ChangeColor();
                    }
                };
			}
		}

        public virtual void ChangeColor(bool closeOnAccept = true)
		{
			Find.WindowStack.Add(new Dialog_ChangeEyesColor(this, closeOnAccept));
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
				return XaG_UiUtility.GetRemoteControllerGizmo(pawn, remoteControllerCached, this);
			}
			return null;
		}

	}

}
