using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;


namespace WVC_XenotypesAndGenes
{

    // [StaticConstructorOnStartup]
    public static class TemplatesUtility
    {

        public static GeneDef GetFromTemplate_XenotypeForcer(XenotypeForcerGeneTemplateDef template, XenotypeDef def, int displayOrderBase)
        {
            GeneDef geneDef = new()
            {
                defName = template.defName + "_" + def.defName + "_XenoForcer",
                geneClass = template.geneClass,
                label = template.label.Formatted(def.label),
                // iconPath = template.iconPath.Formatted(def.defName),
                iconPath = def.iconPath,
                description = template.description.Formatted(def.label),
                labelShortAdj = template.labelShortAdj.Formatted(def.label),
                selectionWeight = template.selectionWeight,
                // iconColor = template.iconColor,
                biostatCpx = template.biostatCpx,
                biostatMet = template.biostatMet,
                biostatArc = template.biostatArc,
                displayCategory = template.displayCategory,
                // displayOrderInCategory = template.displayOrderInCategory,
                customEffectDescriptions = template.customEffectDescriptions,
                displayOrderInCategory = displayOrderBase + template.displayOrderOffset,
                minAgeActive = template.minAgeActive,
                modContentPack = template.modContentPack,
                exclusionTags = new List<string>(),
                modExtensions = new List<DefModExtension>
                {
                    new GeneExtension_Giver
                    {
                        xenotypeForcerDef = def,
                        xenotypeIsInheritable = def.inheritable
                    }
                }
            };
            string exclusionTag = "WVC_XenotypesAndGenes_XenotypeTemplateCollection";
            if (def.inheritable)
            {
                exclusionTag += "_Endo";
            }
            else
            {
                exclusionTag += "_Xeno";
            }
            geneDef.exclusionTags.Add(exclusionTag);
            if (template.exclusionTags != null)
            {
                foreach (string item in template.exclusionTags)
                {
                    geneDef.exclusionTags.Add(item);
                }
            }
            if (template.modExtensions != null)
            {
                foreach (DefModExtension item in template.modExtensions)
                {
                    geneDef.modExtensions.Add(item);
                }
            }
            return geneDef;
        }

        public static GeneDef GetFromTemplate_Skills(SkillsGeneTemplateDef template, SkillDef def, int displayOrderBase)
        {
            GeneDef geneDef = new()
            {
                defName = template.defName + "_" + def.defName,
                geneClass = template.geneClass,
                label = template.label.Formatted(def.label),
                iconPath = template.iconPath.Formatted(def.defName),
                description = template.description.Formatted(def.label),
                labelShortAdj = template.labelShortAdj.Formatted(def.label),
                selectionWeight = template.selectionWeight,
                // iconColor = template.iconColor,
                biostatCpx = template.biostatCpx,
                biostatMet = template.biostatMet,
                biostatArc = template.biostatArc,
                displayCategory = template.displayCategory,
                displayOrderInCategory = displayOrderBase + template.displayOrderOffset,
                minAgeActive = template.minAgeActive,
                modContentPack = template.modContentPack,
                modExtensions = template.modExtensions
            };
            if (!template.exclusionTagPrefix.NullOrEmpty())
            {
                geneDef.exclusionTags = new List<string> { template.exclusionTagPrefix + "_" + def.defName };
            }
            if (def is SkillDef skill)
            {
                if (template.aptitudeOffset != 0)
                {
                    geneDef.aptitudes = new List<Aptitude>
                    {
                        new Aptitude(skill, template.aptitudeOffset)
                    };
                }
                if (template.passionModType != 0)
                {
                    geneDef.passionMod = new PassionMod(skill, template.passionModType);
                }
            }
            return geneDef;
        }

