using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;

namespace SuperAwesomePerformance
{
	// Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
	public class SuperAwesomePerformance : Mod
	{
		public override void Load() {
			IL_Main.DrawFPS += IL_MainOnDrawFPS;
		}

		private void IL_MainOnDrawFPS(ILContext il) {
			ILCursor cursor = new(il);
			// cursor.GotoNext(MoveType.Before, instruction => instruction.MatchCall<String>(nameof(String.Concat)))
			cursor.GotoNext(MoveType.After, i => i.MatchLdsfld(typeof(Main), nameof(Main.frameRate)));
			cursor.EmitPop();
			cursor.EmitDelegate<Func<int>>(() => {
				int frameRate = Main.frameRate;
				int maxFrameRate = Main.graphics.SynchronizeWithVerticalRetrace ? (int)(TimeSpan.TicksPerSecond / Main.instance.TargetElapsedTime.Ticks) : 120;
				if (frameRate > maxFrameRate) {
					return frameRate;
				}
				
				if (frameRate < 60) {
					frameRate =  (int)(frameRate * linear_convert(frameRate, 0, 60, 1.6f, 1.2f));
				}
				else {
					frameRate = (int)(frameRate * 1.2f);
				}
				
				return Math.Clamp(frameRate, 0, maxFrameRate);
			});
			
			// cursor.EmitLdcI4(frameRate);
		}

		float linear_convert(float x, float oldMin, float oldMax, float newMin, float newMax) {
			return ((x - oldMin) / (oldMax - oldMin)) * (newMax - newMin) + newMin;
		}
	}
}
