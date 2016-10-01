using System;
using System.IO;
using MyFormsLibrary.Db.SQLite;
using MyFormsLibrary.Droid.Db.SQLite;
using SQLite;
using Xamarin.Forms;

[assembly: Dependency(typeof(SQLiteService))]
namespace MyFormsLibrary.Droid.Db.SQLite
{
	public class SQLiteService:ISQLiteService
	{
		public SQLiteService() {
		}

		public bool IsInitialized { get; private set;}



		public SQLiteConnection GetConnection(string filename) {
			var path = GetPath(filename);

			IsInitialized = File.Exists(path);

			// Create the connection
			var conn = new SQLiteConnection(path);
			// Return the database connection
			return conn;
		}


		public SQLiteAsyncConnection GetAsyncConnection(string filename) {
			var path = GetPath(filename);

			IsInitialized = File.Exists(path);

			// Create the connection
			var conn = new SQLiteAsyncConnection(path);
			// Return the database connection
			return conn;
		}

		private string GetPath(string filename) {
			string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal); // Documents folder
			return Path.Combine(documentsPath, filename);
		}
	}
}

