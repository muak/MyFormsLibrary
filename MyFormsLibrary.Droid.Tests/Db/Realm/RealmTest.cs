using System;
using NUnit.Framework;
using Realms;
using MyFormsLibrary.Db.Realm;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Xml;

namespace MyFormsLibrary.Droid.Tests.Db.Realm
{
    [TestFixture]
    public class RealmTest
    {
        private void init() {
            var p = new List<Person>{
                new Person{Name = "Delete", Age = 20, Blood = "A" },
                new Person{Name = "Delete", Age = 21, Blood = "B" },
                new Person{Name = "Delete", Age = 22, Blood = "AB" },
                new Person{Name = "Delete", Age = 20, Blood = "O"},
                new Person{Name = "Test1", Age = 20, Blood = "A" },
                new Person{Name = "Test2", Age = 21, Blood = "B" },
                new Person{Name = "Test3", Age = 22, Blood = "AB" },
                new Person{Name = "Test4", Age = 20, Blood = "O"}
            };
            tbl.Insert(p);
            tbl.Delete(zz => zz.Name == "UpdatedMulti");
            tbl.Delete(zz => zz.Name == "UpdateMulti");

            //ObservableCollectionの同期確認はここでは行わない
            obsv = tbl.Get().Where(x => !x.Name.Contains("Exclusion")).OrderByDescending(x => x.Id).ToObservableTable();

        }

        [SetUp]
        public void SetUp() {

            db = Realms.Realm.GetInstance();
            tbl = new TableController<Person>(db);
            init();

        }
        [TearDown]
        public void TearDown() {

        }

        private Realms.Realm db;
        private TableController<Person> tbl;
        public ObservableTable<Person> obsv { get; set; }


        [TestCase("tarou")]
        [TestCase("Exclusion")]
        public void InsertOne(string name) {
            var precnt = tbl.Get().Count();
            var p = new Person { Name = name, Age = 20, Blood = "A" };

            var nextid = tbl.Insert(p);
            var cnt = tbl.Get().Count();

            var newP = tbl.Get().First(x => x.Id == nextid);

            (cnt - precnt).Is(1);
            IsPropertyOK(newP, p, nextid);

        }


        [TestCase("test")]
        [TestCase("Exclusion")]
        public void InsertMulti(string name) {
            var precnt = tbl.Get().Count();
            var p = new List<Person>{
                new Person{Name = name+"1", Age = 20, Blood = "A" },
                new Person{Name = name+"2", Age = 21, Blood = "B" },
                new Person{Name = name+"3", Age = 22, Blood = "AB" },
                new Person{Name = name+"4", Age = 20, Blood = "O"}
            };

            var id = tbl.Insert(p) - (p.Count - 1);
            var cnt = tbl.Get().Count();


            var newP = tbl.Get().Where(x => x.Id >= id);

            (cnt - precnt).Is(p.Count);
            IsPropertyOKAll(newP.ToArray(), p.ToArray(), id);
        }

        [Test]
        public void Z_DeleteOne() {
            var precnt = tbl.Get().Count();
            var targetId = PickRandom().Id;
            var delcnt = tbl.Delete(x => x.Id == targetId);
            var cnt = tbl.Get().Count();

            (cnt - precnt).Is(-1);
            delcnt.Is(1);

            tbl.Get().Any(x => x.Id == targetId).IsFalse();

            init();
        }

        [Test]
        public void Z_DeleteMulti() {
            var precnt = tbl.Get().Count(zz => zz.Name == "Delete");

            var delcnt = tbl.Delete(x => x.Name == "Delete");
            var cnt = tbl.Get().Count(zz => zz.Name == "Delete");

            (precnt - cnt).Is(delcnt);
            delcnt.Is(precnt);

            tbl.Get().Any(zz => zz.Name == "Delete").IsFalse();

            init();
        }

        [Test]
        public void Z_DeleteAt() {
            var precnt = tbl.Get().Count();
            var targetId = PickRandom().Id;
            var delcnt = tbl.DeleteAt(targetId);
            var cnt = tbl.Get().Count();

            (cnt - precnt).Is(-1);
            delcnt.Is(1);

            tbl.Get().Any(x => x.Id == targetId).IsFalse();

            init();
        }

