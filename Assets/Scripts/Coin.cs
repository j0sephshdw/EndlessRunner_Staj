using UnityEngine;

public class Coin : MonoBehaviour
{
    public float spinSpeed = 150f;

    void Update()
    {
        // Altının kendi etrafında dönme animasyonu
        transform.Rotate(spinSpeed * Time.deltaTime, spinSpeed * Time.deltaTime, 0);
    }
}