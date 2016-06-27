using System;
using MyFormsLibrary.Db.Realm;
using Realms;
namespace MyFormsLibrary.Droid.Tests.Db.Realm
{
    public class Person : RealmObject, IAutoIncrement
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Blood { get; set; }

    }


}

