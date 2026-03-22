using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Thought_PackMentality : Thought_Memory
	{

		public override bool ShouldDiscard => !Gene_PackMentality.ThePack.Contains(pawn);

		public override float MoodOffset()
		{
			//return base.MoodOffset() * Gene_PackMentality.ThePack.Count;
			return Gene_PackMentality.ThePack.Count - 1;
		}

	}

	public class Thought_Social_PackMentality : Thought_MemorySocial_NoFade
	{

		public override bool ShouldDiscard => !Gene_PackMentality.ThePack.Contains(pawn) || !Gene_PackMentality.ThePack.Contains(otherPawn);

	}

}