        [Test]
        public void ZZ_DeleteAll() {
            var precnt = tbl.Get().Count();
            var delcnt = tbl.DeleteAll();
            var cnt = tbl.Get().Count();

            precnt.Is(delcnt);
            cnt.Is(0);

            init();
        }

        [Test]
        public void UpdateAt() {
            var targetId = PickRandom().Id;
            var newP = new Person {
                Name = "Updated",
                Age = 99,
                Blood = "O",
                Id = 999
            };

            var cnt = tbl.UpdateAt(targetId, newP);

            var p = tbl.Get().First(x => x.Id == targetId);

            cnt.Is(1);
            UpdateOK(p, newP);

            tbl.DeleteAt(targetId);
        }

        [Test]
        public void UpdateAtFilterNew() {
            var targetId = PickRandom().Id;
            var newP = new Person {
                Name = "Updated",
                Age = 99,
                Blood = "X",
                Id = 999
            };

            var cnt = tbl.UpdateAt(targetId, newP, xx => new { xx.Name, xx.Age });

            var p = tbl.Get().First(x => x.Id == targetId);

            cnt.Is(1);
            p.Id.IsNot(newP.Id);
            p.Name.Is(newP.Name);
            p.Age.Is(newP.Age);
            p.Blood.IsNot(newP.Blood);

            tbl.DeleteAt(targetId);
        }

        [Test]
        public void UpdateAtFilterMember() {
            var targetId = PickRandom().Id;
            var newP = new Person {
                Name = "Updated",
                Age = 99,
                Blood = "X",
                Id = 999
            };

            var cnt = tbl.UpdateAt(targetId, newP, xx => xx.Name);

            var p = tbl.Get().First(x => x.Id == targetId);

            cnt.Is(1);
            p.Id.IsNot(newP.Id);
            p.Name.Is(newP.Name);
            p.Age.IsNot(newP.Age);
            p.Blood.IsNot(newP.Blood);

            tbl.DeleteAt(targetId);
        }


        static IEnumerable<TestCaseData> SrcSelect {
            get {
                yield return new TestCaseData(MakeSelect(xx => xx.Age == 99));
                yield return new TestCaseData(MakeSelect(xx => xx.Id + 1));
                yield return new TestCaseData(MakeSelect(xx => xx.Name.Select(x => true)));
                yield return new TestCaseData(MakeSelect(xx => new { Hoge = 1, Fuga = "ABC" }));
            }
        }

        [TestCaseSource("SrcSelect")]
        public void UpdateAtFilterInvalid(Expression<Func<Person, object>> select) {
            var targetId = PickRandom().Id;
            var newP = new Person {
                Name = "Updated",
                Age = 99,
                Blood = "X",
                Id = 999
            };

            var cnt = tbl.UpdateAt(targetId, newP, select);

            var p = tbl.Get().First(x => x.Id == targetId);

            cnt.Is(0);
            p.Id.IsNot(newP.Id);
            p.Name.IsNot(newP.Name);
            p.Age.IsNot(newP.Age);
            p.Blood.IsNot(newP.Blood);

            tbl.DeleteAt(targetId);
        }

        [Test]
        public void UpdateOne() {
            var targetId = PickRandom().Id;
            var newP = new Person {
                Name = "Updated",
                Age = 99,
                Blood = "O",
                Id = 999
            };

            var cnt = tbl.Update(newP, zz => zz.Id == targetId);

            var p = tbl.Get().First(x => x.Id == targetId);

            cnt.Is(1);
            UpdateOK(p, newP);

            tbl.DeleteAt(targetId);
        }

