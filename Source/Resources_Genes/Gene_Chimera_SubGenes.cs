using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

    public class Gene_ChimeraDependant : Gene
	{

		// public GeneExtension_Giver Giver => def?.GetModExtension<GeneExtension_Giver>();

		public virtual bool EnableCooldown => false;
		public virtual bool DisableSubActions => false;
		public virtual IntRange? ReqMetRange => null;

		public GeneExtension_Spawner Spawner => def?.GetModExtension<GeneExtension_Spawner>();

		[Unsaved(false)]
		private Gene_Chimera cachedChimeraGene;

		public Gene_Chimera Chimera
		{
			get
			{
				if (cachedChimeraGene == null || !cachedChimeraGene.Active)
				{
					cachedChimeraGene = pawn?.genes?.GetFirstGeneOfType<Gene_Chimera>();
				}
				return cachedChimeraGene;
			}
		}

		public override void TickInterval(int delta)
		{
			
		}

    }

	public class Gene_ChimeraGenesGen : Gene_ChimeraDependant
	{

		private int nextTick = 62556;

		public override void PostAdd()
		{
			base.PostAdd();
			ResetTicker();
		}

		public override void TickInterval(int delta)
		{
			//base.Tick();
			nextTick -= delta;
			if (nextTick > 0)
			{
				return;
			}
			ResetTicker();
			TryGenGene();
		}

		public void ResetTicker()
		{
			if (Spawner != null)
			{
				nextTick = Spawner.spawnIntervalRange.RandomInRange;
			}
			else
			{
				nextTick = 60000;
			}
		}

		public virtual void TryGenGene()
		{
			if (Chimera == null)
			{
				return;
			}
			if (Rand.Chance(Spawner.chance))
			{
				GetRandomGene();
			}
		}

		public virtual void GetRandomGene()
		{
			List<GeneDef> geneDefs = DefDatabase<GeneDef>.AllDefsListForReading;
			if (geneDefs.Where((GeneDef x) => !Chimera.AllGenes.Contains(x) && (x.canGenerateInGeneSet && x.selectionWeight > 0f || x.IsVanillaDef()) && x.biostatArc == 0).TryRandomElementByWeight((GeneDef gene) => (1f + gene.selectionWeight * (gene.biostatArc != 0 ? 0.01f : 1f)) + (gene.prerequisite == Chimera.def && gene.GetModExtension<GeneExtension_General>() != null ? gene.GetModExtension<GeneExtension_General>().selectionWeight : 0f), out GeneDef result))
			{
				Chimera.TryAddGene(result);
				Messages.Message("WVC_XaG_GeneGeneticThief_GeneObtained".Translate(pawn.NameShortColored, result.label), pawn, MessageTypeDefOf.NeutralEvent, historical: false);
			}
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: GenGenes",
					action = delegate
					{
						TryGenGene();
					}
				};
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref nextTick, "nextTick", -1);
		}

	}

	public class Gene_ChimeraDigestorGen : Gene_ChimeraGenesGen
	{

		public override void TryGenGene()
		{
			if (Chimera == null)
			{
				return;
			}
			int countSpawn = Spawner.summonRange.RandomInRange;
			float geneChance = Spawner.chance;
			bool playSound = false;
			for (int i = 0; i < countSpawn; i++)
			{
				if (Chimera.EatedGenes.NullOrEmpty())
				{
					break;
				}
				Chimera.DestroyGene(Chimera.EatedGenes.RandomElement());
				if (Rand.Chance(geneChance))
				{
					GetRandomGene();
					geneChance -= 0.1f;
				}
				else
				{
					geneChance += 0.1f;
				}
				playSound = true;
			}
			if (playSound && pawn.Map != null)
			{
				SoundDef soundDef = Spawner.soundDef;
				if (soundDef != null)
				{
					soundDef.PlayOneShot(new TargetInfo(pawn.Position, pawn.Map));
				}
			}
		}

		public override void GetRandomGene()
		{
			List<GeneDef> geneDefs = DefDatabase<GeneDef>.AllDefsListForReading;
			if (geneDefs.Where((GeneDef x) => !Chimera.AllGenes.Contains(x) && (x.canGenerateInGeneSet && x.selectionWeight > 0f || x.IsVanillaDef())).TryRandomElementByWeight((GeneDef gene) => (1f + gene.selectionWeight * (gene.biostatArc != 0 ? 0.01f : 1f)) + (gene.prerequisite == Chimera.def && gene.GetModExtension<GeneExtension_General>() != null ? gene.GetModExtension<GeneExtension_General>().selectionWeight : 0f), out GeneDef result))
			{
				Chimera.TryAddGene(result);
				Messages.Message("WVC_XaG_GeneGeneticThief_GeneObtained".Translate(pawn.NameShortColored, result.label), pawn, MessageTypeDefOf.NeutralEvent, historical: false);
			}
		}

	}

	[Obsolete]
	public class Gene_GeneDigestor : Gene_ChimeraDigestorGen
	{


	}

	public class Gene_ChimeraDestroyGen : Gene_ChimeraGenesGen
	{

		public override void TryGenGene()
		{
			if (Chimera == null)
			{
				return;
			}
			int countSpawn = Spawner.summonRange.RandomInRange;
			float geneChance = Spawner.chance;
			for (int i = 0; i < countSpawn; i++)
			{
				if (Chimera.DestroyedGenes.NullOrEmpty())
				{
					break;
				}
				Chimera.RemoveDestroyedGene(Chimera.DestroyedGenes.RandomElement());
				if (Rand.Chance(geneChance))
				{
					GetRandomGene();
				}
			}
		}

	}

	public class Gene_ChimeraCosmeticGen : Gene_ChimeraGenesGen
	{

		public override void GetRandomGene()
		{
			List<GeneDef> geneDefs = DefDatabase<GeneDef>.AllDefsListForReading;
			if (geneDefs.Where((GeneDef x) => !Chimera.AllGenes.Contains(x) && (x.canGenerateInGeneSet && x.selectionWeight > 0f || x.IsVanillaDef()) && x.biostatArc == 0 && x.biostatMet == 0 && x.biostatCpx == 0).TryRandomElementByWeight((GeneDef gene) => 1f + gene.selectionWeight, out GeneDef result))
			{
				Chimera.TryAddGene(result);
				Messages.Message("WVC_XaG_GeneGeneticThief_GeneObtained".Translate(pawn.NameShortColored, result.label), pawn, MessageTypeDefOf.NeutralEvent, historical: false);
			}
		}

	}

	public class Gene_ChimeraSelfCopy : Gene_ChimeraDependant
	{

        public override void PostAdd()
        {
            base.PostAdd();
			if (MiscUtility.GameNotStarted())
            {
				Chimera?.TryAddGenesFromList(pawn.genes.GenesListForReading);
            }
        }

        public override void TickInterval(int delta)
		{
			if (!pawn.IsHashIntervalTick(143450, delta))
			{
				return;
			}
			if (!pawn.Faction.IsPlayer)
            {
				return;
            }
			SelfCopy();
		}

		public void SelfCopy()
        {
			if (Chimera.TryGetGene(pawn, out GeneDef result))
            {
				Messages.Message("WVC_XaG_GeneGeneticThief_GeneObtained".Translate(pawn.NameShortColored, result.label), pawn, MessageTypeDefOf.NeutralEvent, historical: false);
			}
        }

	}

}
