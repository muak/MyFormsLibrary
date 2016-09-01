using System;
using SQLite;
using System.Threading.Tasks;
namespace MyFormsLibrary.Db.SQLite
{
	public static class SQLiteExtensions
	{
		public static bool TableExists<T>(this SQLiteConnection con) {
			var query = "SELECT name FROM sqlite_master WHERE type='table' AND name=?";
			var ret = con.ExecuteScalar<string>(query, new object[] { typeof(T).Name });

			return ret != null;
		}

		public static async Task<bool> TableExistsAsync<T>(this SQLiteAsyncConnection con) {
			var query = "SELECT name FROM sqlite_master WHERE type='table' AND name=?";
			var ret = await con.ExecuteScalarAsync<string>(query, new object[] { typeof(T).Name});

			return ret != null;
		}

		public static void DeleteWhere<T>(this SQLiteConnection con,string where) {
			var tableName = con.GetMapping(typeof(T)).TableName;
			var query = $"Delete From {tableName} Where {where}";

			con.Execute(query);
		}

		public static async void DeleteWhereAsync<T>(this SQLiteAsyncConnection con, string where) {
			var tableName = typeof(T).Name;
			var query = $"Delete From {tableName} Where {where}";

			await con.ExecuteAsync(query);
		}
	}
}

