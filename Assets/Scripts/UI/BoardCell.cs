using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TicTacToe.Events;

[RequireComponent(typeof(Button))]
public class BoardCell : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button _button;
    [SerializeField] private TextMeshProUGUI _symbolText;
    [SerializeField] private Image _background;
    
    [Header("Visuals")]
    [SerializeField] private Color _normalColor = Color.white;
    [SerializeField] private Color _hoverColor = new Color(0.9f, 0.9f, 0.9f);
    [SerializeField] private Color _winColor = Color.green;
    
    private int _row;
    private int _column;
    private bool _isOccupied;
    
    private void Awake()
    {
        if (_button == null)
            _button = GetComponent<Button>();
        
        if (_symbolText == null)
            _symbolText = GetComponentInChildren<TextMeshProUGUI>();
        
        if (_background == null)
            _background = GetComponent<Image>();
        
        _button.onClick.AddListener(OnCellClicked);
    }
    
    public void Initialize(int row, int column)
    {
        _row = row;
        _column = column;
        Clear();
    }
    
    private void OnCellClicked()
    {
        if (_isOccupied)
            return;
        
        // Publish cell clicked event
        EventBus.Publish(new CellClickedEvent
        {
            Row = _row,
            Column = _column
        });
    }
    
    public void SetSymbol(string symbol, Color color)
    {
        _symbolText.text = symbol;
        _symbolText.color = color;
        _isOccupied = true;
        _button.interactable = false;
    }
    
    public void Clear()
    {
        _symbolText.text = "";
        _isOccupied = false;
        _button.interactable = true;
        _background.color = _normalColor;
    }
    
    public void HighlightAsWinning(Color winColor)
    {
        _background.color = winColor;
    }
    
    public void SetHoverState(bool isHovering)
    {
        if (!_isOccupied && !isHovering)
        {
            _background.color = _normalColor;
        }
        else if (!_isOccupied && isHovering)
        {
            _background.color = _hoverColor;
        }
    }
    
    public bool IsOccupied => _isOccupied;
    public int Row => _row;
    public int Column => _column;
}
