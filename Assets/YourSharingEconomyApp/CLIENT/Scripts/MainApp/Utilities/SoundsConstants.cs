using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YourCommonTools;

namespace YourSharingEconomyApp
{
	public class SoundsConstants
	{
		// ----------------------------------------------
		// CONSTANTS
		// ----------------------------------------------
		public const string SOUND_MAIN_MENU = "SOUND_MAIN_MENU";
		public const string SOUND_SELECTION_FX = "SOUND_FX_SELECTION";
		public const string SOUND_FX_SUB_SELECTION = "SOUND_FX_SUB_SELECTION";

		// -------------------------------------------
		/* 
		 * PlayMainMenu
		 */
		public static void PlayMainMenu()
		{			
			SoundsController.Instance.PlayLoopSound(SOUND_MAIN_MENU);
		}

		// -------------------------------------------
		/* 
		 * PlaySingleSound
		 */
		public static void PlayFxSelection()
		{
			SoundsController.Instance.Enabled = false;
			SoundsController.Instance.PlaySingleSound(SOUND_SELECTION_FX);
		}

		// -------------------------------------------
		/* 
		 * PlayFxSubSelection
		 */
		public static void PlayFxSubSelection()
		{
			SoundsController.Instance.PlaySingleSound(SOUND_FX_SUB_SELECTION);
		}

	}
}
