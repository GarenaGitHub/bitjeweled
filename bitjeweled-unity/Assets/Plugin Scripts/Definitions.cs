using UnityEngine;

/// <summary>
/// This is the place where to start customizing the Match-3
/// </summary>
public enum PieceColour
{
	Blue = 0,
	Green,
	Orange,
	Purple,
	Red,
	Yellow,
	Violet,
	Indigo,
	Pink,
	LightGreen,
	Golden,
	Special,
	Bomb
}

/// <summary>
/// Tile type.
/// </summary>
public enum TileType
{
	NoTile = -1,
	Done = 0,
	TodoNormal = 1,
	TodoStrong = 2,
	TodoExtraStrong = 3,
	TodoSuperStrong = 4,
	BlockedTile = 5
}

public class Point
{
	public int x;
	public int y;
}

public class GridPoint
{
	public int x;
	public int y;
	
	/// <summary>
	/// Determines whether a specified instance of <see cref="GridPoint"/> is equal to another specified <see cref="GridPoint"/>.
	/// </summary>
	/// <param name='a'>
	/// The first <see cref="GridPoint"/> to compare.
	/// </param>
	/// <param name='b'>
	/// The second <see cref="GridPoint"/> to compare.
	/// </param>
	/// <returns>
	/// <c>true</c> if <c>a</c> and <c>b</c> are equal; otherwise, <c>false</c>.
	/// </returns>
	public static bool operator == (GridPoint a, GridPoint b)
	{
		return (a.x == b.x) && (a.y == b.y);
	}
	
	/// <summary>
	/// Determines whether a specified instance of <see cref="GridPoint"/> is not equal to another specified <see cref="GridPoint"/>.
	/// </summary>
	/// <param name='a'>
	/// The first <see cref="GridPoint"/> to compare.
	/// </param>
	/// <param name='b'>
	/// The second <see cref="GridPoint"/> to compare.
	/// </param>
	/// <returns>
	/// <c>true</c> if <c>a</c> and <c>b</c> are not equal; otherwise, <c>false</c>.
	/// </returns>
	public static bool operator != (GridPoint a, GridPoint b)
	{
		return (a.x != b.x) || (a.y != b.y);
	}
	
	/// <summary>
	/// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="GridPoint"/>.
	/// </summary>
	/// <param name='obj'>
	/// The <see cref="System.Object"/> to compare with the current <see cref="GridPoint"/>.
	/// </param>
	/// <returns>
	/// <c>true</c> if the specified <see cref="System.Object"/> is equal to the current <see cref="GridPoint"/>;
	/// otherwise, <c>false</c>.
	/// </returns>
	public override bool Equals (object obj)
	{
		GridPoint tmp = (GridPoint)obj;
		if (this == tmp)
			return true;
		else
			return false;
	}
	
	/// <summary>
	/// Serves as a hash function for a <see cref="GridPoint"/> object.
	/// </summary>
	/// <returns>
	/// A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a hash table.
	/// </returns>
	public override int GetHashCode ()
	{
		return base.GetHashCode ();
	}
	
	/// <summary>
	/// Subtracts a <see cref="GridPoint"/> from a <see cref="GridPoint"/>, yielding a new <see cref="System.Int32"/>.
	/// </summary>
	/// <param name='a'>
	/// The <see cref="GridPoint"/> to subtract from (the minuend).
	/// </param>
	/// <param name='b'>
	/// The <see cref="GridPoint"/> to subtract (the subtrahend).
	/// </param>
	/// <returns>
	/// The <see cref="System.Int32"/> that is the <c>a</c> minus <c>b</c>.
	/// </returns>
	public static int operator - (GridPoint a, GridPoint b)
	{
		// not a real subtraction, just a trick to speed up things
		if (a.x == b.x)
			return a.y - b.y;
		else if (a.y == b.y)
			return a.x - b.x;
		else
			return 100; // Just to make sure we don't process the move
	}
}

