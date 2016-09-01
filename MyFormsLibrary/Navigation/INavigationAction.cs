﻿using System;

namespace MyFormsLibrary.Navigation
{
    public interface INavigationAction
    {
        void OnNavigatedBack();
        void OnNavigatedFoward();
		void OnNavigatedTo(INavigationParameter param);
    }
}

