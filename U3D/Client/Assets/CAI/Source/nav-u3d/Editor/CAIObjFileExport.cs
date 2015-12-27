using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;

struct SObjFileMaterial
{
	public string 		Name;
	public string 		TextureName;
}

/// <summary>
/// Editor object exporter.
/// </summary>
public class CAIObjFileExport : ScriptableObject
{
	private static int VertexOffset 	= 0;
	private static int NormalOffset 	= 0;
	private static int UVOffset 		= 0;

	public static void ExportColliderToObj(string szFilePath)
	{
		Collider[] arySelection = GameObject.FindObjectsOfType<Collider> ();
		if (arySelection.Length != 0)
		{
			List<GameObject> 
				goList = new List<GameObject>();

			int nExported = 0;

			ArrayList aryList = new ArrayList ();
			for(int i=0; i<arySelection.Length; i++)
			{
				int nChildCount = arySelection[i].gameObject.transform.childCount;
				if (nChildCount > 0)
				{
					for(int c=0; c<nChildCount; c++)
					{
						Transform child = arySelection[i].gameObject.transform.GetChild(c);
						if (child)
							GameObject.Destroy(child.gameObject);

						Debug.Log("destroy old box cube mesh " + child.name);
					}
				}

				GameObject goMesh = GameObject.CreatePrimitive(PrimitiveType.Cube);
				if (!goMesh)
					throw new System.NullReferenceException();

				goMesh.transform.parent 		= arySelection[i].gameObject.transform;
				goMesh.transform.localScale 	= Vector3.one;
				goMesh.transform.localPosition 	= Vector3.zero;

				goList.Add(goMesh);

				MeshFilter mf = goMesh.GetComponent<MeshFilter>();
				if (mf)
				{
					nExported ++;

					// save cube
					aryList.Add(mf);
				}
			}
			
			if (nExported > 0)
			{
				MeshFilter[] aryMeshFilter = new MeshFilter[aryList.Count];
				for (int i=0; i<aryList.Count; i++)
				{
					aryMeshFilter[i] = (MeshFilter)aryList[i];
				}
				
				string curFileName = EditorApplication.currentScene + "-" + nExported;
				
				int nStripIndex = curFileName.LastIndexOf ('/');
				if (nStripIndex >= 0)
					curFileName = curFileName.Substring (nStripIndex + 1).Trim ();
				
				MeshesToFile (aryMeshFilter, szFilePath, curFileName);
			}

			foreach(GameObject cube in goList)
			{
				GameObject.DestroyImmediate(cube);
			}
		}
	}

	/// <summary>
	/// Exports the object.
	/// </summary>
	/// <param name="szFilePath">Size file path.</param>
	public static void ExportObj(string szFilePath)
	{
		Transform[] arySelection = GameObject.FindObjectsOfType<Transform> ();
		if (arySelection.Length != 0)
		{
			int nExported = 0;
			
			ArrayList aryList = new ArrayList ();
			for (int i = 0; i < arySelection.Length; i++)
			{
				Component[] aryMeshFilter = arySelection [i].GetComponentsInChildren (typeof(MeshFilter));
				for (int j = 0; j < aryMeshFilter.Length; j++)
				{
					nExported ++;
					
					aryList.Add (
						aryMeshFilter [j]
						);
				}
			}
			
			if (nExported > 0)
			{
				MeshFilter[] aryMeshFilter = new MeshFilter[aryList.Count];
				for (int i=0; i<aryList.Count; i++)
				{
					aryMeshFilter[i] = (MeshFilter)aryList[i];
				}
				
				string curFileName = EditorApplication.currentScene + "-" + nExported;
				
				int nStripIndex = curFileName.LastIndexOf ('/');
				if (nStripIndex >= 0)
					curFileName = curFileName.Substring (nStripIndex + 1).Trim ();
				
				MeshesToFile (aryMeshFilter, szFilePath, curFileName);
			}
		}
	}

	/// <summary>
	/// Clear this instance.
	/// </summary>
	static void Clear ()
	{
		VertexOffset 	= 0;
		NormalOffset 	= 0;
		UVOffset 		= 0;
	}

