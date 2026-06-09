using System.Collections.Generic;
using UnityEngine;

public class CloudLooper : MonoBehaviour
{
    [Header("Cloud Selection")]
    public string cloudNameFilter = "awan";

    [Header("Movement")]
    public float speed = 1.2f;
    public float wrapPadding = 1.5f;
    public float minCloudSpacing = 2f;

    [Header("Camera Bounds")]
    public Camera targetCamera;

    private readonly List<SpriteRenderer> clouds = new List<SpriteRenderer>();

    void Awake()
    {
        CacheClouds();
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }
    }

    void Update()
    {
        if (targetCamera == null || clouds.Count == 0)
        {
            return;
        }

        float leftEdge = GetCameraWorldX(0f);
        float deltaX = speed * Time.deltaTime;

        for (int i = 0; i < clouds.Count; i++)
        {
            SpriteRenderer cloud = clouds[i];
            if (cloud == null)
            {
                continue;
            }

            cloud.transform.position += Vector3.left * deltaX;

            if (cloud.bounds.max.x < leftEdge - wrapPadding)
            {
                MoveCloudToRight(cloud);
            }
        }
    }

    void CacheClouds()
    {
        clouds.Clear();

        SpriteRenderer[] childRenderers = GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < childRenderers.Length; i++)
        {
            SpriteRenderer childRenderer = childRenderers[i];
            if (childRenderer.name.ToLowerInvariant().Contains(cloudNameFilter.ToLowerInvariant()))
            {
                clouds.Add(childRenderer);
            }
        }
    }

    void MoveCloudToRight(SpriteRenderer cloud)
    {
        float rightMostCloudEdge = GetRightMostCloudEdge(cloud);
        float cameraRightEdge = GetCameraWorldX(1f);
        float cloudHalfWidth = cloud.bounds.extents.x;
        float newX = Mathf.Max(rightMostCloudEdge + minCloudSpacing + cloudHalfWidth, cameraRightEdge + wrapPadding + cloudHalfWidth);

        Vector3 position = cloud.transform.position;
        position.x = newX;
        cloud.transform.position = position;
    }

    float GetRightMostCloudEdge(SpriteRenderer ignoredCloud)
    {
        float rightMost = GetCameraWorldX(1f);

        for (int i = 0; i < clouds.Count; i++)
        {
            SpriteRenderer cloud = clouds[i];
            if (cloud != null && cloud != ignoredCloud && cloud.bounds.max.x > rightMost)
            {
                rightMost = cloud.bounds.max.x;
            }
        }

        return rightMost;
    }

    float GetCameraWorldX(float viewportX)
    {
        float distanceFromCamera = Mathf.Abs(transform.position.z - targetCamera.transform.position.z);
        return targetCamera.ViewportToWorldPoint(new Vector3(viewportX, 0.5f, distanceFromCamera)).x;
    }
}
