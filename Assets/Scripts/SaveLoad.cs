using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Serialization;
using System.Text;
using UnityEngine;

public class TileXml
{
	public int locX;
	public int locY;
	public int rowNb;
	public int colNb;
	public int height;
	public int hasSub;
	public int neighbor0;
	public int neighbor1;
	public int neighbor2;
	public int neighbor3;
}

public class MapXmlContainer
{
	public List<TileXml> tiles = new List<TileXml>();
}

public static class SaveLoad
{
	public static MapXmlContainer CreateMapContainer(List<Tile> map)
	{
		List<TileXml> Tiles = new List<TileXml>();
		for (int i = 0; i < map.Count; i++)
		{
			Tiles.Add(SaveLoad.CreateTileXml(map[i]));
		}
		return new MapXmlContainer() { tiles = Tiles };
	}

	public static TileXml CreateTileXml(Tile tile)
	{
		return new TileXml()
		{
			locX = (int)(tile.transform.position.x * 10),
			locY = (int)(tile.transform.position.y * 10),
			rowNb = tile.rowNb,
			colNb = tile.colNb,
			height = tile.height,
			hasSub = tile.hasSubSprite ? 1 : 0,
			neighbor0 = tile.neighbours[0] != null ? tile.neighbours[0].tileId : -9,
			neighbor1 = tile.neighbours[1] != null ? tile.neighbours[1].tileId : -9,
			neighbor2 = tile.neighbours[2] != null ? tile.neighbours[2].tileId : -9,
			neighbor3 = tile.neighbours[3] != null ? tile.neighbours[3].tileId : -9,
		};
	}

	public static void SaveMapData(List<Tile> map, string filename)
	{
		MapXmlContainer mapContainer = SaveLoad.CreateMapContainer(map);
		var serializer = new XmlSerializer(typeof(MapXmlContainer));
		var encoding = Encoding.GetEncoding("UTF-8");
		using (var stream = new StreamWriter(filename, false, encoding))
		{
			serializer.Serialize(stream, mapContainer);
		}
	}

	public static MapXmlContainer LoadMapData(string filename)
	{
		var serializer = new XmlSerializer(typeof(MapXmlContainer));
		using (var stream = new FileStream(filename, FileMode.Open))
		{
			return serializer.Deserialize(stream) as MapXmlContainer;

		}
	}

	public static MapXmlContainer LoadMapDataFromResources(string filename)
	{
		var serializer = new XmlSerializer(typeof(MapXmlContainer));
		TextAsset textAsset = (TextAsset)Resources.Load(filename, typeof(TextAsset));
		MemoryStream assetStream = new MemoryStream(textAsset.bytes);
		MapXmlContainer container = serializer.Deserialize(assetStream) as MapXmlContainer;
		assetStream.Close();
		return container;
	}

	public static void LoadMap(List<Tile> map, GameObject mapObject)
	{
		//MapXmlContainer loadData = SaveLoad.LoadMapData(Path.Combine(Application.dataPath, @"Resources\" + "Maps.Xml")); //For Window
		MapXmlContainer loadData = SaveLoad.LoadMapData(Path.Combine(Application.dataPath, @"Resources/" + "Maps.Xml"));
		int subSpriteCount = 0;
		for (int i = 0; i < loadData.tiles.Count; i++)
		{
			TileXml data = loadData.tiles[i];
			Tile tile = ((GameObject)Transform.Instantiate(PrefabHolder.Instance.Tile)).GetComponent<Tile>();
			tile.tileId = i;
			tile.rowNb = data.rowNb;
			tile.colNb = data.colNb;
			tile.transform.position = new Vector3(data.locX / 10.0f, data.locY / 10.0f, tile.rowNb * 1 - tile.colNb * 0.1f);
			tile.hasSubSprite = data.hasSub == 1;
			tile.height = data.height;
			tile.transform.parent = mapObject.transform;

			if (tile.hasSubSprite)
			{
				//tile.transform.GetChild (0).gameObject.SetActive (true);
				tile.GetComponent<SpriteRenderer>().sprite = AssetHolder.Instance.Map2SubSprite[subSpriteCount];
				subSpriteCount++;
			}

			map.Add(tile);
		}
		for (int i = 0; i < loadData.tiles.Count; i++)
		{
			Tile tile = map[i];
			TileXml data = loadData.tiles[i];
			tile.neighbours[0] = data.neighbor0 != -9 ? map[data.neighbor0] : null;
			tile.neighbours[1] = data.neighbor1 != -9 ? map[data.neighbor1] : null;
			tile.neighbours[2] = data.neighbor2 != -9 ? map[data.neighbor2] : null;
			tile.neighbours[3] = data.neighbor3 != -9 ? map[data.neighbor3] : null;
		}
	}

}
