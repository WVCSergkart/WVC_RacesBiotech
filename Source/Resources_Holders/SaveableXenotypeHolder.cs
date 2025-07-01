using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{
    public class SaveableXenotypeHolder : XenotypeHolder, IExposable
	{

		public void ExposeData()
		{
			Scribe_Defs.Look(ref xenotypeDef, "xenotypeDef");
			Scribe_Defs.Look(ref iconDef, "iconDef");
			Scribe_Values.Look(ref name, "name");
			Scribe_Values.Look(ref inheritable, "inheritable");
			Scribe_Collections.Look(ref genes, "genes", LookMode.Def);
			if (Scribe.mode == LoadSaveMode.LoadingVars && genes != null && genes.RemoveAll((GeneDef x) => x == null) > 0)
			{
				Log.Warning("Removed null geneDef(s)");
			}
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                PostSetup();
            }
        }

        public void PostSetup()
        {
            if (xenotypeDef == null)
            {
                xenotypeDef = XenotypeDefOf.Baseliner;
            }
            if (genes == null)
            {
                genes = new();
            }
            if (xenotypeDef != XenotypeDefOf.Baseliner)
            {
                genes = xenotypeDef.genes;
                name = null;
                iconDef = null;
                inheritable = xenotypeDef.inheritable;
            }
        }

        public SaveableXenotypeHolder()
		{

		}

		public SaveableXenotypeHolder(XenotypeHolder holder)
		{
			xenotypeDef = holder.xenotypeDef;
			name = holder.name;
			iconDef = holder.iconDef;
			genes = holder.genes;
			inheritable = holder.inheritable;
		}

		public SaveableXenotypeHolder(XenotypeDef xenotypeDef)
		{
			//genes = new();
			//SaveableXenotypeHolder newHolder = new();
			this.xenotypeDef = xenotypeDef;
			this.genes = xenotypeDef.genes;
			this.inheritable = xenotypeDef.inheritable;
			this.iconDef = null;
			this.name = null;
			//xenotypeHolder = newHolder;
		}

		public SaveableXenotypeHolder(XenotypeDef xenotypeDef, List<GeneDef> genes, bool inheritable, XenotypeIconDef icon, string name)
		{
			//genes = new();
			//SaveableXenotypeHolder newHolder = new();
			this.xenotypeDef = xenotypeDef;
			this.genes = genes;
			this.inheritable = inheritable;
			this.iconDef = icon;
			this.name = name;
			//xenotypeHolder = newHolder;
		}

	}

	public class ReferencableXenotypeHolder : XenotypeHolder, IExposable
	{

		public void ExposeData()
		{
			Scribe_Defs.Look(ref xenotypeDef, "xenotypeDef");
			Scribe_Values.Look(ref name, "name");
		}

		public ReferencableXenotypeHolder()
		{

		}

		public ReferencableXenotypeHolder(Pawn pawn)
        {
			xenotypeDef = pawn.genes.Xenotype;
			name = pawn.genes.CustomXenotype?.name;
		}

        public ReferencableXenotypeHolder(XenotypeDef xenotypeDef)
		{
			this.xenotypeDef = xenotypeDef;
		}

	}

}
