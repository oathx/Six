using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Mono.Data.Sqlite;
using System.Text;

/// <summary>
/// I sql meta.
/// </summary>
public abstract class ISqlPackage : IEventArgs
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ISqlPackage"/> class.
	/// </summary>
	public ISqlPackage()
	{}
	
	/// <summary>
	/// Decode the specified sdr.
	/// </summary>
	/// <param name="sdr">Sdr.</param>
	public abstract void 	Decode (SqliteDataReader sdr);
}

/// <summary>
/// I net message factory.
/// </summary>
public abstract class ISqlPackageFactory
{
	/// <summary>
	/// Creates the net message.
	/// </summary>
	/// <returns>The net message.</returns>
	/// <param name="bytes">Bytes.</param>
	public abstract ISqlPackage	Create (SqliteDataReader sdr);
	
	/// <summary>
	/// Gets the type of the package.
	/// </summary>
	/// <returns>The package type.</returns>
	public abstract string		GetTypeName ();
}

/// <summary>
/// Default net message factory.
/// </summary>
public class DefaultSqlPackageFactory<T> : ISqlPackageFactory where T : ISqlPackage, new()
{
	/// <summary>
	/// Creates the net message.
	/// </summary>
	/// <returns>The net message.</returns>
	/// <param name="bytes">Bytes.</param>
	public override ISqlPackage	Create (SqliteDataReader sdr)
	{
		T sql = new T ();
		sql.Decode (sdr);
		
		return sql;
	}
	
	/// <summary>
	/// Gets the type of the package.
	/// </summary>
	/// <returns>The package type.</returns>
	public override string		GetTypeName ()
	{
		return typeof(T).Name;
	}
}

/// <summary>
/// Sql buffer.
/// </summary>
public class SqlBuffer <K, T> where T : ISqlPackage
{
	/// <summary>
	/// The table.
	/// </summary>
	protected Dictionary<K, T> 
		m_Table = new Dictionary<K, T> ();
	
	/// <summary>
	/// Add the specified k and t.
	/// </summary>
	/// <param name="k">K.</param>
	/// <param name="t">T.</param>
	public void 	Add(K k, T t)
	{
		if (!m_Table.ContainsKey(k))
		{
			m_Table.Add(k, t);
		}
	}
	
	/// <summary>
	/// Remove the specified k.
	/// </summary>
	/// <param name="k">K.</param>
	public void 	Remove(K k)
	{
		if (m_Table.ContainsKey(k))
		{
			m_Table.Remove(k);
		}
	}
	
	/// <summary>
	/// Query the specified k.
	/// </summary>
	/// <param name="k">K.</param>
	public T		Query(K k)
	{
		if (!m_Table.ContainsKey (k))
			throw new System.NullReferenceException ("Can't find key = " + k.ToString ());
		
		return m_Table [k];
	}
	
	/// <summary>
	/// Clearup this instance.
	/// </summary>
	public void 	Clearup()
	{
		m_Table.Clear ();
	}
}

/// <summary>
/// Game engine.
/// </summary>
public class GameSqlLite : SimpleSingleton<GameSqlLite>
{
	/// <summary>
	/// The sql factory.
	/// </summary>
	protected Dictionary<string, ISqlPackageFactory> m_SqlFactory = new Dictionary<string, ISqlPackageFactory>();
	
	// sql base data type
	protected enum SqlTyp {
		INTEGER,
		TEXT,
		REAL,
	}
	
	/// <summary>
	/// The sql connection.
	/// </summary>
	protected SqliteConnection	m_SqlConnection;
	
	/// <summary>
	/// The m_ memory cache.
	/// </summary>
	protected Dictionary<string, SqlBuffer<int, ISqlPackage>> 
		m_MemoryCache = new Dictionary<string, SqlBuffer<int, ISqlPackage>> ();
	
	/// <summary>
	/// The is open memory cache.
	/// </summary>
	protected bool				m_bCache = true;
	
	/// <summary>
	/// Initializes a new instance of the <see cref="GameSqlLite"/> class.
	/// </summary>
	protected void OnDestroy()
	{
		CloseDB ();
	}
	
