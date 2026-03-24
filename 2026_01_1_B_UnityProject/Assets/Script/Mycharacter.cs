using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public int Health = 100;
    public float Timer = 1.0f;


    void Start()
    {
        Health = Health + 100;
    }

    // Update is called once per frame
    void Update()
    {
        Timer = Timer - Time.deltaTime;

        if(Timer <= 0)
        {
            Timer = 1.0f;
            Health = Health - 20;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Health = Health + 2;
        }

        if (Health <- 0)
        {
            Destroy(this.gameObject);
        }

    }
}
