using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Collectible : MonoBehaviour
{
    [SerializeField] private int reward = 0;
    [SerializeField] private SpriteAnimator animator;

    private void Awake()
    {
        var collider = GetComponent<Collider2D>();
        collider.isTrigger = true;
    }

    public void ConfigureAnimation(Sprite[] frames, float speed)
    {
        animator = gameObject.AddComponent<SpriteAnimator>();
        animator.SetFrames(frames, speed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
        {
            return;
        }

        int payout = reward > 0 ? reward : GameManager.Instance.ScorePerCollectible;
        GameManager.Instance.AddScore(payout);
        Destroy(gameObject);
    }
}