using System;
using UnityEngine;

[System.SerializableAttribute]
public class MyData : ICloneable{
	public enum Type{
		NONE,
		LEFT,
		RIGHT
	}

	public int id;
	public string name;
	public Texture2D image;
	public Color color;
	public int type;

	public object Clone ()
	{
		MyData copy = new MyData();
		copy.id = id;
		copy.name = name;
		copy.color = color;
		copy.type = type;

		return copy;
	}
}