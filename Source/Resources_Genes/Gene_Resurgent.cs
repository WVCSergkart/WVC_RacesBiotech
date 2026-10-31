using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	//public class Gene_Resurgent : Gene_Resource, IGeneResourceDrain
	//{

	//	//public bool woundClottingAllowed = true;
	//	//public bool ageReversionAllowed = true;
	//	//public bool totalHealingAllowed = true;

	//	public Gene_Resource Resource => this;

	//	public Pawn Pawn => pawn;

	//	public bool CanOffset
	//	{
	//		get
	//		{
	//			if (Active)
	//			{
	//				return true;
	//			}
	//			return false;
	//		}
	//	}

	//	// ===========================

	//	//public void Notify_GenesRecache(Gene changedGene)
	//	//{
	//	//	cachedResurgentTotalHealing = null;
	//	//	cachedResurgentClotting = null;
	//	//	cachedResurgentAgeless = null;
	//	//}

	//	//[Unsaved(false)]
	//	//private Gene_ResurgentTotalHealing cachedResurgentTotalHealing;
	//	//[Unsaved(false)]
	//	//private Gene_ResurgentClotting cachedResurgentClotting;
	//	//[Unsaved(false)]
	//	//private Gene_ResurgentAgeless cachedResurgentAgeless;

	//	//public Gene_ResurgentTotalHealing ResurgentTotalHealing
	//	//{
	//	//	get
	//	//	{
	//	//		if (cachedResurgentTotalHealing == null || !cachedResurgentTotalHealing.Active)
	//	//		{
	//	//			cachedResurgentTotalHealing = pawn?.genes?.GetFirstGeneOfType<Gene_ResurgentTotalHealing>();
	//	//		}
	//	//		return cachedResurgentTotalHealing;
	//	//	}
	//	//}
	//	//public Gene_ResurgentClotting ResurgentClotting
	//	//{
	//	//	get
	//	//	{
	//	//		if (cachedResurgentClotting == null || !cachedResurgentClotting.Active)
	//	//		{
	//	//			cachedResurgentClotting = pawn?.genes?.GetFirstGeneOfType<Gene_ResurgentClotting>();
	//	//		}
	//	//		return cachedResurgentClotting;
	//	//	}
	//	//}
	//	//public Gene_ResurgentAgeless ResurgentAgeless
	//	//{
	//	//	get
	//	//	{
	//	//		if (cachedResurgentAgeless == null || !cachedResurgentAgeless.Active)
	//	//		{
	//	//			cachedResurgentAgeless = pawn?.genes?.GetFirstGeneOfType<Gene_ResurgentAgeless>();
	//	//		}
	//	//		return cachedResurgentAgeless;
	//	//	}
	//	//}

	//	// ===========================

	//	public string DisplayLabel => Label + " (" + "Gene".Translate() + ")";

	//	public float ResourceLossPerDay => def.resourceLossPerDay;

	//	public override float InitialResourceMax => 1.0f;

	//	public override float MinLevelForAlert => 0.05f;

	//	public override float MaxLevelOffset => 0.20f;

	//	protected override Color BarColor => new ColorInt(93, 101, 126).ToColor;
	//	protected override Color BarHighlightColor => new ColorInt(123, 131, 156).ToColor;

	//	//protected override Color BarColor => new ColorInt(126, 121, 93).ToColor;
	//	//protected override Color BarHighlightColor => new ColorInt(156, 156, 123).ToColor;

	//	//[Unsaved(false)]
	//	//private List<IGeneResourceDrain> cachedDrainGenes = new();

	//	//public List<IGeneResourceDrain> GetDrainGenes
	//	//{
	//	//	get
	//	//	{
	//	//		if (!cachedDrainGenes.NullOrEmpty())
	//	//		{
	//	//			return cachedDrainGenes;
	//	//		}
	//	//		cachedDrainGenes = new();
	//	//		List<Gene> genesListForReading = pawn.genes.GenesListForReading;
	//	//		for (int i = 0; i < genesListForReading.Count; i++)
	//	//		{
	//	//			Gene gene = genesListForReading[i];
	//	//			if (!gene.Active)
	//	//			{
	//	//				continue;
	//	//			}
	//	//			if (gene is IGeneResourceDrain geneResourceDrain && geneResourceDrain.Resource == Resource)
	//	//			{
	//	//				cachedDrainGenes.Add(geneResourceDrain);
	//	//			}
	//	//		}
	//	//		return cachedDrainGenes;
	//	//	}
	//	//}

	//	public override void PostAdd()
	//	{
	//		base.PostAdd();
	//		Reset();
	//	}

	//	public override void Reset()
	//	{
	//		base.Reset();
	//		if (ModsUtility.GameNotStarted())
	//		{
	//			Value = new FloatRange(0.06f, 0.97f).RandomInRange;
	//		}
	//		else
	//		{
	//			Value = 0.5f;
	//		}
	//	}

	//	public override void TickInterval(int delta)
	//	{
	//		//base.Tick();
	//		if (pawn.IsHashIntervalTick(2400, delta))
	//		{
	//			GeneResourceUtility.TickResourceDrain(this, 2400);
	//		}
	//	}

	//	public override IEnumerable<Gizmo> GetGizmos()
	//	{
	//		foreach (Gizmo gizmo in base.GetGizmos())
	//		{
	//			yield return gizmo;
	//		}
	//		foreach (Gizmo resourceDrainGizmo in GeneResourceUtility.GetResourceDrainGizmos(this))
	//		{
	//			yield return resourceDrainGizmo;
	//		}
	//	}

	//	//public override void ExposeData()
	//	//{
	//	//	base.ExposeData();
	//	//	Scribe_Values.Look(ref woundClottingAllowed, "woundClottingAllowed", defaultValue: true);
	//	//	Scribe_Values.Look(ref ageReversionAllowed, "ageReversionAllowed", defaultValue: true);
	//	//	Scribe_Values.Look(ref totalHealingAllowed, "totalHealingAllowed", defaultValue: true);
	//	//}
	//}

	//[Obsolete]
	//public class Gene_ResurgentCells : Gene_Resurgent
	//{

	//}

	public class Gene_Resurgent : XaG_Gene, IGeneOverriddenBy, IGeneRecacheable
	{

		[Unsaved(false)]
		private Hediff_ResurgentCells cachedHediff_ResurgentCells;
		public Hediff_ResurgentCells ResurgentCells
		{
			get
			{
				if (cachedHediff_ResurgentCells == null)
				{
					cachedHediff_ResurgentCells = (Hediff_ResurgentCells)pawn.health.GetOrAddHediff(Extension_Giver.hediffDef);
				}
				return cachedHediff_ResurgentCells;
			}
		}

		public float Value
		{
			get
			{
				if (ResurgentCells == null)
				{
					return 0;
				}
				return ResurgentCells.Value;
			}
			set
			{
				OffsetResource(-value);
			}
		}

		private float? cachedResourceLossPerDay;
		public float ResourceLossPerDay
		{
			get
			{
				if (cachedResourceLossPerDay == null)
				{
					float value = 0f;
					foreach (Gene gene in pawn.genes.GenesListForReading)
					{
						if (gene is Gene_ResurgentOffset gene_ResurgentOffset)
						{
							if (gene.Active)
							{
								value += gene_ResurgentOffset.ResourceLossPerDay;
							}
							else
							{
								value -= gene_ResurgentOffset.ResourceLossPerDay;
							}
						}
						else if (gene.Overridden)
						{
							value -= 0.01f;
						}
					}
					cachedResourceLossPerDay = value;
				}
				return cachedResourceLossPerDay.Value;
			}
		}

		public override void TickInterval(int delta)
		{
			if (pawn.IsHashIntervalTick(2400, delta))
			{
				//Log.Error(ResourceLossPerDay.ToString());
				OffsetResource(ResourceLossPerDay / 60000 * 2400);
			}
		}

		public void OffsetResource(float offset = 0)
		{
			ResurgentCells?.OffsetResource(offset);
		}

		private Gizmo gizmo;

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (XaG_GeneUtility.SkipIf_SelectorActiveFactionMap(pawn, this))
			{
				yield break;
			}
			if (gizmo == null)
			{
				gizmo = (Gizmo)Activator.CreateInstance(def.resourceGizmoType, this);
			}
			yield return gizmo;
		}

		public void Recache()
		{
			cachedResourceLossPerDay = null;
		}

		public void Notify_GenesRecache(Gene changedGene)
		{
			Recache();
		}

		public void Notify_OverriddenBy(Gene overriddenBy)
		{
			Recache();
		}

		public void Notify_Override()
		{
			Recache();
		}

	}

}
