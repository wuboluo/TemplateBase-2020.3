using UnityEngine;
using UnityEngine.UI;

public class OutlineFlicker : MonoBehaviour
{
    private static readonly int LineWidth = Shader.PropertyToID("_lineWidth");

    public bool flickering;

    [SerializeField] private OutlineType outlineType;

    private bool _appear;
    private float _width;

    private Image _image;
    private Material _material;
    private SpriteRenderer _spriteRenderer;

    private const float TexSpeed = 2f;
    private const float ShaderSpeed = 6f;

    private float _speed;

    private void Start()
    {
        switch (outlineType)
        {
            case OutlineType.Sprite:
                _spriteRenderer = GetComponent<SpriteRenderer>();
                break;

            case OutlineType.Image:
                _image = GetComponent<Image>();
                break;

            case OutlineType.Shader:
                _material = GetComponent<SpriteRenderer>().material;
                break;
        }
    }

    private void Update()
    {
        if (!flickering) return;

        _speed = Time.deltaTime * (outlineType == OutlineType.Shader ? ShaderSpeed : TexSpeed);
        
        switch (outlineType)
        {
            case OutlineType.Sprite:
                if (_spriteRenderer.color.a <= 0) _appear = true;
                if (_spriteRenderer.color.a >= 1) _appear = false;
                
                if (_appear) _spriteRenderer.color += new Color(0, 0, 0, _speed);
                else _spriteRenderer.color -= new Color(0, 0, 0, _speed);
                break;

            case OutlineType.Image:
                if (_image.color.a <= 0) _appear = true;
                if (_image.color.a >= 1) _appear = false;
                
                if (_appear) _image.color += new Color(0, 0, 0, _speed);
                else _image.color -= new Color(0, 0, 0, _speed);
                break;

            case OutlineType.Shader:
                if (_width <= 0) _appear = true;
                if (_width >= 3) _appear = false;
                
                if (_appear) _width += _speed;
                else _width -= _speed;
                
                _material.SetFloat(LineWidth, _width);
                break;
        }
    }
}

public enum OutlineType
{
    Sprite,
    Image,
    Shader
}