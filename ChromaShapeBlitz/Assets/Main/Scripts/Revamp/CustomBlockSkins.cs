using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct SkinIdsInUse
{
    public int Blue;
    public int Green;
    public int Orange;
    public int Purple;
    public int Magenta;
    public int Yellow;
}

[Serializable]
public class CustomBlockSkins
{
    public readonly List<int> DefaultSkins = new List<int> { 1,2,3,4,5,6 };
    public List<int> Owned;
    public SkinIdsInUse InUse;
}