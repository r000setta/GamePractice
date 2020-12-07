using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class HexGrid : MonoBehaviour
{
    public int width=6;
    public int height=6;

    public HexCell cellPrefab;

    HexCell[] cells;

    public Text cellLabelPrefab;

    Canvas gridCanvas;

    HexMesh hexMesh;
    public Color defaultColor=Color.white;
    public Color touchedColor=Color.magenta;

    public Texture2D noiseSource;

    private void Awake() {
        HexMetrics.noiseSource = noiseSource;
        gridCanvas = GetComponentInChildren<Canvas>();
        hexMesh=GetComponentInChildren<HexMesh>();
        cells=new HexCell[height*width];
        for(int z=0,i=0;z<height;++z){
            for(int x=0;x<width;++x){
                CreateCell(x,z,i++);
            }
        }
    }

    private void OnEnable() {
        HexMetrics.noiseSource=noiseSource;
    }

    void CreateCell(int x,int z,int i){
        Vector3 position;
        position.x=(x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
        position.y=0f;
        position.z=z*(HexMetrics.outerRadius*1.5f);

        HexCell cell=cells[i]=Instantiate<HexCell>(cellPrefab);
        cell.transform.SetParent(transform,false);
        cell.transform.localPosition=position;
        cell.coordinates=HexCoordinates.FromOffsetCoordinates(x,z);
        cell.color=defaultColor;
        if(x>0){
            cell.SetNeighbor(HexDirection.W,cells[i-1]);
        }
        if(z>0){
            if((z&1)==0){
                cell.SetNeighbor(HexDirection.SE,cells[i-width]);
                if(x>0){
                    cell.SetNeighbor(HexDirection.SW,cells[i-width-1]);
                }
            }else{
                cell.SetNeighbor(HexDirection.SW, cells[i - width]);
				if (x < width - 1) {
					cell.SetNeighbor(HexDirection.SE, cells[i - width + 1]);
				}
            }
        }

        Text label=Instantiate<Text>(cellLabelPrefab);
        label.rectTransform.SetParent(gridCanvas.transform,false);
        label.rectTransform.anchoredPosition=new Vector2(position.x,position.z);
        label.text=cell.coordinates.ToStringOnSeparteLines();

        cell.uiRect=label.rectTransform;
        cell.Elevation=0;
    }

    private void Start() {
        hexMesh.Triangulate(cells);
    }

    public HexCell GetCell(Vector3 position){
        position=transform.InverseTransformPoint(position);
        HexCoordinates coordinates=HexCoordinates.FromPosition(position);
        int idx=coordinates.X+coordinates.Z*width+coordinates.Z/2;
        return cells[idx];
        // Debug.Log("touched at "+coordinates.ToString());
    }

    public void Refresh(){
        hexMesh.Triangulate(cells);
    }
}
