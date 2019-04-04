using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MyFormsLibrary.Infrastructure
{
    public static class SvgLoader
	{
		static Type AppType;

		public static void Init(Type appType) {
			AppType = appType;
		}

        public static Stream GetResourceStream(string path)
        {
            if (AppType == null) return null;
            if (path == null) return null;

            var asm = AppType.GetTypeInfo().Assembly;

            var resource = asm.GetManifestResourceNames()
                              .FirstOrDefault(x => x.EndsWith(path, StringComparison.CurrentCultureIgnoreCase));
            if (resource == null)
            {
                return null;
            }
            return asm.GetManifestResourceStream(resource);
        }

    }
}