	/// <summary>
	/// Prepares the file write.
	/// </summary>
	/// <returns>The file write.</returns>
	static 	Dictionary<string, SObjFileMaterial> PrepareFileWrite ()
	{
		Clear ();

		return new Dictionary<string, SObjFileMaterial> ();
	}

	/// <summary>
	/// Mesheses to file.
	/// </summary>
	/// <param name="aryMeshFilter">Ary mesh filter.</param>
	/// <param name="szFolder">Size folder.</param>
	/// <param name="szFilename">Size filename.</param>
	static void MeshesToFile (MeshFilter[] aryMeshFilter, string szFolder, string szFilename)
	{
		Dictionary<string, SObjFileMaterial> dMaterialList = PrepareFileWrite ();
		
		using (StreamWriter sw = new StreamWriter(szFolder +"/" + szFilename + ".obj")) {
			sw.Write ("mtllib ./" + szFilename + ".mtl");
			
			for (int i = 0; i < aryMeshFilter.Length; i++)
			{
				sw.Write (MeshToString (aryMeshFilter [i], dMaterialList));
			}
		}
	}

	/// <summary>
	/// Meshs to string.
	/// </summary>
	/// <returns>The to string.</returns>
	/// <param name="mf">Mf.</param>
	/// <param name="materialList">Material list.</param>
	static string MeshToString (MeshFilter mf, Dictionary<string, SObjFileMaterial> materialList)
	{
		if (!mf.sharedMesh)
			return string.Empty;

		Mesh m 			= mf.sharedMesh;
		Material[] mats = mf.GetComponent<Renderer>().sharedMaterials;

		StringBuilder sb = new StringBuilder ();
		sb.Append ("g ").Append (mf.name).Append (string.Empty);
		foreach (Vector3 lv in m.vertices) 
		{
			Vector3 wv = mf.transform.TransformPoint (lv);

			//This is sort of ugly - inverting x-component since we're in
			//a different coordinate system than "everyone" is "used to".
			sb.Append (
				string.Format ("\nv {0} {1} {2}", -wv.x, wv.y, wv.z)
				);
		}

		sb.Append (string.Empty);
		foreach (Vector3 lv in m.normals)
		{
			Vector3 wv = mf.transform.TransformDirection (lv);
			sb.Append (
				string.Format ("\nvn {0} {1} {2}", -wv.x, wv.y, wv.z)
				);
		}
		sb.Append (string.Empty);

		foreach (Vector3 v in m.uv) 
		{
			sb.Append (
				string.Format ("\nvt {0} {1}", v.x, v.y)
				);
		}

		for (int material=0; material < m.subMeshCount; material ++) {
			sb.Append (string.Empty);
			sb.Append ("\nusemtl ").Append (mats [material].name).Append (string.Empty);
			sb.Append ("\nusemap ").Append (mats [material].name).Append (string.Empty);

			//See if this material is already in the materiallist.
			try {
				SObjFileMaterial objMaterial = new SObjFileMaterial ();
				objMaterial.Name = mats [material].name;

				if (mats [material].mainTexture)
				{
					objMaterial.TextureName = EditorUtility.GetAssetPath (mats [material].mainTexture);
				}
				else
				{
					objMaterial.TextureName = string.Empty;
				}

				if (!materialList.ContainsKey(objMaterial.Name))
					materialList.Add (objMaterial.Name, objMaterial);
			}
			catch (ArgumentException e) 
			{
				//Already in the dictionary
				Debug.LogException(e);
			}

			int[] triangles = m.GetTriangles (material);
			for (int i=0; i<triangles.Length; i+=3) 
			{
				//Because we inverted the x-component, we also needed to alter the triangle winding.
				sb.Append (string.Format ("\nf {1}/{1}/{1} {0}/{0}/{0} {2}/{2}/{2}",
				                          triangles [i] + 1 + VertexOffset, triangles [i + 1] + 1 + NormalOffset, triangles [i + 2] + 1 + UVOffset));
			}

			sb.Append("\n");
		}

		VertexOffset 	+= m.vertices.Length;
		NormalOffset 	+= m.normals.Length;
		UVOffset 		+= m.uv.Length;

		return sb.ToString ();
	}
}
