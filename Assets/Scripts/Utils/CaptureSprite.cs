using System.Collections;
using UnityEngine;
using System.IO;

/// <summary>
/// Class used to take a photo of the door (scene 1) for the first game cinematic. Not really needed for the game anymore.
/// </summary>
public class CaptureSprite : MonoBehaviour
{
    public Camera captureCamera; 
    public RenderTexture renderTexture; 

    private void Start()
    {
        StartCoroutine(TakeCapture());
    }

    private IEnumerator TakeCapture()
    {
        yield return new WaitForSeconds(2);
        Capture();
    }

    public void Capture()
    {
        RenderTexture.active = renderTexture;
        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
        captureCamera.Render();
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();
        RenderTexture.active = null;

        byte[] bytes = texture.EncodeToPNG();
        string filePath = Application.dataPath + "/CapturedSprites/" + gameObject.name + ".png";
        File.WriteAllBytes(filePath, bytes);
        Debug.Log("sprite saved at: " + filePath);

        Destroy(texture);
    }
}
