using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Prevents subtitles from being obstructed by geometry. Moves canvas away on Z axis if blocked.
/// </summary>
public class SubtitleAvoidObstruction : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 400f;
    [SerializeField] private float checkRadius = 0.25f;
    [SerializeField] private float maxOffset = 700f;
    [SerializeField] private LayerMask meshLayers;
    private Vector3 originalLocalPos;
    private float currentOffset = 0f;
    private bool isBlocked;

    void Start()
    {
        originalLocalPos = transform.localPosition;
    }
  
    void Update()
    {
        Vector3 worldCheckPosition = transform.position;

        isBlocked = Physics.CheckSphere(worldCheckPosition, checkRadius, meshLayers);
        Collider[] hits = Physics.OverlapSphere(worldCheckPosition, checkRadius, meshLayers);
        isBlocked = hits.Length > 0;

        // Testing porpuses
        //if (isBlocked) 
        //{
        //    foreach (var hit in hits)
        //    {
        //        Debug.Log($"Subtitles collisioning with: {hit.gameObject.name} in layer {LayerMask.LayerToName(hit.gameObject.layer)}");
        //    }
        //}

        if (isBlocked && currentOffset < maxOffset)
        {
            currentOffset += moveSpeed * Time.deltaTime;
            currentOffset = Mathf.Min(currentOffset, maxOffset);
        }
        else if (!isBlocked && currentOffset > 0f)
        {
            currentOffset -= moveSpeed * Time.deltaTime;
            currentOffset = Mathf.Max(currentOffset, 0f);
        }

        // Moves only in local Z axis
        transform.localPosition = originalLocalPos + -new Vector3(0, 0, currentOffset);
    }

    public void ResetPositionSubs()
    {
        currentOffset = 0f;
        transform.localPosition = originalLocalPos;
    }
}
