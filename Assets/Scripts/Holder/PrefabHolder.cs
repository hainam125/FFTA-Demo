using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabHolder : MonoBehaviour {
  public GameObject Tile;
  public GameObject UserPlayer;
  public GameObject AIPlayer;
  public GameObject Cursor;

  private static PrefabHolder _instance;
  public static PrefabHolder Instance { get { return _instance;} }
  void Awake(){
    _instance = this;
  }
}
