using UnityEngine;

public class Test : MonoBehaviour
{

    string 대장 = "김태용";

    int 부대장 = 5;

    int 장혁 = 99;

    int 황혜린 = 43;

    int 준;




    void Start()
    {
        준 = 대장.Length;

        if(준 == 3)
        {
            Debug.LogWarning("대박");
        }

        if (대장.Length>부대장)
        {
            Debug.LogWarning("안녕");
        }
        else
        {
            Debug.LogWarning("빠이");
        }
        
    }
}
