using UnityEngine;

/// <summary>
/// A single subtitle entry for a given audio clip.
/// It includes the text and the time it should remain visible.
/// </summary>
[System.Serializable]
public class SubtitleSegment 
{
    [TextArea(2,4)]
    public string text;
    public float delay; // Duration this subtitle remains visible
}
