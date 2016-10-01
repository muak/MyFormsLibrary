using System;

namespace MyFormsLibrary.Navigation
{
    public interface INavigationAction
    {
        void OnNavigatedBack();
        void OnNavigatedForward();
		void OnNavigatedTo(INavigationParameter param);
    }
}