        public static GeneDef GetFromTemplate_InheritableImmune(InheritableImmuneGeneTemplateDef template)
        {
            GeneDef geneDef = new()
            {
                defName = template.defName + "_" + template.inheritableGeneDef.defName,
                geneClass = template.geneClass,
                label = template.label,
                iconPath = template.iconPath,
                description = template.description,
                labelShortAdj = template.labelShortAdj,
                selectionWeight = template.selectionWeight,
                customEffectDescriptions = template.customEffectDescriptions,
                // iconColor = template.iconColor,
                biostatCpx = template.biostatCpx,
                biostatMet = template.biostatMet,
                biostatArc = template.biostatArc,
                prerequisite = template.prerequisite,
                displayCategory = template.displayCategory,
                displayOrderInCategory = template.displayOrderInCategory,
                resourceLossPerDay = template.resourceLossPerDay,
                makeImmuneTo = new List<HediffDef>(),
                hediffGiversCannotGive = new List<HediffDef>(),
                exclusionTags = new List<string>(),
                minAgeActive = template.minAgeActive,
                modContentPack = template.modContentPack,
                modExtensions = template.modExtensions
            };
            // if (!template.exclusionTagPrefix.NullOrEmpty())
            // {
            // geneDef.exclusionTags = new List<string> { template.exclusionTagPrefix + "_" + def.defName };
            // }
            // template
            if (template.exclusionTags != null)
            {
                foreach (string item in template.exclusionTags)
                {
                    geneDef.exclusionTags.Add(item);
                }
            }
            if (template.makeImmuneTo != null)
            {
                foreach (HediffDef item in template.makeImmuneTo)
                {
                    geneDef.makeImmuneTo.Add(item);
                }
            }
            if (template.hediffGiversCannotGive != null)
            {
                foreach (HediffDef item in template.hediffGiversCannotGive)
                {
                    geneDef.hediffGiversCannotGive.Add(item);
                }
            }
            // inherit
            if (template.inheritableGeneDef.exclusionTags != null)
            {
                foreach (string item in template.inheritableGeneDef.exclusionTags)
                {
                    geneDef.exclusionTags.Add(item);
                }
            }
            if (template.inheritableGeneDef.makeImmuneTo != null)
            {
                foreach (HediffDef item in template.inheritableGeneDef.makeImmuneTo)
                {
                    geneDef.makeImmuneTo.Add(item);
                }
            }
            if (template.inheritableGeneDef.hediffGiversCannotGive != null)
            {
                foreach (HediffDef item in template.inheritableGeneDef.hediffGiversCannotGive)
                {
                    geneDef.hediffGiversCannotGive.Add(item);
                }
            }
            // if (def is SkillDef skill)
            // {
            // if (template.aptitudeOffset != 0)
            // {
            // geneDef.aptitudes = new List<Aptitude>
            // {
            // new Aptitude(skill, template.aptitudeOffset)
            // };
            // }
            // if (template.passionModType != 0)
            // {
            // geneDef.passionMod = new PassionMod(skill, template.passionModType);
            // }
            // }
            return geneDef;
        }

        // ===============================================================

        public static float XenotypeCost(XenotypeDef xenotype)
        {
            // float num = 0f;
            float num = (float)((Archites(xenotype) * 0.6) + (Complexity(xenotype) * 0.2) + (-1 * (Metabolism(xenotype) * 0.3)));
            return num;
        }

        public static float Archites(XenotypeDef xenotype)
        {
            float num = 0f;
            List<GeneDef> genesListForReading = xenotype.genes;
            for (int i = 0; i < genesListForReading.Count; i++)
            {
                num += genesListForReading[i].biostatArc;
            }
            return num;
        }

        public static float Complexity(XenotypeDef xenotype)
        {
            float num = 0f;
            List<GeneDef> genesListForReading = xenotype.genes;
            for (int i = 0; i < genesListForReading.Count; i++)
            {
                num += genesListForReading[i].biostatCpx;
            }
            return num;
        }

        public static float Metabolism(XenotypeDef xenotype)
        {
            float num = 0f;
            List<GeneDef> genesListForReading = xenotype.genes;
            for (int i = 0; i < genesListForReading.Count; i++)
            {
                num += genesListForReading[i].biostatMet;
            }
            return num;
        }

