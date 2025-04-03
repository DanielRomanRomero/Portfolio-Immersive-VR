using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


/// <summary>
/// Detects when the lamp hits the player and transitions to the next scene using a fade.
/// </summary>
public class LampFade : MonoBehaviour
{
    [SerializeField] private Transform targetPoint;
    [SerializeField] private GameObject fadeImage;
    [SerializeField] private SkinnedMeshRenderer[] handsPlayer;
    [SerializeField] private MeshRenderer[] meshTransparentRenderers;
    [SerializeField] private LampController lampController;

    private bool doItOnces = false;

    private void Update()
    {
        if (!lampController.FinalCutsceneHasStarted) return;

        if(Vector3.Distance(transform.position, targetPoint.position) < 0.3f && !doItOnces)
        {
            fadeImage.SetActive(true);

            foreach(SkinnedMeshRenderer meshRenderer in handsPlayer)
            {
                meshRenderer.enabled = false;
            }

            foreach(MeshRenderer meshRenderer in meshTransparentRenderers)
            {
                meshRenderer.enabled = false;
            }

            StartCoroutine(LoadSceneAfterASecond());
            doItOnces = true;
        }
    }

    private IEnumerator LoadSceneAfterASecond()
    {
        yield return new WaitForSeconds(1);

        GameManager.Instance.DaniDevRoomCompleted();
     
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
