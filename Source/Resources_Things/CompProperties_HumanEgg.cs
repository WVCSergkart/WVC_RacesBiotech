using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_HumanEgg : CompProperties_Hatcher
	{

		//public float hatcherDaystoHatch = -1f;

		public string uniqueTag = "XaG_Egg";

		public float eggStyleChance = 0.2f;
		public List<ThingStyleDef> possibleEggStyles;

		public CompProperties_HumanEgg()
		{
			compClass = typeof(CompHumanEgg);
		}

		public override void ResolveReferences(ThingDef parentDef)
		{
			base.ResolveReferences(parentDef);
			if (hatcherDaystoHatch <= 0f)
			{
				hatcherDaystoHatch = ThingDefOf.Human.race.gestationPeriodDays;
			}
		}

	}

	public class CompHumanEgg : CompHatcher
	{

		public new CompProperties_HumanEgg Props => (CompProperties_HumanEgg)props;

		//public Pawn otherParent;
		//public Pawn hatcheeParent;

		//private float gestateProgress;

		public XenotypeHolder_Exposable xenotypeHolder;
		public GeneSet geneSet;

		public void SetupEgg(Hediff hediff)
		{
			if (hediff is Hediff_Pregnant pregnancy)
			{
				otherParent = pregnancy.Father;
				hatcheeParent = pregnancy.Mother;
				geneSet = pregnancy.geneSet;
				xenotypeHolder = new(otherParent, hatcheeParent, pregnancy.geneSet.GenesListForReading);
			}
		}

		public void SetupEgg(Pawn pawn, float progress = 0.1f)
		{
			if (pawn.gender == Gender.Female)
			{
				hatcheeParent = pawn;
			}
			else
			{
				otherParent = pawn;
			}
			GeneSet geneSet = new();
			foreach (Gene gene in pawn.genes.GenesListForReading)
			{
				geneSet.AddGene(gene.def);
			}
			this.geneSet = geneSet;
			gestateProgress = progress;
			xenotypeHolder = new(otherParent, hatcheeParent, geneSet.GenesListForReading);
		}

		public override void CompTick()
		{

		}

		//private CompTemperatureRuinable FreezerComp => parent.GetComp<CompTemperatureRuinable>();

		//private CompIngredients Ingredients => parent.GetComp<CompIngredients>();

		//public override void PostSpawnSetup(bool respawningAfterLoad)
		//{
		//	base.PostSpawnSetup(respawningAfterLoad);
		//	if (respawningAfterLoad)
		//	{
		//		if (Rand.Chance(Props.eggStyleChance) && !Props.possibleEggStyles.NullOrEmpty())
		//		{
		//			parent.SetStyleDef(Props.possibleEggStyles.RandomElement());
		//		}
		//	}
		//}

		public override void CompTickInterval(int delta)
		{
			if (!TemperatureDamaged)
			{
				float num = delta / (Props.hatcherDaystoHatch * 60000f);
				gestateProgress += num;
				if (gestateProgress > 1f && parent.Map != null)
				{
					Hatch();
				}
			}
		}

		public new void Hatch()
		{
			try
			{
				SpawnHatchee(hatcheeParent, otherParent, xenotypeHolder, geneSet, parent, parent.stackCount, "WVC_XaG_HumanEggHatchLabel", "WVC_XaG_HumanEggHatchDesc");
			}
			catch (Exception arg)
			{
				Log.Error("Failed hatch child from human egg. Reason: " + arg);
			}
			parent.Destroy(DestroyMode.QuestLogic);
		}

		public static void SpawnHatchee(Pawn mother, Pawn father, XenotypeHolder xenotypeHolder, GeneSet geneSet, Thing parent, int stackCount, string letterLabel, string letterDesc)
		{
			Pawn pawn = mother ?? father;
			if (pawn == null && parent is Pawn possiblePawn)
			{
				pawn = possiblePawn;
			}
			bool shouldNotify = true;
			for (int i = 0; i < stackCount; i++)
			{
				GestationUtility.TrySpawnHatchedOrBornPawn(pawn, parent, GestationUtility.NewBornRequest(pawn.kindDef, pawn.Faction), out Pawn child, xenotypeDef: XenotypeDefOf.Baseliner, parent2: father != null && father != pawn ? father : null);
				//AgelessUtility.SetAge();
				ReimplanterUtility.SetCustomGenes(child, geneSet?.GenesListForReading ?? xenotypeHolder.genes, xenotypeHolder.iconDef, xenotypeHolder.name, true);
				ReimplanterUtility.TryFixPawnXenotype_Beta(child);
				if (mother?.genes?.Xenotype == father?.genes?.Xenotype || AnyParentIsNull())
				{
					child.genes.SetXenotypeDirect(pawn?.genes?.Xenotype);
				}
				if (!AnyParentIsNull())
				{
					child.genes.hybrid = mother.genes?.Xenotype != father.genes?.Xenotype;
				}
				//if (ShouldUpdateChild(child))
				//{
				//}
				if (PawnUtility.ShouldSendNotificationAbout(child) && shouldNotify)
				{
					shouldNotify = false;
					Find.LetterStack.ReceiveLetter(letterLabel.Translate(), letterDesc.Translate(child.LabelShort.Colorize(ColoredText.NameColor)), MainDefOf.WVC_XaG_GestationEvent, new LookTargets(child));
				}
			}

			bool AnyParentIsNull()
			{
				if (mother == null)
				{
					return true;
				}
				if (father == null)
				{
					return true;
				}
				return false;
			}
		}

		public bool ShouldUpdateChild(Pawn child)
		{
			List<GeneDef> genesListForReading = child.genes.GenesListForReading.ConvertToDefs();
			foreach (GeneDef geneDef in xenotypeHolder.genes)
			{
				if (!genesListForReading.Contains(geneDef))
				{
					return true;
				}
			}
			return false;
		}

		//public override void Notify_Killed(Map prevMap, DamageInfo? dinfo = null)
		//{
		//	base.Notify_Killed(prevMap, dinfo);
		//	EggKilled();
		//	//if (hatcheeParent != null)
		//	//{
		//	//}
		//	//if (otherParent != null)
		//	//{
		//	//}
		//}

		private void EggKilled()
		{
			hatcheeParent?.needs?.mood?.thoughts?.memories?.TryGainMemory(ThoughtDefOf.PartnerMiscarried, hatcheeParent);
			otherParent?.needs?.mood?.thoughts?.memories?.TryGainMemory(ThoughtDefOf.PartnerMiscarried, hatcheeParent ?? otherParent);
		}

		public override void Notify_AbandonedAtTile(PlanetTile tile)
		{
			base.Notify_AbandonedAtTile(tile);
			EggKilled();
		}

		public override void PostDestroy(DestroyMode mode, Map previousMap)
		{
			base.PostDestroy(mode, previousMap);
			if (mode != DestroyMode.QuestLogic)
			{
				EggKilled();
			}
		}

		//public bool TemperatureDamaged
		//{
		//	get
		//	{
		//		if (FreezerComp != null)
		//		{
		//			return FreezerComp.Ruined;
		//		}
		//		return false;
		//	}
		//}

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
			if (otherCompHumanEgg.otherParent != otherParent || otherCompHumanEgg.hatcheeParent != hatcheeParent || otherCompHumanEgg.xenotypeHolder != xenotypeHolder)
			{
				return false;
			}
			return base.AllowStackWith(other);
		}

		//public override string CompInspectStringExtra()
		//{
		//	if (!TemperatureDamaged)
		//	{
		//		return "EggProgress".Translate() + ": " + gestateProgress.ToStringPercent() + "\n" + "HatchesIn".Translate() + ": " + "PeriodDays".Translate((Props.hatcherDaystoHatch * (1f - gestateProgress)).ToString("F1"));
		//	}
		//	return null;
		//}

		//public override IEnumerable<StatDrawEntry> SpecialDisplayStats()
		//{
		//	foreach (StatDrawEntry item in base.SpecialDisplayStats())
		//	{
		//		yield return item;
		//	}
		//	if (geneSet == null)
		//	{
		//		yield break;
		//	}
		//	foreach (StatDrawEntry item2 in geneSet.SpecialDisplayStats())
		//	{
		//		yield return item2;
		//	}
		//}

		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			yield return XaG_UiUtility.ITab_InspectBabyGenes();
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
		}

		public override void PostSplitOff(Thing piece)
		{
			CompHumanEgg comp = ((ThingWithComps)piece).GetComp<CompHumanEgg>();
			//comp.gestateProgress = gestateProgress;
			//comp.otherParent = otherParent;
			//comp.hatcheeParent = hatcheeParent;
			comp.xenotypeHolder = xenotypeHolder;
			base.PostSplitOff(piece);
		}

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_References.Look(ref otherParent, "father_" + Props.uniqueTag, saveDestroyedThings: true);
			Scribe_References.Look(ref hatcheeParent, "mother_" + Props.uniqueTag, saveDestroyedThings: true);
			Scribe_Deep.Look(ref xenotypeHolder, "xenotypeHolder_" + Props.uniqueTag);
			Scribe_Deep.Look(ref geneSet, "geneSet_" + Props.uniqueTag);
			//Scribe_Values.Look(ref gestateProgress, "gestateProgress_" + Props.uniqueTag, 0f);
		}

	}

	//public class CompTemperatureRuinable_HumanEgg : CompTemperatureRuinable
	//{

	//	public void SetEgg(CompHumanEgg humanEgg)
	//	{

	//	}

	//}

}