        // =================================================================

        public static ThingDef GetFromThingTemplate(SerumTemplateDef template, XenotypeDef def, int displayOrderBase)
        {
            // switch (template.xenotypeForcerType)
            // {
            // case SerumTemplateDef.XenotypeForcerType.Base:
            // if (XenotypeFilterUtility.BlackListedXenotypesForSingleSerums().Contains(def))
            // {
            // return null;
            // }
            // break;
            // case SerumTemplateDef.XenotypeForcerType.Hybrid:
            // if (XenotypeFilterUtility.BlackListedXenotypesForHybridSerums().Contains(def))
            // {
            // return null;
            // }
            // break;
            // }
            ThingDef thingDef = new()
            {
                defName = template.defName + "_" + def.defName + "_RB",
                label = template.label + " (" + def.label + ")",
                description = template.description,
                // + " MarketValue: " + (template.marketValueOffset + xenotypeCost)
                // descriptionHyperlinks = template.descriptionHyperlinks,
                descriptionHyperlinks = new List<DefHyperlink>
                {
                    def
                },
                thingClass = template.thingClass,
                resourceReadoutPriority = template.resourceReadoutPriority,
                useHitPoints = template.useHitPoints,
                graphicData = template.graphicData,
                // statBases = template.statBases,
                statBases = new List<StatModifier>
                {
                    new StatModifier
                    {
                        stat = StatDefOf.MarketValue,
                        value = template.marketValueOffset + (100 * (float)XenotypeCost(def))
                    }
                },
                costList = new List<ThingDefCountClass>
                {
                    // Arc
                    // new ThingDefCountClass
                    // {
                    // thingDef = ThingDefOf.ArchiteCapsule,
                    // count = 0 + (int)(1 * (TemplatesUtility.Archites(def) * 0.05))
                    // },
                    // Cpx
                    // new ThingDefCountClass
                    // {
                    // thingDef = ThingDefOf.Silver,
                    // count = 50 + (int)(1 * (TemplatesUtility.Complexity(def) * 0.1))
                    // },
                    // Met
                    // new ThingDefCountClass
                    // {
                    // thingDef = ThingDefOf.Gold,
                    // count = 20 + (int)(1 * (TemplatesUtility.Metabolism(def) * 0.2))
                    // }
                },
                stackLimit = template.stackLimit,
                techLevel = template.techLevel,
                tradeTags = template.tradeTags,
                tradeability = template.tradeability,
                thingSetMakerTags = template.thingSetMakerTags,
                // ResourceBase
                allowedArchonexusCount = template.allowedArchonexusCount,
                pathCost = template.pathCost,
                // recipeMaker = template.recipeMaker,
                rotatable = template.rotatable,
                drawGUIOverlay = template.drawGUIOverlay,
                alwaysHaulable = template.alwaysHaulable,
                altitudeLayer = template.altitudeLayer,
                selectable = template.selectable,
                drawerType = template.drawerType,
                category = template.category,
                thingCategories = template.thingCategories,
                // recipeTemplateDef = template.recipeTemplateDef,
                // ResourceBase
                comps = new List<CompProperties>
                {
                    new CompProperties_UseEffect_XenotypeForcer
                    {
                        xenotypeDef = def,
                        xenotypeForcerType = (CompProperties_UseEffect_XenotypeForcer.XenotypeForcerType)template.xenotypeForcerType,
                        removeEndogenes = template.removeEndogenes,
                        removeXenogenes = template.removeXenogenes
                    }
                },
                modExtensions = template.modExtensions
            };
            // float marketValue = StatExtension.GetStatValue(thingDef, StatDefOf.MarketValue);
            // thingDef.SetStatBaseValue(StatDefOf.MarketValue, template.marketValueOffset + xenotypeCost);
            float xenotypeArc = Archites(def);
            float xenotypeCpx = Complexity(def);
            float xenotypeMet = Metabolism(def);
            if (template.costList != null)
            {
                if (xenotypeArc > 0)
                {
                    int countArc = 1 + (int)(1 * (xenotypeArc * 0.1));
                    ThingDefCountClass itemArc = new()
                    {
                        // thingDef = ThingDefOf.ArchiteCapsule,
                        thingDef = WVC_GenesDefOf.WVC_XenotypeSerumUltraCapsule,
                        count = countArc
                    };
                    thingDef.costList.Add(itemArc);
                }
                if (xenotypeCpx > 0)
                {
                    int countCpx = 2 + (int)(1 * (xenotypeCpx * 0.1));
                    ThingDefCountClass itemCpx = new()
                    {
                        // thingDef = ThingDefOf.Silver,
                        thingDef = ThingDefOf.ComponentIndustrial,
                        count = countCpx
                    };
                    thingDef.costList.Add(itemCpx);
                }
                if (xenotypeMet > 0)
                {
                    int countMet = 1 + (int)(1 * (xenotypeMet * 0.1));
                    ThingDefCountClass itemMet = new()
                    {
                        thingDef = ThingDefOf.ComponentSpacer,
                        // thingDef = ThingDefOf.Gold,
                        count = countMet
                    };
                    thingDef.costList.Add(itemMet);
                }
                foreach (ThingDefCountClass item in template.costList)
                {
                    // if (item.thingDef != StatDefOf.MarketValue || )
                    // {
                    // }
                    thingDef.costList.Add(item);
                }
            }
            if (template.statBases != null)
            {
                foreach (StatModifier item in template.statBases)
                {
                    if (item.stat != StatDefOf.MarketValue)
                    {
                        thingDef.statBases.Add(item);
                    }
                }
            }
            if (template.comps != null)
            {
                foreach (CompProperties item in template.comps)
                {
                    thingDef.comps.Add(item);
                }
            }
            if (template.descriptionHyperlinks != null)
            {
                foreach (DefHyperlink item in template.descriptionHyperlinks)
                {
                    thingDef.descriptionHyperlinks.Add(item);
                }
            }
            if (template.recipeTemplateDef != null && WVC_Biotech.settings.serumsForAllXenotypes_Recipes)
            {
                DefGenerator.AddImpliedDef(GetFromRecipeTemplate(template.recipeTemplateDef, def, thingDef));
            }
            if (template.geneTemplateDef != null && WVC_Biotech.settings.serumsForAllXenotypes_Spawners)
            {
                DefGenerator.AddImpliedDef(GetFromGeneTemplate(template.geneTemplateDef, def, thingDef, displayOrderBase));
            }
            if (template.recipeMaker != null)
            {
                DefGenerator.AddImpliedDef(TemplatesUtility.CreateRecipeDefFromMaker(thingDef, template.recipeMaker, 1, template.basicResearchProjectDef, template.architeResearchProjectDef, xenotypeArc));
            }
            return thingDef;
        }

