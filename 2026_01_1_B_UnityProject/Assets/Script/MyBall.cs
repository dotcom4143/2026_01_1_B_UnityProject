using UnityEngine;

public class MyBall : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name + " 와 충돌");

        if (collision.gameObject.tag == "Ground")
        {
            Debug.Log("땅과 충돌");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("트리거 안에 들어옴");
    }
    private void OnTriggerExit(Collider other)
    {
        Debug.Log("트리거 밖으로 나감");
    }

}
