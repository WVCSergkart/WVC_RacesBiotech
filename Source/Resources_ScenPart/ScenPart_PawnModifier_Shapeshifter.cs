// RimWorld.StatPart_Age
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class ScenPart_PawnModifier_Shapeshifter : ScenPart_PawnModifier
	{

		public override void PostGameStart()
		{
			base.PostGameStart();
			SetGeneral();
		}

		public override void ExposeData()
		{
			base.ExposeData();
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				SetGeneral();
			}
		}

		public override void PostWorldGenerate()
		{
			SetupGenes();
		}

		private void SetGeneral()
		{
			SetupGenes();
			Gene_Shapeshifter.xenotypesOverride = ListsUtility.GetAllXenotypesHolders().Where(xenos => !xenos.genes.Any(gene => gene.biostatArc != 0)).ToList();
		}

		private static bool genesSetted = false;
		private void SetupGenes()
		{
			if (genesSetted)
			{
				return;
			}
			try
			{
				foreach (GeneDef geneDef in DefDatabase<GeneDef>.AllDefsListForReading.Where(gene => !gene.IsAndroid()))
				{
					geneDef.biostatCpx /= 2;
					if (geneDef.biostatArc > 0)
					{
						int newArc = geneDef.biostatArc / 2;
						if (newArc < 1)
						{
							newArc = 1;
						}
						geneDef.biostatArc = newArc;
					}
				}
			}
			catch (Exception arg)
			{
				Log.Error("Failed setup genes. Reason: " + arg.Message);
			}
			genesSetted = true;
		}

	}

}
