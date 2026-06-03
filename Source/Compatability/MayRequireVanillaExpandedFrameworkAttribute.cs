using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WVC_XenotypesAndGenes
{

	public class MayRequireVanillaExpandedFrameworkAttribute : MayRequireAttribute
	{
		public MayRequireVanillaExpandedFrameworkAttribute()
			: base("oskarpotocki.vanillafactionsexpanded.core")
		{
		}
	}

}
