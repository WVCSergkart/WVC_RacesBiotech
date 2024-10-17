using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class PatchOperationOptional : PatchOperation
	{
		public string settingName;
		public PatchOperation caseTrue;
		public PatchOperation caseFalse;

		protected override bool ApplyWorker(XmlDocument xml)
		{
			if (WVC_Biotech.settings.GetEnabledSettings.Contains(settingName) && caseTrue != null)
			{
				return caseTrue.Apply(xml);
			}
			else if (WVC_Biotech.settings.GetEnabledSettings.Contains(settingName) != true && caseFalse != null)
			{
				return caseFalse.Apply(xml);
			}
			return true;
		}
	}

	public class PatchOperationSafeAdd : PatchOperationPathed
	{

		public int safetyDepth = -1;

		public XmlContainer value;

		protected override bool ApplyWorker(XmlDocument xml)
		{
			XmlNode node = value.node;
			foreach (XmlNode item in xml.SelectNodes(xpath))
			{
				// XmlNode xmlNode = item as XmlNode;
				foreach (XmlNode childNode in node.ChildNodes)
				{
					TryAddNode(item, childNode, 0);
				}
			}
			return true;
		}

		private void TryAddNode(XmlNode parent, XmlNode addNode, int depth)
		{
			XmlNode foundNode = null;
			if (!ContainsNode(parent, addNode, ref foundNode) || depth >= safetyDepth)
			{
				parent.AppendChild(parent.OwnerDocument.ImportNode(addNode, deep: true));
			}
			else
			{
				if (!addNode.HasChildNodes || !addNode.FirstChild.HasChildNodes)
				{
					return;
				}
				foreach (XmlNode childNode in addNode.ChildNodes)
				{
					TryAddNode(foundNode, childNode, depth + 1);
				}
			}
		}

		private bool ContainsNode(XmlNode parent, XmlNode node, ref XmlNode foundNode)
		{
			foreach (XmlNode childNode in parent.ChildNodes)
			{
				if (childNode.Name != node.Name && childNode.InnerText != node.InnerText)
				{
					continue;
				}
				foundNode = childNode;
				return true;
			}
			foundNode = null;
			return false;
		}

	}

	// public class PatchOperationRemove_XenoGenes : PatchOperationPathed
	// {

		// protected override bool ApplyWorker(XmlDocument xml)
		// {
			// bool result = false;
			// XmlNode[] array = xml.SelectNodes(xpath).Cast<XmlNode>().ToArray();
			// foreach (XmlNode xmlNode in array)
			// {
				// if (xmlNode)
				// {
					// result = true;
					// xmlNode.ParentNode.RemoveChild(xmlNode);
				// }
			// }
			// return result;
		// }

	// }

	// public class PawnRenderNode_FurIsSkin : PawnRenderNode
	// {

		// protected override Shader DefaultShader => ShaderDatabase.CutoutSkinOverlay;

		// public PawnRenderNode_FurIsSkin(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree)
			// : base(pawn, props, tree)
		// {
		// }

		// public override Graphic GraphicFor(Pawn pawn)
		// {
			// string bodyPath = TexPathFor(pawn);
			// if (bodyPath == null)
			// {
				// return null;
			// }
			// if (gene is not Gene_Exoskin gene_Exoskin)
			// {
				// return DefaultGraphic(pawn, bodyPath);
			// }
			// GeneExtension_Graphic modExtension = gene_Exoskin.Graphic;
			// if (modExtension == null)
			// {
				// return DefaultGraphic(pawn, bodyPath);
			// }
			// if (modExtension.furIsSkinWithHair)
			// {
				// return GraphicDatabase.Get<Graphic_Multi>(bodyPath, ShaderDatabase.CutoutComplex, Vector2.one, pawn.story.SkinColor, pawn.story.HairColor);
			// }
			// return DefaultGraphic(pawn, bodyPath);
		// }

		// protected override string TexPathFor(Pawn pawn)
		// {
			// return pawn?.story?.furDef?.GetFurBodyGraphicPath(pawn);
		// }

		// public override Color ColorFor(Pawn pawn)
		// {
			// return pawn.story.SkinColor;
		// }

		// public Graphic DefaultGraphic(Pawn pawn, string bodyPath)
		// {
			// return GraphicDatabase.Get<Graphic_Multi>(bodyPath, ShaderFor(pawn), Vector2.one, ColorFor(pawn));
		// }

	// }

	public class SettingsDef : Def
	{

		public int presetOrder = 0;

		// Settings
		public bool hideXaGGenes = false;
		public bool disableFurGraphic = false;
		public bool enable_FurskinIsSkinAutopatch = false;
		public bool disableAllGraphic = false;
		public bool disableUniqueGeneInterface = false;
		public bool disableEyesGraphic = false;
		public bool useMaskForFurskinGenes = true;
		// public bool enableBodySizeGenes = true;
		// Generator
		public bool generateSkillGenes = true;
		public bool generateXenotypeForceGenes = false;
		public bool generateResourceSpawnerGenes = false;
		public bool generateSkinHairColorGenes = false;
		// Genes
		public bool onlyXenotypesMode = false;
		public bool canNonPlayerPawnResurrect = true;
		public bool allowShapeshiftAfterDeath = true;
		public bool totalHealingIgnoreScarification = true;
		// public bool genesRemoveMechlinkUponDeath = false;
		// public bool enableCustomMechLinkName = false;
		// public bool shapeshifterGeneUnremovable = true;
		public bool enableIncestLoverGene = true;
		public bool disableNonAcceptablePreyGenes = false;
		public bool enableHarmonyTelepathyGene = false;
		public bool enable_OverOverridableGenesMechanic = true;
		// public bool useAlternativeDustogenicFoodJob = true;
		public bool learningTelepathWorkForBothSides = false;
		public bool disableUniqueXenotypeScenarios = false;
		public bool restoreBodyPartsWithFullHP = false;
		// public bool reimplantResurrectionRecruiting = false;
		public bool thrallMaker_ThrallsInheritMasterGenes = true;
		// Info
		public bool enable_xagHumanComponent = true;
		public bool enable_StartingFoodPolicies = true;
		// public bool enableGeneSpawnerGizmo = true;
		// public bool enableGeneWingInfo = false;
		// public bool enableGeneBlesslinkInfo = true;
		// public bool enableGeneUndeadInfo = false;
		// public bool enableGeneScarifierInfo = false;
		// public bool enableGeneInstabilityInfo = true;
		// public bool enableGeneGauranlenConnectionInfo = true;
		// Serums
		// public bool serumsForAllXenotypes = false;
		// public bool serumsForAllXenotypes_GenBase = true;
		// public bool serumsForAllXenotypes_GenUltra = false;
		// public bool serumsForAllXenotypes_GenHybrid = false;
		// public bool serumsForAllXenotypes_Recipes = true;
		// public bool serumsForAllXenotypes_Spawners = false;
		// ExtraSettings
		public bool genesCanTickOnlyOnMap = false;
		public bool enable_flatGenesSpawnChances = false;
		// public float flatGenesSpawnChances_selectionWeight = 0.001f;
		public bool enable_ReplaceSimilarGenesAutopatch = false;
		// Fix
		public bool fixVanillaGeneImmunityCheck = true;
		public bool spawnXenoForcerSerumsFromTraders = true;
		public bool fixGenesOnLoad = false;
		public bool fixGeneAbilitiesOnLoad = false;
		public bool fixGeneTypesOnLoad = false;
		// public bool fixThrallTypesOnLoad = false;
		// Gestator
		public bool enable_birthQualityOffsetFromGenes = true;
		public float xenotypeGestator_GestationTimeFactor = 1f;
		public float xenotypeGestator_GestationMatchPercent = 0.4f;
		// Reincarnation
		public bool reincarnation_EnableMechanic = true;
		public float reincarnation_MinChronoAge = 200f;
		public float reincarnation_Chance = 0.12f;
		// Hemogenic
		public bool harmony_EnableGenesMechanicsTriggers = true;
		public bool bloodeater_SafeBloodfeed = false;
		public bool bloodeater_disableAutoFeed = false;
		public bool bloodfeeder_AutoBloodfeed = false;
		public float hemogenic_ImplanterFangsChanceFactor = 1f;
		// Thralls
		public bool enableInstabilityLastChanceMechanic = true;
		// Links
		public bool link_addedMechlinkWithGene = true;
		public bool link_addedPsylinkWithGene = true;
		public bool link_removeMechlinkWithGene = false;
		public bool link_removePsylinkWithGene = false;
		public float golemnoids_ShutdownRechargePerTick = 1f;
		public bool golembond_ShrinesStatPartOffset = false;
		public IntRange golemlink_spawnIntervalRange = new(240000, 420000);
		public IntRange golemlink_golemsToSpawnRange = new(1, 3);
		public IntRange falselink_spawnIntervalRange = new(480000, 960000);
		public IntRange falselink_mechsToSpawnRange = new(1, 6);
		// Shapeshifter
		// public bool shapeshifter_enableStyleButton = true;
		public float shapeshifer_GeneCellularRegeneration = 1f;
		// Chimera
		public bool enable_chimeraMetabolismHungerFactor = true;
		public float chimeraMinGeneCopyChance = 0.35f;
		public float chimeraStartingGenes = 5f;
		public bool enable_chimeraStartingTools = true;
		// DryadQueen
		public bool enable_dryadQueenMechanicGenerator = true;
		public float gestatedDryads_FilthRateFactor = 0.1f;
		public float gestatedDryads_AnomalyRegeneration = 0f;
		// Rechargeable
		public bool rechargeable_enablefoodPoisoningFromFood = true;
		// Xenotypes
		public bool enable_spawnXenotypesInFactions = false;
		// public bool increasedXenotypesFactionlessGenerationWeight_MainSwitch = false;
		public bool disableXenotypes_MainSwitch = false;
		public bool disableXenotypes_Undeads = false;
		public bool disableXenotypes_Psycasters = false;
		public bool disableXenotypes_Mechalike = false;
		public bool disableXenotypes_GolemMasters = false;
		public bool disableXenotypes_Bloodeaters = false;
		public bool disableXenotypes_Misc = false;

	}


}
