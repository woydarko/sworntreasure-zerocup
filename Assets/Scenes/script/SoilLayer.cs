using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class SoilLayer
{
    public string layerName; 
    public int startY;       
    public int endY;         
    public TileBase tile;    
}