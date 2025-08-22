using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;


namespace WVC_XenotypesAndGenes
{

    public class Hediff_GeneMechlink : Verse.Hediff_Mechlink
    {

        //public override bool Visible => false;

        public override string LabelBase => "WVC_NaturalMechlink".Translate();

        //public override void PostAdd(DamageInfo? dinfo)
        //{
        //    base.PostAdd(dinfo);
        //    RemoveDeathLetter();
        //}

        //private void RemoveDeathLetter()
        //{
        //    foreach (HediffComp hediffComp in comps.ToList())
        //    {
        //        if (hediffComp is HediffComp_LetterOnDeath)
        //        {
        //            comps.Remove(hediffComp);
        //        }
        //    }
        //}

        public override void PostRemoved()
        {
            base.PostRemoved();
            if (!ModsConfig.IdeologyActive)
            {
                return;
            }
            try
            {
                if (pawn.Corpse != null)
                {
                    int num = GenRadial.NumCellsInRadius(3);
                    for (int i = 0; i < num; i++)
                    {
                        IntVec3 c2 = pawn.Corpse.PositionHeld + GenRadial.RadialPattern[i];
                        Log.Error("0");
                        List<Thing> items = c2.GetItems(pawn.Corpse.MapHeld).ToList();
                        foreach (Thing item in items)
                        {
                            Log.Error(item.def.defName);
                            if (item is Mechlink)
                            {
                                item.SetStyleDef(DefDatabase<ThingStyleDef>.GetNamed("WVC_NaturalMechlink"));
                            }
                        }
                    }
                }
            }
            catch
            {
                Log.Warning("Failed change mechlink skin.");
            }
        }

        //public override void ExposeData()
        //{
        //    base.ExposeData();
        //    if (Scribe.mode == LoadSaveMode.LoadingVars)
        //    {
        //        RemoveDeathLetter();
        //    }
        //}

    }

}
