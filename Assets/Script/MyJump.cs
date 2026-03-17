using UnityEngine;
using UnityEngine.UI;

public class MyJump : MonoBehaviour
{
    public Rigidbody rigidbody;                                         //강체 (형태와 크기가 고정된 고체) 물리 현상이 동작 하게 해주는 컴포넌트
    public float power = 200f;                                           //변수 힘(power)을 선언 함
    public float timer = 0;
    public Text TextUI;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer = timer = Time.deltaTime;
        TextUI.text = timer.ToString();

        if (Input.GetKeyDown(KeyCode.Space))                            //스페이스 키를 눌렀을 때,
        {
            power = power + Random.Range(-100, 200);                    //Power를 랜덤으로 변경 시킨다. (-100 ~ 200)사이의 값을 더한다.
            rigidbody.AddForce(transform.up * power);                   //변수(power)의 위쪽으로 힘을 준다
        }

        if (this.gameObject.transform.position.y > 5 || this.gameObject.transform.position.y < -3)
        {
            //이 오브젝트의 y좌표점 위치가 5보다 크거나 -3보다 적을 경우
            Destroy(this.gameObject);                    //오브젝트를 파괴한다.
        }
    }
}