	/// <summary>
	/// Opens the D.
	/// </summary>
	/// <param name="connectionString">Connection string.</param>
	public void 				OpenDB (string connectionString, bool bMemeoryCache)
	{
		try
		{
#if OPEN_DEBUG_LOG
			Debug.Log("Sqlite : connection string " + connectionString);
#endif
			m_SqlConnection = new SqliteConnection ("Data Source=" + connectionString);
			m_SqlConnection.Open();
		}
		catch(System.Exception e)
		{
			Debug.LogException(e);
		}
		
		// set memory cache
		m_bCache = bMemeoryCache;
	}
	
	/// <summary>
	/// Closes the D.
	/// </summary>
	public void 				CloseDB()
	{
		ClearCache (string.Empty);
		
		if (m_SqlFactory.Count > 0)
			m_SqlFactory.Clear ();
		
#if OPEN_DEBUG_LOG
		Debug.Log("Sqlite closed!");
#endif
		
		m_SqlConnection.Close ();
	}
	
	/// <summary>
	/// Query the specified sqlQuery.
	/// </summary>
	/// <param name="sqlQuery">Sql query.</param>
	public SqliteDataReader 	ExecuteSqlQuery(string sqlQuery)	
	{
		if (m_SqlConnection.State != System.Data.ConnectionState.Open)
			throw new System.NullReferenceException (sqlQuery);
		
		SqliteCommand cmd = m_SqlConnection.CreateCommand ();
		cmd.CommandText = sqlQuery;
		
		return cmd.ExecuteReader ();	
	}
	
	/// <summary>
	/// Executes the sql query.
	/// </summary>
	/// <returns>The sql query.</returns>
	/// <param name="sqlQuery">Sql query.</param>
	public DataTable			QueryTable(string sqlTableName)
	{
		string sqlQuery = string.Format ("SELECT * FROM {0}", sqlTableName);
		
		if (m_SqlConnection.State != System.Data.ConnectionState.Open)
			throw new System.NullReferenceException (sqlQuery);
		
		SqliteDataAdapter adapter = new SqliteDataAdapter(
			new SqliteCommand(sqlQuery, m_SqlConnection)
			);
		
		DataTable table = new DataTable(); 
		adapter.Fill(table);
		
		return table;
	}
	
	/// <summary>
	/// Queries the table.
	/// </summary>
	/// <returns>The table.</returns>
	/// <param name="sqlTableName">Sql table name.</param>
	/// <param name="queryFieldName">Query field name.</param>
	public DataTable			QueryTable(string sqlTableName, Dictionary<string, string> queryFieldName)
	{
		StringBuilder sqlQuery = new StringBuilder();
		sqlQuery.Append(
			string.Format ("SELECT * FROM {0} WHERE ", sqlTableName)
			);
		
		List<string> aryField = new List<string>(queryFieldName.Keys);
		
		for(int idx=0; idx<aryField.Count; idx++)
		{
			string curKey = aryField[idx];
			
			sqlQuery.Append(
				string.Format(" {0} = {1} ", curKey, queryFieldName[curKey])
				);
			if (idx < aryField.Count - 1)
				sqlQuery.Append("AND");
		}
		
		#if OPEN_DEBUG_LOG
		Debug.Log("Execute sql query " + sqlQuery.ToString());
		#endif
		
		if (m_SqlConnection.State != System.Data.ConnectionState.Open)
			throw new System.NullReferenceException (sqlQuery.ToString());
		
		SqliteDataAdapter adapter = new SqliteDataAdapter(
			new SqliteCommand(sqlQuery.ToString(), m_SqlConnection)
			);
		
		DataTable table = new DataTable(); 
		adapter.Fill(table);
		
		return table;
		
	}
	
	/// <summary>
	/// Select the specified szTableName, items, col, operation and values.
	/// </summary>
	/// <param name="szTableName">Size table name.</param>
	/// <param name="items">Items.</param>
	/// <param name="col">Col.</param>
	/// <param name="operation">Operation.</param>
	/// <param name="values">Values.</param>
	public SqliteDataReader		ExecuteSqlQuery(string szTableName, int nID)
	{
		string szQuerySql = string.Format ("SELECT * FROM {0} WHERE ID = {1}", szTableName, nID);
		#if OPEN_DEBUG_LOG
		Debug.Log("SQLite : " + szQuerySql);
		#endif
		return ExecuteSqlQuery (szQuerySql);
	}
	
