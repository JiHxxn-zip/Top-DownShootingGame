using System.Collections;
using UnityEngine;

public class Shell : MonoBehaviour
{
    [SerializeField] private Rigidbody myRigidbody;

    [SerializeField] private float force;
    [SerializeField] private float forceMin;
    [SerializeField] private float forceMax;

    float lifeTime = 4f;
    float fadeTime = 4f;


    void Start()
    {
        float force = Random.Range(forceMin, forceMax);
        myRigidbody.AddForce(transform.right * force);

        // 회전
        myRigidbody.AddTorque(Random.insideUnitSphere * force);

        StartCoroutine(Fade());
    }

    IEnumerator Fade()
    {
        yield return new WaitForSeconds(lifeTime);

        float percent = 0f;
        float fadeSpeed = 1 / fadeTime;

        // 색상 조절(초기화)
        Material mat = GetComponent<Renderer>().material;
        Color initialColour = mat.color;

        while(percent < 1)
        {
            percent += Time.deltaTime * fadeSpeed;
            mat.color = Color.Lerp(initialColour, Color.clear, percent);

            yield return null;
        }

        Destroy(gameObject);
    }
}
