using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{
    namespace HarmonyPatches
    {

        [HarmonyPatch]
        public static class DrawGeneEyes_Patch
        {
            public static MethodBase TargetMethod()
            {
                Type[] nestedTypes = typeof(PawnRenderer).GetNestedTypes(AccessTools.all);
                foreach (Type type in nestedTypes)
                {
                    MethodInfo[] methods = type.GetMethods(AccessTools.all);
                    foreach (MethodInfo methodInfo in methods)
                    {
                        if (methodInfo.Name.Contains("DrawExtraEyeGraphic"))
                        {
                            return methodInfo;
                        }
                    }
                }
                return null;
            }

            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codeInstructions)
            {
                Type nestedType = typeof(PawnRenderer).GetNestedTypes(AccessTools.all).First((Type c) => c.Name.Contains("c__DisplayClass54_0"));
                FieldInfo thisClass = AccessTools.Field(nestedType, "<>4__this");
                FieldInfo pawnField = AccessTools.Field(thisClass.FieldType, "pawn");
                AccessTools.Field(typeof(GeneGraphicData), "drawScale");
                FieldInfo offsetField = AccessTools.Field(typeof(BodyTypeDef.WoundAnchor), "offset");
                yield return new CodeInstruction(OpCodes.Ldarg_0);
                yield return new CodeInstruction(OpCodes.Ldfld, thisClass);
                yield return new CodeInstruction(OpCodes.Ldfld, pawnField);
                yield return new CodeInstruction(OpCodes.Ldarg_2);
                yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(DrawGeneEyes_Patch), "SetEyeScale"));
                yield return new CodeInstruction(OpCodes.Starg_S, 2);
                int patched = 0;
                List<CodeInstruction> codes = codeInstructions.ToList();
                for (int i = 0; i < codes.Count; i++)
                {
                    CodeInstruction code = codes[i];
                    yield return code;
                    int num;
                    if (patched < 2 && code.LoadsField(offsetField))
                    {
                        if (!(codes[i - 1].opcode == OpCodes.Ldloc_3))
                        {
                            if (codes[i - 1].opcode == OpCodes.Ldloc_S)
                            {
                                object operand = codes[i - 1].operand;
                                if (operand is LocalBuilder lb)
                                {
                                    num = ((lb.LocalIndex == 4) ? 1 : 0);
                                    goto IL_0382;
                                }
                            }
                            num = 0;
                        }
                        else
                        {
                            num = 1;
                        }
                    }
                    else
                    {
                        num = 0;
                    }
                    goto IL_0382;
                IL_04a4:
                    int num2;
                    if (num2 != 0)
                    {
                        yield return new CodeInstruction(OpCodes.Ldloc_S, 6);
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(OpCodes.Ldfld, thisClass);
                        yield return new CodeInstruction(OpCodes.Ldfld, pawnField);
                        yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(DrawGeneEyes_Patch), "ChangeOffset"));
                        yield return new CodeInstruction(OpCodes.Stloc_S, 6);
                        continue;
                    }
                    int num3;
                    if (code.opcode == OpCodes.Stloc_S)
                    {
                        object operand = code.operand;
                        if (operand is LocalBuilder lb3)
                        {
                            num3 = ((lb3.LocalIndex == 8) ? 1 : 0);
                            goto IL_05f1;
                        }
                    }
                    num3 = 0;
                    goto IL_05f1;
                IL_05f1:
                    if (num3 != 0)
                    {
                        yield return new CodeInstruction(OpCodes.Ldloc_S, 8);
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(OpCodes.Ldfld, thisClass);
                        yield return new CodeInstruction(OpCodes.Ldfld, pawnField);
                        yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(DrawGeneEyes_Patch), "ChangeOffset"));
                        yield return new CodeInstruction(OpCodes.Stloc_S, 8);
                    }
                    continue;
                IL_0382:
                    if (num != 0)
                    {
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(OpCodes.Ldfld, thisClass);
                        yield return new CodeInstruction(OpCodes.Ldfld, pawnField);
                        yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(DrawGeneEyes_Patch), "ChangeOffset"));
                        patched++;
                        continue;
                    }
                    if (patched < 2)
                    {
                        continue;
                    }
                    if (code.opcode == OpCodes.Stloc_S)
                    {
                        object operand = code.operand;
                        if (operand is LocalBuilder lb2)
                        {
                            num2 = ((lb2.LocalIndex == 6) ? 1 : 0);
                            goto IL_04a4;
                        }
                    }
                    num2 = 0;
                    goto IL_04a4;
                }
            }

            public static float SetEyeScale(Pawn pawn, float scale)
            {
                foreach (Gene item in pawn.genes.GenesListForReading.Where((Gene x) => x.Active))
                {
                    if (item.Active)
                    {
                        GeneExtension_General modExtension = item.def.GetModExtension<GeneExtension_General>();
                        if (modExtension != null && modExtension.eyesShouldBeInvisble)
                        {
                            scale *= 0;
                        }
                    }
                }
                return scale;
            }

            public static Vector3 ChangeOffset(Vector3 offset, Pawn pawn)
            {
                foreach (Gene item in pawn.genes.GenesListForReading.Where((Gene x) => x.Active))
                {
                    if (item.Active)
                    {
                        GeneExtension_General modExtension = item.def.GetModExtension<GeneExtension_General>();
                        if (modExtension != null && modExtension.eyesShouldBeInvisble)
                        {
                            offset *= 0;
                        }
                    }
                }
                return offset;
            }
        }

    }


}
