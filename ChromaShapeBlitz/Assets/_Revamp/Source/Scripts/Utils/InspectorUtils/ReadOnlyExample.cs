using System.Collections.Generic;
using UnityEngine;

public class ReadOnlyExample : MonoBehaviour {
    [BeginReadOnlyGroup] // tag a group of fields as ReadOnly
    public string a;
    public int b;
    public Material c;
    public List<int> d = new List<int>();
    //public CustomTypeWithPropertyDrawer e; // Works!
    [EndReadOnlyGroup]

    [ReadOnly] public string a2;
    //[ReadOnly] public CustomTypeWithPropertyDrawer e2; // DOES NOT USE CustomPropertyDrawer!

    [BeginReadOnlyGroup]
    public int b2;
    public Material c2;
    public List<int> d2 = new List<int>();
    // Attribute tags apply to the next field of which there are no more so Unity/C# complains.
    // Since there are no more fields, we can omit the closing tag.
    // [EndReadOnlyGroup]

}

// Thanks @fuzzylogic
// https://discussions.unity.com/t/how-to-make-a-readonly-property-in-inspector/75448/7