	/// <summary>
	/// Query the specified szTableName and nID.
	/// </summary>
	/// <param name="szTableName">Size table name.</param>
	/// <param name="nID">N I.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public T					QueryFromSql<T>(string szTableName, int nID) where T : ISqlPackage
	{
		// the query data type name
		string szTypeName = typeof(T).Name;
		
		// query the sql factory
		if (!m_SqlFactory.ContainsKey (szTypeName))
			throw new System.NullReferenceException ("Can't find sql factory " + szTypeName);
		
		SqliteDataReader sdr = ExecuteSqlQuery (szTableName, nID);
		return (T)m_SqlFactory [szTypeName].Create (sdr);
	}
	
	/// <summary>
	/// Query the specified szTableName, nID and type.
	/// </summary>
	/// <param name="szTableName">Size table name.</param>
	/// <param name="nID">N I.</param>
	/// <param name="type">Type.</param>
	public ISqlPackage			Query(string szTableName, int nID, string typeName)
	{
		try{
			if (m_bCache)
				return GetFromCache(szTableName, nID);
		}
		catch(System.Exception e)
		{
			//Debug.LogWarning(e.Message);
		}
		
		// query the sql factory
		if (!m_SqlFactory.ContainsKey (typeName))
			throw new System.NullReferenceException ("Can't find sql factory " + typeName);
		
		// execute sql query
		SqliteDataReader sdr 	= ExecuteSqlQuery (szTableName, nID);
		
		// create data package
		ISqlPackage package		= m_SqlFactory [typeName].Create (sdr);
		if (m_bCache)
			PushBuffer (szTableName, nID, package);
		
		return package;
	}
	
	/// <summary>
	/// Query the specified szTableName and nID.
	/// </summary>
	/// <param name="szTableName">Size table name.</param>
	/// <param name="nID">N I.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public T					Query<T>(string szTableName, int nID) where T : ISqlPackage
	{
		try{
			if (m_bCache)
				return (T)(GetFromCache(szTableName, nID));
		}
		catch(System.Exception e)
		{
			//Debug.LogWarning(e.Message);
		}
		
		// query data from sql data base
		T package = QueryFromSql<T>(szTableName, nID);
		if (m_bCache)
			PushBuffer (szTableName, nID, package);
		
		return package;
	}
	
	/// <summary>
	/// Update the specified szTableName and val.
	/// </summary>
	/// <param name="szTableName">Size table name.</param>
	/// <param name="val">Value.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public SqliteDataReader		Write(string szTableName, int nID, string field, string szText)
	{
		string szUpdateSql = string.Format ("UPDATE {0} SET {1}='{2}' WHERE ID = {3}", szTableName, field, szText, nID);
		return ExecuteSqlQuery (szUpdateSql);
	}
	
	/// <summary>
	/// Writes the bool.
	/// </summary>
	/// <returns>The bool.</returns>
	/// <param name="szTableName">Size table name.</param>
	/// <param name="nID">N I.</param>
	/// <param name="field">Field.</param>
	/// <param name="bValue">If set to <c>true</c> b value.</param>
	public SqliteDataReader		Write(string szTableName, int nID, string field, bool bValue)
	{
		return Write (szTableName, nID, field, bValue.ToString ());
	}
	
	/// <summary>
	/// Writes the text.
	/// </summary>
	/// <returns>The text.</returns>
	/// <param name="szTableName">Size table name.</param>
	/// <param name="nID">N I.</param>
	/// <param name="szName">Size name.</param>
	/// <param name="szText">Size text.</param>
	public SqliteDataReader		Write(string szTableName, int nID, string field, int nValue)
	{
		string szUpdateSql = string.Format ("UPDATE {0} SET {1}={2} WHERE ID = {3}", szTableName, field, nValue, nID);
		return ExecuteSqlQuery (szUpdateSql);
	}
	
