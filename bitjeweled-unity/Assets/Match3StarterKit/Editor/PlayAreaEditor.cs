using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class PlayAreaEditor : EditorWindow
{
    // Constants
    private const string STR_RESETBOARD = "RESET BOARD";
    private const string STR_BoardNo = "Board #";
    private const string STR_LOADBOARD = "LOAD BOARD";
    private const string STR_DataBoard = "Data/board";
    private const string STR_SAVEBOARD = "SAVE BOARD";
    private const string STR_ResourcesDataPath = "/Resources/Data";
    private const string STR_DataPath = "Data path: ";
    private const string STR_Board = "board";
    private const string STR_TxtExtension = ".txt";
    private const string STR_X = "X";
    private const string STR_Normal = "Normal";
    private const string STR_Strong = "Strong";
    private const string STR_ExtraStrong = "Extra Strong";
    private const string STR_SuperStrong = "Super Strong";
    private const string STR_BlockedTile = "Blocked Tile";
    private const string STR_NOTile = "NO Tile";
    private const string STR_BoardEditor = "Board Editor";
    private const string STR_CONFIRM = "CONFIRM";

    // Private members
    private static int columns = 10;
    private static int rows = 10;
    private static int rowsTMP = 10;
    private static int columnsTMP = 10;
    private static int[,] gdesc = new int[columns, rows];

    // Public members
    public GUISkin skin;

    [MenuItem("Match 3/Play Area Editor")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        PlayAreaEditor window = (PlayAreaEditor)EditorWindow.GetWindow(typeof(PlayAreaEditor));
        window.title = STR_BoardEditor;
        window.StartArea();
    }

    void StartArea()
    {
        gdesc = new int[columns, rows];
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                gdesc[x, y] = (int)TileType.TodoNormal;
            }
        }
    }

    static string[] r = { STR_Normal, STR_Strong, STR_ExtraStrong, STR_SuperStrong, STR_BlockedTile, STR_NOTile };
    static int sel = 0;
    static string tt = string.Empty;
    static int boardN = 1;
    void OnGUI()
    {
        GUI.skin = skin;
        sel = GUILayout.SelectionGrid(sel, r, r.Length, skin.button);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button(STR_RESETBOARD, skin.button))
        {
            StartArea();
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label(STR_BoardNo, skin.label);
        int.TryParse(GUILayout.TextField(boardN.ToString(), skin.textField), out boardN);
        if (GUILayout.Button(STR_LOADBOARD, skin.button))
        {
            TextAsset TXTFile = (TextAsset)Resources.Load(STR_DataBoard + boardN.ToString() + "." + rows.ToString() + "." + columns.ToString());
            if (TXTFile == null)
            {
                StartArea();
            }
            else
            {
                for (int y = 0; y < rows; y++)
                {
                    for (int x = 0; x < columns; x++)
                    {
                        gdesc[x, y] = new int();
                        if (!int.TryParse((TXTFile.text[x + y * columns].ToString()), out gdesc[x, y]))
                            gdesc[x, y] = (int)TileType.NoTile;
                    }
                }
            }
        }
        if (GUILayout.Button(STR_SAVEBOARD, skin.button))
        {
            string p = Application.dataPath + STR_ResourcesDataPath;
            Debug.Log(STR_DataPath + p);
            TextWriter tw = new StreamWriter(Path.Combine(p, STR_Board + boardN.ToString() + "." + rows.ToString() + "." + columns.ToString() + STR_TxtExtension), false, Encoding.ASCII);
            string f = string.Empty;
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    if (gdesc[x, y] == (int)TileType.NoTile)
                        f += STR_X;
                    else
                        f += gdesc[x, y].ToString();
                }
            }
            tw.Write(f);
            tw.Flush();
            tw.Close();
            AssetDatabase.Refresh();
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("# of Rows", skin.label);
        int.TryParse(GUILayout.TextField(rowsTMP.ToString(), skin.textField), out rowsTMP);
        GUILayout.Label("# of Columns", skin.label);
        int.TryParse(GUILayout.TextField(columnsTMP.ToString(), skin.textField), out columnsTMP);
        if (rows != rowsTMP || columns != columnsTMP)
        {
            if (GUILayout.Button(STR_CONFIRM, skin.button))
            {
                columns = columnsTMP;
                rows = rowsTMP;
                StartArea();
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginArea(new Rect(0, 100, Screen.width, Screen.height));
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                switch (gdesc[x, y])
                {
                    case (int)TileType.NoTile:
                        tt = r[5];
                        break;
                    case (int)TileType.TodoNormal:
                        tt = r[0];
                        break;
                    case (int)TileType.TodoStrong:
                        tt = r[1];
                        break;
                    case (int)TileType.TodoExtraStrong:
                        tt = r[2];
                        break;
                    case (int)TileType.TodoSuperStrong:
                        tt = r[3];
                        break;
                    case (int)TileType.BlockedTile:
                        tt = r[4];
                        break;
                }
                if (GUI.Button(new Rect(50 + x * 30, 50 + y * 30, 30, 30), string.Empty, tt))
                {
                    switch (sel)
                    {
                        case 0:
                            gdesc[x, y] = (int)TileType.TodoNormal;
                            break;
                        case 1:
                            gdesc[x, y] = (int)TileType.TodoStrong;
                            break;
                        case 2:
                            gdesc[x, y] = (int)TileType.TodoExtraStrong;
                            break;
                        case 3:
                            gdesc[x, y] = (int)TileType.TodoSuperStrong;
                            break;
                        case 4:
                            gdesc[x, y] = (int)TileType.BlockedTile;
                            break;
                        case 5:
                            gdesc[x, y] = (int)TileType.NoTile;
                            break;
                    }
                }
            }
            Vector2 tmp = this.minSize;
            tmp.x = 100 + columns * 30;
            tmp.y = 160 + rows * 30;
            this.minSize = tmp;
        }
        GUILayout.EndArea();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
