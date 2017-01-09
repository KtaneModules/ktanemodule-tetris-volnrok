using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TetrisModule : MonoBehaviour {

	public GameObject Board;

	public Material BoxFull;
	public Material BoxEmpty;
	public Material BoxError;

	public KMSelectable MoveLeftButton;
	public KMSelectable MoveRightButton;
	public KMSelectable TurnLeftButton;
	public KMSelectable TurnRightButton;
	public KMSelectable DownButton;

	private TetrisBoard GameBoard;

	private const int G_WIDTH = 8; // Width of grid

	private KMNeedyModule NeedyModule;

	private GameObject[,] ObjectGrid;

	private Tetromino tetr;
	private int piecesLeft;
	private int activation = 0;

	void SetMaterial(GameObject go, Material mat)
	{
		go.GetComponent<MeshRenderer> ().material = mat;
	}

	void UpdateGrid()
	{
		for (int y = 0; y < G_WIDTH; y++) {
			for (int x = 0; x < G_WIDTH; x++) {
				GameObject go = ObjectGrid [x, y];

				if (GameBoard.get (x, y) != 0) {
					go.SetActive (true);
					SetMaterial (go, BoxFull);
				} else {
					go.SetActive (false);
				}
			}
		}

		if (tetr != null) {
			List<IntPair> list = tetr.GetTileCoordinates ();
			foreach (IntPair p in list) {
				GameObject go = ObjectGrid [p.x, p.y];
				go.SetActive (true);
				if (tetr.isValid ()) {
					SetMaterial (go, BoxEmpty);
				} else {
					SetMaterial (go, BoxError);
				}
			}
		}
	}

	void ApplyTetromino() {
		if (tetr != null) {
			List<IntPair> list = tetr.GetTileCoordinates ();
			if (tetr.isValid ()) {
				foreach (IntPair p in list) {
					GameBoard.set (p.x, p.y, 1);
				}

				List<int> rows = GameBoard.getCompletedRows ();
				if (rows.Count > 0) {
					GameBoard.deleteRows (rows);
				}

				piecesLeft--;
				if (piecesLeft > 0) {
					tetr = new Tetromino (G_WIDTH, GameBoard);
				} else {
					NeedyModule.HandlePass ();
					tetr = null;
				}
				UpdateGrid ();
			}
		}
	}

	void Awake()
	{
		NeedyModule = GetComponent<KMNeedyModule> ();
		NeedyModule.OnNeedyActivation += OnNeedyActivation;
		NeedyModule.OnNeedyDeactivation += OnNeedyDeactivation;
		NeedyModule.OnTimerExpired += OnTimerExpired;

		/*MoveLeftButton.OnInteract += delegate() { return MoveLeft (); };
		MoveRightButton.OnInteract += delegate() { return MoveRight (); };
		TurnLeftButton.OnInteract += delegate() { return TurnLeft (); };
		TurnRightButton.OnInteract += delegate() { return TurnRight (); };
		DownButton.OnInteract += delegate() { return Down (); };*/

		ObjectGrid = new GameObject[G_WIDTH, G_WIDTH];

		GameBoard = new TetrisBoard (G_WIDTH, G_WIDTH);

		// Populate the grid
		for (int x = 0; x < G_WIDTH; x++) {
			GameObject col = Board.transform.Find ("Col" + x).gameObject;
			for (int y = 0; y < G_WIDTH; y++) {
				GameObject go = col.transform.Find ("Quad" + y).gameObject;
				go.SetActive (false);

				ObjectGrid [x, G_WIDTH - y - 1] = go;
			}
		}

		tetr = null;
		piecesLeft = 0;

		UpdateGrid ();
	}

	void Start()
	{
		MoveLeftButton.OnInteract += delegate() { return MoveLeft (); };
		MoveRightButton.OnInteract += delegate() { return MoveRight (); };
		TurnLeftButton.OnInteract += delegate() { return TurnLeft (); };
		TurnRightButton.OnInteract += delegate() { return TurnRight (); };
		DownButton.OnInteract += delegate() { return Down (); };
	}

	protected void OnNeedyActivation()
	{
		piecesLeft = 3;
		tetr = new Tetromino (G_WIDTH, GameBoard);
		UpdateGrid ();

		string sound = "";
		switch (activation) {
		case 0:
			sound = "part1";
			break;
		case 1:
			sound = "part2";
			break;
		case 2:
			sound = "part3";
			break;
		case 3:
			sound = "part4";
			break;
		case 4:
		case 6:
			sound = "part5";
			break;
		case 5:
			sound = "part6";
			break;
		case 7:
			sound = "part7";
			break;
		}

		if (sound.Length > 0) {
			GetComponent<KMAudio> ().PlaySoundAtTransform (sound, transform);
		}

		activation = (activation + 1) % 8;
	}

	protected void OnNeedyDeactivation()
	{

	}

	protected void OnTimerExpired()
	{
		NeedyModule.OnStrike();
		tetr = null;
		UpdateGrid ();
	}

	bool MoveLeft()
	{
		//GetComponent<KMAudio> ().PlaySoundAtTransform ("part1.wav", transform);
		GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
		if (tetr != null) {
			tetr.MoveLeft ();
			UpdateGrid ();
		}
		return false;
	}

	bool MoveRight()
	{
		GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
		if (tetr != null) {
			tetr.MoveRight ();
			UpdateGrid ();
		}
		return false;
	}

	bool TurnLeft()
	{
		GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
		if (tetr != null) {
			tetr.TurnLeft ();
			UpdateGrid ();
		}
		return false;
	}

	bool TurnRight()
	{
		GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
		if (tetr != null) {
			tetr.TurnRight ();
			UpdateGrid ();
		}
		return false;
	}

	bool Down()
	{
		GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
		ApplyTetromino ();
		return false;
	}

	/*void Update()
	{
		if (tetr != null) {
			bool moved = false;
			if (Input.GetKeyDown ("q")) {
				tetr.MoveLeft ();
				moved = true;
			}
			if (Input.GetKeyDown ("e")) {
				tetr.MoveRight ();
				moved = true;
			}
			if (Input.GetKeyDown ("a")) {
				tetr.TurnLeft ();
				moved = true;
			}
			if (Input.GetKeyDown ("d")) {
				tetr.TurnRight ();
				moved = true;
			}
			if (Input.GetKeyDown ("s")) {
				ApplyTetromino ();
				moved = true;
			}

			if (moved) {
				UpdateGrid ();
			}
		}
	}*/
}
