using UnityEngine;

public class ZAxisMover : MonoBehaviour
{
    public float speed = 5.0f;
    public float timer = 5.0f;


    void Update()
    {
        transform.Translate(0, 0, speed * Time.deltaTime);

        timer -= Time.deltaTime;

        if(timer < 0)
        {
            Destroy(gameObject);
        }
    }
}
