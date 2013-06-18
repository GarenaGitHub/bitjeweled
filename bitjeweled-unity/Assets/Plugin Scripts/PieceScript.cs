using UnityEngine;

/// <summary>
/// Piece script.
/// </summary>
[AddComponentMenu("Match 3/Piece Script")] 
public class PieceScript : MonoBehaviour
{
    public bool Active = false;
    public AudioClip moveSound;

    internal TileType currentStrenght = TileType.TodoNormal;

	private GridPoint _Gp;
    private GridPoint _Gp1;
    private const float FLOAT_DragDetection = 10f;
    private bool moving = false;
    private bool mouseEnterAnimation = false;
    private float counter = 0f;
    private Vector3 yAxis = new Vector3(0, 1, 0);
    private Vector3 destination = Vector3.zero;
    private Vector3 velocity = Vector3.zero;
    private Vector3 previousPoint = Vector3.zero;
    private Vector3 currentPoint = Vector3.zero;
    private bool mouseDown = false;
    private bool mouseClick = false;
    private Transform myTransform;
    private float dragDelay = 0f;

	/// <summary>
	/// Gets a value indicating whether this <see cref="PieceScript"/> is moving.
	/// </summary>
	/// <value>
	/// <c>true</c> if moving; otherwise, <c>false</c>.
	/// </value>
    internal bool Moving
    {
        get
        {
            return moving;
        }
    }
	
	/// <summary>
	/// Gets my transform.
	/// </summary>
	/// <value>
	/// My transform.
	/// </value>
    internal Transform MyTransform
    {
        get
        {
            return myTransform;
        }
    }
	
    // Use this for initialization
    void Start()
    {
        myTransform = transform;
    }
	
	/// <summary>
	/// Moves to the given position using X and Y (2D coordinate).
	/// </summary>
	/// <param name='x'>
	/// X.
	/// </param>
	/// <param name='y'>
	/// Y.
	/// </param>
    public void MoveTo(int x, int y)
    {
		// This could become just a call like this:
		// MoveTo(x, y, myTransform.position.z); Why we didn't do so?
		// Because a call within a call adds more CPU cost than just 
		// these few lines that have to be executed anyway, so we decided
		// to keep it this way even if it's not elegant.
        moving = true;
		Board.PlayerCanMove = false;
		Board.wasMoving = true;
        destination = new Vector3(GridPositions.GetVector(x, y).x, GridPositions.GetVector(x, y).y, myTransform.position.z);
    }
	
	/// <summary>
	/// Moves to the given position using X,Y and Z (3D coordinate).
	/// </summary>
	/// <param name='x'>
	/// X.
	/// </param>
	/// <param name='y'>
	/// Y.
	/// </param>
	/// <param name='z'>
	/// Z.
	/// </param>
    public void MoveTo(int x, int y, float z)
    {
        moving = true;
		Board.PlayerCanMove = false;
		Board.wasMoving = true;
        destination = new Vector3(GridPositions.GetVector(x, y).x, GridPositions.GetVector(x, y).y, z);
    }
	
