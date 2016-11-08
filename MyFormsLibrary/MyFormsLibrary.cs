using System;
using MyFormsLibrary.Infrastructure;
namespace MyFormsLibrary
{
    public static class MyFormsLibrary
    {
        public static void Init(Type appType) {
            SvgLoader.Init(appType);
        }
    }
}
