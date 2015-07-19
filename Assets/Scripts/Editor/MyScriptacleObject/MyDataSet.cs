using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MyDataSet : ScriptableObject
{
	public List<MyData> list = null;
	
	void OnEnable()
	{
		hideFlags = HideFlags.NotEditable;
		
		if(list == null) {
			list = new List<MyData>();
		}
	}
	
	public void Add(int id, MyData _data)
	{
		if(list != null) list.Add(_data);
	}
	
	public void Clear()
	{
		if(list != null) list.Clear();
	}
}