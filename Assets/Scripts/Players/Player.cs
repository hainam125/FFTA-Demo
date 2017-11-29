using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour {
  public int movement = 4;
  public int faceDirection;
  public bool moving;
  public int attackDistance = 1;
  public int nonAttackDistance = 0;
  protected SpriteRenderer _displayedImage;
  protected List<Sprite> FrontAttack = new List<Sprite> ();
  protected List<Sprite> BackAttack = new List<Sprite> ();
  protected List<Sprite> TopLeftWalking = new List<Sprite> ();
  protected List<Sprite> DownLeftWalking = new List<Sprite> ();
  protected List<Sprite> TopLeftLowJump = new List<Sprite> ();
  protected List<Sprite> DownLeftLowJump = new List<Sprite> ();
  protected List<Sprite> TopLeftHighJump = new List<Sprite> ();
  protected List<Sprite> DownLeftHighJump = new List<Sprite> ();
  protected List<Sprite> _currentSprites = new List<Sprite>();

  public string Name;

  public int Level;
  public int Exp;
  public int Hp;
  public int Mp;
  public int Move;
  public int Jump;
  public int Evade;
  public int Atk;
  public int Def;
  public int Pow;
  public int Res;
  public int Speed;


  private void Awake (){
    
  }

  private void Start () {
    
	}

	private void Update () {
    
	}


  public void CreateAnimation(){
    if(!GameManager.Instance.attacking)
      _displayedImage.sprite = _currentSprites [(GameManager.Instance.FrameCount / 6) % _currentSprites.Count];
  }



  private void AssignSprite(List<Sprite> walking, List<Sprite> lowJump, List<Sprite> highJump, Tile from, Tile to, int rotation){
    int height = Mathf.Abs (from.height - to.height);
    float currentDistance = Vector3.Distance (transform.position.Vector2 (), from.transform.position.Vector2 ());
    _displayedImage.transform.rotation = Quaternion.Euler(new Vector3(0,rotation,0));
    if (height == 0) {
      _currentSprites = walking;
      if (currentDistance > DataHolder.WalkingChangeLayer) {
        transform.position = new Vector3 (transform.position.x, transform.position.y, to.transform.position.z);
      }
      _displayedImage.transform.localPosition = new Vector3 (0, 0.75f, _displayedImage.transform.localPosition.z);
    }
    else if (height == 1) {
      
      float offset = CalculateOffsetForLowJump (from, to, currentDistance);
      if (currentDistance > DataHolder.LowJumpChangeLayer) {
        transform.position = new Vector3 (transform.position.x, transform.position.y, to.transform.position.z);
      }
      _currentSprites = new List<Sprite> (){ lowJump [GameManager.Instance.preJumpStype] };
      _displayedImage.transform.localPosition = new Vector3 (0, 0.75f + offset, _displayedImage.transform.localPosition.z);
    }
    else if (height == 2) {
      float offset = CalculateOffsetForHighJump (from, to);
      if ((currentDistance > DataHolder.HighJumpChangeLayer3 && to.height < from.height) || (currentDistance > DataHolder.HighJumpChangeLayer4 && to.height > from.height)) {
        transform.position = new Vector3 (transform.position.x, transform.position.y, to.transform.position.z);
      }
      _currentSprites = new List<Sprite> (){ highJump [GetSpriteForHighJump (Mathf.Abs (transform.position.x - from.transform.position.x), Mathf.Abs (to.transform.position.x - from.transform.position.x))] };
      _displayedImage.transform.localPosition = new Vector3 (0, 0.75f + offset, _displayedImage.transform.localPosition.z);
    }
  }

  private int GetSpriteForHighJump(float currentDistance, float totalDistance){
    if (currentDistance < DataHolder.HighJumpChangeLayer1 || (totalDistance - currentDistance) < DataHolder.HighJumpChangeLayer2)
      return 0;
    else
      return 1;
  }

  //calulate the height of high speed
  private float CalculateOffsetForHighJump(Tile from, Tile to){
    //TopRight + TopLeft
    if (Mathf.Abs(to.transform.position.y - from.transform.position.y) == 1.5f) {
      //jump parabol: y = -3.6x^2 + 4.9x normal walk y = 1.5x
      //offset y = -4x^2 + 4x
      float x = Mathf.Abs(transform.position.x - from.transform.position.x);
      return -3.6f * x * x + 3.4f * x;
    }
    //DownRight + DownLeft
    else {
      //jump parabol: y = -3.6x^2 + 4.9x normal walk y = 0.5x
      //offset y = -4x^2 + 4x
      float x = Mathf.Abs(transform.position.x - from.transform.position.x);
      return -3.2f * x * x + 3.1f * x;
    }
  }

  //calulate the height of low speed
  private float CalculateOffsetForLowJump(Tile from, Tile to, float currentDistance){
    //float totalDistance = Vector3.Distance (from.transform.position.Vector2 (), to.transform.position.Vector2 ());
    float horizontalCurrentDistance = currentDistance * Mathf.Sin (Mathf.PI / 4);
    float horizontalRestDistance = 1 - horizontalCurrentDistance; //1= totalDistance * sin 45
    float verticalTotalDistance = Mathf.Sqrt(1 - horizontalRestDistance * horizontalRestDistance);
    return verticalTotalDistance - horizontalCurrentDistance;
  }

  public void HighlightAttack(){
    List<Tile> attackRange = Highlight.Attack (currentTile (), attackDistance, nonAttackDistance);
    for (int i = 0; i < attackRange.Count; i++) {
      attackRange [i].Highlight (true);
    }
  }

  public Tile currentTile(){
    return GameManager.Instance.map.Where (x => x.transform.position.x == transform.position.x && x.transform.position.y == transform.position.y).First ();
  }

  public void MovingAnimation(int movingDirection, Tile from, Tile to){
    if(movingDirection == 0){
      AssignSprite (TopLeftWalking, TopLeftLowJump, TopLeftHighJump, from, to, 0);
    }
    else if(movingDirection == 1){
      AssignSprite (TopLeftWalking, TopLeftLowJump, TopLeftHighJump, from, to, 180);
    }
    else if(movingDirection == 2){
      AssignSprite (DownLeftWalking, DownLeftLowJump, DownLeftHighJump, from, to, 180);
    }
    else if(movingDirection == 3){
      AssignSprite (DownLeftWalking, DownLeftLowJump, DownLeftHighJump, from, to, 0);
    }
  }

  public IEnumerator AttackAnimation(){
    while (true) {
      _displayedImage.sprite = _currentSprites [(GameManager.Instance.FrameCount / 6) % _currentSprites.Count];
      yield return new WaitForSeconds (DataHolder.FrameTime);
    }
  }

  public IEnumerator TargetAnimation(bool hit){
    while (true) {
      _displayedImage.sprite = _currentSprites [(GameManager.Instance.FrameCount / 6) % _currentSprites.Count];
      yield return new WaitForSeconds (DataHolder.FrameTime);
    }
  }
}
