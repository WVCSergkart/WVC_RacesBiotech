using RimWorld;
using RimWorld.Planet;
using System;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_HumanEgg : CompProperties
	{

		public float hatcherDaystoHatch = -1f;

		public string uniqueTag = "XaG_Egg";

		public CompProperties_HumanEgg()
		{
			compClass = typeof(CompHumanEgg);
		}

		public override void ResolveReferences(ThingDef parentDef)
		{
			base.ResolveReferences(parentDef);
			if (hatcherDaystoHatch <= 0f)
			{
				hatcherDaystoHatch = ThingDefOf.Human.race.gestationPeriodDays / 2;
			}
		}

	}

	public class CompHumanEgg : ThingComp
	{

		public CompProperties_HumanEgg Props => (CompProperties_HumanEgg)props;

		public Pawn father;
		public Pawn mother;

		private float gestateProgress;

		public SaveableXenotypeHolder xenotypeHolder;

		public void SetupEgg(Hediff hediff)
		{
			if (hediff is Hediff_Pregnant pregnancy)
			{
				father = pregnancy.Father;
				mother = pregnancy.Mother;
				xenotypeHolder = new(father, mother, pregnancy.geneSet.GenesListForReading);
			}
		}

		private CompTemperatureRuinable FreezerComp => parent.GetComp<CompTemperatureRuinable>();
		public override void CompTickInterval(int delta)
		{
			if (!TemperatureDamaged)
			{
				float num = delta / (Props.hatcherDaystoHatch * 60000f);
				gestateProgress += num;
				if (gestateProgress > 1f)
				{
					Hatch();
				}
			}
		}

		private void Hatch()
		{
			try
			{
				Pawn pawn = mother ?? father;
				GestationUtility.TrySpawnHatchedOrBornPawn(pawn, parent, GestationUtility.NewBornRequest(pawn.kindDef, pawn.Faction), out _, xenotypeHolder: xenotypeHolder, parent2: father != null && father != pawn ? father : null);
			}
			catch (Exception arg)
			{
				Log.Error("Failed hatch child from human egg. Reason: " + arg);
			}
			parent.Destroy();
		}

		public bool TemperatureDamaged
		{
			get
			{
				if (FreezerComp != null)
				{
					return FreezerComp.Ruined;
				}
				return false;
			}
		}

		public override void PreAbsorbStack(Thing otherStack, int count)
		{
			float t = (float)count / (float)(parent.stackCount + count);
			float b = ((ThingWithComps)otherStack).GetComp<CompHumanEgg>().gestateProgress;
			gestateProgress = Mathf.Lerp(gestateProgress, b, t);
		}

		public override bool AllowStackWith(Thing other)
		{
			CompHumanEgg otherCompHumanEgg = other.TryGetComp<CompHumanEgg>();
			if (otherCompHumanEgg == null)
			{
				return false;
			}
			if (otherCompHumanEgg.father != father || otherCompHumanEgg.mother != mother || otherCompHumanEgg.xenotypeHolder != xenotypeHolder)
			{
				return false;
			}
			return base.AllowStackWith(other);
		}

		public override string CompInspectStringExtra()
		{
			if (!TemperatureDamaged)
			{
				return "EggProgress".Translate() + ": " + gestateProgress.ToStringPercent() + "\n" + "HatchesIn".Translate() + ": " + "PeriodDays".Translate((Props.hatcherDaystoHatch * (1f - gestateProgress)).ToString("F1"));
			}
			return null;
		}

		public override void PostSplitOff(Thing piece)
		{
			CompHumanEgg comp = ((ThingWithComps)piece).GetComp<CompHumanEgg>();
			comp.gestateProgress = gestateProgress;
			comp.father = father;
			comp.mother = mother;
			comp.xenotypeHolder = xenotypeHolder;
		}

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_References.Look(ref father, "father_" + Props.uniqueTag, saveDestroyedThings: true);
			Scribe_References.Look(ref mother, "mother_" + Props.uniqueTag, saveDestroyedThings: true);
			Scribe_Deep.Look(ref xenotypeHolder, "xenotypeHolder_" + Props.uniqueTag);
			Scribe_Values.Look(ref gestateProgress, "gestateProgress_" + Props.uniqueTag, 0f);
		}

	}

	//public class CompTemperatureRuinable_HumanEgg : CompTemperatureRuinable
	//{

	//	public void SetEgg(CompHumanEgg humanEgg)
	//	{

	//	}

	//}

}
