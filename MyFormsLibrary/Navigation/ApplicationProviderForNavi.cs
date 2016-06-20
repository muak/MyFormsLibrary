using System;
using Xamarin.Forms;

namespace MyFormsLibrary.Navigation
{
    public class ApplicationProviderForNavi:IApplicationProviderForNavi
    {
        public ApplicationProviderForNavi() {
            Application.Current.ModalPopped += (sender, e) => {
                ModalPopped(sender, e);
            };
        }

        public Page MainPage {
            get {
                return Application.Current.MainPage;
            }
            set {
                Application.Current.MainPage = value;
            }
        }

        public Action<object, ModalPoppedEventArgs> ModalPopped { get; set; }

    }
}

