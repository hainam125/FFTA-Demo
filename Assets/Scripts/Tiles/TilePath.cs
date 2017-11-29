using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TilePath
{
	public List<Tile> listOfTiles = new List<Tile>();
	public Tile lastTile;
	public int pathCost = 0;

	public void addTile(Tile t)
	{
		listOfTiles.Add(t);
		lastTile = t;
		pathCost += t.movementCost;
	}

	public void addFirstTile(Tile t)
	{
		listOfTiles.Add(t);
		lastTile = t;
		pathCost = 0;
	}

	public void addNStaticTile(Tile t, int staticValue)
	{
		listOfTiles.Add(t);
		lastTile = t;
		pathCost += staticValue;
	}

	public TilePath() { }

	public TilePath(TilePath t)
	{
		listOfTiles = t.listOfTiles.ToList();
		lastTile = t.lastTile;
		pathCost = t.pathCost;
	}
}