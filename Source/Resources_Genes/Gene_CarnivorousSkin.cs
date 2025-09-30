using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{
    public class Gene_FlycatcherSkin : Gene, IGeneRemoteControl
	{
		public string RemoteActionName => XaG_UiUtility.OnOrOff(eatPawnApparel);

		public TaggedString RemoteActionDesc => "WVC_XaG_CarnivorousSkinDesc".Translate();

		public void RemoteControl_Action(Dialog_GenesSettings genesSettings)
		{
			eatPawnApparel = !eatPawnApparel;
		}

		public bool RemoteControl_Hide => !Active;

		public bool RemoteControl_Enabled
		{
			get
			{
				return enabled;
			}
			set
			{
				enabled = value;
				remoteControllerCached = false;
			}
		}

		public override void PostRemove()
		{
			base.PostRemove();
			XaG_UiUtility.SetAllRemoteControllersTo(pawn);
		}

		public bool enabled = true;
		public bool remoteControllerCached = false;

		public void RemoteControl_Recache()
		{
			XaG_UiUtility.RecacheRemoteController(pawn, ref remoteControllerCached, ref enabled);
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (enabled)
			{
				yield return XaG_UiUtility.GetRemoteControllerGizmo(pawn, remoteControllerCached, this);
			}
		}

		public bool eatPawnApparel = false;

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref eatPawnApparel, "eatPawnApparel", defaultValue: false);
		}

		// ================

		public override void TickInterval(int delta)
        {
            //base.TickInterval(delta);
			if (pawn.IsHashIntervalTick(4449, delta))
			{
				EatSmallInsects(4449);
			}
			if (!eatPawnApparel)
            {
				return;
            }
			if (pawn.IsHashIntervalTick(17753, delta))
			{
				EatPawnApparel(17753);
			}
		}

		public void EatPawnApparel(int tick)
		{
			if (pawn.InSpace())
            {
				return;
			}
			if (!GeneResourceUtility.TryGetNeedFood(pawn, out Need_Food need_Food))
			{
				return;
			}
			List<Apparel> apparels = pawn.apparel.WornApparel;
			foreach (Apparel apparel in apparels)
			{
				apparel.HitPoints = Mathf.Clamp(apparel.HitPoints - (int)(20f / 60000 * tick), 1, apparel.MaxHitPoints);
				need_Food.CurLevelPercentage += Mathf.Clamp((0.02f / 60000 * tick), 0.001f, apparel.HitPoints * 0.01f);
			}
		}

		public void EatSmallInsects(int tick)
		{
			BiomeDef biome = pawn.MapHeld?.Biome;
			if (biome == null)
            {
				return;
            }
			if (biome.inVacuum)
            {
				return;
            }
            if (biome.plantDensity <= 0.2f)
            {
				//Log.Error("No plants");
				return;
            }
			if (!GeneResourceUtility.TryGetNeedFood(pawn, out Need_Food need_Food))
            {
				return;
            }
            float outdoorTemp = pawn.MapHeld.mapTemperature.OutdoorTemp;
            if (outdoorTemp < 9f || outdoorTemp > 64f)
            {
				return;
			}
			//Log.Error("Plant dens: " + biome.plantDensity);
			//Log.Error("Nutr: " + (((0.1f * biome.plantDensity * biome.animalDensity) / 60000) * tick).ToString());
			need_Food.CurLevelPercentage += ((0.1f * biome.plantDensity * biome.animalDensity) / 60000) * tick;
		}

    }

}