        // =================================================================

        public static RecipeDef GetFromRecipeTemplate(SerumRecipeTemplateDef template, XenotypeDef def, ThingDef thingDef)
        {
            RecipeDef recipeDef = new()
            {
                defName = template.defName + "_" + def.defName + "_RB",
                label = template.label + " (" + def.label + ")",
                description = template.description + " " + def.label + ".",
                jobString = template.jobString + " " + def.label + ".",
                // RecipeBase
                recipeUsers = template.recipeUsers,
                surgeryOutcomeEffect = template.surgeryOutcomeEffect,
                dontShowIfAnyIngredientMissing = template.dontShowIfAnyIngredientMissing,
                workAmount = template.workAmount,
                anesthetize = template.anesthetize,
                targetsBodyPart = template.targetsBodyPart,
                workerClass = template.workerClass,
                adjustedCount = template.adjustedCount,
                // fixedIngredientFilter = template.fixedIngredientFilter,
                // ingredients = template.ingredients,
                // ingredients = new List<IngredientCount>
                // {
                // new ThingFilter()
                // {
                // thingDef
                // }
                // },
                // RecipeBase
                modExtensions = template.modExtensions
            };
            // RecipeDefGenerator.SetIngredients(recipeDef, thingDef, recipeDef.adjustedCount);
            IngredientCount ingredientCount = new();
            ingredientCount.SetBaseCount(1);
            ingredientCount.filter.SetAllow(thingDef, true);
            recipeDef.ingredients.Add(ingredientCount);
            recipeDef.fixedIngredientFilter.SetAllow(thingDef, true);
            // if (template.ingredients != null)
            // {
            // foreach (IngredientCount item in template.ingredients)
            // {
            // recipeDef.ingredients.Add(item);
            // }
            // }
            // if (template.fixedIngredientFilter != null)
            // {
            // recipeDef.fixedIngredientFilter.Add(template.fixedIngredientFilter);
            // }
            return recipeDef;
        }

