using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
[RequireComponent(typeof(SpriteRenderer))]
public class AutoColliderSize : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;

    private Vector2 lastSize;

    private void OnEnable()
    {
        SyncCollider();
    }

    private void OnValidate()
    {
        SyncCollider();
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();

            if (spriteRenderer != null && spriteRenderer.size != lastSize)
            {
                SyncCollider();
            }
        }
#endif
    }

    private void SyncCollider()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (boxCollider == null)
            boxCollider = GetComponent<BoxCollider2D>();

        if (boxCollider == null)
        {
#if UNITY_EDITOR
            boxCollider = Undo.AddComponent<BoxCollider2D>(gameObject);
#else
            boxCollider = gameObject.AddComponent<BoxCollider2D>();
#endif
        }

        boxCollider.size = spriteRenderer.size;
        boxCollider.offset = Vector2.zero;

        lastSize = spriteRenderer.size;

#if UNITY_EDITOR
        EditorUtility.SetDirty(boxCollider);
#endif
    }
}