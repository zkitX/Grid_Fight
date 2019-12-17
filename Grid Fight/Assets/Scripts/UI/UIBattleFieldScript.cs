using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBattleFieldScript : MonoBehaviour
{

    public BaseCharacter CharOwner;
    [SerializeField]
    private Image CharacterHealthBar;
    [SerializeField]
    private Image CharacterStaminaBar;
    private Camera mCamera;
    public Canvas CanvasParent;
    // Start is called before the first frame update
    void Start()
    {
        mCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (CharOwner != null)
        {
            transform.position = mCamera.WorldToScreenPoint(CharOwner.transform.position);
            CharacterHealthBar.rectTransform.anchoredPosition = new Vector2(CharacterHealthBar.rectTransform.rect.width - ((CharacterHealthBar.rectTransform.rect.width / 100) * CharOwner.CharInfo.HealthPerc), 0);
            CharacterStaminaBar.rectTransform.anchoredPosition = new Vector2(CharacterStaminaBar.rectTransform.rect.width - ((CharacterStaminaBar.rectTransform.rect.width / 100) * CharOwner.CharInfo.StaminaPerc), 0);
            if (CharOwner.CharInfo.Health <= 0)
            {
                gameObject.SetActive(false);
                CharOwner = null;
            }
        }
    }
}
