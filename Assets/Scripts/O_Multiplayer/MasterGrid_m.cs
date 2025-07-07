using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterGrid_m {

    private int length;
    private int width;
    private int height;
    bool[,,] gridArray;

    public MasterGrid_m(int length, int width, int height) {
        this.length = length;
        this.width = width;
        this.height = height;

        gridArray = new bool[length, width, height];

        for (int i = 0; i < length; i++) {
            for (int j = 0; j < height; j++) {
                for (int k = 0; k < width; k++) {
                    gridArray[i, j, k] = false;
                }
            }
        }
    }

    public void SwitchCellValue(int x, int y, int z) {
        gridArray[x, y, z] = !gridArray[x, y, z];
    }

    public bool GetCellValue(int x, int y, int z) {
        if (IsValidPosition(x, y, z)) {
            return gridArray[x, y, z];
        }
        else {
            return true;
        }
    }

    public Vector3 GridToWorld(int x, int y, int z) {
        Vector3 offset = new Vector3(0f, height, 0f);
        int centreX = length / 2;
        int centreY = height / 2;
        int centreZ = width / 2;
        return new Vector3(x - centreX, y - centreY, z - centreZ) + offset;
    }

    public Vector3Int WorldToGrid(Vector3 worldPosition) {
        Vector3 offset = new Vector3(0f, height, 0f);
        Vector3 adjustedPos = worldPosition - offset;

        int centreX = length / 2;
        int centreY = height / 2;
        int centreZ = width / 2;

        int x = Mathf.RoundToInt(adjustedPos.x) + centreX;
        int y = Mathf.RoundToInt(adjustedPos.y) + centreY;
        int z = Mathf.RoundToInt(adjustedPos.z) + centreZ;

        return new Vector3Int(x, y, z);
    }

    private bool IsValidPosition(int x, int y, int z) {
        return x >= 0 && y >= 0 && z >= 0 && x < length && y < height && z < width;
    }

    public int GetLength() {
        return length;
    }

    public int GetWidth() {
        return width;
    }

    public int GetHeight() {
        return height;
    }
}