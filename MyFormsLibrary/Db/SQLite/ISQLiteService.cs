using System;
using SQLite;
namespace MyFormsLibrary.Db.SQLite
{
	public interface ISQLiteService
	{
		bool IsInitialized { get; }
		SQLiteConnection GetConnection(string filename);
		SQLiteAsyncConnection GetAsyncConnection(string filename);
	}
}

