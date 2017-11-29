using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance;
	public float PlayerMoveSpeed;
	public int FrameCount;
	private GameObject cursor;
	private GameObject mapObject;

	private Vector3[] cursorNextMove = new Vector3[4];
	private Tile currentTile;
	private Tile previousTile;
	private Tile originTile;
	private Player currentPlayer;
	private Player currentTarget;
	public List<Tile> TilesQueueForPlayer = new List<Tile>();
	public Vector3 prePosition;
	public int preJumpStype;
	public List<Tile> map = new List<Tile>();
	public List<Player> players = new List<Player>();
	private Camera mainCamera;
	private bool choosingTarget;
	public bool attacking;



	private void Awake()
	{
		mainCamera = Camera.main;
		Instance = this;
		PlayerMoveSpeed = 3.5f;
		FrameCount = 0;
		mapObject = transform.Find("mapObject").gameObject;
	}
	private void Start()
	{
		SaveLoad.LoadMap (map, mapObject);
		//createMap();
		CreateCursor();
		SetCursorPosition();
		var p1 = ((GameObject)Instantiate(PrefabHolder.Instance.UserPlayer)).GetComponent<UserPlayer>();
		var p2 = ((GameObject)Instantiate(PrefabHolder.Instance.AIPlayer)).GetComponent<AIPlayer>();
		p1.transform.position = map[0].transform.position;
		players.Add(p1);
		p2.transform.position = map[4].transform.position;
		players.Add(p2);
	}

	private void Update()
	{
		if (choosingTarget)
		{

		}
		else
		{
			MoveCurrentPlayer();
		}

		KeyControll();

		IncreateFrameCount();
	}

	private void IncreateFrameCount()
	{
		FrameCount++;
		if (FrameCount > 10000) FrameCount = 0;
	}

	public void MoveCurrentPlayer()
	{
		if (TilesQueueForPlayer.Count > 0)
		{
			Vector3 currentPosition = TilesQueueForPlayer[0].transform.position;

			prePosition = transform.position;
			currentPlayer.transform.position += (currentPosition.Vector2() - currentPlayer.transform.position.Vector2()).normalized * Time.deltaTime * PlayerMoveSpeed;
			if (Vector3.Distance(currentPosition.Vector2(), currentPlayer.transform.position.Vector2()) <= 1.2f * PlayerMoveSpeed * Time.deltaTime)
			{
				currentPlayer.transform.position = currentPosition;
				previousTile = TilesQueueForPlayer[0];
				if (TilesQueueForPlayer.Count > 1) GetMovingDirection();
				TilesQueueForPlayer.RemoveAt(0);
				preJumpStype = Mathf.Abs(1 - preJumpStype);
				if (TilesQueueForPlayer.Count == 0)
				{
					currentTile = previousTile;
					previousTile = null;
					currentPlayer.MovingAnimation(currentPlayer.faceDirection, currentTile, currentTile);
					//currentPlayer.HighlightAttack();
					//choosingTarget = true;
					currentPlayer = null;
				}
			}
			if (TilesQueueForPlayer.Count > 0) currentPlayer.MovingAnimation(currentPlayer.faceDirection, previousTile, TilesQueueForPlayer[0]);
		}
	}

	private void GetMovingDirection()
	{
		for (int i = 0; i < TilesQueueForPlayer[0].neighbours.Count(); i++)
		{
			if (TilesQueueForPlayer[0].neighbours[i] == TilesQueueForPlayer[1])
			{
				currentPlayer.faceDirection = i;
				break;
			}
		}
	}

	private void CreateCursor()
	{
		Debug.Log(map.Count);
		cursor = (GameObject)Instantiate(PrefabHolder.Instance.Cursor, map[0].transform.position, Quaternion.identity);
	}

	private void SetCursorPosition()
	{
		cursorNextMove[0] = map[0].transform.position;
		GetCursorNextMove(0);
	}

	private void GetCursorNextMove(int direction)
	{
		cursor.transform.position = cursorNextMove[direction];
		MoveCamera(cursor.transform.position);
		currentTile = map.Where(x => x.transform.position == cursor.transform.position).First();
		for (int i = 0; i < 4; i++)
		{
			if (currentTile.neighbours[i] != null)
				cursorNextMove[i] = currentTile.neighbours[i].transform.position;
		}
	}

	private void MoveCamera(Vector3 follower)
	{
		if (follower.x - mainCamera.transform.position.x > 5.5f)
		{
			mainCamera.transform.position += Vector3.right;
		}
		else if (follower.x - mainCamera.transform.position.x < -5)
		{
			mainCamera.transform.position += Vector3.left;
		}

		if (follower.y - mainCamera.transform.position.y > 3.5f)
		{
			mainCamera.transform.position += Vector3.up;
		}
		else if (follower.y - mainCamera.transform.position.y < -2.5f)
		{
			mainCamera.transform.position += Vector3.down;
		}
	}

	public void KeyControll()
	{
		if (Input.GetKeyDown(KeyCode.S))
		{
			SaveLoad.SaveMapData(map, Path.Combine(Application.dataPath, @"Resources\" + "Maps.Xml"));
		}

		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			GetCursorNextMove(1);
		}
		else if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			GetCursorNextMove(3);
		}
		else if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			GetCursorNextMove(0);
		}
		else if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			GetCursorNextMove(2);
		}
		else if (Input.GetKeyDown(KeyCode.P))
		{
			Tile current = map.Where(x => x.transform.position == cursor.transform.position).First();
			List<Tile> highlightTile = Highlight.Movement(current, 4, new List<Tile>());
			OnHighlightTiles(highlightTile, true);
		}
		else if (Input.GetKeyUp(KeyCode.P))
		{
			Tile current = map.Where(x => x.transform.position == cursor.transform.position).First();
			List<Tile> highlightTile = Highlight.Movement(current, 4, new List<Tile>());
			OnHighlightTiles(highlightTile, false);
		}
		else if (Input.GetKeyDown(KeyCode.Space))
		{
			if (choosingTarget)
			{
				if (players.Where(x => x.currentTile() == currentTile).Count() > 0)
				{
					currentTarget = players.Where(x => x.currentTile() == currentTile).First();
					StartCoroutine(Attacking());
				}
				else
				{
					Debug.Log("Invalid Tile!");
				}
			}
			else if (previousTile == null)
			{
				if (players.Where(x => x.transform.position.x == cursor.transform.position.x && x.transform.position.y == cursor.transform.position.y).Count() > 0)
				{
					currentPlayer = players.Where(x => x.transform.position.x == cursor.transform.position.x && x.transform.position.y == cursor.transform.position.y).First();
					previousTile = currentTile;
					originTile = currentTile;
				}
			}
			else if (originTile != currentTile)
			{
				TilesQueueForPlayer = Highlight.FindPath(originTile, currentTile, new List<Tile>());
				GetMovingDirection();
			}
		}
	}

	IEnumerator Attacking()
	{
		attacking = true;
		int damage = Calculation.Dam(currentPlayer, currentTarget);
		bool hit = Random.Range(1, 100) < Calculation.Hit(currentPlayer, currentTarget);
		Debug.Log("attack this player");
		yield return new WaitForSeconds(0);
		StartCoroutine(currentPlayer.AttackAnimation());
		StartCoroutine(currentTarget.TargetAnimation(hit));
	}

	private void OnHighlightTiles(List<Tile> tiles, bool status)
	{
		for (int i = 0; i < tiles.Count; i++)
		{
			tiles[i].Highlight(status);
		}
	}
}
