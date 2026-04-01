using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class ReincarnationSet : IExposable
	{

		public Pawn asker;
		public List<QuestScriptDef> questScriptDefs;

		public ReincarnationSet()
		{

		}

		public ReincarnationSet(Pawn pawn, QuestScriptDef questScriptDef, int babies = 1)
		{
			asker = pawn;
			questScriptDefs = new();
			for (int i = 0; i < babies; i++)
			{
				questScriptDefs.Add(questScriptDef);
			}
		}

		public void ExposeData()
		{
			Scribe_References.Look(ref asker, "asker", saveDestroyedThings: true);
			Scribe_Collections.Look(ref questScriptDefs, "questScriptDefs", LookMode.Def);
			if (Scribe.mode == LoadSaveMode.LoadingVars && questScriptDefs != null && questScriptDefs.RemoveAll((QuestScriptDef x) => x == null) > 0)
			{
				Log.Warning("Removed null questScriptDef(s)");
			}
		}

	}

}
