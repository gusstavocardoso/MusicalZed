namespace MusicalZed.Web.Services;

public class CartStateService
{
    public string SessionId { get; } = Guid.NewGuid().ToString();
    public int ItemCount { get; private set; }
    public event Action? OnChange;

    public void SetItemCount(int count)
    {
        ItemCount = count;
        OnChange?.Invoke();
    }
}
