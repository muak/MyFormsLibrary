using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Realms;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;

namespace MyFormsLibrary.Db.Realm
{
	
	public class TableController<T> 
		where T : RealmObject, IAutoIncrement, new()
	{
		
		static IdStore<T> Id;
		class IdStore<TDummy>
		{
			public IdStore(int lastid) {
				_id = lastid;
			}
			private int _id;
			public int Current { get { return _id; } }
			public int Next() {
				return ++_id;
			}
		}

		static ActionCache<T> SetProperty;
		class ActionCache<TDummy>:Dictionary<string, Action<TDummy, TDummy>>{}

		private Realms.Realm Db;

		public TableController(Realms.Realm db = null) {
			Db = db ?? Realms.Realm.GetInstance();

			if (Id == null) {
				var last = Db.All<T>().Any() ? Db.All<T>().OrderByDescending(x => x.Id).First().Id : 0;
				Id = new IdStore<T>(last);
			}
			if (SetProperty == null) {
				CreateSetPropertyAction();
			}
		}

		public RealmResults<T> Get() {
			return Db.All<T>();
		}

		public int Insert(T record) {
			Db.Write(() => {
				var newRec = Db.CreateObject<T>();

				CopyAll(newRec,record);
				newRec.Id = Id.Next();
			});
			return Id.Current;
		}

		public int Insert(IEnumerable<T> records) {

			Db.Write(() => {
				foreach (var rec in records) {
					var newRec = Db.CreateObject<T>();

					CopyAll(newRec,rec);
					newRec.Id = Id.Next();
				}
			});

			return Id.Current;
		}

        public int DeleteAt(int id) {

            var delcnt = 0;
            var rec = Db.All<T>().First(x => x.Id == id);
            if (rec == null) return delcnt;

            Db.Write(() => {
                Db.Remove(rec);
                delcnt++;
            });

            return delcnt;
        }

		public int Delete(Expression<Func<T, bool>> where) {

			var delcnt = 0;
			var recs = Db.All<T>().Where(where);

			Db.Write(() => {
				delcnt = recs.Count();
				Db.RemoveRange<T>(recs as RealmResults<T>);
			});

			return delcnt;
		}

		public int DeleteAll() {

			int delcnt = Db.All<T>().Count();

			Db.Write(() => {
				Db.RemoveAll<T>();
			});

			return delcnt;
		}

		public int UpdateAt(int id, T newValue) {

			int updcnt = 0;
			var rec = Db.All<T>().First(x => x.Id == id);
			if (rec == null) return updcnt;

			Db.Write(() => {
				CopyAll(rec,newValue);
				updcnt++;
			});

			return updcnt;
		}

		public int UpdateAt(int id, T newValue,Expression<Func<T,object>> select) {

			int updcnt = 0;
			var names = FilterPropertyName(select);
			if (names == null)
				return updcnt;


			var rec = Db.All<T>().First(x => x.Id == id);
			if (rec == null) return updcnt;

			Db.Write(() => {
				CopyRenge(rec,newValue,names);
				updcnt++;

			});

			return updcnt;
		}

		public int Update(T newValue,Expression<Func<T, bool>> where) {

			int updcnt = 0;
			var recs = Db.All<T>().Where(where).ToList();
           
			Db.Write(() => {
                foreach (var r in recs) {
					CopyAll(r,newValue);
					updcnt++;
                }
			});
          
			return updcnt;
		}

		public int Update(T newValue,Expression<Func<T, bool>> where, Expression<Func<T, object>> select) {

			int updcnt = 0;

			var names = FilterPropertyName(select);
			if (names == null)
				return updcnt;

			var recs = Db.All<T>().Where(where).ToList();

			Db.Write(() => {
				foreach (var r in recs) {
					CopyRenge(r,newValue,names);
					updcnt++;
				}

			});

			return updcnt;
		}

		private void CreateSetPropertyAction() {
			SetProperty = new ActionCache<T>();

			var prop = typeof(T).GetRuntimeProperties()
								.Where(x => x.DeclaringType == typeof(T) &&
									   x.Name != nameof(IAutoIncrement.Id));

			var setobj = Expression.Parameter(typeof(T), "set object");
			var getobj = Expression.Parameter(typeof(T), "get object");

			foreach (var p in prop) {
				var lambda = Expression.Lambda<Action<T, T>>(
					Expression.Assign(
						Expression.PropertyOrField(setobj, p.Name),
						Expression.PropertyOrField(getobj, p.Name)
					),
					setobj, getobj
				);

				SetProperty[p.Name] = lambda.Compile();
			}
		}

		private void CopyRenge(T to, T from, IEnumerable<string> propertyNames) {
			foreach (var name in propertyNames) {
				if (SetProperty.ContainsKey(name)) {
					SetProperty[name](to, from);
				}
			}
		}

		private void CopyAll(T to, T from) {
			foreach (var kv in SetProperty) {
				kv.Value(to, from);
			}
		}

		private IEnumerable<string> FilterPropertyName(Expression<Func<T, object>> filter) {
			var list = new List<string>();
			if (filter.Body is NewExpression) {
				var body = filter.Body as NewExpression;
				foreach (var m in body.Members) {
                    if (SetProperty.ContainsKey(m.Name)){
                        list.Add(m.Name);
                    }
				}
			}
			else if (filter.Body is MemberExpression) {
				var body = filter.Body as MemberExpression;
                if (SetProperty.ContainsKey(body.Member.Name)){
                    list.Add(body.Member.Name);
                }
			}
			else {
				return null;
			}

            if (list.Count == 0) {
                return null;
            }
			return list;
		}

	}

	public static class RealmExtensions
	{

		public static ObservableTable<T> ToObservableTable<T>(this IOrderedQueryable<T> realm)
			where T : RealmObject 
		{
			return Create<T>(realm);
		}
		public static ObservableTable<T> ToObservableTable<T>(this IQueryable<T> realm)
			where T : RealmObject 
		{
			return Create<T>(realm);
		}
		public static ObservableTable<T> ToObservableTable<T>(this RealmResults<T> realm)
			where T : RealmObject 
		{
			return Create<T>(realm);
		}

		private static ObservableTable<T> Create<T>(object realm) where T : RealmObject 
		{
			return new ObservableTable<T>(realm as RealmResults<T>);
		}
	}

	public class ObservableTable<T> : ObservableCollection<T>, IDisposable
		where T : RealmObject
	{
		IDisposable sub;

		public ObservableTable(RealmResults<T> realm) : base(realm) {
            
			sub = realm.SubscribeForNotifications((sender, changes, error) => {
				
				if (changes != null) {
					var ret = sender.ToList();
					foreach (var ins in changes.InsertedIndices) {
						InsertItem(ins, ret[ins]);
					}
                    foreach (var del in changes.DeletedIndices.Reverse()) {
                        if (this.Count <= del){
                            continue;
                        }
						RemoveAt(del);
					}
					foreach (var mod in changes.ModifiedIndices) {
						SetItem(mod, ret[mod]);
					}
				}
			});
		}

		public void Dispose() {
			sub.Dispose();
		}
	}
}

