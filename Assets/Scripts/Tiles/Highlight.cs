using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Highlight
{
	private const int minHeightToClimb = 3;
	public static Vector3 Vector2(this Vector3 vector)
	{
		return new Vector3(vector.x, vector.y, 0);
	}

	//hightlight attack
	public static List<Tile> Attack(Tile startingTile, int attackDistance, int nonAttackDistance)
	{
		if (nonAttackDistance >= attackDistance) return new List<Tile>();

		List<Tile> closed = new List<Tile>();
		List<Tile> closed2 = new List<Tile>();
		List<TilePath> open = new List<TilePath>();
		List<TilePath> open2 = new List<TilePath>();
		TilePath originTilePath = new TilePath();
		originTilePath.addFirstTile(startingTile);
		open.Add(originTilePath);
		open2.Add(originTilePath);
		/*Highlight.OpenAttackTiles (open, closed, attackDistance, nonAttackDistance);
		Highlight.OpenAttackTiles (open2, closed2, nonAttackDistance);
		//with first tile
		*/
		while (open.Count > 0)
		{
			TilePath current = open[0];
			open.RemoveAt(0);
			if (closed.Contains(current.lastTile)) continue;
			if (current.pathCost > attackDistance) continue;
			closed.Add(current.lastTile);
			if (current.pathCost <= nonAttackDistance) closed2.Add(current.lastTile);

			List<Tile> xq = current.lastTile.neighbours.Where(x => x != null).ToList();
			for (int i = 0; i < xq.Count; i++)
			{
				TilePath newTilePath = new TilePath(current);
				newTilePath.addNStaticTile(xq[i], 1);
				open.Add(newTilePath);
				if (current.pathCost <= nonAttackDistance) open2.Add(newTilePath);
			}
		}
		foreach (Tile t in closed2)
			if (closed.Contains(t)) closed.Remove(t);
		return closed;
	}

	public static void OpenAttackTiles(List<TilePath> open, List<Tile> closed, int range)
	{
		while (open.Count > 0)
		{
			TilePath current = open[0];
			open.RemoveAt(0);
			if (closed.Contains(current.lastTile)) continue;
			if (current.pathCost > range) continue;
			closed.Add(current.lastTile);

			List<Tile> xq = current.lastTile.neighbours.Where(x => x != null).ToList();
			for (int i = 0; i < xq.Count; i++)
			{
				TilePath newTilePath = new TilePath(current);
				newTilePath.addNStaticTile(xq[i], 1);
				open.Add(newTilePath);
			}
		}
	}

	public static List<Tile> Movement(Tile startingTile, int range, List<Tile> enemyLocation)
	{
		List<TilePath> closed = new List<TilePath>();
		List<TilePath> open = new List<TilePath>();
		TilePath originTilePath = new TilePath();
		originTilePath.addFirstTile(startingTile);
		open.Add(originTilePath);
		Highlight.OpenMovementTiles(open, closed, enemyLocation, range);
		closed.Remove(originTilePath);
		return closed.Select(x => x.lastTile).ToList();
	}

	public static List<Tile> FindPath(Tile startingTile, Tile endingTile, List<Tile> enemyLocation)
	{
		List<TilePath> closed = new List<TilePath>();
		List<TilePath> open = new List<TilePath>();
		TilePath originPath = new TilePath();
		originPath.addFirstTile(startingTile);
		open.Add(originPath);
		while (open.Count > 0)
		{
			TilePath current = open[0];
			open.Remove(open[0]);

			if (closed.Select(x => x.lastTile).ToList().Contains(current.lastTile))
			{
				if (current.pathCost >= closed.Where(x => x.lastTile == current.lastTile).First().pathCost)
					continue;
				else
					closed.Remove(closed.Where(x => x.lastTile == current.lastTile).First());
			}

			if (current.lastTile == endingTile)
			{
				//current.listOfTiles.Remove (startingTile);
				return current.listOfTiles;
			}

			closed.Add(current);
			Highlight.AllExpand(current, enemyLocation, open);
		}
		return null;
	}

	private static void OpenMovementTiles(List<TilePath> open, List<TilePath> closed, List<Tile> enemyLocation, int range)
	{
		while (open.Count > 0)
		{
			TilePath current = open[0];
			open.RemoveAt(0);


			if (closed.Select(x => x.lastTile).ToList().Contains(current.lastTile))
			{
				//if closed has a tile with lower or equal pathCost => next
				if (current.pathCost >= closed.Where(x => x.lastTile == current.lastTile).First().pathCost)
					continue;
				//if closed has a tile with higher pathCost => remove that tile
				else
					closed.Remove(closed.Where(x => x.lastTile == current.lastTile).First());
			}

			if (current.pathCost > range)
				continue;

			closed.Add(current);
			Highlight.AllExpand(current, enemyLocation, open);
		}
	}

	private static void AllExpand(TilePath current, List<Tile> enemyLocation, List<TilePath> open)
	{
		List<Tile> neighbours = current.lastTile.neighbours.Where(x => x != null).ToList();
		Highlight.NormalExpand(current, enemyLocation, neighbours, open);
	}

	private static void NormalExpand(TilePath current, List<Tile> enemyLocation, List<Tile> neighbours, List<TilePath> open)
	{
		for (int i = 0; i < neighbours.Count; i++)
		{
			if (enemyLocation.Contains(neighbours[i]))
				continue;
			else if (Mathf.Abs(current.lastTile.height - neighbours[i].height) >= minHeightToClimb)
				continue;

			TilePath newTilePath = new TilePath(current);
			newTilePath.addTile(neighbours[i]);

			open.Add(newTilePath);
		}
	}
}
