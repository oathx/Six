using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Magic state.
/// </summary>
public enum MagicState {
	MAGIC_STRAT,
	MAGIC_RUNING,
	MAGIC_EXIT,
}

/// <summary>
/// I magic.
/// </summary>
public class MagicShape : INullObject
{
	/// <summary>
	/// Gets the magic I.
	/// </summary>
	/// <value>The magic I.</value>
	public SqlMagic			Magic { get; private set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="MagicShape"/> class.
	/// </summary>
	public MagicShape()
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="Magic"/> class.
	/// </summary>
	/// <param name="nMagicID">N magic I.</param>
	public MagicShape(SqlMagic sqlMagic)
	{
		Magic = sqlMagic;
	}

	/// <summary>
	/// Raises the init event.
	/// </summary>
	/// <param name="sqlMagic">Sql magic.</param>
	public virtual void 	OnInit(SqlMagic sqlMagic)
	{
		Magic = sqlMagic;
	}

	/// <summary>
	/// Raises the start event.
	/// </summary>
	public virtual bool		OnStart()
	{
		#if OPEN_DEBUG_LOG
		Debug.Log(GetType().Name + " OnStart " + " Magic ID = " + MagicID);
		#endif
		return true;
	}
	
	/// <summary>
	/// Raises the update event.
	/// </summary>
	public virtual bool		OnUpdate()
	{
		return true;
	}
	
	/// <summary>
	/// Raises the exit event.
	/// </summary>
	public virtual bool		OnExit()
	{
		#if OPEN_DEBUG_LOG
		Debug.Log(GetType().Name + " OnExit " + " Magic ID = " + MagicID);
		#endif
		return true;
	}
}

