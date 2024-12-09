using PurrNet;
using UnityEngine;

public class PlayerColor : NetworkBehaviour
{
    SyncVar<Color> color = new(Color.white, ownerAuth: true);

    void Awake()
    {
        color.onChanged += OnColorChanged;
    }

    void Start()
    {
        color.value = GetRandomColor();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            color.value = GetRandomColor();   
    }

    Color GetRandomColor() => new Color(Random.value, Random.value, Random.value);

    private void OnColorChanged(Color color)
    {
        Debug.Log($"Player color changed to {color}");
        GetComponent<Renderer>().material.color = color;
    }
}
