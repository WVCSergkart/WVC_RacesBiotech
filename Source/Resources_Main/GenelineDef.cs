using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class GenelineDef : GeneDef
    {

        public int biostatEvo;
        public int biostatDeg;
		public int biostatCos;

		[NoTranslate]
		public string backgroundIconPath;

		[Unsaved(false)]
		private CachedTexture cachedBackIcon;
		public CachedTexture BackgroundIcon
		{
			get
			{
				if (cachedBackIcon == null)
				{
					if (backgroundIconPath.NullOrEmpty())
					{
						cachedBackIcon = XaG_UiUtility.GeneBackground_ArchiteEndogene;
					}
					else
					{
						cachedBackIcon = new(backgroundIconPath);
					}
				}
				return cachedBackIcon;
			}
		}

	}

}
