using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Attachments : XaG_Gene, IGeneCustomGraphic
	{

		private GeneExtension_Graphic cachedGeneExtension;
		public GeneExtension_Graphic Graphic
		{
			get
			{
				if (cachedGeneExtension == null)
				{
					cachedGeneExtension = def.GetModExtension<GeneExtension_Graphic>();
				}
				return def?.GetModExtension<GeneExtension_Graphic>();
			}
		}

		public override void PostAdd()
		{
			base.PostAdd();
			SetRandomStyle();
		}

		public virtual void SetRandomStyle()
		{
			try
			{
				StyleGeneDef = StyleGeneDef.GetRandomDefForStyleGene(this);
			}
			catch (Exception arg)
			{
				Log.Warning("Failed set random style for gene: " + def.defName + ". Reason: " + arg.Message);
			}
		}

		private Color color = Color.white;
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

		public int StyleId => Graphic.styleId;

		public List<Color> AllColors
		{
			get
			{
				if (Graphic.colors != null)
				{
					return Graphic.colors;
				}
				return new();
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref color, "color", Color.white);
			Scribe_Defs.Look(ref styleGeneDef, "styleGeneDef");
		}

	}

}
