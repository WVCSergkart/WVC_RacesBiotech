using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class Gene_Overrider : Gene_ShapeshifterDependant, IGeneMetabolism, IGeneOverridden, IGeneChargeable, IGeneNotifyGenesChanged, IGeneRemoteControl
	{
		public string RemoteActionName => addPsychicSensitivity ? "WVC_XaG_Increase".Translate() : "WVC_XaG_Decrease".Translate();

		public TaggedString RemoteActionDesc => "WVC_XaG_Gene_OverriderDesc".Translate();

		public void RemoteControl_Action(Dialog_GenesSettings genesSettings)
		{
			addPsychicSensitivity = !addPsychicSensitivity;
			Notify_HediffReset();
		}

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
				yield return XaG_UiUtility.GetRemoteControllerGizmo(pawn, remoteControllerCached, this);
			}
		}

		//===========

		private static int? cahcedSubGeneCount;
		public static int SubGenesCount
		{
			get
			{
				if (!cahcedSubGeneCount.HasValue)
				{
					cahcedSubGeneCount = DefDatabase<GeneDef>.AllDefsListForReading.Where((geneDef) => geneDef.IsGeneDefOfType<Gene_OverriderDependant>()).ToList().Count;
				}
				return cahcedSubGeneCount.Value;
			}
		}

		//public int CurrentGenes => pawn.genes.GenesListForReading.Where((gene) => gene.def.IsGeneDefOfType<Gene_MainframeDependant>()).ToList().Count;

		public HediffDef MetHediffDef => Giver?.metHediffDef;
		public HediffDef PsyHediffDef => Giver?.hediffDef;

		public List<HediffDef> Hediffs
		{
			get
			{
				List<HediffDef> list = new();
				list.Add(MetHediffDef);
				list.Add(PsyHediffDef);
				return list;
			}
		}

		public void Notify_OverriddenBy(Gene overriddenBy)
		{
			HediffUtility.RemoveHediffsFromList(pawn, Hediffs);
			//HediffUtility.TryRemoveHediff(MetHediffDef, pawn);
		}

		public void Notify_Override()
		{
			UpdateMetabolism();
		}

		public override void PostAdd()
		{
			base.PostAdd();
			//UpdateMetabolism();
			HediffUtility.TryAddOrRemoveHediff(PsyHediffDef, pawn, this);
		}

		public override void PostRemove()
		{
			base.PostRemove();
			//HediffUtility.TryRemoveHediff(MetHediffDef, pawn);
			HediffUtility.RemoveHediffsFromList(pawn, Hediffs);
			XaG_UiUtility.SetAllRemoteControllersTo(pawn);
		}

		public void UpdateMetabolism()
		{
			HediffUtility.TryAddOrUpdMetabolism(MetHediffDef, pawn, this);
		}

		//private int resource = 0;

		public bool addPsychicSensitivity = true;

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref addPsychicSensitivity, "addPsychicSensitivity", true);
		}

		private bool? isShapeshifter;
		public bool IsShapeshifter
		{
			get
			{
				if (!isShapeshifter.HasValue)
				{
					isShapeshifter = Shapeshifter != null;
				}
				return isShapeshifter.Value;
			}
		}

		public void Notify_GenesChanged(Gene changedGene)
		{
			isShapeshifter = null;
			Notify_HediffReset();
			UpdateMetabolism();
		}

		public void Notify_Charging(float chargePerTick, int tick, float factor)
		{
			//Log.Error("0");
			if (IsShapeshifter)
			{
				//Log.Error("1");
				Shapeshifter.TryOffsetResource(Mathf.Clamp(chargePerTick, 0.01f, 1f));
			}
		}

		public void Notify_HediffReset()
		{
			foreach (Hediff hediff in pawn.health.hediffSet.hediffs)
			{
				if (hediff is Hediff_PsychicSensitivity hediffWithComps)
				{
					hediffWithComps.Reset();
				}
			}
		}

	}

}
