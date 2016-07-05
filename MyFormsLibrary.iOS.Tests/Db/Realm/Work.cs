using System;
using NUnit.Framework;
using NUnit.Framework.Internal;
using MyFormsLibrary.Db.Realm;

namespace MyFormsLibrary.iOS.Tests.Db.Realm
{
    [TestFixture]
    public class Work
    {
        private TableController<Cat> Cat;

        [SetUp]
        public void SetUp() {
            Cat = new TableController<Cat>();
        }
        [TearDown]
        public void TearDonw() {
            var ret = Cat.Get();
            var ret2 = new TableController<Person>().Get();
        }

        [Test]
        public void Relation() {
            Cat.Insert(new Cat {
                Name = "Tama",
                Owner = new Person {
                    Name="Boss",
                    Age=26,
                }
            });

        }

        [Test]
        public void Update() {
            Cat.UpdateAt(1,new Cat {
                Name = "Koro",
                Owner = new Person {
                    Name = "Papa",
                    Age = 31,
                }
            });
        }
    }
}

