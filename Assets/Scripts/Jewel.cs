using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jewel : MonoBehaviour
{
    [SerializeField] private int _id;
    [SerializeField] private int _column;
    [SerializeField] private int _row;

    [SerializeField] private SpriteRenderer _spriteRenderer;

    private Transform _goTransform;

    private float _deltaPos = 0.002f;

    private bool _isSelected = false;
    private bool _isAnimationUp = false;
    private bool _isAlterable = false;

    private int _counter;

    public delegate void OnJewelClick(bool _isSelected, int _column, int _row);
    public static event OnJewelClick onJewelClick;

    private void Start()
    {
        _goTransform = gameObject.transform;
    }

    public void SetJewelData(int id, Sprite sprite)
    {
        _id = id;
        _spriteRenderer.sprite = sprite;
    }
    public void SetJewelPositionOnGrid(int row, int column)
    {
        _row = row;
        _column = column;
    }

    public int GetJewelID()
    {
        return _id;
    }

    private void OnMouseDown()
    {
        if (_isSelected == false) onJewelClick?.Invoke(false, _row, _column); else onJewelClick?.Invoke(true, _row, _column);
    }

    public void IsSelected(bool isSelected)
    {
        _isSelected = isSelected;
        if (isSelected) StartCoroutine(SelectingAnimationCoroutine());
    }

    public void IsAlterable(bool isAlterable)
    {
        _isAlterable = isAlterable;
        if (isAlterable) _spriteRenderer.sprite = null;
    }

    public bool IsAlterable() 
    {
        return _isAlterable;
    }

    private void SelectingAnimation() 
    {
        _isAnimationUp = _counter < 6 ? true : false;
        if (_isAnimationUp) _goTransform.position += new Vector3(0, _deltaPos, 0);
        else _goTransform.position -= new Vector3(0, _deltaPos, 0);
        _counter++;
        if (_counter > 11) _counter = 0;
    }

    public void SetJewelPosition() 
    {
        _goTransform.position = new Vector3(-0.81f + _column * 0.18f, 0.81f - _row * 0.18f, 0);
    }
    public void StartFallingAnimation(int numOfJewels) 
    {
        StartCoroutine(FallingAnimationCoroutine(numOfJewels));
    }
    private IEnumerator FallingAnimationCoroutine(int numOfJewels) 
    {
        for (int i = 0; i < numOfJewels * 6; i++) 
        {
            _goTransform.position -= new Vector3(0, 0.03f, 0); 
            yield return new WaitForSeconds(0.02f);
        }
        SetJewelPosition();
    }

    private IEnumerator SelectingAnimationCoroutine() 
    {
        while(_isSelected || _counter > 0) 
        {
            SelectingAnimation();
            yield return new WaitForSeconds(0.05f);
        }
    }
}