	/// <summary>
	/// Process the click/touch event.
	/// </summary>
    public void Clicked()
    {
		// Check if the click is legal at this time or else return
        if (!Board.PlayerCanMove ||
            currentStrenght == TileType.BlockedTile ||
            currentStrenght == TileType.TodoExtraStrong ||
            currentStrenght == TileType.TodoStrong ||
            currentStrenght == TileType.TodoSuperStrong)
        {
            return;
        }
		// Check if we are the active piece
        _Gp1 = GridPositions.GetGridPosition(new Vector2(myTransform.position.x, myTransform.position.y));
        if (_Gp1.x != -1 && Board.ActivePiece.x == -1)
        {
            Board.ActivePiece = _Gp1;
        }
        else if (_Gp1.x != -1) // No, so we become the active piece
        {
            if (Board.Instance.gameStyle == GameStyle.Standard && Mathf.Abs(_Gp1 - Board.ActivePiece) > 1)
            {
                Board.ActivePiece = _Gp1;
                return;
            }
			
			// See if moving to the new position is a legal move
            if (
                (!Board.Instance.isMatch4 &&
                 ((Board.Instance.CheckTileMatchX(_Gp1.x, _Gp1.y, Board.ActivePiece.x, Board.ActivePiece.y, true)) ||
                  (Board.Instance.CheckTileMatchX(Board.ActivePiece.x, Board.ActivePiece.y, _Gp1.x, _Gp1.y, true)) ||
                  (Board.Instance.CheckTileMatchY(_Gp1.x, _Gp1.y, Board.ActivePiece.x, Board.ActivePiece.y, true)) ||
                  (Board.Instance.CheckTileMatchY(Board.ActivePiece.x, Board.ActivePiece.y, _Gp1.x, _Gp1.y, true)))) ||
                (Board.Instance.isMatch4 &&
                 ((Board.Instance.CheckTileMatchX4(_Gp1.x, _Gp1.y, Board.ActivePiece.x, Board.ActivePiece.y, true)) ||
                  (Board.Instance.CheckTileMatchX4(Board.ActivePiece.x, Board.ActivePiece.y, _Gp1.x, _Gp1.y, true)) ||
                  (Board.Instance.CheckTileMatchY4(_Gp1.x, _Gp1.y, Board.ActivePiece.x, Board.ActivePiece.y, true)) ||
                  (Board.Instance.CheckTileMatchY4(Board.ActivePiece.x, Board.ActivePiece.y, _Gp1.x, _Gp1.y, true))))
                )
            {
                MoveTo(Board.ActivePiece.x, Board.ActivePiece.y);
                Board.PlayingPieces[Board.ActivePiece.x, Board.ActivePiece.y].pieceScript.MoveTo(_Gp1.x, _Gp1.y);
                Board.PlayingPieces[_Gp1.x, _Gp1.y].Active = true;
                Board.PlayingPieces[Board.ActivePiece.x, Board.ActivePiece.y].Active = false;
                PlayingPiece tmp = Board.PlayingPieces[_Gp1.x, _Gp1.y];
                Board.PlayingPieces[_Gp1.x, _Gp1.y] = Board.PlayingPieces[Board.ActivePiece.x, Board.ActivePiece.y];
                Board.PlayingPieces[Board.ActivePiece.x, Board.ActivePiece.y] = tmp;
                Board.ActivePiece = _Gp1;
                audio.PlayOneShot(moveSound);
                if (Board.Instance.gameStyle == GameStyle.Standard)
                    Board.ActivePiece.x = -1;
            }
            else if (Board.Instance.gameStyle == GameStyle.Standard)
            {
                Board.ActivePiece = _Gp1;
            }

        }
    }
	
