using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;


namespace WVC_XenotypesAndGenes
{

	public class Hediff_ResurgentCells : Hediff
	{
		public override string LabelInBrackets => ((float)Math.Round(currentResource * 100, 0)).ToString();

		private HediffStage curStage;

		public override bool ShouldRemove => currentResource <= 0f;

		public override bool Visible => true;

		public virtual float MaxLimit => 2f;

		public override HediffStage CurStage
		{
			get
			{
				if (curStage == null)
				{
					curStage = new();
				}
				return curStage;
			}
		}

		public float Value
		{
			get
			{
				return currentResource;
			}
		}

		public override void PostMake()
		{
			base.PostMake();
			if (ModsUtility.GameNotStarted())
			{
				currentResource = new FloatRange(0.06f, 0.87f).RandomInRange;
			}
			else
			{
				currentResource = 0.001f;
			}
		}

		public override void TickInterval(int delta)
		{
			//base.TickInterval(delta);
			if (pawn.IsHashIntervalTick(2400, delta))
			{
				OffsetResource((0.01f / 60000) * 2400);
			}
		}

		private static readonly CachedTexture Icon = new("WVC/UI/XaG_General/ThrallMaker_XenoMenu_Gizmo_v0");

		private bool? cachedCanCreateSkeleton;
		public bool CanCreateSkeleton
		{
			get
			{
				if (cachedCanCreateSkeleton == null)
				{
					cachedCanCreateSkeleton = XaG_GeneUtility.AnyArchites(pawn);
				}
				return cachedCanCreateSkeleton.Value;
			}
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (CanCreateSkeleton && currentResource >= 1f)
			{
				yield return new Command_Action
				{
					defaultLabel = "WVC_GeneResurgent_CreateSkeleton".Translate(),
					defaultDesc = "WVC_GeneResurgent_CreateSkeletonDesc".Translate(),
					icon = Icon.Texture,
					action = delegate
					{
						GeneResurgnet?.CreateSkeleton();
					}
				};
			}
		}

		private Gene_Resurgent GeneResurgnet => pawn.genes?.GetFirstGeneOfType<Gene_Resurgent>();

		private float currentResource = 0;

		public void OffsetResource(float offset = 0)
		{
			currentResource = Mathf.Clamp(currentResource - offset, 0f, MaxLimit);
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref currentResource, "resource", defaultValue: 0);
		}

		public override void PostRemoved()
		{
			base.PostRemoved();
			GeneResurgnet?.Recache();
		}

	}

}
