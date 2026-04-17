using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using static HarmonyLib.Code;

namespace WVC_XenotypesAndGenes
{


	public class Gene_Bodyparts : XaG_Gene, IGeneCustomGraphic
	{

		public GeneExtension_Graphic Graphic => def?.GetModExtension<GeneExtension_Graphic>();

		public override void PostAdd()
		{
			base.PostAdd();
			SetRandomGraphic();
		}

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
				return Color.white;
			}
			set
			{

			}
		}

		//public virtual Color? DefaultColor => Color.white;

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
			}
		}

		public virtual List<Color> AllColors => new();

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Defs.Look(ref styleGeneDef, "styleGeneDef");
		}


	}

	[Obsolete]
	public class Gene_Mechaskin : Gene_Bodyparts
	{

		public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
		{
			if (!Active)
			{
				return;
			}
			HealingUtility.SetRottable(pawn);
		}

		public override void PostRemove()
		{
			base.PostRemove();
			HealingUtility.SetRottable(pawn, false);
		}

	}
}
