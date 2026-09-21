using System.Runtime.InteropServices;
using Unity.Mathematics;
using UnityEngine;

public class Compass : MonoBehaviour
{
    public delegate void ChanceDirEventHandler(Direction dir);
    public event ChanceDirEventHandler OnChechedDir;

    public void RaiseOnChechedDir(Direction dir)
    {
        OnChechedDir.Invoke(dir);
    }

    public void Start() => OnChechedDir += ChanceCompassDir;
    

    public void ChanceCompassDir(Direction dir)
    {
        var eulerAngles = transform.eulerAngles;
        transform.rotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y, ConvertDirToInt(dir));
    }
    public static int ConvertDirToInt(Direction dir)
    {
        switch (dir)
        {
            case Direction.North: return 0;
            case Direction.East: return 90;
            case Direction.South: return 180;
            case Direction.West: return -90;
            default: return 0;
        }
    }
    
}
