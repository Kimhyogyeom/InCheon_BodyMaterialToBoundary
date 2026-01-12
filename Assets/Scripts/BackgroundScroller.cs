using UnityEngine;
using UnityEngine.UI;

public class BackgroundScroller : MonoBehaviour
{
    [Header("스크롤 설정")]
    [SerializeField] private float scrollSpeedX = 0.1f;
    [SerializeField] private float scrollSpeedY = -0.1f;

    private RawImage _rawImage;
    private Material _material;
    private Vector2 _offset;

    void Start()
    {
        // UI RawImage 체크
        _rawImage = GetComponent<RawImage>();
        if (_rawImage != null)
        {
            return; // RawImage는 Update에서 uvRect로 처리
        }

        // SpriteRenderer 체크
        var spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            _material = spriteRenderer.material;
        }
    }

    void Update()
    {
        _offset.x += scrollSpeedX * Time.deltaTime;
        _offset.y += scrollSpeedY * Time.deltaTime;

        // RawImage 사용
        if (_rawImage != null)
        {
            _rawImage.uvRect = new Rect(_offset.x, _offset.y, 1, 1);
            return;
        }

        // SpriteRenderer 사용 (Custom/ScrollingSprite 셰이더)
        if (_material != null)
        {
            _material.SetFloat("_ScrollX", _offset.x);
            _material.SetFloat("_ScrollY", _offset.y);
        }
    }
}
