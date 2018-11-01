using Prism.Unity;
using Sample.Views;
using Xamarin.Forms;

namespace Sample
{
	public partial class App : PrismApplication
	{
		public App(IPlatformInitializer initializer = null) : base(initializer) { }

		protected override void OnInitialized()
		{
			InitializeComponent();
            MyFormsLibrary.MyFormsLibrary.Init(this.GetType());
            NavigationService.NavigateAsync("MyNavigationPage/MenuPage");
		}

		protected override void RegisterTypes()
		{
            Container.RegisterTypeForNavigation<MenuPage>();
			Container.RegisterTypeForNavigation<MainPage>();
            Container.RegisterTypeForNavigation<MyNavigationPage>();
		}
	}
}

