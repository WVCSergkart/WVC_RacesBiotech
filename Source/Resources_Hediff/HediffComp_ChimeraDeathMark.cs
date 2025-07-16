using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class HediffCompProperties_ChimeraDeathMark : HediffCompProperties
	{

        public bool sendOnComplete = false;

        public int tickFreq = 8000;

        public HediffCompProperties_ChimeraDeathMark()
		{
			compClass = typeof(HediffComp_ChimeraDeathMark);
		}

	}

	public class HediffComp_ChimeraDeathMark : HediffComp
	{

		public Pawn caster = null;

		public List<GeneDef> victimGenes = new();

		public HediffCompProperties_ChimeraDeathMark Props => (HediffCompProperties_ChimeraDeathMark)props;

		public override bool CompShouldRemove => caster == null;

		private int nextTick = -1;
        public override string CompLabelInBracketsExtra
        {
            get
            {
                if (caster == null)
                {
                    return "";
                }
                return caster.NameShortColored + " " + (CopiedGenesCount + "/" + ParentGenesCount);
            }
        }

        private int? cachedGenesCount;
        private int ParentGenesCount
        {
            get
            {
				if (!cachedGenesCount.HasValue)
                {
                    cachedGenesCount = Pawn.genes.GenesListForReading.Count;
                }
                return cachedGenesCount.Value;
            }
        }

        private int? cachedCopiedGenesCount;
        private int CopiedGenesCount
        {
            get
            {
                if (!cachedCopiedGenesCount.HasValue)
                {
                    int count = 0;
                    foreach (GeneDef geneDef in victimGenes)
                    {
                        foreach (Gene gene in Pawn.genes.GenesListForReading)
                        {
                            if (geneDef == gene.def)
                            {
                                count++;
                            }
                        }
                    }
                    cachedCopiedGenesCount = count;
                }
                return cachedCopiedGenesCount.Value;
            }
        }

        //private float? cachedCasterPsySens;
        //private float CasterPsySense
        //{
        //    get
        //    {
        //        if (!cachedCasterPsySens.HasValue)
        //        {
        //            cachedCasterPsySens = caster?.GetStatValue(StatDefOf.PsychicSensitivity);
        //        }
        //        return cachedCasterPsySens.Value;
        //    }
        //}

        public override void CompPostTickInterval(ref float severityAdjustment, int delta)
        {
            if (!GeneResourceUtility.CanTick(ref nextTick, Props.tickFreq, delta))
            {
                return;
            }
            TryCopy();
        }

        private void TryCopy()
        {
            if (caster == null)
            {
                Pawn.health.RemoveHediff(parent);
                return;
            }
            CopyGene();
            if (caster.Dead)
            {
                Pawn.health.RemoveHediff(parent);
            }
            if (Props.sendOnComplete && CopiedGenesCount >= ParentGenesCount)
            {
                GetGenes();
            }
        }

        private void CopyGene()
        {
            List<Gene> genes = Pawn.genes.GenesListForReading.Where((gene) => !victimGenes.Contains(gene.def)).ToList();
            if (!genes.NullOrEmpty())
            {
                victimGenes.Add(genes.RandomElement().def);
            }
            // In case victim genes changed
            cachedGenesCount = null;
            cachedCopiedGenesCount = null;
            //cachedCasterPsySens = null;
        }

        public override IEnumerable<Gizmo> CompGetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: Mark CopyVictimGene",
					action = delegate
                    {
                        TryCopy();
                    }
				};
			}
		}

		public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
        {
            if (caster == null)
            {
                return;
            }
            GetGenes();
        }

        private void GetGenes()
        {
            Gene_Chimera chimera = caster.genes.GetFirstGeneOfType<Gene_Chimera>();
            if (chimera == null)
            {
                return;
            }
            float chance = caster.GetStatValue(StatDefOf.PsychicSensitivity);
            foreach (GeneDef geneDef in victimGenes)
            {
                if (Rand.Chance(chance))
                {
                    chimera.TryAddGene(geneDef);
                }
            }
            Pawn.health?.RemoveHediff(parent);
            Messages.Message("WVC_XaG_ChimeraDeathMark_GenesObtained".Translate(Pawn.NameShortColored), new LookTargets(Pawn.Corpse, caster), MessageTypeDefOf.NeutralEvent, historical: false);
        }

        public override void CompExposeData()
		{
			base.CompExposeData();
			Scribe_Values.Look(ref nextTick, "nextTick", -1);
			Scribe_References.Look(ref caster, "caster", saveDestroyedThings: true);
			Scribe_Collections.Look(ref victimGenes, "victimGenes", LookMode.Def);
            if (Scribe.mode == LoadSaveMode.LoadingVars && victimGenes != null && victimGenes.RemoveAll((GeneDef x) => x == null) > 0)
            {
                Log.Warning("Removed null geneDef(s)");
            }
        }

	}

}
