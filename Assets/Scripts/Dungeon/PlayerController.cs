using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Character
{
    [Range(0,10)]public float moveSpeed=1;
    private DungeonManager dungeonManager;
    private Vector2Int gridPosition;
    private Direction facing = Direction.North;
     Quaternion newRotation;
     Vector3 newPosition;
     float moveProgress=1;

    private Compass compass;

    public void Initialize(DungeonManager manager, Vector2Int startPosition, Compass uicompass)
    {
        dungeonManager = manager;
        gridPosition = startPosition;
        compass = uicompass;
        transform.position = new Vector3(gridPosition.x, 0, gridPosition.y);
        newPosition=transform.position;
        newRotation=transform.rotation;
        UpdateRotation();
    }

    void Update()
    {
        if (moveProgress<1)
        {
            moveProgress+=Time.deltaTime*moveSpeed;
            transform.position=Vector3.Lerp(transform.position,newPosition,moveProgress);
            transform.rotation=Quaternion.Lerp(transform.rotation,newRotation,moveProgress);
        }
        
        var keyboard = Keyboard.current;
        if (keyboard == null) return;   // intet tastatur fundet - undgår NullReferenceException

        if (keyboard.qKey.wasPressedThisFrame)
        {
            facing = DirectionUtils.RotateCounterClockwise(facing);
            UpdateRotation();
        }
        else if (keyboard.eKey.wasPressedThisFrame)
        {
            facing = DirectionUtils.RotateClockwise(facing);
            UpdateRotation();
        }

        if (keyboard.wKey.wasPressedThisFrame) TryMove(facing);
        else if (keyboard.sKey.wasPressedThisFrame) TryMove(DirectionUtils.Opposite(facing));
        else if (keyboard.aKey.wasPressedThisFrame) TryMove(DirectionUtils.RotateCounterClockwise(facing));
        else if (keyboard.dKey.wasPressedThisFrame) TryMove(DirectionUtils.RotateClockwise(facing));
    }

    private void TryMove(Direction direction)
    {
        Vector2Int offset = DirectionUtils.ToVector(direction);
        Vector2Int target = gridPosition + offset;

        if (dungeonManager.IsWalkable(target))
        {
            gridPosition = target;
            if (new Vector3(gridPosition.x, 0, gridPosition.y)!=newPosition)
            {
                newPosition = new Vector3(gridPosition.x, 0, gridPosition.y);
                moveProgress=0;
            }
            
        }
    }

    private void UpdateRotation()
    {
        Vector2Int dirVector = DirectionUtils.ToVector(facing);
        Vector3 lookDirection = new Vector3(dirVector.x, 0, dirVector.y);
        if (Quaternion.LookRotation(lookDirection, Vector3.up)!=newRotation)
        {
            newRotation=Quaternion.LookRotation(lookDirection, Vector3.up);
            moveProgress=0;
        }
        compass.RaiseOnChechedDir(facing);
    }
}

public static class DirectionUtils
{
    public static Vector2Int ToVector(Direction dir)
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

    public static Direction Opposite(Direction dir)
    {
        switch (dir)
        {
            case Direction.North: return Direction.South;
            case Direction.South: return Direction.North;
            case Direction.East: return Direction.West;
            case Direction.West: return Direction.East;
            default: return dir;
        }
    }

    public static Direction RotateClockwise(Direction dir)
    {
        switch (dir)
        {
            case Direction.North: return Direction.East;
            case Direction.East: return Direction.South;
            case Direction.South: return Direction.West;
            case Direction.West: return Direction.North;
            default: return dir;
        }
    }

    public static Direction RotateCounterClockwise(Direction dir)
    {
        switch (dir)
        {
            case Direction.North: return Direction.West;
            case Direction.West: return Direction.South;
            case Direction.South: return Direction.East;
            case Direction.East: return Direction.North;
            default: return dir;
        }
    }
}

public enum Direction
{
    North,
    South,
    East,
    West
}