        // =================================================================

        public static GeneDef GetFromGeneTemplate(SerumGeneTemplateDef template, XenotypeDef def, ThingDef thingDef, int displayOrderBase)
        {
            GeneDef geneDef = new()
            {
                defName = template.defName + "_" + def.defName + "_RB",
                label = template.label + " (" + def.label + ")",
                geneClass = template.geneClass,
                iconPath = template.iconPath,
                description = template.description,
                labelShortAdj = template.labelShortAdj.Formatted(def.label),
                selectionWeight = template.selectionWeight,
                // Base,
                customEffectDescriptions = template.customEffectDescriptions,
                marketValueFactor = template.marketValueFactor,
                randomChosen = template.randomChosen,
                exclusionTags = template.exclusionTags,
                canGenerateInGeneSet = template.canGenerateInGeneSet,
                // biostatCpx = template.biostatCpx,
                // biostatCpx = template.biostatCpx,
                // biostatCpx = template.biostatCpx,
                // biostatCpx = template.biostatCpx,
                // biostatCpx = template.biostatCpx,
                // biostatCpx = template.biostatCpx,
                // biostatCpx = template.biostatCpx,
                // biostatCpx = template.biostatCpx,
                // Base,
                biostatCpx = template.biostatCpx,
                biostatMet = template.biostatMet,
                biostatArc = template.biostatArc,
                displayCategory = template.displayCategory,
                displayOrderInCategory = displayOrderBase + template.displayOrderOffset,
                minAgeActive = template.minAgeActive,
                modContentPack = template.modContentPack,
                modExtensions = new List<DefModExtension>
                {
                    new GeneExtension_Spawner
                    {
                        thingDefToSpawn = thingDef,
                        stackCount = template.stackCount,
                        spawnIntervalRange = template.spawnIntervalRange
                    }
                }
                // modExtensions = template.modExtensions
            };
            // geneDef.iconColor = template.iconColor;
            if (template.modExtensions != null)
            {
                foreach (DefModExtension item in template.modExtensions)
                {
                    geneDef.modExtensions.Add(item);
                }
            }
            // if (!template.exclusionTagPrefix.NullOrEmpty())
            // {
            // geneDef.exclusionTags = new List<string> { template.exclusionTagPrefix + "_" + def.defName };
            // }
            // if (def is SkillDef skill)
            // {
            // if (template.aptitudeOffset != 0)
            // {
            // geneDef.aptitudes = new List<Aptitude>
            // {
            // new Aptitude(skill, template.aptitudeOffset)
            // };
            // }
            // if (template.passionModType != 0)
            // {
            // geneDef.passionMod = new PassionMod(skill, template.passionModType);
            // }
            // }
            return geneDef;
        }

        // =================================================================

