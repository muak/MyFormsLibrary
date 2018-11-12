using System.Linq;
using System.Reflection;
using Prism;
using Prism.Ioc;
using Prism.Unity;
using Sample.Views;
using Xamarin.Forms.Internals;
using Xamarin.Forms;
using MyFormsLibrary.Navigation;
using Prism.Mvvm;
using System.Collections.Generic;
using Prism.Navigation;
using MyFormsLibrary.CustomRenderers;

[assembly: Xamarin.Forms.Xaml.XamlCompilation(Xamarin.Forms.Xaml.XamlCompilationOptions.Compile)]
namespace Sample
{
    public partial class App : PrismApplication
    {
        public App(IPlatformInitializer initializer = null) : base(initializer) { }


        protected override void OnInitialized()
        {
            InitializeComponent();
            MyFormsLibrary.MyFormsLibrary.Init(this.GetType());

            //NavigationService.NavigateAsync("TopNavi/MainPage");


            var nav = (MyPageNavigationService)Container.Resolve<INavigationServiceEx>(MyPageNavigationService.MyNavigationServiceName);


            if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.iOS) {
                MainPage = nav.CreateMainPageTabbedHasNavigation(nameof(MyTabbed), new List<NavigationPage> {
                    nav.CreateNavigationPage(nameof(NaviA),nameof(MainPage)),
                    nav.CreateNavigationPage(nameof(NaviB),nameof(SubPage)),
                });
            }
            else if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.Android) {
                MainPage = nav.CreateMainPageNavigationHasTabbed(nameof(TopNavi), nameof(MyTabbed),
                    new List<ContentPage>{
                        nav.CreateContentPage(nameof(SubPage)),
                        nav.CreateContentPage(nameof(MainPage)),
                        nav.CreateContentPage(nameof(SubPage)),
                        nav.CreateContentPage(nameof(SubPage)),
                }, new List<NavigationParameters>());
            }
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<ContentPage>();
            containerRegistry.RegisterForNavigation<CoordinatorPage>();

            this.GetType().GetTypeInfo().Assembly
            .DefinedTypes
            .Where(t => t.Namespace.EndsWith(".Views", System.StringComparison.Ordinal))
            .ForEach(t => {
                containerRegistry.RegisterForNavigation(t.AsType(), t.Name);
            });

        }

        protected override void RegisterRequiredTypes(IContainerRegistry containerRegistry) {
            base.RegisterRequiredTypes(containerRegistry);

            containerRegistry.Register<INavigationServiceEx, MyPageNavigationService>();    // VM以外でDIするために必要
            containerRegistry.Register<INavigationServiceEx, MyPageNavigationService>(MyPageNavigationService.MyNavigationServiceName);
        }

        protected override void ConfigureViewModelLocator() {
            ViewModelLocationProvider.SetDefaultViewModelFactory((view, type) => {
                return Container.ResolveViewModelForView(view, type);
            });
        }
    }
}

