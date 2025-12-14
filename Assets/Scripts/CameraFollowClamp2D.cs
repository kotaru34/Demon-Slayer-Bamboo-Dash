using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollowClamp2D : MonoBehaviour
{
    public Transform target;
    public WorldBoundsFromSprite boundsSource; // Background
    public float smoothTime = 0.15f;
    public Vector2 offset = Vector2.zero;

    private Camera cam;
    private Vector3 velocity;

    void Awake()
    {
      cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
      if (target == null) return;

      Vector3 desired = new Vector3(
        target.position.x + offset.x,
        target.position.y + offset.y,
        transform.position.z
      );

      Vector3 smoothed = Vector3.SmoothDamp(transform.position, desired, ref velocity, smoothTime);

      // Clamp until bg border
      if (boundsSource != null && cam.orthographic)
      {
        Rect r = boundsSource.WorldRect;

        float halfH = cam.orthographicSize;
        float halfW = halfH * cam.aspect;

        float minX = r.xMin + halfW;
        float maxX = r.xMax - halfW;
        float minY = r.yMin + halfH;
        float maxY = r.yMax - halfH;

        // center it if the camera is bigger than bg
        float clampedX = (minX > maxX) ? r.center.x : Mathf.Clamp(smoothed.x, minX, maxX);
        float clampedY = (minY > maxY) ? r.center.y : Mathf.Clamp(smoothed.y, minY, maxY);

        smoothed.x = clampedX;
        smoothed.y = clampedY;
      }

    transform.position = smoothed;
  }
}