	/// <summary>
	/// Writes the double.
	/// </summary>
	/// <returns>The double.</returns>
	/// <param name="szTableName">Size table name.</param>
	/// <param name="nID">N I.</param>
	/// <param name="field">Field.</param>
	/// <param name="nValue">N value.</param>
	public SqliteDataReader		Write(string szTableName, int nID, string field, double fValue)
	{
		string szUpdateSql = string.Format ("UPDATE {0} SET {1}={2} WHERE ID = {3}", szTableName, field, fValue, nID);
		return ExecuteSqlQuery (szUpdateSql);
	}
	
	/// <summary>
	/// Writes the text.
	/// </summary>
	/// <returns>The text.</returns>
	/// <param name="szTableName">Size table name.</param>
	/// <param name="nID">N I.</param>
	/// <param name="szName">Size name.</param>
	/// <param name="szText">Size text.</param>
	public SqliteDataReader		WriteText(string szTableName, int nID, string szName, string szText)
	{
		string szUpdateSql = string.Format ("UPDATE {0} SET {1}='{2}' WHERE ID = {3}", szTableName, szName, szText, nID);
		return ExecuteSqlQuery (szUpdateSql);
	}
	
	/// <summary>
	/// Pushs the buffer.
	/// </summary>
	/// <param name="szTableName">Size table name.</param>
	/// <param name="nID">N I.</param>
	/// <param name="package">Package.</param>
	public void 				PushBuffer(string szTableName, int nID, ISqlPackage package)
	{
		if (!m_MemoryCache.ContainsKey(szTableName))
		{
			m_MemoryCache.Add(szTableName, new SqlBuffer<int, ISqlPackage>());
		}
		
		#if OPEN_DEBUG_LOG
		Debug.Log("Push sql data buffer : " + szTableName + " id = " + nID.ToString());
		#endif
		m_MemoryCache [szTableName].Add (nID, package);
	}
	
	/// <summary>
	/// Gets from buffer.
	/// </summary>
	/// <returns>The from buffer.</returns>
	/// <param name="szTableName">Size table name.</param>
	/// <param name="nID">N I.</param>
	public ISqlPackage			GetFromCache(string szTableName, int nID)
	{
		if (!m_MemoryCache.ContainsKey(szTableName))
			throw new System.NullReferenceException("The " + szTableName + " no buffer");
		
		return m_MemoryCache [szTableName].Query (nID);
	}
	
	/// <summary>
	/// Clears the buffer.
	/// </summary>
	public void 				ClearCache(string szTableName)
	{
		if (!string.IsNullOrEmpty(szTableName))
		{
			foreach(KeyValuePair<string, SqlBuffer<int, ISqlPackage>> it in m_MemoryCache)
				it.Value.Clearup();
		}
		else
		{
			if (m_MemoryCache.ContainsKey(szTableName))
			{
				m_MemoryCache[szTableName].Clearup();
			}
		}
	}
	
	/// <summary>
	/// Registers the sql package factory.
	/// </summary>
	/// <param name="factory">Factory.</param>
	public void 				RegisterSqlPackageFactory(string szName, ISqlPackageFactory factory)
	{
		if (!m_SqlFactory.ContainsKey(szName))
		{
			#if OPEN_DEBUG_LOG
			Debug.Log("Register sql package factory : " + szName);
			#endif
			m_SqlFactory.Add(szName, factory);
		}
	}
	
	/// <summary>
	/// Gets the sql package factory.
	/// </summary>
	/// <returns>The sql package factory.</returns>
	/// <param name="szName">Size name.</param>
	public ISqlPackageFactory	GetSqlPackageFactory(string szName)
	{
		if (!m_SqlFactory.ContainsKey (szName))
			throw new System.NullReferenceException ("Can't find sql factory " + szName);
		
		return m_SqlFactory [szName];
	}
	
	/// <summary>
	/// Unregisters the sql package factory.
	/// </summary>
	/// <param name="szName">Size name.</param>
	public void 				UnregisterSqlPackageFactory(string szName)
	{
		if (m_SqlFactory.ContainsKey(szName))
		{
			#if OPEN_DEBUG_LOG
			Debug.Log("Unregister sql package factory : " + szName);
			#endif
			m_SqlFactory.Remove(szName);
		}
	}
	
}

