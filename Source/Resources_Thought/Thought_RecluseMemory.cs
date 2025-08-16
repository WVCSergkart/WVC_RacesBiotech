using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

    public class Thought_RecluseMemory : Thought_Memory
    {

        public override float MoodOffset()
        {
            return base.MoodOffset() * (1 + (StaticCollectionsClass.cachedNonDeathrestingColonistsCount * 0.2f) - 0.4f);
        }

    }

    public class Thought_HumanCentricMemory : Thought_Memory
    {

        public override float MoodOffset()
        {
            return base.MoodOffset() * (1 + (StaticCollectionsClass.cachedNonHumansCount * 0.1f) - 0.1f);
        }

    }

    public class Thought_PackMentality : Thought_Memory
    {

        public override float MoodOffset()
        {
            return base.MoodOffset() * Gene_PackMentality.ThePack.Count;
        }

    }

}
