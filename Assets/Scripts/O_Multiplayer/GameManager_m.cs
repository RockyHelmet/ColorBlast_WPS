using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager_m : NetworkBehaviour {

    public List<string> colors = new List<string>();

    private void Start() {
        colors.Add("Red");
        colors.Add("Yellow");
        colors.Add("Green");
        colors.Add("Blue");
    }
}
