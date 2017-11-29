using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MapCreator : MonoBehaviour
{
	public List<Tile> map = new List<Tile>();
	// Use this for initialization
	void Start()
	{
		List<List<int>> rawMap = new List<List<int>>() {
	  new List<int> (){ 1, 1, 1, 2, 2, 2, 2, 2, 2, 2 },
	  new List<int> (){ 1, 1, 1, 2, 2, 2, 2, 2, 3, 3 },
	  new List<int> (){ 2, 2, 2, 2, 2, 5, 2, 2, 2, 3 },
	  new List<int> (){ 3, 2, 2, 2, 2, 2, 2, 2, 2, 3 },
	  new List<int> (){ 3, 2, 2, 2, 2, 2, 2, 2, 2, 2 },
	  new List<int> (){ 3, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 },
	  new List<int> (){ 3, 3, 2, 2, 2, 2, 2, 2, 2, 2, 2 },
	  new List<int> (){ 3, 3, 3, 3, 6, 3, 2, 2, 2, 2, 2, 3, 3 },
	  new List<int> (){ -9, -9, -9, -9, 3, 3, 3, 2, 2, 3, 3, 3, 3 },
	  new List<int> (){ -9, -9, -9, -9, 3, 3, 3, 3, 3, 3, 3, 4, 4 },
	  new List<int> (){ -9, -9, -9, -9, -9, 4, 4, 3, 3, 3, 3, 4, 4, 4, 4 },
	  new List<int> (){ -9, -9, -9, -9, -9, 4, 4, 5, 5, 5, 5, 4, 4, 4, 5 },
	  new List<int> (){ -9, -9, -9, -9, -9, -9, -9, 5, 5, 5, 5, 5, 5, 4, 4 },
	  new List<int> (){ -9, -9, -9, -9, -9, -9, -9, 5, 5, 5, 8, 5, 5, 5, 4 },
	  new List<int> (){ -9, -9, -9, -9, -9, -9, -9, 5, 5, 5, 5, 5, 5, 5, 5},
	  new List<int> (){ -9, -9, -9, -9, -9, -9, -9, 5, 5, 5, 5, 5, 5, 5, 5}
	};
		CreateMap(rawMap, PrefabHolder.Instance.Tile);
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void CreateMap(List<List<int>> rawMap, GameObject TileObject)
	{
		List<List<Tile>> extendedMap = new List<List<Tile>>();

		int height = rawMap.Count;
		int width = rawMap.OrderBy(x => x.Count).Last().Count();
		int count = 0;
		int tileId = 0;
		for (int rowNb = 0; rowNb < rawMap.Count; rowNb++)
		{

			List<Tile> extendedRow = new List<Tile>();
			List<int> row = rawMap[rowNb];
			for (int colNb = 0; colNb < row.Count; colNb++)
			{

				if (row[colNb] != -9)
				{
					Tile tile = ((GameObject)Instantiate(TileObject)).GetComponent<Tile>();
					Tile refTile = null;
					tile.rowNb = rowNb;
					tile.colNb = colNb;
					tile.height = row[colNb];
					tile.tileId = tileId;

					//first tile of the first row
					if (count == 0)
					{
						tile.transform.position = new Vector3(0, 0, -2) + (DataHolder.directionMath["Up"] * tile.height);
					}
					//first tile of other rows
					else if (extendedRow.Where(x => x != null).Count() == 0)
					{
						refTile = extendedMap[rowNb - 1][colNb];
						tile.transform.position =
						  refTile.transform.position +
						  DataHolder.directionMath["TopRight"] +
						  DataHolder.directionMath["Up"] * (tile.height - refTile.height);

						tile.neighbours[3] = refTile;
						refTile.neighbours[1] = tile;
					}
					//2nd to last tile in a row
					else if (colNb > 0 && colNb < width)
					{
						refTile = extendedRow[colNb - 1];
						tile.transform.position =
						  refTile.transform.position +
						  DataHolder.directionMath["DownRight"] +
						  DataHolder.directionMath["Up"] * (tile.height - refTile.height);

						tile.neighbours[0] = refTile;
						refTile.neighbours[2] = tile;

						if (rowNb > 0)
						{
							refTile = (colNb < extendedMap[rowNb - 1].Count) ? extendedMap[rowNb - 1][colNb] : null;
							if (refTile != null)
							{
								tile.neighbours[3] = refTile;
								refTile.neighbours[1] = tile;
							}
						}
					}
					tile.transform.GetChild(0).gameObject.SetActive(true);
					tile.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, rowNb * 1 - colNb * 0.1f); //set visual priority
					tile.transform.parent = transform;
					tileId++;
					map.Add(tile);
					extendedRow.Add(tile);
				}
				else
				{
					extendedRow.Add(null);
				}
				count++;
			}
			extendedMap.Add(extendedRow);
		}
	}
}
