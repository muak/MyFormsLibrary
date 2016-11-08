using System;
using System.IO;
using System.Linq;
using System.Reflection;
using NGraphics;
using System.Collections.Generic;

namespace MyFormsLibrary.Infrastructure
{
	public static class SvgLoader
	{
		static Type AppType;
		static Dictionary<string, Graphic> IconCache;

		public static void Init(Type appType) {
			AppType = appType;
			IconCache = new Dictionary<string, Graphic>();
		}

		public static Graphic GetResourceAndLoadSvg(string path) {
			if (AppType == null) return null;
			if (path == null) return null;

			if (IconCache.ContainsKey(path)) {
				return IconCache[path];
			}

			var asm = AppType.GetTypeInfo().Assembly;

			var resource = asm.GetManifestResourceNames()
							  .FirstOrDefault(x => x.EndsWith(path, StringComparison.CurrentCultureIgnoreCase));
			if (resource == null) {
				return null;
			}
			using (var stream = asm.GetManifestResourceStream(resource)) {
				using (var sr = new StreamReader(stream)) {
					IconCache[path] = Graphic.LoadSvg(sr);
					return IconCache[path];
				}
			}

		}

	}
}

