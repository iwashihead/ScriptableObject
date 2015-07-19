using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public class MyDataEditor : EditorWindow {
	public static MyDataSet _data;
	public static int selected = -1;
	public static Vector2 scrollPos;
	public static bool isEditMode;

	private MyData copyInstance = null;

	// ウインドウの表示
	[MenuItem("Window/MyDataEditor")]
	public static void ShowWindow()
	{
		EditorWindow.GetWindow(typeof(MyDataEditor), false, "MyDataEditor");
	}

	string DataPath
	{
		get { return "Assets/Data/" + typeof(MyDataEditor).FullName + ".asset"; }
	}

	// 画面レイアウト
	void OnGUI () {
		
		if (_data == null) {
			// データの読み込み
			_data = AssetDatabase.LoadAssetAtPath(DataPath, typeof(MyDataSet)) as MyDataSet;
			if(_data == null)
			{
				_data = ScriptableObject.CreateInstance<MyDataSet>() as MyDataSet;
				AssetDatabase.CreateAsset(_data, DataPath);
				AssetDatabase.Refresh();
			}
		}

		EditorGUILayout.BeginHorizontal();

		// 要素の追加ボタン
		Color mainColor = GUI.color;
		GUI.color = Color.green;
		if (GUILayout.Button("+", GUILayout.Width(20))) {
			MyData addData = new MyData();
			MyData[] copy = new MyData[_data.list.Count];
			Array.Copy(_data.list.ToArray(), copy, _data.list.Count);
			Array.Sort(copy, (MyData a, MyData b)=>{ return a.id - b.id; });
			int newId = (copy!=null && copy.Length>0) ? copy[0].id : 0;
			// 最小のモンスターIDを探す
			for (int i=0; i<copy.Length; i++)
			{
				if (newId < copy[i].id) {
					break;
				}
				else {
					newId++;
				}
			}
			addData.id = newId;

			// データの追加
			_data.list.Insert(newId, addData);
		}
		GUI.color = mainColor;

		// 要素のコピー
		GUI.color = (copyInstance==null) ? Color.white : Color.green;
		if (GUILayout.Button("copy", GUILayout.Width(50))) {
			if (selected != -1)
			{
				copyInstance = (MyData)_data.list[selected].Clone();
				Debug.Log("copy : " + copyInstance.id + "  " +  copyInstance.name);
			}
		}
		GUI.color = mainColor;

		// 要素のペースト
		GUI.color = (copyInstance==null ? Color.white : Color.green);
		if (GUILayout.Button("paste", GUILayout.Width(50))) {
			if (selected != -1)
			{
				int pasteId = _data.list[selected].id;
				copyInstance.id = pasteId;
				_data.list[selected] = copyInstance;
				Debug.Log("paste : " + copyInstance.id + "  " +  copyInstance.name);
			}
		}
		GUI.color = mainColor;

		EditorGUILayout.EndHorizontal();


		scrollPos = GUILayout.BeginScrollView(scrollPos);
		Color orgCol = GUI.backgroundColor;
		
		// 配置オブジェクトのリストを表示
		if (_data != null && _data.list != null && _data.list.Count > 0)
		{
			for(int i = 0; i < _data.list.Count; i++) {
				orgCol = GUI.backgroundColor;
				GUI.backgroundColor = _data.list[i].color;// 色設定

				GUILayout.BeginHorizontal();
				bool prev = i == selected;
				if (prev) GUI.backgroundColor = Color.magenta;
				bool flag = GUILayout.Toggle((i == selected), _data.list[i].id.ToString() + " : " + _data.list[i].name, "BoldLabel");

				GUI.backgroundColor = orgCol;
				if (GUILayout.Button("Edit",GUILayout.Width(60))) {
					isEditMode = !isEditMode;
					selected = i;// 選択中にする
				}
				GUI.backgroundColor = Color.red;
				if (GUILayout.Button("Remove", GUILayout.Width(60))) {
					// 削除確認のダイアログ
					bool result = EditorUtility.DisplayDialog(
						"Remove",
						"Remove this object.",
						"OK",
						"Cancel");
					
					if(result){
						_data.list.RemoveAt(i);
						GUI.FocusControl("");
					}
				}
				GUI.backgroundColor = orgCol;
				EditorGUILayout.EndHorizontal();
				if (flag){
					selected = i;
					
					// 編集モード
					if (isEditMode){
						// TODO : 変数代入のエディタスクリプトを記述
						int id_new				= EditorGUILayout.IntField("\tid", _data.list[i].id);
						if (id_new != _data.list[i].id)
						{
							// すでにIDが使われていないかチェック
							if (_data.list.Find(a=>a.id==id_new) == null)
								_data.list[i].id = id_new;
						}

						_data.list[i].name		= EditorGUILayout.TextField("\tname", _data.list[i].name);
						_data.list[i].image		= EditorGUILayout.ObjectField("\timage", _data.list[i].image, typeof(Texture2D), true) as Texture2D;
						_data.list[i].color		= EditorGUILayout.ColorField("\tcolor", _data.list[i].color);
						_data.list[i].type		= (int)Enum.ToObject(typeof(MyData.Type), EditorGUILayout.EnumPopup("\ttype", (MyData.Type)_data.list[i].type));
					}
				}
				else{
					// トグルを二度押ししたら非選択状態にする
					if (prev){
						selected = -1;
						isEditMode = false;
					}
				}
				GUI.color = orgCol;
			}
		}
		
		// スクロールバー終了
		GUILayout.EndScrollView();
		// Apply Data
		ScriptableObject scriptable = AssetDatabase.LoadAssetAtPath(DataPath, typeof(ScriptableObject)) as ScriptableObject;
		EditorUtility.SetDirty(scriptable);
	}
}
