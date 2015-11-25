using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Data;
// sqlite support
using Mono.Data.Sqlite;


/// <summary>
/// Sql shape.
/// </summary>
public class SqlCamera : ISqlPackage
{
	/// <summary>
	/// Gets the shape I.
	/// </summary>
	/// <value>The shape I.</value>
	public int 				ID
	{ get; set; }
	
	
	/// <summary>
	/// Gets or sets the offset position.
	/// </summary>
	/// <value>The offset position.</value>
	public Vector3 		OffsetPosition 
	{ get; set; }
	
	/// <summary>
	/// Gets or sets the offset position lerp.
	/// </summary>
	/// <value>The offset position lerp.</value>
	public float 		OffsetPositionLerp 
	{ get; set; }
	
	/// <summary>
	/// Gets or sets the offset euler angles.
	/// </summary>
	/// <value>The offset euler angles.</value>
	public Vector3 		OffsetEulerAngles 
	{ get; set; }
	
	/// <summary>
	/// Gets or sets the offset euler angles lerp.
	/// </summary>
	/// <value>The offset euler angles lerp.</value>
	public float 		OffsetEulerAnglesLerp 
	{ get; set; }
	
	/// <summary>
	/// Gets or sets the field of view.
	/// </summary>
	/// <value>The field of view.</value>
	public float 		FieldOfView 
	{ get; set; }
	
	/// <summary>
	/// Gets or sets a value indicating whether this instance is relative.
	/// </summary>
	/// <value><c>true</c> if this instance is relative; otherwise, <c>false</c>.</value>
	public bool 		IsRelative 
	{ get; set; }
	
	/// <summary>
	/// Decode the specified sdr.
	/// </summary>
	/// <param name="sdr">Sdr.</param>
	public override void 	Decode (SqliteDataReader sdr)
	{
		ID 						= System.Convert.ToInt32(sdr ["id"]);
		OffsetPosition 			= MathfEx.ToVector3(System.Convert.ToString(sdr ["lockPosition"]));
		OffsetPositionLerp 		= System.Convert.ToSingle(sdr ["lockPositionLerp"]);
		OffsetEulerAngles 		= MathfEx.ToVector3(System.Convert.ToString(sdr ["lockRotation"]));
		OffsetEulerAnglesLerp 	= System.Convert.ToSingle(sdr ["lockRotationLerp"]);
		FieldOfView 			= System.Convert.ToSingle(sdr ["fieldOfView"]);
		IsRelative 				= System.Convert.ToInt32 (sdr ["Relative"]) != 0 ? true : false;
	}

	/// <summary>
	/// Encode the specified row.
	/// </summary>
	/// <param name="row">Row.</param>
	public override void 	Encode (DataRow row)
	{
		ID 						= System.Convert.ToInt32(row ["id"]);
		OffsetPosition 			= MathfEx.ToVector3(System.Convert.ToString(row ["lockPosition"]));
		OffsetPositionLerp 		= System.Convert.ToSingle(row ["lockPositionLerp"]);
		OffsetEulerAngles 		= MathfEx.ToVector3(System.Convert.ToString(row ["lockRotation"]));
		OffsetEulerAnglesLerp 	= System.Convert.ToSingle(row ["lockRotationLerp"]);
		FieldOfView 			= System.Convert.ToSingle(row ["fieldOfView"]);
		IsRelative 				= System.Convert.ToInt32 (row ["Relative"]) != 0 ? true : false;
	}
}


