using System;

namespace MyFormsLibrary.Navigation
{
	public interface ITabAction
	{
		void OnTabChangedFrom();
		void OnTabChangedTo(bool IsFirst);
	}
}