        [Test]
        public void UpdateMulti() {
            var targetId = PickRandom().Id;
            var testdata = new List<Person>{
                new Person{Name = "UpdateMulti", Age = 999, Blood = "XA" },
                new Person{Name = "UpdateMulti", Age = 998, Blood = "XB" },
                new Person{Name = "UpdateMulti", Age = 997, Blood = "AXB" },
            };
            tbl.Insert(testdata);

            var newP = new Person {
                Name = "UpdatedMulti",
                Age = 99,
                Blood = "O",
                Id = 999
            };

            var cnt = tbl.Update(newP, zz => zz.Name == "UpdateMulti");

            var p = tbl.Get().Where(x => x.Name == "UpdatedMulti");

            cnt.Is(3);
            UpdateOKAll(p.ToArray(), newP);

            tbl.Delete(zz => zz.Name == "UpdatedMulti");
        }


        static IEnumerable<TestCaseData> SrcUpdateFilter {
            get {
                yield return new TestCaseData(MakeSelect(xx => xx.Name));
                yield return new TestCaseData(MakeSelect(xx => new { xx.Name }));
            }
        }
        [TestCaseSource("SrcUpdateFilter")]
        public void UpdateFilter(Expression<Func<Person, object>> select) {
            var targetId = PickRandom().Id;
            var newP = new Person {
                Name = "Updated",
                Age = 99,
                Blood = "X",
                Id = 999
            };

            var cnt = tbl.Update(newP, xx => xx.Id == targetId, select);

            var p = tbl.Get().First(x => x.Id == targetId);

            cnt.Is(1);
            p.Id.IsNot(newP.Id);
            p.Name.Is(newP.Name);
            p.Age.IsNot(newP.Age);
            p.Blood.IsNot(newP.Blood);

            tbl.DeleteAt(targetId);
        }


        static IEnumerable<TestCaseData> SrcUpdateFilterInvalid {
            get {
                yield return new TestCaseData(MakeSelect(xx => xx.Age == 99));
                yield return new TestCaseData(MakeSelect(xx => xx.Id + 1));
                yield return new TestCaseData(MakeSelect(xx => xx.Name.Select(x => true)));
                yield return new TestCaseData(MakeSelect(xx => new { Hoge = 1, Fuga = "ABC" }));
            }
        }
        [TestCaseSource("SrcUpdateFilterInvalid")]
        public void UpdateFilterInvalid(Expression<Func<Person, object>> select) {
            var targetId = PickRandom().Id;
            var newP = new Person {
                Name = "Updated",
                Age = 99,
                Blood = "X",
                Id = 999
            };

            var cnt = tbl.Update(newP, zz => zz.Id == targetId, select);

            var p = tbl.Get().First(x => x.Id == targetId);

            cnt.Is(0);
            p.Id.IsNot(newP.Id);
            p.Name.IsNot(newP.Name);
            p.Age.IsNot(newP.Age);
            p.Blood.IsNot(newP.Blood);

            tbl.DeleteAt(targetId);
        }


        static Expression<Func<Person, object>> MakeSelect(Expression<Func<Person, object>> select) {
            return select;
        }

        private void UpdateOK(Person a, Person b) {
            a.Id.IsNot(b.Id);
            a.Name.Is(b.Name);
            a.Age.Is(b.Age);
            a.Blood.Is(b.Blood);
        }

        private void UpdateOKAll(Person[] a, Person b) {
            for (var i = 0; i < a.Length; i++) {
                a[i].Id.IsNot(b.Id);
                a[i].Name.Is(b.Name);
                a[i].Age.Is(b.Age);
                a[i].Blood.Is(b.Blood);
            }
        }

        private void IsPropertyOK(Person a, Person b, int id) {
            a.Id.Is(id);
            a.Name.Is(b.Name);
            a.Age.Is(b.Age);
            a.Blood.Is(b.Blood);
        }

        private void IsPropertyOKAll(Person[] a, Person[] b, int id) {
            for (var i = 0; i < a.Length; i++) {
                a[i].Id.Is(id++);
                a[i].Name.Is(b[i].Name);
                a[i].Age.Is(b[i].Age);
                a[i].Blood.Is(b[i].Blood);
            }
        }

        private Person PickRandom() {
            var recs = tbl.Get().ToArray();
            Random r = new Random();
            var idx = r.Next(0, recs.Count() - 1);
            return recs[idx];

        }
    }


}

