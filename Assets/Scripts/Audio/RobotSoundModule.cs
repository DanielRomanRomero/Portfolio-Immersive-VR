using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// ScriptableObject that contains all audio data for a specific room or gameplay section.
/// Each entry includes a clip and its subtitles via RobotAudioData.
/// </summary>
[CreateAssetMenu(fileName = "RobotSoundModule")]
public class RobotSoundModule : ScriptableObject
{
    public string room;
    public List<RobotAudioData> sounds = new List<RobotAudioData>();


}
