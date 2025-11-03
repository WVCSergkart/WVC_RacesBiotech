using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	/// <summary>
	/// Timers. "Next resource_name in: 4.3 days"
	/// </summary>
	public interface IGeneInspectInfo
	{

		string GetInspectInfo { get; }

	}

	/// <summary>
	/// For implanters. "Force implantation (Gene_Name)"
	/// </summary>
	public interface IGeneFloatMenuOptions
	{

		IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn);

	}

	/// <summary>
	/// Life stage trigger. For bodies and rechaches.
	/// </summary>
	public interface IGeneLifeStageStarted
	{

		void Notify_LifeStageStarted();

	}

	/// <summary>
	/// Overridden by trigger. Typically used to reset the cache when genes are changed, often in conjunction with IGeneNotifyGenesChanged.
	/// </summary>
	public interface IGeneOverridden
	{

		void Notify_OverriddenBy(Gene overriddenBy);

		void Notify_Override();

	}

	/// <summary>
	/// Over override mechanics. For override xenogenes by endogens.
	/// </summary>
	public interface IGeneOverOverridable
	{

		void Notify_OverriddenBy(Gene overriddenBy);

	}

	/// <summary>
	/// Dryad queen sub-genes.
	/// </summary>
	public interface IGeneDryadQueen
	{

		void Notify_DryadSpawned(Pawn dryad);

	}

	/// <summary>
	/// Triggered before PawnDied(). Used for Gene_Undead and Gene_Voidlink.
	/// Depends on Human comp.
	/// </summary>
	public interface IGeneNotifyOnKilled
	{

		void Notify_PawnKilled();

		//void Notify_PawnResurrected();

	}

	// public interface IGeneAddOrRemoveHediff
	// {

	// void AddOrRemoveHediff(HediffDef hediffDef, Pawn pawn, Gene gene, List<BodyPartDef> bodyparts = null);

	// void RemoveHediff(HediffDef hediffDef, Pawn pawn);

	// }

	public interface IGeneBloodfeeder
	{

		// float Order { get; }

		void Notify_Bloodfeed(Pawn victim);

	}

	//public interface IGeneCellsfeeder
	//{

	//	void Notify_Cellsfeed(Pawn victim);

	//}

	public interface IGeneShapeshift
	{

		void Notify_PreShapeshift(Gene_Shapeshifter shapeshiftGene);

		void Notify_PostShapeshift(Gene_Shapeshifter newShapeshiftGene);

	}

	/// <summary>
	/// For internal gene cache reset. Not triggered directly in vanilla, but triggered by post debug.
	/// </summary>
	public interface IGeneNotifyGenesChanged
	{

		void Notify_GenesChanged(Gene changedGene);

	}

	public interface IGenePregnantHuman
	{

		void Notify_PregnancyStarted(Hediff_Pregnant pregnancy);

		bool Notify_CustomPregnancy(Hediff_Pregnant pregnancy);

	}

	/// <summary>
	/// Dynamic gene settings.
	/// Used for all genes without a gizmo, but with some buttons, like simple switches.
	/// It has a minimal performance impact, but is very inconvenient to use.
	/// </summary>
	public interface IGeneRemoteControl
	{

		void RemoteControl_Action(Dialog_GenesSettings genesSettings);

		//void Notify_PawnResurrected();

		string RemoteActionName { get; }

		TaggedString RemoteActionDesc { get; }

		bool RemoteControl_Enabled { get; set; }

		void RemoteControl_Recache();

		bool RemoteControl_Hide { get; }

	}

	//public interface IGeneRemoteMainframe
	//{

	//	void RemoteMainframe_Reset();

	//}

	/// <summary>
	/// Charger sub-genes.
	/// </summary>
	public interface IGeneChargeable
	{

		void Notify_Charging(float chargePerTick, int tick, float factor);

	}

	/// <summary>
	/// Chimera custom "eat" mechanic.
	/// </summary>
	public interface IGeneCustomChimeraEater
	{

		void ChimeraEater(ref List<GeneDef> selectedGenes);

		string ChimeraEater_Name { get; }

		TaggedString ChimeraEater_Desc { get; }

	}

	/// <summary>
	/// For meat explosion effects when shapeshift or changing custom graphic.
	/// </summary>
	public interface IGeneWithEffects
	{

		void DoEffects();

		void DoEffects(Pawn pawn);

	}

	/// <summary>
	/// Scarifier sub-genes.
	/// </summary>
	public interface IGeneScarifier
	{

		void Notify_Scarified();

	}

	/// <summary>
	/// Metabolism mark. Chimera, shapeshifter, overrider.
	/// </summary>
	public interface IGeneMetabolism
	{

		void UpdateMetabolism();

	}

	/// <summary>
	/// Main hivemind mark. Used for all hivemind genes.
	/// </summary>
	public interface IGeneHivemind
	{

	}

	/// <summary>
	/// No sync hivemind mark. Used only for hivemind genes.
	/// </summary>
	public interface IGeneNonSync
	{

	}

	/// <summary>
	/// General custom graphic mark. Used for custom hairs (tentacle hair), eyes (colorful eyes), bodies (archo lines), etc.
	/// </summary>
	public interface IGeneCustomGraphic
	{

		//string Label { get; }

		int CurrentTextID { get; set; }

		void DoAction();

		Color CurrentColor { get; }
		Color? DefaultColor { get; }

		void SetColor(Color color, bool visible);

		List<GeneralHolder> ColorHolder { get; }

	}

	public interface IGeneAddOrRemoveHediff
	{

		void Local_AddOrRemoveHediff();

		void Local_RemoveHediff();

	}

}
