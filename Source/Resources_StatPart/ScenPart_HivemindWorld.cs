// RimWorld.StatPart_Age
using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class ScenPart_HivemindWorld : ScenPart_PawnModifier_CustomWorld
	{

		public HediffDef hediffDef;
		public int nonPlayerHivemindSize = 50;
		public GoodwillSituationDef hivemindHatred;
		//public MemeDef memeDef;
		public List<GeneSetPresets> hivemindPresets;

		protected override void ModifyPawnPostGenerate(Pawn pawn, bool redressed)
		{
			if (pawn.Faction == Faction.OfPlayerSilentFail)
			{
				return;
			}
			TrySetupCustomWorldXenotype(pawn, xenotypeDefs);
			if (pawn.skills == null)
			{
				return;
			}
			foreach (SkillRecord item in pawn.skills.skills)
			{
				if (item.Level < 20)
				{
					item.Level = 20;
					item.xpSinceLastLevel = item.XpRequiredForLevelUp * 0.2f;
				}
			}
			if (hivemindPresets.TryRandomElementByWeight((item) => item.selectionWeight, out GeneSetPresets genesSet) && Rand.Chance(genesSet.selectionWeight))
			{
				XaG_GeneUtility.AddGenesToChimera(pawn, genesSet.geneDefs, true);
			}
		}

		//public override void PostWorldGenerate()
		//{
		//	base.PostWorldGenerate();
		//}

		public override void PostGameStart()
		{
			base.PostGameStart();
			SetGoodwill();
			SetFactions();
			SetNonPlayerHivemind();
		}

		//private void SetFactions()
		//{
		//	if (memeDef == null)
		//	{
		//		return;
		//	}
		//	try
		//	{
		//		IdeoGenerationParms parms = new(Faction.OfPlayer.def);
		//		parms.forNewFluidIdeo = true;
		//		parms.forcedMemes = new() { memeDef };
		//		Ideo newIdeo = IdeoGenerator.GenerateIdeo(parms);
		//		foreach (Faction faction in Find.FactionManager.AllFactionsListForReading)
		//		{
		//			if (!faction.def.displayInFactionSelection || faction.def.hidden || faction.ideos == null)
		//			{
		//				continue;
		//			}
		//			//faction.def.forcedMemes = new() { memeDef };
		//			//faction.ideos?.RemoveAll();
		//			faction.ideos.SetPrimary(newIdeo);
		//		}
		//		Find.IdeoManager.RemoveUnusedStartingIdeos();
		//	}
		//	catch (Exception arg)
		//	{
		//		Log.Error("Failed set factions for hivemind world. Reason: " + arg);
		//	}
		//}

		//public override void PostWorldGenerate()
		//{
		//	base.PostWorldGenerate();
		//	SetFactions();
		//}

		//private int nextTick = 720;
		//public override void Tick()
		//{
		//	if (nextTick > 0)
		//	{
		//		nextTick--;
		//		return;
		//	}
		//	nextTick = 2500;
		//	UpdHivemind();
		//}

		//private void UpdHivemind()
		//{
		//	if (hediffDef == null)
		//	{
		//		return;
		//	}
		//	foreach (Pawn hiver in HivemindUtility.HivemindPawns)
		//	{
		//		if (hiver.health.hediffSet.HasHediff<Hediff_HivemindHatred>())
		//		{
		//			continue;
		//		}
		//	}
		//}

		private void SetFactions()
		{
			//if (memeDef == null)
			//{
			//	return;
			//}
			try
			{
				//FactionRelation relation = new FactionRelation(Faction.OfPlayer, FactionRelationKind.Hostile);
				//relation.baseGoodwill = -75;
				foreach (Faction item in Find.FactionManager.AllFactionsListForReading)
				{
					if (item == Faction.OfPlayer || item.ideos == null)
					{
						continue;
					}
					item.ideos.RemoveAll();
					item.ideos.SetPrimary(Faction.OfPlayer.ideos.PrimaryIdeo);
					//if (item.HasGoodwill && item.GoodwillWith(Faction.OfPlayer) > -100)
					//{
					//	item.SetRelation(relation);
					//}
				}
				Faction.OfPlayer.ideos.PrimaryIdeo.Fluid = true;
				Find.IdeoManager.RemoveUnusedStartingIdeos();
			}
			catch (Exception arg)
			{
				Log.Error("Failed set factions for hivemind world. Reason: " + arg);
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref nonPlayerHivemindSize, "nonPlayerHivemindSize", 50);
			Scribe_Defs.Look(ref hivemindHatred, "hivemindHatred");
			Scribe_Defs.Look(ref hediffDef, "hediffDef");
			Scribe_Collections.Look(ref hivemindPresets, "hivemindPresets", LookMode.Deep);
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				SetGoodwill();
				SetNonPlayerHivemind();
			}
		}

		private void SetGoodwill()
		{
			hivemindHatred?.naturalGoodwillOffset = -999;
		}

		private void SetNonPlayerHivemind()
		{
			HivemindUtility.nonPlayerHivemindSize = nonPlayerHivemindSize;
		}


	}

}
