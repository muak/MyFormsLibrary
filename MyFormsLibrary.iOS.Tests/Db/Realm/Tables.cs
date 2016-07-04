using System;
using MyFormsLibrary.Db.Realm;
using Realms;
using System.ComponentModel;
namespace MyFormsLibrary.iOS.Tests.Db.Realm
{
    public class Person : RealmObject, IAutoIncrement,INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Blood { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }


}