/// <summary>
/// Playing piece.
/// </summary>
internal class PlayingPiece
{
	internal bool Active = false;
	internal PieceColour Type = PieceColour.Yellow;
	internal bool SpecialPiece = false;
	internal bool Bomb = false;
	private bool selected = false;
	private GameObject piece = null;
	private PieceScript ps = null;
	private PieceValue pv = null;
	
	/// <summary>
	/// Gets the super strong value.
	/// </summary>
	/// <value>
	/// The super strong value.
	/// </value>
	public int Value {
		get {
			if (pv != null) {
				return pv.Value;
			} else {
				int r = 0;
				switch (ps.currentStrenght) {
				case TileType.TodoNormal:
					r = Board.Instance.PointsNormal;
					break;
				case TileType.TodoStrong:
					r = Board.Instance.PointsStrong;
					break;
				case TileType.TodoSuperStrong:
					r = Board.Instance.PointsSuperStrong;
					break;
				case TileType.TodoExtraStrong:
					r = Board.Instance.PointsExtraStrong;
					break;
				}
				return r;
			}
		}
	}
	
	/// <summary>
	/// Gets or sets the piece.
	/// </summary>
	/// <value>
	/// The piece.
	/// </value>
	public GameObject Piece {
		get {
			return piece;
		}
		set {
			piece = value;
			if (piece != null)
				ps = piece.GetComponent<PieceScript> ();
		}
	}
	
	/// <summary>
	/// Gets a value indicating whether this <see cref="PlayingPiece"/> is moving.
	/// </summary>
	/// <value>
	/// <c>true</c> if moving; otherwise, <c>false</c>.
	/// </value>
	internal bool Moving {
		get {
			if (ps != null)
				return ps.Moving;
			else
				return false;
		}
	}

	/// <summary>
	/// Gets the piece script.
	/// </summary>
	/// <value>
	/// The piece script.
	/// </value>
	public PieceScript pieceScript {
		get {
			return ps;
		}
	}
	
	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="PlayingPiece"/> is selected.
	/// </summary>
	/// <value>
	/// <c>true</c> if selected; otherwise, <c>false</c>.
	/// </value>
	internal bool Selected {
		get {
			return selected;
		}
		set {
			if (piece != null) {
				if (ps.Moving)
					selected = false;
				else
					selected = value;
			}
		}
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="PlayingPiece"/> class.
	/// </summary>
	/// <param name='obj'>
	/// Object.
	/// </param>
	/// <param name='planet'>
	/// Planet.
	/// </param>
	internal PlayingPiece (GameObject obj, PieceColour planet)
	{
		Piece = obj;
		Type = planet;
		pv = obj.GetComponent<PieceValue> ();
		if (planet == PieceColour.Bomb)
			Bomb = true;
	}
}

/// <summary>
/// Board tile.
/// </summary>
internal class BoardTile
{
	public GameObject Tile = null;
	internal TileType Type = TileType.TodoNormal;

	public BoardTile (GameObject tile, TileType type)
	{
		Tile = tile;
		Type = type;
	}
}

/// <summary>
/// Game style.
/// </summary>
public enum GameStyle
{
	Standard = 0,
	Marinas
}

/// <summary>
/// This is the player's data class. Feel free to change it as it suits you better
/// </summary>
[System.Serializable]
internal class gameState
{
	internal string PlayerName { get; set; } // This is the key used in the dictionary
	internal int CurrentLevel { get; set; }

	internal int CurrentStage { get; set; }

	internal long TotalScore { get; set; }

	internal bool gotAchievement_1 { get; set; }

	internal bool gotAchievement_2 { get; set; }

	internal bool gotAchievement_3 { get; set; }

	internal bool gotAchievement_4 { get; set; }

	internal bool gotAchievement_5 { get; set; }

	internal bool gotAchievement_6 { get; set; }

	internal bool gotAchievement_7 { get; set; }

	internal bool gotAchievement_8 { get; set; }

	internal bool gotAchievement_9 { get; set; }

	internal bool gotAchievement_10 { get; set; }
}

/// <summary>
/// This is the class that will be actually serialized.
/// It's indexed on the player's name
/// </summary>
[System.Serializable]
internal class ScoreTable : System.Collections.Generic.Dictionary<string, gameState>
{
}
