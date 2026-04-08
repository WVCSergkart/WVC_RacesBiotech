using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using static HarmonyLib.Code;

namespace WVC_XenotypesAndGenes
{

	public class Hediff_BudPregnancy : Hediff
	{

		//public override string Label => base.Label + " " + SeverityLabel;
		public override string LabelInBrackets => "x" + budSize.ToString() + " " + gestateProgress.ToStringPercent();

		public override bool ShouldRemove => false;

		public override Color LabelColor => HediffDefOf.PregnantHuman.defaultLabelColor;

		public override bool Visible => true;

		private int budSize = 1;

		private HediffStage curStage;
		public override HediffStage CurStage
		{
			get
			{
				if (curStage == null)
				{
					curStage = new();
					curStage.painOffset = (0.08f * budSize);
					curStage.hungerRateFactor = 1f + (budSize * 0.1f);
				}
				return curStage;
			}
		}

		private Pawn mother;
		private Pawn father;
		public GeneSet geneSet;
		private XenotypeHolder_Exposable xenotypeHolder;

		public void SetupBud(Hediff hediff)
		{
			if (hediff is Hediff_Pregnant pregnancy)
			{
				curStage = null;
				father = pregnancy.Father;
				mother = pregnancy.Mother;
				geneSet = pregnancy.geneSet;
				xenotypeHolder = new(father, mother, pregnancy.geneSet.GenesListForReading);
				budSize = GestationUtility.GetLitterSize(pawn);
			}
		}

		public void SecondPregnancy()
		{
			curStage = null;
			budSize += GestationUtility.GetLitterSize(pawn);
		}

		private float gestateProgress = 0;
		public override void TickInterval(int delta)
		{
			float num = delta / ((pawn.RaceProps.gestationPeriodDays - 5) * 60000f);
			gestateProgress += num;
			if (gestateProgress > 1f && pawn.Map != null)
			{
				Hatch();
			}
		}

		public void Hatch()
		{
			try
			{
				CompHumanEgg.SpawnHatchee(mother, father, xenotypeHolder, pawn, budSize, "WVC_XaG_HumanBudHatchLabel", "WVC_XaG_HumanBudHatchDesc");
			}
			catch (Exception arg)
			{
				Log.Error("Failed hatch child from cancer bud. Reason: " + arg);
			}
			pawn.health.RemoveHediff(this);
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: +10%",
					action = delegate
					{
						gestateProgress += 0.1f;
					}
				};
				yield return new Command_Action
				{
					defaultLabel = "DEV: -10%",
					action = delegate
					{
						gestateProgress -= 0.1f;
					}
				};
			}
			if (!pawn.Drafted)
			{
				yield return new Command_Action
				{
					defaultLabel = "InspectBabyGenes".Translate() + "...",
					defaultDesc = "InspectGenesHediffDesc".Translate(),
					icon = GeneSetHolderBase.GeneticInfoTex.Texture,
					action = delegate
					{
						InspectPaneUtility.OpenTab(typeof(ITab_GenesCustomPregnancy));
					}
				};
			}
		}

		//public override string SeverityLabel => gestateProgress.ToStringPercent();

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_References.Look(ref father, "father", saveDestroyedThings: true);
			Scribe_References.Look(ref mother, "mother", saveDestroyedThings: true);
			Scribe_Values.Look(ref budSize, "budSize", 1);
			Scribe_Values.Look(ref gestateProgress, "gestateProgress", 0);
			Scribe_Deep.Look(ref xenotypeHolder, "xenotypeHolder");
			Scribe_Deep.Look(ref geneSet, "geneSet");
		}

	}

}