        public static RecipeDef CreateRecipeDefFromMaker(ThingDef def, RecipeMakerProperties recipeMaker, int adjustedCount = 1, ResearchProjectDef basicResearchProjectDef = null, ResearchProjectDef architeResearchProjectDef = null, float xenotypeArc = 0)
        {
            // RecipeMakerProperties recipeMaker = def.recipeMaker;
            string text = def.label;
            RecipeDef recipeDef = new()
            {
                defName = "Make_" + def.defName,
                label = "RecipeMake".Translate(text),
                jobString = "RecipeMakeJobString".Translate(text),
                modContentPack = def.modContentPack,
                displayPriority = recipeMaker.displayPriority + adjustedCount - 1,
                workAmount = recipeMaker.workAmount * adjustedCount,
                workSpeedStat = recipeMaker.workSpeedStat,
                efficiencyStat = recipeMaker.efficiencyStat,
                useIngredientsForColor = recipeMaker.useIngredientsForColor,
                defaultIngredientFilter = recipeMaker.defaultIngredientFilter,
                targetCountAdjustment = recipeMaker.targetCountAdjustment * adjustedCount,
                skillRequirements = recipeMaker.skillRequirements.ListFullCopyOrNull(),
                workSkill = recipeMaker.workSkill,
                workSkillLearnFactor = recipeMaker.workSkillLearnPerTick,
                requiredGiverWorkType = recipeMaker.requiredGiverWorkType,
                unfinishedThingDef = recipeMaker.unfinishedThingDef,
                recipeUsers = recipeMaker.recipeUsers.ListFullCopyOrNull(),
                mechanitorOnlyRecipe = recipeMaker.mechanitorOnlyRecipe,
                effectWorking = recipeMaker.effectWorking,
                soundWorking = recipeMaker.soundWorking,
                researchPrerequisite = recipeMaker.researchPrerequisite,
                memePrerequisitesAny = recipeMaker.memePrerequisitesAny,
                // researchPrerequisites = recipeMaker.researchPrerequisites,
                researchPrerequisites = new List<ResearchProjectDef> { },
                factionPrerequisiteTags = recipeMaker.factionPrerequisiteTags,
                fromIdeoBuildingPreceptOnly = recipeMaker.fromIdeoBuildingPreceptOnly
            };
            recipeDef.descriptionHyperlinks = recipeDef.products.Select((ThingDefCountClass p) => new DefHyperlink(p.thingDef)).ToList();
            recipeDef.products.Add(new ThingDefCountClass(def, recipeMaker.productCount * adjustedCount));
            string[] items = recipeDef.products.Select((ThingDefCountClass p) => (p.count != 1) ? p.Label : Find.ActiveLanguageWorker.WithIndefiniteArticle(p.thingDef.label)).ToArray();
            recipeDef.description = "RecipeMakeDescription".Translate(items.ToCommaList(useAnd: true));
            if (adjustedCount != 1)
            {
                text = text + " x" + adjustedCount;
            }
            if (recipeMaker.researchPrerequisites != null)
            {
                foreach (ResearchProjectDef item in recipeMaker.researchPrerequisites)
                {
                    recipeDef.researchPrerequisites.Add(item);
                }
                // recipeDef.researchPrerequisites.Add(architeResearchProjectDef);
                // researchPrerequisites = recipeMaker.researchPrerequisites,
            }
            if (architeResearchProjectDef != null && xenotypeArc > 0)
            {
                recipeDef.researchPrerequisites.Add(architeResearchProjectDef);
                // recipeDef.researchPrerequisites = architeResearchProjectDef;
            }
            else if (basicResearchProjectDef != null)
            {
                recipeDef.researchPrerequisites.Add(basicResearchProjectDef);
                // recipeDef.researchPrerequisites = basicResearchProjectDef;
            }
            RecipeDefGenerator.SetIngredients(recipeDef, def, adjustedCount);
            if (def.costListForDifficulty != null)
            {
                recipeDef.regenerateOnDifficultyChange = true;
            }
            if (adjustedCount != 1)
            {
                recipeDef.defName += "Bulk";
            }
            if (adjustedCount != 1 && recipeDef.workAmount < 0f)
            {
                recipeDef.workAmount = recipeDef.WorkAmountTotal(null) * (float)adjustedCount;
            }
            return recipeDef;
        }

    }
}
