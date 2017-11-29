using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
	public int tileId;
	public int height = 0;
	public int movementCost = 1;
	public int rowNb;
	public int colNb;
	public bool hasSubSprite;
	public Tile[] neighbours = new Tile[4] { null, null, null, null };//0 TopLeft //1 TopRight //2 DownRight // DownLeft

	private GameObject _highlight;

	private void Awake()
	{
		_highlight = transform.GetChild(0).gameObject;
	}

	public void Highlight(bool status)
	{
		_highlight.SetActive(status);
	}

	public int NeighbourNb(Tile tile)
	{
		for (int i = 0; i < neighbours.Length; i++)
		{
			if (neighbours[i] == tile)
			{
				return i;
			}
		}
		return -9;//not a neighbour
	}
}
