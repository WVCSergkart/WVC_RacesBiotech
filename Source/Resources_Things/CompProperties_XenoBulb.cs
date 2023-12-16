using RimWorld;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class CompXenoBulb : ThingComp
	{

		public int geneticMaterial_Cpx = 0;
		public int geneticMaterial_Met = 0;
		public int geneticMaterial_Arc = 0;

		public CompProperties_XenoTree Props => (CompProperties_XenoTree)props;

		public override void Initialize(CompProperties props)
		{
			base.Initialize(props);
			Reset();
		}

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			base.PostSpawnSetup(respawningAfterLoad);
			if (!respawningAfterLoad)
			{
				Reset();
			}
		}

		public void Reset()
		{
			geneticMaterial_Cpx = Props.geneticMaterial_Cpx.RandomInRange;
			geneticMaterial_Met = Props.geneticMaterial_Met.RandomInRange;
			geneticMaterial_Arc = Props.geneticMaterial_Arc.RandomInRange;
			// if (parent.Position.IsPolluted(parent.Map))
			// {
				// geneticMaterial_Cpx -= 2;
				// geneticMaterial_Met -= 1;
				// geneticMaterial_Arc += 1;
			// }
		}

		// public override void CompTick()
		// {
			// base.CompTick();
			// Tick(1);
		// }

		// public override void CompTickRare()
		// {
			// base.CompTickRare();
			// Tick(250);
		// }

		// public override void CompTickLong()
		// {
			// base.CompTickRare();
			// Tick(2000);
		// }

		// public void Tick(int tick)
		// {
			// tickCounter -= tick;
			// if (tickCounter > 0 && spawnerIsActive)
			// {
				// return;
			// }
			// TryDoSpawn();
			// ResetCounter();
		// }

		public override string CompInspectStringExtra()
		{
			StringBuilder stringBuilder = new(base.CompInspectStringExtra());
			stringBuilder.AppendLine(string.Format("{0}", "WVC_XaG_XenoTreeXenoBulb_Cpx".Translate(geneticMaterial_Cpx.ToString())));
			stringBuilder.AppendLine(string.Format("{0}", "WVC_XaG_XenoTreeXenoBulb_Met".Translate(geneticMaterial_Met.ToString())));
			stringBuilder.Append(string.Format("{0}", "WVC_XaG_XenoTreeXenoBulb_Arc".Translate(geneticMaterial_Arc.ToString())));
			return stringBuilder.ToString();
		}

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look(ref geneticMaterial_Cpx, "geneticMaterial_Cpx_" + Props.uniqueTag, 0);
			Scribe_Values.Look(ref geneticMaterial_Met, "geneticMaterial_Met_" + Props.uniqueTag, 0);
			Scribe_Values.Look(ref geneticMaterial_Arc, "geneticMaterial_Arc_" + Props.uniqueTag, 0);
		}
	}

}