	/// <summary>
	/// Processes the mouse drag event.
	/// </summary>
    void OnMouseDrag()
    {
        if (
            (!Board.PlayerCanMove || (Board.Instance.gameStyle != GameStyle.Standard)) ||
            currentStrenght == TileType.BlockedTile ||
            currentStrenght == TileType.TodoExtraStrong ||
            currentStrenght == TileType.TodoStrong ||
            currentStrenght == TileType.TodoSuperStrong
            )
            return;
        dragDelay += Time.deltaTime;
        if (dragDelay < 0.2f)
            return;
        dragDelay = 0;
        previousPoint = currentPoint;
        currentPoint = Input.mousePosition;
        _Gp = GridPositions.GetGridPosition(new Vector2(myTransform.position.x, myTransform.position.y));
        Vector3 dir = currentPoint - previousPoint;
        if (dir.x < -FLOAT_DragDetection)
        {
            if (
                (!Board.Instance.isMatch4 &&
                 ((Board.Instance.CheckTileMatchX(_Gp.x, _Gp.y, _Gp.x - 1, _Gp.y, true)) ||
                  (Board.Instance.CheckTileMatchX(_Gp.x - 1, _Gp.y, _Gp.x, _Gp.y, true)) ||
                (Board.Instance.CheckTileMatchY(_Gp.x, _Gp.y, _Gp.x - 1, _Gp.y, true)) ||
                (Board.Instance.CheckTileMatchY(_Gp.x - 1, _Gp.y, _Gp.x, _Gp.y, true))
                  )) ||
                (Board.Instance.isMatch4 &&
                 ((Board.Instance.CheckTileMatchX4(_Gp.x, _Gp.y, _Gp.x - 1, _Gp.y, true)) ||
                  (Board.Instance.CheckTileMatchX4(_Gp.x - 1, _Gp.y, _Gp.x, _Gp.y, true)) ||
                (Board.Instance.CheckTileMatchY4(_Gp.x, _Gp.y, _Gp.x - 1, _Gp.y, true)) ||
                 (Board.Instance.CheckTileMatchY4(_Gp.x - 1, _Gp.y, _Gp.x, _Gp.y, true))
                  ))
                )
            {
                MoveTo(_Gp.x - 1, _Gp.y);
                Board.PlayingPieces[_Gp.x - 1, _Gp.y].pieceScript.MoveTo(_Gp.x, _Gp.y);
                PlayingPiece tmp = Board.PlayingPieces[_Gp.x, _Gp.y];
                Board.PlayingPieces[_Gp.x, _Gp.y] = Board.PlayingPieces[_Gp.x - 1, _Gp.y];
                Board.PlayingPieces[_Gp.x - 1, _Gp.y] = tmp;
                Board.ActivePiece.x = -1;
                //Board.PlayerCanMove = false;
                audio.PlayOneShot(moveSound);
                mouseClick = false;
                mouseDown = false;
                dragDelay = 0f;
                return;
            }
        }
        if (dir.x > FLOAT_DragDetection)
        {
            if (
                (!Board.Instance.isMatch4 &&
                 ((Board.Instance.CheckTileMatchX(_Gp.x, _Gp.y, _Gp.x + 1, _Gp.y, true)) ||
                  (Board.Instance.CheckTileMatchX(_Gp.x + 1, _Gp.y, _Gp.x, _Gp.y, true)) ||
                (Board.Instance.CheckTileMatchY(_Gp.x, _Gp.y, _Gp.x + 1, _Gp.y, true)) ||
                (Board.Instance.CheckTileMatchY(_Gp.x + 1, _Gp.y, _Gp.x, _Gp.y, true))
                  )) ||
                (Board.Instance.isMatch4 &&
                 ((Board.Instance.CheckTileMatchX4(_Gp.x, _Gp.y, _Gp.x + 1, _Gp.y, true)) ||
                  (Board.Instance.CheckTileMatchX4(_Gp.x + 1, _Gp.y, _Gp.x, _Gp.y, true)) ||
                (Board.Instance.CheckTileMatchY4(_Gp.x, _Gp.y, _Gp.x + 1, _Gp.y, true)) ||
                (Board.Instance.CheckTileMatchY4(_Gp.x + 1, _Gp.y, _Gp.x, _Gp.y, true))
                  ))
                )
            {
                MoveTo(_Gp.x + 1, _Gp.y);
                Board.PlayingPieces[_Gp.x + 1, _Gp.y].pieceScript.MoveTo(_Gp.x, _Gp.y);
                PlayingPiece tmp = Board.PlayingPieces[_Gp.x, _Gp.y];
                Board.PlayingPieces[_Gp.x, _Gp.y] = Board.PlayingPieces[_Gp.x + 1, _Gp.y];
                Board.PlayingPieces[_Gp.x + 1, _Gp.y] = tmp;
                Board.ActivePiece.x = -1;
                //Board.PlayerCanMove = false;
                audio.PlayOneShot(moveSound);
                dragDelay = 0f;
                mouseClick = false;
                mouseDown = false;
                return;
            }
        }
        if (dir.y > FLOAT_DragDetection)
        {
            if (
                (!Board.Instance.isMatch4 &&
                 ((Board.Instance.CheckTileMatchX(_Gp.x, _Gp.y, _Gp.x, _Gp.y - 1, true)) ||
                  (Board.Instance.CheckTileMatchX(_Gp.x, _Gp.y - 1, _Gp.x, _Gp.y, true)) ||
                (Board.Instance.CheckTileMatchY(_Gp.x, _Gp.y, _Gp.x, _Gp.y - 1, true)) ||
                (Board.Instance.CheckTileMatchY(_Gp.x, _Gp.y - 1, _Gp.x, _Gp.y, true))
                  )) ||
                (Board.Instance.isMatch4 &&
                 ((Board.Instance.CheckTileMatchX4(_Gp.x, _Gp.y, _Gp.x, _Gp.y - 1, true)) ||
                  (Board.Instance.CheckTileMatchX4(_Gp.x, _Gp.y - 1, _Gp.x, _Gp.y, true)) ||
                  (Board.Instance.CheckTileMatchY4(_Gp.x, _Gp.y, _Gp.x, _Gp.y - 1, true)) ||
                  (Board.Instance.CheckTileMatchY4(_Gp.x, _Gp.y - 1, _Gp.x, _Gp.y, true))
                  ))
                )
            {
                MoveTo(_Gp.x, _Gp.y - 1);
                Board.PlayingPieces[_Gp.x, _Gp.y - 1].pieceScript.MoveTo(_Gp.x, _Gp.y);
                PlayingPiece tmp = Board.PlayingPieces[_Gp.x, _Gp.y];
                Board.PlayingPieces[_Gp.x, _Gp.y] = Board.PlayingPieces[_Gp.x, _Gp.y - 1];
                Board.PlayingPieces[_Gp.x, _Gp.y - 1] = tmp;
                Board.ActivePiece.x = -1;
                //Board.PlayerCanMove = false;
                audio.PlayOneShot(moveSound);
                dragDelay = 0f;
                mouseClick = false;
                mouseDown = false;
                return;
            }
        }
        if (dir.y < -FLOAT_DragDetection)
        {
            if (
                (!Board.Instance.isMatch4 &&
                 ((Board.Instance.CheckTileMatchX(_Gp.x, _Gp.y, _Gp.x, _Gp.y + 1, true)) ||
                  (Board.Instance.CheckTileMatchX(_Gp.x, _Gp.y + 1, _Gp.x, _Gp.y, true)) ||
                (Board.Instance.CheckTileMatchY(_Gp.x, _Gp.y, _Gp.x, _Gp.y + 1, true)) ||
                (Board.Instance.CheckTileMatchY(_Gp.x, _Gp.y + 1, _Gp.x, _Gp.y, true))
                  )) ||
                (Board.Instance.isMatch4 &&
                 ((Board.Instance.CheckTileMatchX4(_Gp.x, _Gp.y, _Gp.x, _Gp.y + 1, true)) ||
                  (Board.Instance.CheckTileMatchX4(_Gp.x, _Gp.y + 1, _Gp.x, _Gp.y, true)) ||
                (Board.Instance.CheckTileMatchY4(_Gp.x, _Gp.y, _Gp.x, _Gp.y + 1, true)) ||
                (Board.Instance.CheckTileMatchY4(_Gp.x, _Gp.y + 1, _Gp.x, _Gp.y, true))
                  ))
                )
            {
                MoveTo(_Gp.x, _Gp.y + 1);
                Board.PlayingPieces[_Gp.x, _Gp.y + 1].pieceScript.MoveTo(_Gp.x, _Gp.y);
                PlayingPiece tmp = Board.PlayingPieces[_Gp.x, _Gp.y];
                Board.PlayingPieces[_Gp.x, _Gp.y] = Board.PlayingPieces[_Gp.x, _Gp.y + 1];
                Board.PlayingPieces[_Gp.x, _Gp.y + 1] = tmp;
                Board.ActivePiece.x = -1;
                //Board.PlayerCanMove = false;
                audio.PlayOneShot(moveSound);
                dragDelay = 0f;
                mouseClick = false;
                mouseDown = false;
                return;
            }
        }
    }
	
