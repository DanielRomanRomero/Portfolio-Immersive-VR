using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data structure that stores an AudioClip along with its corresponding list of subtitle segments.
/// Used inside RobotSoundModule ScriptableObject.
/// </summary>
[System.Serializable]
public class RobotAudioData 
{
    public AudioClip clip;

    public List<SubtitleSegment> subtitles = new();

}
