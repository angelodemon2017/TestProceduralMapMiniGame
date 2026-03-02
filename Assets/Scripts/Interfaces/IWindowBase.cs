using UnityEngine;

public interface IWindowBase
{
    Transform GetTransform { get; }
    void Show();
    void Hide();
}