/*
 * Copyright (c) 2012 Stephen A. Pratt
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
using UnityEngine;
using UnityEditor;
using org.critterai.nav;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using org.critterai.u3d.editor;
using org.critterai.nav.u3d.editor;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;

/// <summary>
/// <see cref="CAIBakedNavmesh"/> editor.
/// </summary>
/// <exclude />
[CustomEditor(typeof(CAIBakedNavmesh))]
public sealed class CAIBakedNavmeshEditor
    : Editor
{
	static bool bExportObj = false;
	static bool bSaveToAssetbundle = true;

    /// <summary>
    /// Controls behavior of the inspector.
    /// </summary>
    public override void OnInspectorGUI()
    {
        CAIBakedNavmesh targ = (CAIBakedNavmesh)target;

        EditorGUILayout.Separator();

        EditorGUILayout.LabelField("Status", (targ.HasNavmesh ? "Has mesh" : "Empty"));
        EditorGUILayout.LabelField("Version", targ.Version.ToString());
        EditorGUILayout.LabelField("Input Scene", NavEditorUtil.SceneDisplayName(targ.BuildInfo));

        EditorGUILayout.Separator();

        NavmeshSceneDraw.Instance.OnGUI(targ, "Show Mesh", true, true);
		bExportObj = EditorGUILayout.Toggle("Export obj modle", bExportObj);
		bSaveToAssetbundle = EditorGUILayout.Toggle("Save asssetbundle", bSaveToAssetbundle);

        EditorGUILayout.Separator();

        EditorGUILayout.BeginHorizontal();
		
        GUI.enabled = targ.HasNavmesh;
        if (GUILayout.Button("Save"))
        {
			string szOutName = GetFileName( targ.BuildInfo.inputScene, true);

            string filePath = EditorUtility.SaveFilePanel(
                "Save Navigation Mesh"
                , ""
				, szOutName
				, "bytes");
            SaveMesh(targ, filePath);
        }
        GUI.enabled = true;

        if (GUILayout.Button("Load"))
        {
            string filePath = EditorUtility.OpenFilePanel(
                "Select Serialized Navmesh"
                , ""
				, "bytes");
            if (LoadMesh(targ, filePath))
                GUI.changed = true;
        }

		if (GUILayout.Button("ExportOBJ"))
		{
			string filePath = EditorUtility.SaveFilePanel(
				"Save collider to obj"
				, ""
				, targ.name
				, "obj");

			int nStart 	= filePath.LastIndexOf("/");
			int nEnd 	= filePath.LastIndexOf(".");
			filePath = filePath.Substring (0, nStart + 1);

			CAIObjFileExport.ExportColliderToObj(filePath);
		}

        EditorGUILayout.EndHorizontal();

        if (targ.HasNavmesh)
        {
            EditorGUILayout.Separator();

            if (GUILayout.Button("Log Mesh State"))
                Debug.Log(targ.GetMeshReport());
        }

        EditorGUILayout.Separator();

        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }

    private static bool LoadMesh(CAIBakedNavmesh targ, string filePath)
    {
		if (string.IsNullOrEmpty (filePath))
			return false;

		string szErrorMessage = string.Empty;
        
		try{
			FileStream fs = new FileStream (
				filePath, FileMode.Open
				);

			BinaryFormatter formatter = new BinaryFormatter();
			System.Object bytes = formatter.Deserialize(fs);

			NavStatus status = targ.Load((byte[])bytes, null);
			if ((status & NavStatus.Sucess) == 0)
				szErrorMessage = status.ToString();

			fs.Close();
		}
		catch(System.Exception e)
		{
			FileStream fs = new FileStream (filePath, FileMode.Open);
			
			byte[] buffer = new byte[fs.Length];
			fs.Read (buffer, 0, (int)fs.Length);
			
			NavStatus status = targ.Load(buffer, null);
			if ((status & NavStatus.Sucess) == 0)
				szErrorMessage = status.ToString();

			fs.Close();
		}

        return !string.IsNullOrEmpty(szErrorMessage);
    }

	/// <summary>
	/// Gets the name of the file.
	/// </summary>
	/// <returns>The file name.</returns>
	/// <param name="szPath">Size path.</param>
	/// <param name="bExt">If set to <c>true</c> b ext.</param>
	public static string	GetFileName(string szPath, bool bExt)
	{
		if (!string.IsNullOrEmpty(szPath))
		{
			string[] aryName = szPath.Split('/');
			if (aryName.Length > 0)
			{
				if (bExt)
				{
					string[] arySplit = aryName[aryName.Length - 1].Split('.');
					if (arySplit.Length > 0)
						return arySplit[0];
				}
				else
				{
					return aryName[aryName.Length - 1];
				}
			}
		}
		
		return string.Empty;
	}

    private static void SaveMesh(CAIBakedNavmesh targ, string szFilePath)
    {
		if (!string.IsNullOrEmpty(szFilePath) && targ.HasNavmesh)
		{
			string szFileName = GetFileName(szFilePath, true);
			if (szFileName.Length > 0)
			{
				FileStream stream = new FileStream(
					szFilePath, FileMode.Create
					);

				// serialize the file stream
				BinaryFormatter formatter = new BinaryFormatter();
				formatter.Serialize(
					stream, targ.GetNavmesh().GetSerializedMesh()
					);
				
				stream.Close();

				int nStart 	= szFilePath.LastIndexOf("/");
				int nEnd 	= szFilePath.LastIndexOf(".");

				string szServerFilePath = szFilePath.Substring (0, nStart + 1);
				if (!string.IsNullOrEmpty(szServerFilePath))
				{
					// save to server file
					FileStream hStream = new FileStream(
						szFilePath, FileMode.Open
						);
					System.Object bytes = formatter.Deserialize(hStream);
					byte[] datas = (byte[])bytes;

					FileStream hServer = new FileStream(szServerFilePath + szFileName + ".snav", FileMode.Create, FileAccess.Write);
					hServer.Write(datas, 0, datas.Length);

					hServer.Close();
					hStream.Close();

					// export obj file
					if (bExportObj)
					{
						CAIObjFileExport.ExportObj (szServerFilePath);
					}
				}
			}
		}
    }

    [MenuItem(EditorUtil.NavAssetMenu + "Baked Navmesh", false, NavEditorUtil.NavAssetGroup)]
    static void CreateAsset()
    {
        CAIBakedNavmesh item = EditorUtil.CreateAsset<CAIBakedNavmesh>(NavEditorUtil.AssetLabel);
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = item;
    }

    [MenuItem(EditorUtil.ViewMenu + "Hide Navmesh", true)]
    static bool HideNavmeshValidate()
    {
        return NavmeshSceneDraw.Instance.IsShown();
    }

    [MenuItem(EditorUtil.ViewMenu + "Hide Navmesh", false, EditorUtil.ViewGroup)]
    static void HideNavmesh()
    {
        NavmeshSceneDraw.Instance.Hide();
    }
}