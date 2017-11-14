using System;
using Prism.Navigation;

namespace MyFormsLibrary.Navigation
{
    public abstract class ParametersBase
    {
        internal const string ParameterKey = "kamusoft";
        /// <summary>
        /// 画面パラメータをPrism.Navigation.NavigationParametersに変換する
        /// </summary>
        /// <returns>The navigation parameters.</returns>
        public NavigationParameters ToNavigationParameters() {
            return new NavigationParameters{
                {ParameterKey,this}
            };
        }
    }
}
