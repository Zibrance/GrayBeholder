using UnityEngine;
using System.Collections.Generic;
public class DungeonManager : MonoBehaviour
{
    public int width = 100;
    public int height = 100;
    public int rooms = 8;
    public Vector2Int entryPoint = new Vector2Int(0, 0);
    private TileType[,] grid;
    private DungeonTile[,] dungeonTiles;
    private bool[,] visited;
    private Stack<Vector2Int> stack;

    [SerializeField] private DungeonTile dungeonTile;
    [SerializeField] private PlayerController playerPrefab;
    [SerializeField] private Camera sceneCamera;


    [SerializeField] private bool genRoom, genMaze;


    void Start()
    {
        SpawnDungeon();
        var player = Instantiate(playerPrefab);
        player.Initialize(this, entryPoint, FindAnyObjectByType<Compass>());
        sceneCamera.transform.SetParent(player.transform);
        sceneCamera.transform.localPosition = new Vector3(0, 0.9f, -0.6f);
        sceneCamera.transform.localRotation = Quaternion.identity;
        player.gameObject.SetActive(true);
    }

    private void SpawnDungeon()
    {
        grid = new TileType[width, height];
        dungeonTiles = new DungeonTile[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                bool isEdge = x == 0 || x == width - 1 || y == 0 || y == height - 1;
                grid[x, y] = isEdge ? TileType.EdgeWall : TileType.Wall;
            }
        }

        if (genMaze)
        {
            entryPoint = GenerateMaze();
        }

        grid[entryPoint.x, entryPoint.y] = TileType.Floor;

