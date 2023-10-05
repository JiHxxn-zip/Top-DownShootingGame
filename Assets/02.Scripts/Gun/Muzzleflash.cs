using UnityEngine;

public class Muzzleflash : MonoBehaviour
{
    [SerializeField] private GameObject flashHolder;

    [Header("[Flash Image]")]
    [SerializeField] private Sprite[] flashSprites;
    [SerializeField] private SpriteRenderer[] spriteRenderers;

    [Header("[Flash Time]")]
    [SerializeField] private float flashTime = 0.4f;

    private void Start()
    {
        Deactivate();
    }

    /// <summary>
    /// 플래쉬 활성화
    /// </summary>
    public void Activate()
    {
        flashHolder.SetActive(true);

        int flashSpriteIndex = Random.RandomRange(0, flashSprites.Length);

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].sprite = flashSprites[flashSpriteIndex];
        }

        Invoke("Deactivate", flashTime);
    }

    private void Deactivate()
    {
        flashHolder.SetActive(false);
    }
}
