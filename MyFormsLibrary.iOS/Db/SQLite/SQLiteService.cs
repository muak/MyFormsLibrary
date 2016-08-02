using System;
using System.IO;
using MyFormsLibrary.Db.SQLite;
using MyFormsLibrary.iOS.Db.SQLite;
using SQLite;
using Xamarin.Forms;

[assembly: Dependency(typeof(SQLiteService))]
namespace MyFormsLibrary.iOS.Db.SQLite
{
	public class SQLiteService:ISQLiteService
	{
		public SQLiteService() {

		}

		public bool IsInitialized { get; private set; }

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
			string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // Documents folder
			string libraryPath = Path.Combine(documentsPath, "..", "Library"); // Library folder
			return Path.Combine(libraryPath, filename);
		}

	}
}

