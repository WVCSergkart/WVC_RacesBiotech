using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class Gene_Faceless : Gene
	{

		public GeneExtension_Giver Props => def?.GetModExtension<GeneExtension_Giver>();

		public bool drawGraphic = true;

		public override void PostAdd()
		{
			base.PostAdd();
			if (!Active)
			{
				return;
			}
			drawGraphic = HeadTypeIsCorrect(pawn);
		}
		// private int nextRecache = 0;

		// public override bool Active
		// {
			// get
			// {
				// if (base.Active)
				// {
					// if (Find.TickManager.TicksGame < nextRecache)
					// {
						// return cachedResult;
					// }
					// cachedResult = HeadTypeIsCorrect(pawn);
					// nextRecache = Find.TickManager.TicksGame + 12000;
					// pawn.Drawer.renderer.graphics.SetAllGraphicsDirty();
					// return HeadTypeIsCorrect(pawn);
				// }
				// return base.Active;
			// }
		// }

		// public override bool Active
		// {
			// get
			// {
				// return base.Active && cachedResult;
			// }
		// }

		public override void Tick()
		{
			base.Tick();
			if (!pawn.IsHashIntervalTick(12000))
			{
				return;
			}
			if (pawn.Map == null)
			{
				return;
			}
			bool active = HeadTypeIsCorrect(pawn);
			if (drawGraphic != active)
			{
				drawGraphic = active;
				pawn.Drawer.renderer.graphics.SetAllGraphicsDirty();
			}
		}

		public bool HeadTypeIsCorrect(Pawn pawn)
		{
			if (pawn?.genes == null || pawn?.story == null)
			{
				return false;
			}
			if (Props.headTypeDefs.Contains(pawn.story.headType))
			{
				if (pawn?.health != null && pawn?.health?.hediffSet != null)
				{
					if (HasEyesGraphic(pawn) || AnyEyeIsMissing(pawn))
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		public bool AnyEyeIsMissing(Pawn pawn)
		{
			List<Hediff_MissingPart> missingPart = pawn.health.hediffSet.GetMissingPartsCommonAncestors();
			for (int i = 0; i < missingPart.Count; i++)
			{
				if (missingPart[i].Part.def.tags.Contains(BodyPartTagDefOf.SightSource))
				{
					return true;
				}
			}
			return false;
		}

		public bool HasEyesGraphic(Pawn pawn)
		{
			List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
			for (int i = 0; i < hediffs.Count; i++)
			{
				if (hediffs[i].def.eyeGraphicSouth != null || hediffs[i].def.eyeGraphicEast != null)
				{
					return true;
				}
			}
			return false;
		}

		public override void ExposeData()
		{
			base.ExposeData();
			// Scribe_Values.Look(ref cachedResult, "faceplateIsVisible", true);
			// Scribe_Values.Look(ref nextRecache, "nextRecache", 0);
			if (Active)
			{
				drawGraphic = HeadTypeIsCorrect(pawn);
			}
		}

	}
}
