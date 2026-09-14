using Unity.VisualScripting;
using UnityEngine;


public enum TileType
{
    Wall,
    EdgeWall,
    RoomWall,
    Floor,
    RoomFloor,
    Door
}

public class DungeonTile : MonoBehaviour
{
    [SerializeField] private GameObject wallVisual;
    [SerializeField] private GameObject floorVisual;
    [SerializeField] private GameObject pillarsVisual;

    public TileType type = TileType.Wall;
    private bool isWalkable = false;
    

    void Start()
    {
    }

    public void SetType(TileType type)
    {   
        this.type = type;
        if(type == TileType.Wall || type == TileType.EdgeWall || type == TileType.RoomWall){
            wallVisual.SetActive(true);
            floorVisual.SetActive(false);
            pillarsVisual.SetActive(false);
            isWalkable = false;
        }
        
        if(type == TileType.Floor || type == TileType.RoomFloor || type == TileType.Door){
            wallVisual.SetActive(false);
            floorVisual.SetActive(true);
            isWalkable = true;
            if (Random.value < 0.3f && type == TileType.Floor)
            {
                pillarsVisual.SetActive(true);
            }
            else
            {
                pillarsVisual.SetActive(false);
            }
        }
    }

    public bool IsWalkable()
    {
        return isWalkable;
    }
}