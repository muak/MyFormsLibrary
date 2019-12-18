using System;
using Prism.Navigation;

namespace MyFormsLibrary.Navigation
{
    public static class NavigationParametersExtensions
    {
        /// <summary>
        /// NavigationParametersを各画面のParametersに変換する
        /// </summary>
        /// <returns>The to.</returns>
        /// <param name="param">Parameter.</param>
        /// <typeparam name="T">変換する画面パラメータの型を指定</typeparam>
        public static T To<T>(this NavigationParameters param) where T : ParametersBase, new() {
            return param[ParametersBase.ParameterKey] as T;
        }

        public static T To<T>(this INavigationParameters param) where T : ParametersBase, new()
        {
            return param[ParametersBase.ParameterKey] as T;
        }
    }
}
