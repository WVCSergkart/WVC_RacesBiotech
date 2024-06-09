using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class ShapeshiftModeDef : Def
	{

		[NoTranslate]
		public string iconPath;

		public Texture2D uiIcon;

		public int uiOrder;

		public List<HediffDef> hediffDefs = new();

		public List<GeneDef> reqGeneDefs;

		public bool xenogermComa = true;

		// public Type modeClass = typeof(ShapeshifterMode);

		public Type workerClass = typeof(ShapeshifterModeWorker);

		[Unsaved(false)]
		private ShapeshifterModeWorker workerInt;

		public ShapeshifterModeWorker Worker
		{
			get
			{
				if (workerInt == null)
				{
					workerInt = (ShapeshifterModeWorker)Activator.CreateInstance(workerClass);
					workerInt.def = this;
				}
				return workerInt;
			}
		}

		public override void PostLoad()
		{
			if (!string.IsNullOrEmpty(iconPath))
			{
				LongEventHandler.ExecuteWhenFinished(delegate
				{
					uiIcon = ContentFinder<Texture2D>.Get(iconPath);
				});
			}
		}

	}

	public class ShapeshifterModeWorker
	{

		public ShapeshiftModeDef def;

		public virtual void Shapeshift(Gene_Shapeshifter gene, Dialog_Shapeshifter dialog)
		{
			UndeadUtility.TryShapeshift(gene, dialog);
		}

	}

	public class ShapeshifterModeWorker_Duplicate : ShapeshifterModeWorker
	{

		public override void Shapeshift(Gene_Shapeshifter gene, Dialog_Shapeshifter dialog)
		{
			UndeadUtility.TryDuplicatePawn(gene.pawn, gene, dialog.selectedXeno, true);
		}

	}

}
