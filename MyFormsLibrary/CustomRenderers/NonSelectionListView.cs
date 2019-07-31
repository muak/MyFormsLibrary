using System;
using System.Windows.Input;
using Xamarin.Forms;
namespace MyFormsLibrary.CustomRenderers
{
	public class NonSelectionListView : ListView
	{

		public NonSelectionListView():base(ListViewCachingStrategy.RecycleElement) 
        {
            SetLoadMoreCompletion = (isEnd) => {
                SetLoadMoreCompletionAction?.Invoke(isEnd);
            };
        }

        public NonSelectionListView(ListViewCachingStrategy strategy) : base(strategy) 
        {
            SetLoadMoreCompletion = (isEnd) => {
                SetLoadMoreCompletionAction?.Invoke(isEnd);
            };
        }

        public static BindableProperty LoadMoreCommandProperty =
            BindableProperty.Create(
                nameof(LoadMoreCommand),
                typeof(ICommand),
                typeof(NonSelectionListView),
                default(ICommand),
                defaultBindingMode: BindingMode.OneWay
            );


        public ICommand LoadMoreCommand {
            get { return (ICommand)GetValue(LoadMoreCommandProperty); }
            set { SetValue(LoadMoreCommandProperty, value); }
        }

        internal Action<bool> SetLoadMoreCompletionAction;

        public static BindableProperty SetLoadMoreCompletionProperty =
            BindableProperty.Create(
                nameof(SetLoadMoreCompletion),
                typeof(Action<bool>),
                typeof(NonSelectionListView),
                default(Action),
                defaultBindingMode: BindingMode.OneWayToSource
            );

        public Action<bool> SetLoadMoreCompletion {
            get { return (Action<bool>)GetValue(SetLoadMoreCompletionProperty); }
            set { SetValue(SetLoadMoreCompletionProperty, value); }
        }

        public static BindableProperty LoadMoreMarginProperty =
            BindableProperty.Create(
                nameof(LoadMoreMargin),
                typeof(int),
                typeof(NonSelectionListView),
                default(int),
                defaultBindingMode: BindingMode.OneWay
            );

        public int LoadMoreMargin {
            get { return (int)GetValue(LoadMoreMarginProperty); }
            set { SetValue(LoadMoreMarginProperty, value); }
        }
    }
}

