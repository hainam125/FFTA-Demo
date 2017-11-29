using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Linq;

public class MapCreatorTest {
  PrefabHolder prefabtHolder;
  MapCreator mapCreator;

  List<List<int>> rawMap = new List<List<int>> () {
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

  [SetUp]
  public void BeforeTest(){
    prefabtHolder = GameObject.Instantiate ((GameObject)Resources.Load ("Prefabs/Holder/PrefabHolder"), Vector3.zero, Quaternion.identity).GetComponent<PrefabHolder> ();
    GameObject gameObject = new GameObject ();
    gameObject.AddComponent<MapCreator> ();
    mapCreator = gameObject.GetComponent<MapCreator> ();
    mapCreator.CreateMap(rawMap, prefabtHolder.Tile);
  }

	[Test]
	public void _should_create_correct_number_of_tiles() {
    Assert.AreEqual (155, mapCreator.map.Count);
	}

  [Test]
  public void _should_have_correct_first_tile(){
    Tile tile = mapCreator.map [0];
    Assert.AreEqual (2, tile.neighbours.Where (x => x != null).Count ());
    Assert.AreEqual (1, tile.height);
    Assert.AreEqual (new Vector3 (0, 0.5f).Vector2 (), tile.transform.position.Vector2 ());
    Assert.AreEqual (new Vector3 (1, 1).Vector2 (), tile.neighbours [1].transform.position.Vector2 ());
  }

  [Test]
  public void _should_have_correct_last_tile_of_first_row(){
    Tile tile = mapCreator.map [rawMap [0].Where (x => x != -9).Count () - 1];
    Assert.AreEqual (2, tile.neighbours.Where (x => x != null).Count ());
    Assert.AreEqual (2, tile.height);
    Assert.AreEqual (new Vector3 (9, -3.5f).Vector2 (), tile.transform.position.Vector2 ());
    Assert.AreEqual (new Vector3 (10, -2.5f).Vector2 (), tile.neighbours [1].transform.position.Vector2 ());
  }

  [Test]
  public void _should_have_correct_last_tile_of_third_row(){
    Tile tile = mapCreator.map [29];
    Assert.AreEqual (3, tile.neighbours.Where (x => x != null).Count ());
    Assert.AreEqual (29, tile.tileId);
    Assert.AreEqual (3, tile.height);
    Assert.AreEqual (new Vector3 (11, -2).Vector2 (), tile.transform.position.Vector2 ());
    Assert.AreEqual (28, tile.neighbours [0].tileId);
    Assert.AreEqual (39, tile.neighbours [1].tileId);
    Assert.AreEqual (null, tile.neighbours [2]);
    Assert.AreEqual (19, tile.neighbours [3].tileId);
  }

  [Test]
  public void _should_have_correct_26th(){
    Tile tile = mapCreator.map [25];
    Assert.AreEqual (4, tile.neighbours.Where (x => x != null).Count ());
    Assert.AreEqual (5, tile.height);
    Assert.AreEqual (new Vector3 (7, 1).Vector2 (), tile.transform.position.Vector2 ());
    Assert.AreEqual (24, tile.neighbours [0].tileId);
    Assert.AreEqual (35, tile.neighbours [1].tileId);
    Assert.AreEqual (26, tile.neighbours [2].tileId);
    Assert.AreEqual (15, tile.neighbours [3].tileId);
  }

  [Test]
  public void _should_have_correct_86th(){
    Tile tile = mapCreator.map [85];
    Assert.AreEqual (3, tile.neighbours.Where (x => x != null).Count ());
    Assert.AreEqual (3, tile.height);
    Assert.AreEqual (new Vector3 (12, 3.5f).Vector2 (), tile.transform.position.Vector2 ());
    Assert.AreEqual (null, tile.neighbours [0]);
    Assert.AreEqual (new Vector3 (11, 4.5f).Vector2 (), tile.neighbours [3].transform.position.Vector2 ());
  }

  [Test]
  public void _a_row_should_be_over_row_after_it(){
    Assert.Greater (mapCreator.map [10].transform.position.z, mapCreator.map [0].transform.position.z);
    Assert.Greater (mapCreator.map [21].transform.position.z, mapCreator.map [11].transform.position.z);
    Assert.Greater (mapCreator.map [98].transform.position.z, mapCreator.map [89].transform.position.z);
  }

  [Test]
  public void _right_tile_should_over_left_tile(){
    Assert.Greater (mapCreator.map [0].transform.position.z, mapCreator.map [1].transform.position.z);
    Assert.Greater (mapCreator.map [15].transform.position.z, mapCreator.map [16].transform.position.z);
    Assert.Greater (mapCreator.map [108].transform.position.z, mapCreator.map [109].transform.position.z);
  }

	// A UnityTest behaves like a coroutine in PlayMode
	// and allows you to yield null to skip a frame in EditMode
	/*[UnityTest]
	public IEnumerator MapCreatorTestWithEnumeratorPasses() {
		// Use the Assert class to test conditions.
		// yield to skip a frame
		yield return null;
	}*/
}
