using System;
using SQLite;
namespace MyFormsLibrary.Db.SQLite
{
	public static class SQLiteExtensions
	{
		public static bool TableExists<T>(this SQLiteConnection con) {
			const string cmdText = "SELECT name FROM sqlite_master WHERE type='table' AND name=?";
			var cmd = con.CreateCommand(cmdText, typeof(T).Name);
			return cmd.ExecuteScalar<string>() != null;
		}

		public static void DeleteWhere<T>(this SQLiteConnection con,string where) {
			var tableName = con.GetMapping(typeof(T)).TableName;
			var query = $"Delete From {tableName} Where {where}";
			con.Execute(query);
		}
	}
}

