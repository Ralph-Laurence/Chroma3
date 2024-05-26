using System;
using System.Collections.Generic;

[Serializable]
public struct EquippedSkinIds
{
    public int Blue;
    public int Green;
    public int Orange;
    public int Purple;
    public int Magenta;
    public int Yellow;
}

[Serializable]
public class Customizations
{
    public List<int> OwnedBackgrounds;
    public List<int> OwnedSkins;
    public List<int> DefaultSkins;

    public EquippedSkinIds EquippedSkinId;
    public int EquippedBackgroundId;
    public int DefaultBackgroundId;
}