using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetHolder : MonoBehaviour {
  public Sprite[] Map2SubSprite;
  public Sprite[] MarcheFrontAttack;
  public Sprite[] MarcheBackAttack;
  public Sprite[] MarcheTopLeftHighJump;
  public Sprite[] MarcheDownLeftHighJump;
  public Sprite[] MarcheTopLeftWalking;
  public Sprite[] MarcheDownLeftWalking;
  public Sprite[] MarcheTopLeftLowJump;
  public Sprite[] MarcheDownLeftLowJump;

  private static AssetHolder _instance;
  public static AssetHolder Instance { get { return _instance;} }
  void Awake(){
    _instance = this;
  }
}
