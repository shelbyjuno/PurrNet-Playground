using UnityEngine;

public class CubeMove : MonoBehaviour
{
    [SerializeField] private float speed = 1f;

    void Update()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        transform.Translate(input * speed * Time.deltaTime);
    }
}
