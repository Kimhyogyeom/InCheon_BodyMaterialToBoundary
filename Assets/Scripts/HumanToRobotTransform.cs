using UnityEngine;

public class HumanToRobotTransform : MonoBehaviour
{
    [Header("Transform Settings")]
    [Range(0, 100)]
    [Tooltip("0 = 완전 사람, 100 = 완전 로봇")]
    public float transformValue = 0;

    [Header("Sprite Renderers")]
    public SpriteRenderer humanSprite;
    public SpriteRenderer androidSprite;

    private Material humanMaterial;
    private Material androidMaterial;

    void Start()
    {
        // Material 인스턴스 생성
        if (humanSprite != null)
        {
            humanMaterial = humanSprite.material;
        }

        if (androidSprite != null)
        {
            androidMaterial = androidSprite.material;
        }
    }

    void Update()
    {
        // 0~100을 0~1로 정규화
        float normalized = transformValue / 100f;

        // 사람: 왼쪽부터 사라짐 (오른쪽만 남음)
        if (humanMaterial != null)
        {
            humanMaterial.SetFloat("_DissolveAmount", normalized);
        }

        // 안드로이드: 왼쪽부터 나타남
        if (androidMaterial != null)
        {
            androidMaterial.SetFloat("_DissolveAmount", normalized);
        }
    }

    /// <summary>
    /// 모든 값 초기화
    /// </summary>
    public void AllInit()
    {
        transformValue = 0;
    }
}
