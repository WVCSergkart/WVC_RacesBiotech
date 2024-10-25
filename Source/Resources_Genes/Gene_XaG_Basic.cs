using RimWorld;
using RimWorld.QuestGen;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public interface IGeneInspectInfo
	{

		string GetInspectInfo { get; }

	}

	public interface IGeneFloatMenuOptions
	{

		IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn);

	}

	public interface IGeneLifeStageStarted
	{

		void Notify_LifeStageStarted();

	}

	public interface IGeneOverridden
	{

		void Notify_OverriddenBy(Gene overriddenBy);

		void Notify_Override();

	}

	public interface IGeneDryadQueen
	{

		void Notify_DryadSpawned(Pawn dryad);

	}

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

	public interface IGeneCellsfeeder
	{

		void Notify_Cellsfeed(Pawn victim);

	}

	public interface IGeneShapeshift
	{

		void Notify_PreShapeshift(Gene_Shapeshifter shapeshiftGene);

		void Notify_PostShapeshift(Gene_Shapeshifter newShapeshiftGene);

	}

	public interface IGeneNotifyGenesChanged
	{

		void Notify_GenesChanged(Gene changedGene);

	}

	public interface IGenePregnantHuman
	{

		void Notify_PregnancyStarted(Hediff_Pregnant pregnancy);

	}

}
