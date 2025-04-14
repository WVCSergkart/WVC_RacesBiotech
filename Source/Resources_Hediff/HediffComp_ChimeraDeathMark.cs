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
                return caster.NameShortColored + " " + (victimGenes.Count + "/" + ParentGenesCount);
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

        public override void CompPostTick(ref float severityAdjustment)
        {
            if (!GeneResourceUtility.CanTick(ref nextTick, 12000))
            {
                return;
            }
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
        }

        private void CopyGene()
        {
            List<Gene> genes = Pawn.genes.GenesListForReading.Where((gene) => !victimGenes.Contains(gene.def)).ToList();
            if (!genes.NullOrEmpty())
            {
                victimGenes.Add(genes.RandomElement().def);
            }
            cachedGenesCount = null;
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
						CopyGene();
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
			Messages.Message("WVC_XaG_ChimeraDeathMark_GenesObtained".Translate(Pawn.NameShortColored), new LookTargets(Pawn.Corpse, caster), MessageTypeDefOf.NeutralEvent, historical: false);
		}

		public override void CompExposeData()
		{
			base.CompExposeData();
			Scribe_Values.Look(ref nextTick, "nextTick", -1);
			Scribe_References.Look(ref caster, "caster", saveDestroyedThings: true);
			Scribe_Collections.Look(ref victimGenes, "victimGenes", LookMode.Def);
		}

	}

}
