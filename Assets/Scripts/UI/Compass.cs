using System.Security.Cryptography.X509Certificates;
using Unity.Mathematics;
using UnityEngine;

public class Compass : MonoBehaviour
{

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
