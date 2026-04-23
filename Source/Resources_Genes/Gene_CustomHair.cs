using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class Gene_CustomHair : XaG_Gene, IGeneCustomGraphic, IGeneOverriddenBy
	{

		public GeneExtension_Graphic Graphic => def?.GetModExtension<GeneExtension_Graphic>();

		private void SetRandomGraphic()
		{
			try
			{
				StyleGeneDef = StyleGeneDef.GetRandomDefForStyleGene(this);
			}
			catch
			{
				Log.Warning("Failed set graphic for def: " + def.defName);
			}
		}

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

		private int? cachedStyleId;
		public virtual int StyleId
		{
			get
			{
				if (cachedStyleId == null)
				{
					cachedStyleId = Graphic != null ? Graphic.styleId : -1;
				}
				return cachedStyleId.Value;
			}
		}
		public virtual List<GeneralHolder> ColorHolder => new();

		private StyleGeneDef styleGeneDef;
		private Color color;

		public virtual StyleGeneDef StyleGeneDef
		{
			get
			{
				return styleGeneDef;
			}
			set
			{
				styleGeneDef = value;
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
			Find.WindowStack.Add(new Dialog_StylingExtra(pawn, this, true, false, null));
		}

		//================RECACHE===================
		//================RECACHE===================
		//================RECACHE===================

		public override void PostAdd()
		{
			base.PostAdd();
			ResetCache();
			SetRandomGraphic();
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

}