        if (genRoom)
        {
            var halls = GenerateRooms(rooms, 100);
            foreach (var room in halls)
            {
                for (int x = room.wallBounds.xMin; x < room.wallBounds.xMax; x++)
                    for (int y = room.wallBounds.yMin; y < room.wallBounds.yMax; y++)
                        grid[x, y] = TileType.RoomWall;

                for (int x = room.innerBounds.xMin; x < room.innerBounds.xMax; x++)
                    for (int y = room.innerBounds.yMin; y < room.innerBounds.yMax; y++)
                        grid[x, y] = TileType.RoomFloor;

                for (int x = room.outerBounds.xMin; x < room.outerBounds.xMax; x++)
                    for (int y = room.outerBounds.yMin; y < room.outerBounds.yMax; y++)
                        if (grid[x, y] != TileType.RoomWall && grid[x, y] != TileType.EdgeWall && grid[x, y] != TileType.RoomFloor)
                        {
                            grid[x, y] = TileType.Floor;
                        }
            }

            foreach (var room in halls)
            {

                var doorPlaced = false;
                while (!doorPlaced)
                {
                    var roomDoor = room.doorCandidates[Random.Range(0, room.doorCandidates.Count)];
                    foreach (var neighbour in GetNeighbours(roomDoor, width, height))
                    {
                        if (grid[neighbour.x, neighbour.y] == TileType.Floor)
                        {
                            grid[roomDoor.x, roomDoor.y] = TileType.Door;
                            doorPlaced = true;
                            break;
                        }
                    }
                }
            }
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 position = new Vector3(x, 0, y);
                dungeonTiles[x, y] = Instantiate(dungeonTile, position, Quaternion.identity);
                dungeonTiles[x, y].SetType(grid[x, y]);
            }
        }
    }

    public bool IsWalkable(Vector2Int pos)
    {
        if (pos.x < 0 || pos.x >= width || pos.y < 0 || pos.y >= height)
            return false;

        return dungeonTiles[pos.x, pos.y].IsWalkable();
    }
    private List<Room> GenerateRooms(int roomCount, int attempts)
    {
        List<Room> rooms = new List<Room>();

        for (int i = 0; i < attempts; i++)
        {
            int w = Random.Range(4, 9);
            int h = Random.Range(4, 9);
            int x = Random.Range(3, width - w - 3);
            int y = Random.Range(3, height - h - 3);

            Room candidateRoom = new Room(new RectInt(x, y, w, h));

            bool overlaps = false;
            for (int j = 0; j < rooms.Count; j++)
            {
                if (rooms[j].wallBounds.Overlaps(candidateRoom.innerBounds) || candidateRoom.outerBounds.Contains(entryPoint))
                {
                    overlaps = true;
                    break;
                }
            }

            if (!overlaps)
            {
                rooms.Add(candidateRoom);
            }

            if (rooms.Count >= roomCount)
                break;
        }

        return rooms;
    }
    private List<Vector2Int> GetNeighbours(Vector2Int cell, int cellsX, int cellsY)
    {
        List<Vector2Int> result = new List<Vector2Int>();

        foreach (Direction dir in System.Enum.GetValues(typeof(Direction)))
        {
            Vector2Int neighbour = cell + GetDirectionVector(dir);

            bool inBounds = neighbour.x >= 0 && neighbour.x < cellsX && neighbour.y >= 0 && neighbour.y < cellsY;

            if (inBounds)
            {
                result.Add(neighbour);
            }
        }

        return result;
    }
    private List<Vector2Int> GetUnvisitedNeighbours(Vector2Int cell, int cellsX, int cellsY)
    {
        List<Vector2Int> result = new List<Vector2Int>();
        foreach (var neighbour in GetNeighbours(cell, cellsX, cellsY))
        {
            if (!visited[neighbour.x, neighbour.y])
            {
                result.Add(neighbour);
            }
        }
        return result;
    }
    private Vector2Int GenerateMaze()
    {
        int cellsX = (width - 3) / 2 + 1;
        int cellsY = (height - 3) / 2 + 1;

        visited = new bool[cellsX, cellsY];
        stack = new Stack<Vector2Int>();

        Vector2Int startCell = new Vector2Int(cellsX / 2, 0);
        visited[startCell.x, startCell.y] = true;
        CarveCell(startCell);
        stack.Push(startCell);

        while (stack.Count > 0)
        {
            Vector2Int current = stack.Peek();
            List<Vector2Int> unvisitedNeighbours = GetUnvisitedNeighbours(current, cellsX, cellsY);

            if (unvisitedNeighbours.Count > 0)
            {
                Vector2Int next = unvisitedNeighbours[Random.Range(0, unvisitedNeighbours.Count)];

                visited[next.x, next.y] = true;
                CarveCell(next);
                CarveWallBetween(current, next);
                stack.Push(next);
            }
            else
            {
                stack.Pop();
            }
        }
        return CellToTile(startCell) + Vector2Int.down;
    }
    private Vector2Int CellToTile(Vector2Int cell)
    {
        return new Vector2Int(1 + cell.x * 2, 1 + cell.y * 2);
    }
    private void CarveCell(Vector2Int cell)
    {
        Vector2Int tile = CellToTile(cell);
        grid[tile.x, tile.y] = TileType.Floor;
    }
    private void CarveWallBetween(Vector2Int cellA, Vector2Int cellB, TileType type = TileType.Floor)
    {
        Vector2Int tileA = CellToTile(cellA);
        Vector2Int tileB = CellToTile(cellB);
        grid[(tileA.x + tileB.x) / 2, (tileA.y + tileB.y) / 2] = type;
    }
    private Vector2Int GetDirectionVector(Direction dir)
    {
        switch (dir)
        {
            case Direction.North: return new Vector2Int(0, 1);
            case Direction.South: return new Vector2Int(0, -1);
            case Direction.East: return new Vector2Int(1, 0);
            case Direction.West: return new Vector2Int(-1, 0);
            default: return Vector2Int.zero;
        }
    }
}

public class Room
{
    public RectInt innerBounds;
    public RectInt wallBounds;
    public RectInt outerBounds;
    public List<Vector2Int> doorCandidates;

    public Room(RectInt bounds)
    {
        doorCandidates = new List<Vector2Int>();
        innerBounds = bounds;
        wallBounds = new RectInt(innerBounds.x - 1, innerBounds.y - 1, innerBounds.width + 2, innerBounds.height + 2);
        outerBounds = new RectInt(wallBounds.x - 1, wallBounds.y - 1, wallBounds.width + 2, wallBounds.height + 2);

        for (int x = wallBounds.xMin; x < wallBounds.xMax; x++)
        {
            for (int y = wallBounds.yMin; y < wallBounds.yMax; y++)
            {
                var candidate = new Vector2Int(x, y);
                bool isCorner = (x == wallBounds.xMin || x == wallBounds.xMax - 1) &&
                                (y == wallBounds.yMin || y == wallBounds.yMax - 1);

                if (!innerBounds.Contains(candidate) && !isCorner)
                {
                    doorCandidates.Add(candidate);
                }
            }
        }
    }
}
