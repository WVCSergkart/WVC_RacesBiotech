using RimWorld;
using System;
using System.Collections.Generic;
using System.Xml;
using Verse;
using Verse.AI;

namespace WVC_XenotypesAndGenes
{
    public class BackCompatibilityConverter_Any : Verse.BackCompatibilityConverter
	{


		public override bool AppliesToVersion(int majorVer, int minorVer)
		{
			return true;
		}

		public override string BackCompatibleDefName(Type defType, string defName, bool forDefInjections = false, XmlNode node = null)
		{
			//Log.Error("0");
			if (defType == typeof(GeneDef))
			{
				if (defName == "WVC_CrossPlate_Yellow" || defName == "WVC_FacelessEyes_ffffff_HEX" || defName == "WVC_FacelessEyes_afafaf_HEX" || defName == "WVC_FacelessEyes_afafaf_HEX" || defName == "WVC_FacelessEyes_7c69ce_HEX" || defName == "WVC_FacelessEyes_69a2ce_HEX" || defName == "WVC_FacelessEyes_69ce7a_HEX" || defName == "WVC_FacelessEyes_c8ce69_HEX" || defName == "WVC_FacelessEyes_ce69c3_HEX" || defName == "WVC_FacelessEyes_ce6969_HEX" || defName == "WVC_FacelessEyes_ceae69_HEX" || defName == "WVC_FacelessEyes_69cec6_HEX")
				{
					return "WVC_Eyes_Holoeyes";
				}
				if (defName == "WVC_Eyes_ffffff_HEX" || defName == "WVC_Eyes_afafaf_HEX" || defName == "WVC_Eyes_7c69ce_HEX" || defName == "WVC_Eyes_69a2ce_HEX" || defName == "WVC_FacelessEyes_7c69ce_HEX" || defName == "WVC_Eyes_69ce7a_HEX" || defName == "WVC_Eyes_c8ce69_HEX" || defName == "WVC_Eyes_ce69c3_HEX" || defName == "WVC_Eyes_ce6969_HEX" || defName == "WVC_Eyes_ceae69_HEX" || defName == "WVC_Eyes_69cec6_HEX")
				{
					return "WVC_Eyes_Colorful";
				}
				if (defName == "WVC_GenePackSpawner_Vanilla" || defName == "WVC_GenePackSpawner_Base" || defName == "WVC_GenePackSpawner_Ultra" || defName == "WVC_GenePackSpawner_Mecha" || defName == "WVC_GenePackSpawner_AlphaBase" || defName == "WVC_GenePackSpawner_AlphaMixed" || defName == "WVC_GenePackSpawner_Disable" || defName == "WVC_XenotypeSerumSpawner_Random" || defName == "WVC_XenotypeSerumSpawner_HybridRandom")
				{
					return "WVC_Genemaker";
				}
				if (defName == "WVC_NaturalUndead")
				{
					return "WVC_Undead";
				}
				if (defName == "WVC_ReimplanterXenotype")
				{
					return "WVC_StorageImplanter";
				}
				if (defName == "WVC_HairColor_DarkGray")
				{
					return "WVC_HairColor_Slate";
				}
				if (defName == "WVC_SkinColor_DarkGray")
				{
					return "WVC_SkinColor_Slate";
				}
				if (defName == "WVC_Mecha_NoEars")
				{
					return "Headbone_Human";
				}
			}
			if (defType == typeof(AbilityDef))
			{
				if (defName == "WVC_ReimplanterXenotype")
				{
					return "WVC_StorageImplanter";
				}
			}
			if (defType == typeof(ScenarioDef))
			{
				if (defName == "WVC_XenotypesAndGenes_Blank")
				{
					return "WVC_XenotypesAndGenes_Meca";
				}
			}
			if (defType == typeof(HediffDef))
			{
				if (defName == "WVC_GeneSavant")
				{
					return "TraumaSavant";
				}
			}
			return null;
		}
		//public override void PostExposeData(object obj)
		//{
		//	if (obj is Pawn pawn)
		//	{
		//		if (pawn.genes.)
		//		{
		//			pawn.abilities = new Pawn_AbilityTracker(pawn);
		//		}
		//		Ability ability = pawn.abilities?.abilities.FirstOrFallback((Ability x) => x.def != null && x.def.defName == "AnimaTreeLinking");
		//		if (ability != null)
		//		{
		//			pawn.abilities.RemoveAbility(ability.def);
		//		}
		//	}
		//}

		public override Type GetBackCompatibleType(Type baseType, string providedClassName, XmlNode node)
		{
			//if (node != null && (providedClassName == "ThingWithComps" || providedClassName == "Verse.ThingWithComps"))
			//{
			//	XmlElement xmlElement = node["def"];
			//	if (xmlElement != null)
			//	{
			//		if (xmlElement.InnerText == "PsychicShockLance")
			//		{
			//			return typeof(Apparel);
			//		}
			//		if (xmlElement.InnerText == "PsychicInsanityLance")
			//		{
			//			return typeof(Apparel);
			//		}
			//		if (xmlElement.InnerText == "OrbitalTargeterBombardment")
			//		{
			//			return typeof(Apparel);
			//		}
			//		if (xmlElement.InnerText == "OrbitalTargeterPowerBeam")
			//		{
			//			return typeof(Apparel);
			//		}
			//		if (xmlElement.InnerText == "OrbitalTargeterMechCluster")
			//		{
			//			return typeof(Apparel);
			//		}
			//		if (xmlElement.InnerText == "TornadoGenerator")
			//		{
			//			return typeof(Apparel);
			//		}
			//	}
			//}
			return null;
		}

        public override void PostExposeData(object obj)
        {

        }

    }

}
