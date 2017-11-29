using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UserPlayer : Player {

	void Awake () {
    _displayedImage = transform.GetChild(0).GetComponent<SpriteRenderer> ();
    faceDirection = 2;
    Hp = 25;
	}

  void Start() {
    TopLeftWalking = AssetHolder.Instance.MarcheTopLeftWalking.ToList ();
    DownLeftWalking = AssetHolder.Instance.MarcheDownLeftWalking.ToList ();
    TopLeftLowJump = AssetHolder.Instance.MarcheTopLeftLowJump.ToList ();
    DownLeftLowJump = AssetHolder.Instance.MarcheDownLeftLowJump.ToList ();
    TopLeftHighJump = AssetHolder.Instance.MarcheTopLeftHighJump.ToList ();
    DownLeftHighJump = AssetHolder.Instance.MarcheDownLeftHighJump.ToList ();
    FrontAttack = AssetHolder.Instance.MarcheFrontAttack.ToList ();
    BackAttack = AssetHolder.Instance.MarcheBackAttack.ToList ();
    MovingAnimation (faceDirection, GameManager.Instance.map [0], GameManager.Instance.map [0]);
  }
	
	void Update () {
    CreateAnimation ();
	}
}
