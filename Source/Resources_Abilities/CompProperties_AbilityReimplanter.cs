using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_AbilityReimplanter : CompProperties_AbilityEffect
	{

		[Obsolete]
		public string afterResurrectionThoughtDef;
		[Obsolete]
		public string resurrectorThoughtDef;
		[Obsolete]
		public string resurrectedThoughtDef;
		[Obsolete]
		public string absorberJob;

		public List<GeneDef> geneDefs;

		public List<GeneDef> inheritableGenes;

		public bool reimplantEndogenes = true;
		public bool reimplantXenogenes = true;

		public XenotypeDef xenotypeDef;

		public ThrallDef defaultThrallDef;

		public CompProperties_AbilityReimplanter()
		{
			compClass = typeof(CompAbilityEffect_NewImplanter);
		}

	}

}
