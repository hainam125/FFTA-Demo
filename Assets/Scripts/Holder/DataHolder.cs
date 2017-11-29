using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DataHolder {
  public static Dictionary<string, Vector3> directionMath = new Dictionary<string, Vector3>() {
    { "TopLeft",   new Vector3 (-1,  0.5f) },
    { "TopRight",  new Vector3 ( 1,  0.5f) },
    { "DownLeft",  new Vector3 (-1, -0.5f) },
    { "DownRight", new Vector3 ( 1, -0.5f) },
    { "Up",        new Vector3 ( 0,  0.5f) }
  };
  public static int TileDistance = 2;//distance of 2 tiles of the same height
  public static float WalkingChangeLayer = 0.45f;
  public static float LowJumpChangeLayer = 0.4f;
  public static float HighJumpChangeLayer1 = 0.025f;
  public static float HighJumpChangeLayer2 = 0.15f;
  public static float HighJumpChangeLayer3 = 0.4f;//for shadow
  public static float HighJumpChangeLayer4 = 0.8f;//for shadow
  public static float FrameTime = 0.1f;
}
