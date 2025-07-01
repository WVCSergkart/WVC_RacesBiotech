using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{
    public class Gene_Overlord : Gene, IGeneOverridden
	{

		//public OverlordMode overlordMode;

		private List<Pawn> undeadArmy = new();
		public List<Pawn> UndeadsListForReading
		{
			get
			{
				List<Pawn> listForReading = new();
				foreach (Pawn dryad in undeadArmy)
				{
					if (dryad == null || dryad.Dead || dryad.Destroyed)
					{
						continue;
					}
					listForReading.Add(dryad);
				}
				return listForReading;
			}
		}

        public override void PostAdd()
        {
            base.PostAdd();
		}

		private Gizmo genesGizmo;

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (XaG_GeneUtility.SelectorDraftedActiveFactionMap(pawn, this))
			{
				yield break;
			}
			if (genesGizmo == null)
			{
				genesGizmo = (Gizmo)Activator.CreateInstance(def.resourceGizmoType, this);
			}
			yield return genesGizmo;
		}

		public override void PostRemove()
        {
            base.PostRemove();
			Notify_OverlordKilled();
		}

		public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
		{
			base.Notify_PawnDied(dinfo, culprit);
			Notify_OverlordKilled();
		}

		public bool gizmoCollapse = WVC_Biotech.settings.geneGizmosDefaultCollapse;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref gizmoCollapse, "gizmoCollapse", WVC_Biotech.settings.geneGizmosDefaultCollapse);
			Scribe_Collections.Look(ref undeadArmy, "controlledUndeads", LookMode.Reference);
			if (undeadArmy != null && undeadArmy.RemoveAll((Pawn x) => x == null || x.Destroyed || x.Dead) > 0)
			{
				Log.Warning("Removed null undead(s) from gene: " + def.defName);
			}
		}

		public void AddUndead(Pawn undead)
		{
			if (!undeadArmy.Contains(undead))
            {
				undeadArmy.Add(undead);
				if (undead.connections == null)
                {
					undead.connections = new(undead);
				}
				foreach (Thing item in undead.connections.ConnectedThings)
				{
					undead.connections.Notify_ConnectedThingLeftBehind(item);
				}
				undead.connections.ConnectTo(pawn);
			}
		}

		public void Notify_OverlordKilled()
		{
			foreach (Pawn item in undeadArmy)
            {
                if (item.connections == null)
                {
                    continue;
                }
                foreach (Thing thing in item.connections.ConnectedThings)
                {
                    item.connections.Notify_ConnectedThingLeftBehind(thing);
                }
            }
			undeadArmy = new();

		}

        public void Notify_OverriddenBy(Gene overriddenBy)
        {
			if (overriddenBy != null)
            {
				Notify_OverlordKilled();
			}
        }

        public void Notify_Override()
        {

        }

        //public float GetConsumed()
        //{
        //	base.PostRemove();
        //}

    }

}
