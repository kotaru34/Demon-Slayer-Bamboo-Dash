using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteAnimator : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Sprite[] frames;
    private float frameTime = 0.2f;
    private float timer;
    private int frameIndex;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetFrames(Sprite[] newFrames, float framesPerSecond)
    {
        frames = newFrames;
        frameTime = framesPerSecond > 0 ? 1f / framesPerSecond : 0.2f;
        timer = 0f;
        frameIndex = 0;
        if (frames != null && frames.Length > 0)
        {
            spriteRenderer.sprite = frames[0];
        }
    }

    private void Update()
    {
        if (frames == null || frames.Length == 0)
        {
            return;
        }

        timer += Time.deltaTime;
        if (timer >= frameTime)
        {
            timer -= frameTime;
            frameIndex = (frameIndex + 1) % frames.Length;
            spriteRenderer.sprite = frames[frameIndex];
        }
    }
}