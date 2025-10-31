using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	// [StaticConstructorOnStartup]
	public class Building_XenoCharger : Building
	{
		private Pawn currentlyChargingMech;

		private float wasteProduced;

		private int wireExtensionTicks = 70;

		private CompWasteProducer wasteProducer;

		private CompThingContainer container;

		private Sustainer sustainerCharging;

		private Mote moteCharging;

		private Mote moteCablePulse;

		public float chargePerTick = 0.000833333354f;

		public const float ChargePerDay = 50f;

		// private const float ChargePerTick = 0.000833333354f;

		// private static readonly Material WasteBarFilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.9f, 0.85f, 0.2f));

		// private static readonly Material WasteBarUnfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.3f, 0.3f, 0.3f, 1f));

		// private const int TicksToExtendWire = 70;

		// private const float MinWireExtension = 0.32f;

		private Material wireMaterial;

		public CompPowerTrader Power => this.TryGetComp<CompPowerTrader>();

		public bool IsPowered => Power.PowerOn;

		public bool IsFullOfWaste
		{
			get
			{
				if (wasteProduced >= WasteProducedPerChargingCycle)
				{
					return Container.innerContainer.Any;
				}
				return false;
			}
		}

		private CompWasteProducer WasteProducer
		{
			get
			{
				if (wasteProducer == null)
				{
					wasteProducer = GetComp<CompWasteProducer>();
				}
				return wasteProducer;
			}
		}

		public CompThingContainer Container
		{
			get
			{
				if (container == null)
				{
					container = GetComp<CompThingContainer>();
				}
				return container;
			}
		}

		public GenDraw.FillableBarRequest BarDrawData => def.building.BarDrawDataFor(base.Rotation);

		private Material WireMaterial
		{
			get
			{
				if (wireMaterial == null)
				{
					wireMaterial = MaterialPool.MatFrom("Other/BundledWires", ShaderDatabase.Transparent, Color.white);
				}
				return wireMaterial;
			}
		}

		private bool IsAttachedToMech
		{
			get
			{
				if (currentlyChargingMech != null)
				{
					return wireExtensionTicks >= 70;
				}
				return false;
			}
		}

		private int WasteProducedPerChargingCycle => Container.Props.stackLimit;

		private float WasteProducedPercentFull => wasteProduced / WasteProducedPerChargingCycle;

		// private float WasteProducedPerTick => currentlyChargingMech.GetStatValue(StatDefOf.WastepacksPerRecharge) * (0.000833333354f / currentlyChargingMech.needs.food.MaxLevel);
		private float WasteProducedPerTick => 0.5f * (chargePerTick / currentlyChargingMech.needs.food.MaxLevel);


		// public override void PostPostMake()
		// {
		// if (!ModLister.CheckBiotech("Mech recharger"))
		// {
		// Destroy();
		// }
		// else
		// {
		// base.PostPostMake();
		// }
		// }

		public bool CanPawnChargeCurrently(Pawn pawn)
		{
			//if (pawn?.needs?.food == null)
			//{
			//	return false;
			//}
			if (Power.PowerNet == null)
			{
				return false;
			}
			if (IsFullOfWaste)
			{
				return false;
			}
			// if (!IsCompatibleWithCharger(pawn?.genes?.GetFirstGeneOfType<Gene_RechargeableStomach>()))
			// {
			// return false;
			// }
			if (IsPowered)
			{
				if (currentlyChargingMech == null)
				{
					return true;
				}
				if (currentlyChargingMech == pawn)
				{
					return true;
				}
			}
			return false;
		}

		// public bool IsCompatibleWithCharger(Gene_RechargeableStomach gene)
		// {
		// return gene.Props.xenoChargerDef == def;
		// }

		// public static bool IsCompatibleWithCharger(ThingDef chargerDef, PawnKindDef kindDef)
		// {
		// return IsCompatibleWithCharger(chargerDef, kindDef.race);
		// }

		// public static bool IsCompatibleWithCharger(ThingDef chargerDef, ThingDef mechRace)
		// {
		// if (mechRace.race.IsMechanoid && mechRace.GetCompProperties<CompProperties_OverseerSubject>() != null)
		// {
		// return chargerDef.building.requiredMechWeightClasses.NotNullAndContains(mechRace.race.mechWeightClass);
		// }
		// return false;
		// }

		private Gene_Rechargeable gene_RechargeableStomach;

		private Gene_Rechargeable RechargeableStomach
		{
			get
			{
				if (gene_RechargeableStomach == null || gene_RechargeableStomach?.pawn != currentlyChargingMech)
				{
					gene_RechargeableStomach = currentlyChargingMech?.genes?.GetFirstGeneOfType<Gene_Rechargeable>();
				}
				return gene_RechargeableStomach;
			}
		}

		protected override void Tick()
		{
			base.Tick();
			if (currentlyChargingMech != null && (RechargeableStomach == null || currentlyChargingMech.CurJob.targetA.Thing != this))
			{
				Log.Warning("Xenos did not clean up his charging job properly");
				StopCharging();
			}
			if (currentlyChargingMech != null && Power.PowerOn)
			{
				//currentlyChargingMech.needs.food.CurLevel += chargePerTick * RechargeableStomach.Props.chargeSpeedFactor;
				RechargeableStomach.Notify_Charging(chargePerTick, 5);
				wasteProduced += WasteProducedPerTick;
				wasteProduced = Mathf.Clamp(wasteProduced, 0f, WasteProducedPerChargingCycle);
				if (wasteProduced >= WasteProducedPerChargingCycle && !Container.innerContainer.Any)
				{
					wasteProduced = 0f;
					GenerateWastePack();
				}
				if (moteCablePulse == null || moteCablePulse.Destroyed)
				{
					moteCablePulse = MoteMaker.MakeInteractionOverlay(ThingDefOf.Mote_ChargingCablesPulse, this, new TargetInfo(InteractionCell, base.Map));
				}
				moteCablePulse?.Maintain();
			}
			if (currentlyChargingMech != null && Power.PowerOn && IsAttachedToMech)
			{
				if (sustainerCharging == null)
				{
					sustainerCharging = SoundDefOf.MechChargerCharging.TrySpawnSustainer(SoundInfo.InMap(this));
				}
				sustainerCharging.Maintain();
				if (moteCharging == null || moteCharging.Destroyed)
				{
					moteCharging = MoteMaker.MakeAttachedOverlay(currentlyChargingMech, ThingDefOf.Mote_MechCharging, Vector3.zero);
				}
				moteCharging?.Maintain();
			}
			else if (sustainerCharging != null && (currentlyChargingMech == null || !Power.PowerOn))
			{
				sustainerCharging.End();
				sustainerCharging = null;
			}
			if (wireExtensionTicks < 70)
			{
				wireExtensionTicks++;
			}
		}

		public void GenerateWastePack()
		{
			WasteProducer.ProduceWaste(WasteProducedPerChargingCycle);
			EffecterDefOf.MechChargerWasteProduced.Spawn(base.Position, base.Map).Cleanup();
		}

		public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn selPawn)
		{
			foreach (FloatMenuOption floatMenuOption in base.GetFloatMenuOptions(selPawn))
			{
				yield return floatMenuOption;
			}
			if (!CanPawnChargeCurrently(selPawn))
			{
				yield break;
			}
			// Log.Error("0");
			Gene_Rechargeable gene = selPawn?.genes?.GetFirstGeneOfType<Gene_Rechargeable>();
			if (gene?.Props?.xenoChargerDef == def)
			{
				// Log.Error("1");
				yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("WVC_ForceRecharge".Translate(), delegate
				{
					Job job = JobMaker.MakeJob(gene.Props.rechargeableStomachJobDef, this);
					job.overrideFacing = Rot4.South;
					selPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc, false);
				}), selPawn, this);
				Gene_HemogenRecharge gene2 = selPawn?.genes?.GetFirstGeneOfType<Gene_HemogenRecharge>();
				if (gene2?.Props?.xenoChargerDef == def)
				{
					// Log.Error("1");
					yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("WVC_ForceHemogenRecharge".Translate(), delegate
					{
						Job job = JobMaker.MakeJob(gene2.Props.rechargeableStomachJobDef, this);
						job.overrideFacing = Rot4.South;
						selPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc, false);
					}), selPawn, this);
				}
			}
			// Log.Error("End 0");
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			foreach (Gizmo gizmo in base.GetGizmos())
			{
				yield return gizmo;
			}
			if (!DebugSettings.ShowDevGizmos)
			{
				yield break;
			}
			Command_Action command_Action = new()
			{
				action = delegate
				{
					wasteProduced = WasteProducedPerChargingCycle;
				},
				defaultLabel = "DEV: Waste 100%"
			};
			yield return command_Action;
			Command_Action command_Action2 = new()
			{
				action = delegate
				{
					wasteProduced += 0.25f * WasteProducedPerChargingCycle;
					wasteProduced = Mathf.Clamp(wasteProduced, 0f, WasteProducedPerChargingCycle);
				},
				defaultLabel = "DEV: Waste +25%"
			};
			yield return command_Action2;
			Command_Action command_Action3 = new()
			{
				action = delegate
				{
					wasteProduced = 0f;
				},
				defaultLabel = "DEV: Waste 0%"
			};
			yield return command_Action3;
			if (!Container.innerContainer.Any)
			{
				Command_Action command_Action4 = new()
				{
					action = delegate
					{
						GenerateWastePack();
					},
					defaultLabel = "DEV: Generate waste"
				};
				yield return command_Action4;
			}
			if (currentlyChargingMech != null)
			{
				Command_Action command_Action5 = new()
				{
					action = delegate
					{
						if (currentlyChargingMech.TryGetNeedFood(out Need_Food food))
						{
							food.CurLevelPercentage = 1f;
						}
					},
					defaultLabel = "DEV: Charge 100%"
				};
				yield return command_Action5;
			}
		}

		public void StartCharging(Pawn mech)
		{
			if (currentlyChargingMech != null)
			{
				Log.Error("Tried charging on already charging mech charger!");
				return;
			}
			currentlyChargingMech = mech;
			gene_RechargeableStomach = currentlyChargingMech?.genes?.GetFirstGeneOfType<Gene_Rechargeable>();
			RechargeableStomach.currentCharger = this;
			wireExtensionTicks = 0;
			SoundDefOf.MechChargerStart.PlayOneShot(this);
		}

		public void StopCharging()
		{
			if (currentlyChargingMech == null)
			{
				Log.Error("Tried stopping charging on currently not charging mech charger!");
				return;
			}
			RechargeableStomach?.Notify_StopCharging();
			gene_RechargeableStomach = null;
			currentlyChargingMech = null;
			wireExtensionTicks = 0;
		}

		public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
		{
			if (currentlyChargingMech != null)
			{
				Messages.Message("MessageMechChargerDestroyedMechGoesBerserk".Translate(currentlyChargingMech.Named("PAWN")), new LookTargets(currentlyChargingMech), MessageTypeDefOf.NegativeEvent);
				currentlyChargingMech.mindState?.mentalStateHandler?.TryStartMentalState(MentalStateDefOf.Berserk);
			}
			WasteProducer.ProduceWaste(Mathf.CeilToInt(wasteProduced));
			base.DeSpawn(mode);
		}

		protected override void DrawAt(Vector3 drawLoc, bool flip = false)
		{
			base.DrawAt(drawLoc, flip);
			GenDraw.FillableBarRequest barDrawData = BarDrawData;
			barDrawData.center = DrawPos + Vector3.up * 0.1f;
			barDrawData.fillPercent = WasteProducedPercentFull;
			barDrawData.filledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.9f, 0.85f, 0.2f));
			barDrawData.unfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.3f, 0.3f, 0.3f, 1f));
			// barDrawData.rotation = base.Rotation;
			barDrawData.rotation = Rot4.South;
			GenDraw.DrawFillableBar(barDrawData);
			Vector3 a = drawLoc;
			float num = EaseInOutQuart(wireExtensionTicks / 70f);
			if (currentlyChargingMech == null)
			{
				num = 1f - num;
			}
			num = Mathf.Max(num, 0.32f);
			Vector3 b = Vector3.Lerp(drawLoc, InteractionCell.ToVector3Shifted(), num);
			b.y = AltitudeLayer.BuildingOnTop.AltitudeFor();
			a.y = AltitudeLayer.BuildingOnTop.AltitudeFor();
			GenDraw.DrawLineBetween(a, b, WireMaterial, 1f);
		}

		private float EaseInOutQuart(float val)
		{
			if (!(val < 0.5))
			{
				return 1f - Mathf.Pow(-2f * val + 2f, 4f) / 2f;
			}
			return 8f * val * val * val * val;
		}

		public override string GetInspectString()
		{
			StringBuilder stringBuilder = new();
			stringBuilder.Append(base.GetInspectString());
			stringBuilder.AppendLineIfNotEmpty();
			stringBuilder.Append("WasteLevel".Translate() + ": " + WasteProducedPercentFull.ToStringPercent());
			return stringBuilder.ToString();
		}

		public override IEnumerable<StatDrawEntry> SpecialDisplayStats()
		{
			IEnumerable<GeneDef> source = DefDatabase<GeneDef>.AllDefs.Where((GeneDef pk) => pk.GetModExtension<GeneExtension_Giver>() != null && pk.GetModExtension<GeneExtension_Giver>().xenoChargerDef == def);
			string text = source.Select((GeneDef pk) => pk.LabelCap.Resolve()).ToLineList(" - ");
			yield return new StatDrawEntry(StatCategoryDefOf.Basics, "WVC_StatsReport_RechargerGeneClass".Translate(), source.Select((GeneDef w) => w.label).ToCommaList().CapitalizeFirst(), "WVC_StatsReport_RechargerGeneClass_Desc".Translate() + ": " + "\n\n" + text, 99999, null, source.Select((GeneDef pk) => new Dialog_InfoCard.Hyperlink(pk)));
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_References.Look(ref currentlyChargingMech, "currentlyChargingMech");
			Scribe_Values.Look(ref wasteProduced, "wasteProduced", 0f);
			Scribe_Values.Look(ref wireExtensionTicks, "wireExtensionTicks", 0);
		}
	}

}
