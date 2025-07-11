using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public static class HediffUtility
	{

		private static List<HediffDef> cachedMutationDefs;
		public static bool TryGetBestMutation(Pawn pawn, out HediffDef mutation)
		{
			if (cachedMutationDefs == null)
			{
				cachedMutationDefs = DefDatabase<HediffDef>.AllDefsListForReading.Where((HediffDef hediffDef) => FilterOutMutations(hediffDef)).ToList();
			}
			mutation = null;
			if (cachedMutationDefs.TryRandomElementByWeight((HediffDef hediffDef) => pawn.health.hediffSet.HasHediff(hediffDef) ? 1f : 100f, out HediffDef mutationHediff))
			{
				mutation = mutationHediff;
			}
			return mutation != null;

			static bool FilterOutMutations(HediffDef hediffDef)
			{
				return hediffDef.CompProps<HediffCompProperties_FleshbeastEmerge>() != null && (BestSolidMutation(hediffDef));

				static bool BestSolidMutation(HediffDef hediffDef)
				{
					return hediffDef.defaultInstallPart != null && hediffDef.IsHediffDefOfType<Hediff_AddedPart>();
				}
				//static bool BestImplantMutation(HediffDef hediffDef)
				//{
				//	return hediffDef.CompProps<HediffCompProperties_FleshbeastEmerge>() != null && hediffDef.IsHediffDefOfType<Hediff_Implant>();
				//}
			}
		}

		public static void SetMutations(Pawn p, float startingMutations)
		{
			if (startingMutations <= 0)
			{
				return;
			}
			int cycleTry = 0;
			int nextMutation = 0;
			while (cycleTry < 10 + startingMutations && nextMutation < startingMutations)
			{
				if (TryGetBestMutation(p, out HediffDef mutation))
				{
					if (HediffUtility.TryGiveFleshmassMutation(p, mutation, false))
					{
						nextMutation++;
					}
				}
				cycleTry++;
			}
		}

		private static HediffDef metHediffDef;
		public static HediffDef MetHediffDef
		{
			get
			{
				if (metHediffDef == null)
                {
					metHediffDef = DefDatabase<HediffDef>.AllDefsListForReading.Where((def) => def.IsHediffDefOfType<HediffWithComps_Metabolism>()).FirstOrDefault();
				}
				return metHediffDef;
			}
		}

        public static void TryAddOrUpdMetabolism(Pawn pawn, Gene gene)
        {
			TryAddOrUpdMetabolism(MetHediffDef, pawn, gene);
		}

        public static void TryAddOrUpdMetabolism(HediffDef metHediffDef, Pawn pawn, Gene gene)
		{
			if (!WVC_Biotech.settings.enable_chimeraMetabolismHungerFactor)
            {
				return;
            }
			if (!HediffUtility.TryAddOrRemoveHediff(metHediffDef, pawn, gene, null))
			{
				GeneResourceUtility.UpdMetabolism(pawn);
			}
		}

		public static List<HediffDef> ConvertToDef(this List<Hediff> list)
		{
			List<HediffDef> newList = new();
			foreach (Hediff item in list)
			{
				newList.Add(item.def);
			}
			return newList;
		}

		public static void AddParentGenes(Pawn parent, GeneSet geneSet)
		{
			if (parent?.genes == null)
			{
				// Log.Error("Parent is null");
				return;
			}
			List<GeneDef> genes = XaG_GeneUtility.ConvertToDefs(parent.genes.Endogenes);
			foreach (GeneDef gene in genes)
			{
				if (geneSet.GenesListForReading.Contains(gene))
				{
					continue;
				}
				geneSet.AddGene(gene);
			}
		}

		public static bool TryGiveFleshmassMutation(Pawn pawn, HediffDef mutationDef, bool bloodLoss = true)
        {
            if (!ModsConfig.AnomalyActive)
            {
                return false;
            }
            if (mutationDef.defaultInstallPart == null)
            {
                Log.ErrorOnce("Attempted to use mutation hediff which didn't specify a default install part (hediff: " + mutationDef.label, 194783821);
                return false;
            }
			//List<BodyPartRecord> allBodyParts = pawn.RaceProps.body.GetPartsWithDef(mutationDef.defaultInstallPart).Where((part) => !pawn.health.hediffSet.HasHediff(mutationDef, part)).ToList();
            List<BodyPartRecord> list = (from part in pawn.RaceProps.body.GetPartsWithDef(mutationDef.defaultInstallPart)
										 where pawn.health.hediffSet.HasMissingPartFor(part)
                                         select part).ToList();
            List<BodyPartRecord> list2 = (from part in pawn.RaceProps.body.GetPartsWithDef(mutationDef.defaultInstallPart)
										  where !pawn.health.hediffSet.HasDirectlyAddedPartFor(part)
                                          select part).ToList();
            BodyPartRecord bodyPartRecord = null;
            if (list.Any())
            {
                bodyPartRecord = list.RandomElement();
            }
            else if (list2.Any())
            {
                bodyPartRecord = list2.RandomElement();
            }
            if (bodyPartRecord == null)
            {
                return false;
			}
            if (!TryMakeFleshmassNucleusHediff(mutationDef, pawn, out HediffAddedPart_FleshmassNucleus hediff, bodyPartRecord))
            {
                return false;
            }
            MedicalRecipesUtility.SpawnThingsFromHediffs(pawn, bodyPartRecord, pawn.PositionHeld, pawn.MapHeld);
            pawn.health.RestorePart(bodyPartRecord);
			pawn.health.AddHediff(hediff, bodyPartRecord);
			//Type currentClass = mutationDef.hediffClass;
			//if (mutationDef.IsHediffDefOfType<Hediff_AddedPart>())
			//{
			//	mutationDef.hediffClass = typeof(HediffAddedPart_FleshmassNucleus);
			//}
			//else if (mutationDef.IsHediffDefOfType<Hediff_Implant>())
			//{
			//	mutationDef.hediffClass = typeof(HediffImplant_FleshmassNucleus);
			//}
			//Hediff hediff = HediffMaker.MakeHediff(mutationDef, pawn);
			//pawn.health.AddHediff(hediff, bodyPartRecord);
   //         mutationDef.hediffClass = currentClass;
			//if (hediff is HediffAddedPart_FleshmassNucleus hediffAddedPart_FleshmassNucleus)
			//{
			//	hediffAddedPart_FleshmassNucleus.maxMutationLevel = maxMutationLevel;
			//}
			//else if (hediff is HediffImplant_FleshmassNucleus hediffImplant_FleshmassNucleus)
   //         {
			//	hediffImplant_FleshmassNucleus.maxMutationLevel = maxMutationLevel;
			//}
			if (bloodLoss)
			{
				MutationMeatSplatter(pawn);
			}
			return true;
        }

        public static void MutationMeatSplatter(Pawn pawn, bool bloodLoss = true, FleshbeastUtility.MeatExplosionSize size = FleshbeastUtility.MeatExplosionSize.Normal)
		{
			if (bloodLoss && !pawn.health.hediffSet.HasHediff(HediffDefOf.BloodLoss))
			{
				Hediff hediff = HediffMaker.MakeHediff(HediffDefOf.BloodLoss, pawn);
				hediff.Severity = 0.2f;
				pawn.health.AddHediff(hediff);
			}
			if (pawn.Spawned)
            {
				MiscUtility.MeatSplatter(pawn, size);
            }
        }

        public static bool IsHediffDefOfType<T>(this HediffDef hediffDef)
		{
			return hediffDef.hediffClass == typeof(T) || typeof(T).IsAssignableFrom(hediffDef.hediffClass);
		}

        public static bool TryMakeFleshmassNucleusHediff(HediffDef def, Pawn pawn, out HediffAddedPart_FleshmassNucleus hediff, BodyPartRecord partRecord = null)
        {
			hediff = null;
			if (pawn == null)
            {
                return false;
            }
			hediff = (HediffAddedPart_FleshmassNucleus)Activator.CreateInstance(typeof(HediffAddedPart_FleshmassNucleus));
            hediff.def = def;
            hediff.pawn = pawn;
            hediff.Part = partRecord;
            hediff.loadID = Find.UniqueIDsManager.GetNextHediffID();
            hediff.PostMake();
            return true;
        }

        public static float SeverityFromLit(Pawn pawn, float exposurePerSecond_Lit, float exposurePerSecond_Unlit, int ticks = 60)
		{
			bool flag = pawn.MapHeld.glowGrid.PsychGlowAt(pawn.PositionHeld) != PsychGlow.Dark;
			return flag ? exposurePerSecond_Lit * (ticks / 60) : (exposurePerSecond_Unlit * (ticks / 60));
		}

		public static void BodyPartsGiver(List<BodyPartDef> bodyparts, Pawn pawn, HediffDef hediffDef, GeneDef geneDef)
		{
			// int num = 0;
			// foreach (BodyPartDef bodypart in bodyparts)
			// {
				// if (!pawn.RaceProps.body.GetPartsWithDef(bodypart).EnumerableNullOrEmpty() && num <= pawn.RaceProps.body.GetPartsWithDef(bodypart).Count)
				// {
					// Hediff hediff = HediffMaker.MakeHediff(hediffDef, pawn);
					// HediffComp_GeneHediff hediff_GeneCheck = hediff.TryGetComp<HediffComp_GeneHediff>();
					// if (hediff_GeneCheck != null)
					// {
						// hediff_GeneCheck.geneDef = geneDef;
					// }
					// pawn.health.AddHediff(hediff, pawn.RaceProps.body.GetPartsWithDef(bodypart).ToArray()[num]);
					// num++;
				// }
			// }
			foreach (BodyPartDef bodypart in bodyparts)
			{
				foreach (BodyPartRecord bodyPartRecord in pawn.RaceProps.body.GetPartsWithDef(bodypart))
				{
					if (pawn.health.hediffSet.PartIsMissing(bodyPartRecord))
					{
						continue;
					}
					Hediff hediff = HediffMaker.MakeHediff(hediffDef, pawn);
					HediffComp_GeneHediff hediff_GeneCheck = hediff.TryGetComp<HediffComp_GeneHediff>();
					if (hediff_GeneCheck != null)
					{
						hediff_GeneCheck.geneDef = geneDef;
					}
					pawn.health.AddHediff(hediff, bodyPartRecord);
				}
			}
		}

		public static bool TryAddOrRemoveHediff(HediffDef hediffDef, Pawn pawn, Gene gene, List<BodyPartDef> bodyparts = null, bool randomizeSeverity = false)
		{
			if (hediffDef == null)
			{
				return false;
			}
			if (gene.Active)
			{
				// if (!pawn.health.hediffSet.HasHediff(hediffDef))
				// {
					// if (!bodyparts.NullOrEmpty())
					// {
						// HediffUtility.BodyPartsGiver(bodyparts, pawn, hediffDef, gene);
						// return true;
					// }
					// Hediff hediff = HediffMaker.MakeHediff(hediffDef, pawn);
					// HediffComp_RemoveIfGeneIsNotActive hediff_GeneCheck = hediff.TryGetComp<HediffComp_RemoveIfGeneIsNotActive>();
					// if (hediff_GeneCheck != null)
					// {
						// hediff_GeneCheck.geneDef = gene.def;
					// }
					// pawn.health.AddHediff(hediff);
					// return true;
				// }
				return TryAddHediff(hediffDef, pawn, gene.def, bodyparts, randomizeSeverity);
			}
			else
			{
				TryRemoveHediff(hediffDef, pawn);
			}
			return false;
		}

		public static bool TryAddHediff(HediffDef hediffDef, Pawn pawn, GeneDef geneDef, List<BodyPartDef> bodyparts = null, bool randomizeSeverity = false)
		{
			if (hediffDef == null)
			{
				return false;
			}
			if (!bodyparts.NullOrEmpty())
			{
				HediffUtility.BodyPartsGiver(bodyparts, pawn, hediffDef, geneDef);
				return true;
			}
			if (!pawn.health.hediffSet.HasHediff(hediffDef))
			{
				// pawn.health.AddHediff(hediffDef);
				Hediff hediff = HediffMaker.MakeHediff(hediffDef, pawn);
				HediffComp_GeneHediff hediff_GeneCheck = hediff.TryGetComp<HediffComp_GeneHediff>();
				if (hediff_GeneCheck != null)
				{
					hediff_GeneCheck.geneDef = geneDef;
				}
				if (randomizeSeverity)
				{
					FloatRange floatRange = new(hediffDef.minSeverity, hediffDef.maxSeverity);
					hediff.Severity = floatRange.RandomInRange;
				}
				pawn.health.AddHediff(hediff);
				return true;
			}
			return false;
		}

		public static bool TryRemoveHediff(HediffDef hediffDef, Pawn pawn)
		{
			if (hediffDef == null)
			{
				return false;
			}
			if (pawn.health.hediffSet.HasHediff(hediffDef))
			{
				// Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(hediffDef);
				// if (firstHediffOfDef != null)
				// {
					// pawn.health.RemoveHediff(firstHediffOfDef);
				// }
				foreach (Hediff hediff in pawn.health.hediffSet.hediffs.ToList())
				{
					if (hediff.def == hediffDef)
					{
						pawn.health.RemoveHediff(hediff);
					}
				}
				return true;
			}
			return false;
		}

		// public static void Notify_GeneRemoved(Gene gene, Pawn pawn)
		// {
		// foreach (Hediff hediff in pawn.health.hediffSet.hediffs.ToList())
		// {
		// hediff?.TryGetComp<HediffComp_GeneHediff>()?.Notify_GeneRemoved(gene);
		// }
		// }

		// Heads

		//public static bool TryInstallImplant_Implant(HediffDef hediffDef, Pawn pawn, BodyPartRecord part)
		//{
		//	return MedicalRecipesUtility.GetFixedPartsToApplyOn(recipe, pawn, delegate (BodyPartRecord record)
		//	{
		//		if (!pawn.health.hediffSet.GetNotMissingParts().Contains(record))
		//		{
		//			return false;
		//		}
		//		if (pawn.health.hediffSet.PartOrAnyAncestorHasDirectlyAddedParts(record))
		//		{
		//			return false;
		//		}
		//		return (!pawn.health.hediffSet.hediffs.Any((Hediff x) => x.Part == record && (x.def == recipe.addsHediff || !recipe.CompatibleWithHediff(x.def)))) ? true : false;
		//	});
		//	pawn.health.AddHediff(hediffDef, part);
		//	return true;
		//}

		public static bool IsImplantable(this ThingDef partDef, Pawn pawn, out IEnumerable<RecipeDef> source)
		{
			source = DefDatabase<RecipeDef>.AllDefs.Where((RecipeDef x) => x.IsIngredient(partDef) && pawn.def.AllRecipes.Contains(x));
			if (source.Any())
			{
				return true;
			}
			return false;
		}

		public static bool TryInstallPart(Pawn pawn, ThingDef partDef)
		{
			//IEnumerable<RecipeDef> source = DefDatabase<RecipeDef>.AllDefs.Where((RecipeDef x) => x.IsIngredient(partDef) && pawn.def.AllRecipes.Contains(x));
			if (IsImplantable(partDef, pawn, out IEnumerable<RecipeDef> source))
			{
				RecipeDef recipeDef = source.RandomElement();
				if (!recipeDef.targetsBodyPart)
				{
					recipeDef.Worker.ApplyOnPawn(pawn, null, null, new(), null);
				}
				else if (recipeDef.Worker.GetPartsToApplyOn(pawn, recipeDef).Any())
				{
					List<BodyPartRecord> parts = recipeDef.Worker.GetPartsToApplyOn(pawn, recipeDef).ToList();
					if (parts.Count > 1)
					{
						Find.WindowStack.Add(new Dialog_ImplantImplanter(pawn, recipeDef, parts));
					}
					else
                    {
                        ApplyImplantOnPawn(pawn, recipeDef, parts);
                    }
                }
				return true;
			}
			return false;
		}

        public static void ApplyImplantOnPawn(Pawn pawn, RecipeDef recipeDef, List<BodyPartRecord> parts)
        {
            recipeDef.Worker.ApplyOnPawn(pawn, parts.RandomElement(), null, new(), null);
        }

        //public static bool HeadTypeIsCorrect(Pawn pawn, List<HeadTypeDef> headTypeDefs)
        //{
        //	if (pawn?.genes == null || pawn?.story == null)
        //	{
        //		return false;
        //	}
        //	if (headTypeDefs.Contains(pawn.story.headType))
        //	{
        //		if (pawn?.health != null && pawn?.health?.hediffSet != null)
        //		{
        //			if (HasEyesGraphic(pawn) || AnyEyeIsMissing(pawn))
        //			{
        //				return false;
        //			}
        //		}
        //		return true;
        //	}
        //	return false;
        //}

        //public static bool AnyEyeIsMissing(Pawn pawn)
        //{
        //	List<Hediff_MissingPart> missingPart = pawn.health.hediffSet.GetMissingPartsCommonAncestors();
        //	for (int i = 0; i < missingPart.Count; i++)
        //	{
        //		if (missingPart[i].Part.def.tags.Contains(BodyPartTagDefOf.SightSource))
        //		{
        //			return true;
        //		}
        //	}
        //	return false;
        //}

        //public static bool HasEyesGraphic(Pawn pawn)
        //{
        //	List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
        //	for (int i = 0; i < hediffs.Count; i++)
        //	{
        //		if (hediffs[i].def.RenderNodeProperties != null || hediffs[i].def.RenderNodeProperties != null)
        //		{
        //			return true;
        //		}
        //	}
        //	return false;
        //}

        // Add and Remove

        public static void AddHediffsFromList(Pawn pawn, List<HediffDef> hediffDefs)
		{
			if (hediffDefs.NullOrEmpty() || pawn?.health?.hediffSet == null)
			{
				return;
			}
			foreach (HediffDef item in hediffDefs.ToList())
			{
				if (pawn.health.hediffSet.HasHediff(item))
				{
					continue;
				}
				pawn.health.AddHediff(item);
			}
		}

		public static void RemoveHediffsFromList(Pawn pawn, List<HediffDef> hediffDefs)
		{
			List<Hediff> hediffs = pawn?.health?.hediffSet?.hediffs;
			if (hediffs.NullOrEmpty() || hediffDefs.NullOrEmpty())
			{
				return;
			}
			foreach (Hediff item in hediffs.ToList())
			{
				if (!hediffDefs.Contains(item.def))
				{
					continue;
				}
				pawn.health.RemoveHediff(item);
			}
		}

		// Misc

		public static Hediff GetFirstHediffPreventsPregnancy(List<Hediff> hediffs)
		{
			for (int i = 0; i < hediffs.Count; i++)
			{
				if (hediffs[i].def.preventsPregnancy || hediffs[i].def.pregnant)
				{
					return hediffs[i];
				}
			}
			return null;
		}

		public static bool HasAnyHediff(List<HediffDef> hediffDefs, Pawn pawn)
		{
			List<Hediff> hediffs = pawn?.health?.hediffSet?.hediffs;
			if (hediffs.NullOrEmpty() || hediffDefs.NullOrEmpty())
			{
				return false;
			}
			for (int i = 0; i < hediffDefs.Count; i++)
			{
				for (int j = 0; j < hediffs.Count; j++)
				{
					if (hediffs[j].def == hediffDefs[i])
					{
						return true;
					}
				}
			}
			return false;
		}

		public static HediffDef GetAnyHediffFromList(List<HediffDef> hediffDefs, Pawn pawn)
		{
			List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
			for (int i = 0; i < hediffDefs.Count; i++)
			{
				for (int j = 0; j < hediffs.Count; j++)
				{
					if (hediffs[j].def == hediffDefs[i])
					{
						return hediffDefs[i];
					}
				}
			}
			return null;
		}

	}
}