	/// <summary>
	/// Processes the mouse down event.
	/// </summary>
    void OnMouseDown()
    {
        currentPoint = Input.mousePosition;
        dragDelay = 0;
        mouseDown = true;
    }
	
	/// <summary>
	/// Processes the mouse up event.
	/// </summary>
    void OnMouseUp()
    {
        if (mouseDown)
        {
            mouseClick = true;
            mouseDown = false;
        }
    }
	
	/// <summary>
	/// Processes the mouse enter event.
	/// </summary>
    void OnMouseEnter()
    {
        mouseDown = false;
        mouseClick = false;
        // Only play the animation for the normal piece
        if (currentStrenght == TileType.TodoNormal)
            mouseEnterAnimation = true;
    }
	
	/// <summary>
	/// Processes the mouse exit event.
	/// </summary>
    void OnMouseExit()
    {
        mouseDown = false;
        mouseClick = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (mouseClick && !moving)
        {
            mouseClick = false;
            Clicked();
        }
		//Do we have to play an animation?
        if (mouseEnterAnimation)
        {
            counter += Time.deltaTime;
            if (counter <= .6f)
            {
                myTransform.Rotate(yAxis, 500f * Time.deltaTime);
            }
            else
            {
                myTransform.rotation = Quaternion.identity;
                mouseEnterAnimation = false;
                counter = 0f;
            }
        }
		// Move this piece to its destination
        if (moving)
        {
            velocity = destination - myTransform.position;
            float damping = velocity.magnitude;
            if (velocity.sqrMagnitude < 0.5f)
            {
                moving = false;
                myTransform.position = destination;
            }
            else
            {
                velocity.Normalize();
                myTransform.position = (myTransform.position + (velocity * Time.deltaTime * (damping / 0.20f)));
            }
        }
    }
}
