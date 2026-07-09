using UnityEngine;

public class Coin : MonoBehaviour
{
    public float spinSpeed = 150f;

    void Update()
    {
        // capraz don
        transform.Rotate(spinSpeed * Time.deltaTime, spinSpeed * Time.deltaTime, 0);

        // Kod calısma kntrolu
        Debug.Log("Altın betiği şu an sahnede çalışıyor!");
    }
}