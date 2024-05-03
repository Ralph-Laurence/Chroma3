using System;
using System.Collections.Generic;

[Serializable]
public struct ThemeItem
{
    public int EquippedSkinId;
    public int EquippedBackgroundId;
}

[Serializable]
public class Customizations
{
    public List<int> OwnedBackgrounds;// = new() {  1, 2, 3, 4, 5, 6 };
    public List<int> OwnedSkins;
    public List<int> DefaultSkins;
    public ThemeItem BlueTheme;
    public ThemeItem GreenTheme;
    public ThemeItem OrangeTheme;
    public ThemeItem PurpleTheme;
    public ThemeItem MagentaTheme;
    public ThemeItem YellowTheme;

}