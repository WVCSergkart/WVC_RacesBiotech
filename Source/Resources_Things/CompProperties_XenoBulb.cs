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

		// public int geneticMaterial_Cpx = 0;
		// public int geneticMaterial_Met = 0;
		// public int geneticMaterial_Arc = 0;
		// public int geneticMaterial_Tox = 0;

		private int tickCounter;

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
				// GetAllXenoTreesOnMap();
				Reset();
			}
		}

		public void Reset()
		{
			tickCounter = Props.ticksBetweenSpawn.RandomInRange;
			// if (!xenoTrees.NullOrEmpty())
			// {
				// mainXenoTree = xenoTrees.RandomElement().TryGetComp<CompXenoTree>();
			// }
		}

		public override void CompTick()
		{
			base.CompTick();
			Tick(1);
		}

		public override void CompTickRare()
		{
			base.CompTickRare();
			Tick(250);
		}

		public override void CompTickLong()
		{
			base.CompTickRare();
			Tick(2000);
		}

		public void Tick(int tick)
		{
			tickCounter -= tick;
			if (tickCounter > 0)
			{
				return;
			}
			GenerateGenMat();
			Reset();
		}

		public void GenerateGenMat()
		{
			// if (xenoTrees.NullOrEmpty() || mainXenoTree == null)
			// {
				// GetAllXenoTreesOnMap();
			// }
			if (mainXenoTree != null)
			{
				if (Rand.Chance(0.8f) && GetGenMatLimit() > mainXenoTree.geneticMaterial_Cpx)
				{
					mainXenoTree.geneticMaterial_Cpx += Props.geneticMaterialProduction.RandomInRange;
				}
				if (Rand.Chance(0.4f) && parent.Position.IsPolluted(parent.Map) && GetGenMatLimit() > mainXenoTree.geneticMaterial_Tox)
				{
					mainXenoTree.geneticMaterial_Tox += Props.geneticMaterialProduction.RandomInRange;
				}
				else if (Rand.Chance(0.6f) && parent.Position.GetFertility(parent.Map) >= Props.minFertilityForMetabolism && GetGenMatLimit() > mainXenoTree.geneticMaterial_Met)
				{
					mainXenoTree.geneticMaterial_Met += Props.geneticMaterialProduction.RandomInRange;
				}
				if (Rand.Chance(0.09f) && GetGenMatLimit() > mainXenoTree.geneticMaterial_Arc)
				{
					mainXenoTree.geneticMaterial_Arc += Props.geneticMaterialProduction.RandomInRange;
				}
			}
		}

		public CompXenoTree mainXenoTree;

		// public List<Thing> xenoTrees;

		private int GetGenMatLimit()
		{
			if (mainXenoTree == null)
			{
				return 0;
			}
			return mainXenoTree.connectedBulbs.Count * Props.geneticMaterialLimitPerBulb;
		}

		private List<Thing> GetAllXenoTreesOnMap()
		{
			List<Thing> trees = new();
			List<Thing> allThings = parent.Map.listerThings.AllThings;
			foreach (Thing item in allThings)
			{
				// CompXenoTree tree = item.TryGetComp<CompXenoTree>();
				if (item.def.plant != null && item.TryGetComp<CompXenoTree>() != null)
				{
					trees.Add(item);
				}
			}
			return trees;
			// mainXenoTree = xenoTrees.RandomElement().TryGetComp<CompXenoTree>();
		}

		private int rootsCooldown = 0;

		public override void PostDrawExtraSelectionOverlays()
		{
			if (mainXenoTree != null && mainXenoTree.parent != null)
			{
				GenDraw.DrawLineBetween(parent.TrueCenter(), mainXenoTree.parent.TrueCenter(), SimpleColor.Yellow);
			}
			// mainXenoTree?.parent?.DrawConnectionLine(parent);
		}

		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: Generate materials",
					action = delegate
					{
						GenerateGenMat();
						Reset();
					}
				};
			}
			if (Find.TickManager.TicksGame < rootsCooldown)
			{
				yield break;
			}
			yield return new Command_Action
			{
				defaultLabel = "WVC_XaG_XenoTreeChooseLabel".Translate(),
				defaultDesc = "WVC_XaG_XenoTreeChooseDesc".Translate(),
				icon = parent.def.uiIcon,
				action = delegate
				{
					List<FloatMenuOption> list = new();
					List<Thing> xenoTrees = GetAllXenoTreesOnMap();
					for (int i = 0; i < xenoTrees.Count; i++)
					{
						Thing xenoTree = xenoTrees[i];
						list.Add(new FloatMenuOption((i + 1).ToString() + " " + xenoTree.def.LabelCap, delegate
						{
							if (mainXenoTree != null)
							{
								mainXenoTree.connectedBulbs.Remove(parent);
							}
							mainXenoTree = xenoTree.TryGetComp<CompXenoTree>();
							xenoTree.TryGetComp<CompXenoTree>().connectedBulbs.Add(parent);
							rootsCooldown = Find.TickManager.TicksGame + Props.rootsConnectionCooldown;
							Messages.Message("WVC_XaG_XenoTreeChooseAssigned".Translate(parent.LabelCap, xenoTree.LabelCap), null, MessageTypeDefOf.NeutralEvent, historical: false);
						}));
					}
					if (list.Any())
					{
						Find.WindowStack.Add(new FloatMenu(list));
					}
				}
			};
		}

		public override void PostDestroy(DestroyMode mode, Map previousMap)
		{
			if (mainXenoTree != null)
			{
				mainXenoTree.connectedBulbs.Remove(parent);
			}
		}

		public override string CompInspectStringExtra()
		{
			if (mainXenoTree == null)
			{
				return "WVC_XaG_XenoTreeXenoBulb_ReqRootConnection".Translate(parent.LabelCap).Colorize(ColorLibrary.RedReadable);
			}
			StringBuilder stringBuilder = new(base.CompInspectStringExtra());
			if (Find.TickManager.TicksGame < rootsCooldown)
			{
				stringBuilder.AppendLine(string.Format("{0}", "WVC_XaG_XenoTreeXenoBulb_CooldownRootConnection".Translate(rootsCooldown.ToStringTicksToPeriod())));
			}
			stringBuilder.AppendLine(string.Format("{0}", "WVC_XaG_XenoTreeXenoBulb_Cpx".Translate(mainXenoTree.geneticMaterial_Cpx.ToString(), GetGenMatLimit().ToString())));
			if (parent.Position.IsPolluted(parent.Map))
			{
				stringBuilder.AppendLine(string.Format("{0}", "WVC_XaG_XenoTreeXenoBulb_Tox".Translate(mainXenoTree.geneticMaterial_Tox.ToString(), GetGenMatLimit().ToString())));
			}
			else if (parent.Position.GetFertility(parent.Map) >= Props.minFertilityForMetabolism)
			{
				stringBuilder.AppendLine(string.Format("{0}", "WVC_XaG_XenoTreeXenoBulb_Met".Translate(mainXenoTree.geneticMaterial_Met.ToString(), GetGenMatLimit().ToString())));
			}
			stringBuilder.AppendLine(string.Format("{0}", "WVC_XaG_XenoTreeXenoBulb_Arc".Translate(mainXenoTree.geneticMaterial_Arc.ToString(), GetGenMatLimit().ToString())));
			stringBuilder.Append(string.Format("{0}", "WVC_XaG_XenoTreeXenoBulb_NextGenMat".Translate(tickCounter.ToStringTicksToPeriod())));
			return stringBuilder.ToString();
		}

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look(ref tickCounter, "tickCounter_" + Props.uniqueTag, 0);
			Scribe_Values.Look(ref rootsCooldown, "rootsCooldown_" + Props.uniqueTag, 0);
			Scribe_Values.Look(ref mainXenoTree, "mainXenoTree");
		}
	}

}
