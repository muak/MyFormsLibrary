﻿using System;
using System.Threading.Tasks;
using Prism.Navigation;
using Xamarin.Forms;

namespace MyFormsLibrary.Navigation
{
    public interface INavigationServiceEx:INavigationService
    {
        Task Navigate<T>(ParametersBase parameters = null, bool animated = true) where T : ContentPage;
        Task NavigateModal<T>(ParametersBase parameters = null, bool animated = true) where T : ContentPage;
        Task NavigateModal<Tnavi, Tpage>(ParametersBase parameters = null, bool animated = true) where Tnavi : NavigationPage where Tpage : ContentPage;

        Task GoBackModalAsync(bool animated = true);
        bool ChangeTab<T>() where T : Page;
    }
}
