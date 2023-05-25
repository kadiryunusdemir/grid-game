using UnityEngine;

public class TileHandler : MonoBehaviour
{
    private SpriteRenderer _renderer;
    public Vector2 position;
    
    private void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }

    private void OnMouseDown()
    {
        if (_renderer.enabled)
        {
            _renderer.enabled = false;
        }
        else if (!_renderer.enabled)
        {
            _renderer.enabled = true;
            GridHandler.Instance.CheckNeighbors(position);
        }
    }

    private void OnMouseEnter()
    {
        _renderer.color = Color.yellow;
    }

    private void OnMouseExit()
    {
        _renderer.color = Color.black;
    }
}
