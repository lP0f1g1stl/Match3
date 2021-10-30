using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridHandler : MonoBehaviour
{
    [SerializeField] private GameObject _tile;
    [SerializeField] private GameObject[,] _tiles;

    [SerializeField] private Sprite[] _sprites;
    [SerializeField] private Menu scoreHandler;

    [SerializeField] private int _rows;
    [SerializeField] private int _colums;

    private int _matchCounterHorisontal;
    private int _matchCounterVertical;

    private int[] _selectedJewelPos;

    private int _counter;
    private int _maxCounter;

    private bool _isPlaying = true;

    private void Start()
    {
        Jewel.onJewelClick += CheckSelectedJewelState;
        _selectedJewelPos = new int[2];
        _tiles = new GameObject[_rows, _colums];
        CreateTiles();
    }
    private void CreateTiles() 
    {
        for (int i = 0; i < _rows; i++) 
        {
            for (int j = 0; j < _colums; j++) 
            {
                _tiles[i, j] = Instantiate(_tile, new Vector3(-0.81f + j * 0.18f, 0.81f - i * 0.18f, 0), Quaternion.identity);
                int rand = RandomTilesWithoutReapiting(i,j);
                _tiles[i, j].GetComponent<Jewel>().SetJewelData(rand, _sprites[rand]);
                _tiles[i, j].GetComponent<Jewel>().SetJewelPosition(i, j);
            }
        }
    }

    private int RandomTilesWithoutReapiting(int i, int j) 
    {
        int rand = Random.Range(0, 6);
        int prevRand = -1;
        if (j - 1 >= 0) 
        {
            int previousLeft = _tiles[i, j - 1].GetComponent<Jewel>().GetJewelID();
            if (rand == previousLeft) _matchCounterHorisontal++; else _matchCounterHorisontal = 0;
            if (_matchCounterHorisontal > 1)
            {
                prevRand = rand;
                rand = (rand + 1) % 6;

            }
        } 
        else _matchCounterHorisontal = 0;
        
        for (int k = 1; k < 3; k++)
        {
            if (i - k >= 0)
            {
                int previousBelow = _tiles[i - k, j].GetComponent<Jewel>().GetJewelID();
                if (rand == previousBelow) _matchCounterVertical++; else _matchCounterVertical = 0;
                if (_matchCounterVertical > 1)
                {
                    rand = (rand + 1) % 6;
                    if (rand == prevRand) rand = (rand + 1) % 6;
                }
            }
        }
        _matchCounterVertical = 0;
        return rand;
    }

    private void CheckSelectedJewelState(bool isSelected, int row, int column) 
    {
        if (_isPlaying)
        {
            if (_counter == 0)
            {
                _tiles[row, column].GetComponent<Jewel>().IsSelected(true);
                _counter++;
                _selectedJewelPos[0] = row;
                _selectedJewelPos[1] = column;
            }
            else
            {
                if (isSelected == true)
                {
                    _tiles[row, column].GetComponent<Jewel>().IsSelected(false);
                    _counter--;
                    _selectedJewelPos[0] = -1;
                    _selectedJewelPos[1] = -1;
                }
                else
                {
                    SwapJewels(row, column);
                }
            }
        }
    }

    private void SwapJewels(int row, int column)
    {
        if (((_selectedJewelPos[0] == row + 1 || _selectedJewelPos[0] == row - 1) && _selectedJewelPos[1] == column) || ((_selectedJewelPos[1] == column + 1 || _selectedJewelPos[1] == column - 1) && _selectedJewelPos[0] == row)) 
        {
            int firstJewelID = _tiles[_selectedJewelPos[0], _selectedJewelPos[1]].GetComponent<Jewel>().GetJewelID();
            int secondJewelID = _tiles[row, column].GetComponent<Jewel>().GetJewelID();
            _tiles[_selectedJewelPos[0], _selectedJewelPos[1]].GetComponent<Jewel>().SetJewelData(secondJewelID, _sprites[secondJewelID]);
            _tiles[row, column].GetComponent<Jewel>().SetJewelData(firstJewelID, _sprites[firstJewelID]);
            _tiles[_selectedJewelPos[0], _selectedJewelPos[1]].GetComponent<Jewel>().IsSelected(false);
            _counter--;
            bool isMatchFound = CheckGrid();
            if (!isMatchFound) 
            {
                _tiles[_selectedJewelPos[0], _selectedJewelPos[1]].GetComponent<Jewel>().SetJewelData(firstJewelID, _sprites[firstJewelID]);
                _tiles[row, column].GetComponent<Jewel>().SetJewelData(secondJewelID, _sprites[secondJewelID]);
            }
            else 
            {
                StartCoroutine(PlayingStateTimer());
            }
            _selectedJewelPos[0] = -1;
            _selectedJewelPos[1] = -1;
        }
    }

    private bool CheckGrid() 
    {
        bool isMatchFound = false;
        for (int i = 0; i < _rows; i++) 
        {
            for (int j = 0; j < _colums; j++) 
            {
                bool isMatch = FindMatches(i, j);
                if (isMatch) isMatchFound = true;
            }
        }
        return isMatchFound;
    }

    private bool FindMatches(int i, int j)
    {
        int currentJewel = _tiles[i, j].GetComponent<Jewel>().GetJewelID();
        int matchesCounter = 0;
        bool isMatchFound = false;
        for (int c = 1; c < _colums; c++)
        {
            if (j + c < _colums && (currentJewel == _tiles[i, j + c].GetComponent<Jewel>().GetJewelID()))
            {
                matchesCounter++;
            }
            else
            {
                if (matchesCounter > 1)
                {
                    for (int g = 0; g < matchesCounter + 1; g++)
                    {
                        _tiles[i, j + g].GetComponent<Jewel>().IsAlterable(true);
                        isMatchFound = true;
                    }
                }
                break;
            }
        }
        matchesCounter = 0;
        for (int c = 1; c < _rows; c++)
        {
            if (i + c < _rows && (currentJewel == _tiles[i + c, j].GetComponent<Jewel>().GetJewelID()))
            {
                matchesCounter++;
            }
            else
            {
                if (matchesCounter > 1)
                {
                    for (int g = 0; g < matchesCounter + 1; g++)
                    {
                        _tiles[i + g, j].GetComponent<Jewel>().IsAlterable(true);
                        isMatchFound = true;
                    }
                }
                break;
            }
        }
        return isMatchFound;
    }

    private void CheckGridForChangeAndStartAnimation()
    {
        int score = 0;
        _maxCounter = 0;
        int counter = 0;
        for (int j = 0; j < _colums; j++)
        {
            for (int i = _rows - 1; i > -1; i--)
            {
                bool isAlterable = _tiles[i, j].GetComponent<Jewel>().IsAlterable();
                if (isAlterable == true)
                {
                    counter++;
                    score += 1;
                }
                else
                {
                    if (counter > 0 && i + counter > 0)
                    {
                        _tiles[i + counter, j].transform.position = _tiles[i, j].transform.position;
                        GameObject currentJewel = _tiles[i, j];
                        _tiles[i, j] = _tiles[i + counter, j];
                        _tiles[i + counter, j] = currentJewel;
                        _tiles[i, j].GetComponent<Jewel>().SetJewelPosition(i, j);
                        _tiles[i + counter, j].GetComponent<Jewel>().SetJewelPosition(i + counter, j);
                        _tiles[i + counter, j].GetComponent<Jewel>().StartFallingAnimation(counter);
                    }
                }
            }
            if (_maxCounter < counter) _maxCounter = counter;
            counter = 0;
        }
        scoreHandler.ChangeScore(score);
    }

    private void ChangeEmptyTiles()
    {
        for (int j = 0; j < _colums; j++)
        {
            for (int i = 0; i < _rows; i++)
            {
                bool isAlterable = _tiles[i, j].GetComponent<Jewel>().IsAlterable();
                if (isAlterable == true)
                {
                    _tiles[i, j].GetComponent<Jewel>().IsAlterable(false);
                    int rand = Random.Range(0, 6);
                    _tiles[i, j].GetComponent<Jewel>().SetJewelData(rand, _sprites[rand]);
                }
                else break;
            }
        } 
    }

    private IEnumerator PlayingStateTimer() 
    {
        bool isMatchFound = true;
        yield return new WaitForSeconds(0.3f);
        while (isMatchFound)
        {
            _isPlaying = false;
            isMatchFound = CheckGrid();
            CheckGridForChangeAndStartAnimation();
            yield return new WaitForSeconds((_maxCounter +1) * 0.3f);
            ChangeEmptyTiles();
            yield return new WaitForSeconds(0.3f);
        }
        _isPlaying = true;
    }
    private void OnDestroy()
    {
        Jewel.onJewelClick -= CheckSelectedJewelState;
    }
}
