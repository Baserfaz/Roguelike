using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialManager : MonoBehaviour {

    public static MaterialManager instance;

    [Header("Materials")]
    public Material mySpriteMaterial;

    void Awake() { instance = this; }
}
