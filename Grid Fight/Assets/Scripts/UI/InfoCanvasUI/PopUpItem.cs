using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUpItem : MonoBehaviour
{
    [SerializeField] protected Animation anim;
    [SerializeField] protected Animation btnAnim;
    [SerializeField] protected Transform popupBox;
    [SerializeField] protected Image displayImage;
    [SerializeField] protected TextMeshProUGUI titleText;
    [SerializeField] protected TextMeshProUGUI descriptionText;

    protected List<Color> defaultColors = new List<Color>(); 

    public bool popupComplete;

    protected BattleState startingBattleState;

    private void Awake()
    {
        defaultColors.Add(popupBox.GetComponent<Image>().color);
        defaultColors.Add(titleText.color);
        defaultColors.Add(descriptionText.color);
    }

    protected void ResetPopup()
    {
        if (anim == null) anim = GetComponentInChildren<Animation>();
        if (btnAnim == null) btnAnim = GetComponentsInChildren<Animation>()[1];
        if (popupBox == null) popupBox = transform.GetChild(0);
        popupComplete = false;
        StopAllCoroutines();
    }

    public IEnumerator TriggerPopup(Vector2 offset, string title, string description, Sprite image, float holdTime, Color boxColor = new Color(), Color titleColor = new Color(), Color decriptionColor = new Color())
    {
        ResetPopup();
        SetupPopupInformation(offset, title, description, image, boxColor, titleColor, decriptionColor);
        yield return PopUpCo(holdTime);
    }

    protected void SetupPopupInformation(Vector2 offset, string title, string description, Sprite image, Color boxColor, Color titleColor, Color decriptionColor)
    {
        popupBox.localPosition = offset;
        displayImage.sprite = image;
        titleText.text = title;
        descriptionText.text = description;
        if (boxColor == new Color()) boxColor = defaultColors[0];
        if (titleColor == new Color()) titleColor = defaultColors[1];
        if (decriptionColor == new Color()) decriptionColor = defaultColors[2];

        popupBox.GetComponent<Image>().color = boxColor;
        titleText.color = titleColor;
        descriptionText.color = decriptionColor;
    }


    IEnumerator PopUpCo(float holdTime)
    {
        //Set and store current game state
        startingBattleState = BattleManagerScript.Instance.CurrentBattleState;
        BattleManagerScript.Instance.CurrentBattleState = BattleState.FungusPuppets;

        //Play intro anim
        if(anim.isPlaying) anim.Stop();
        anim.clip = anim.GetClip("GameUI_PopUp_In");
        anim.Play();
        while (anim.isPlaying) yield return null;

        //Play popup idle anim
        if (anim.isPlaying) anim.Stop();
        anim.clip = anim.GetClip("GameUI_PopUp_Idle");
        anim.Play();

        //Wait for read time
        while (holdTime > 0f)
        {
            holdTime -= Time.unscaledDeltaTime;
            yield return null;
        }
        
        //ShowExit Button and give player power to skip
        InputController.Instance.ButtonAUpEvent -= PlayerCompletePopup;
        InputController.Instance.ButtonAUpEvent += PlayerCompletePopup;
        if (btnAnim.isPlaying) btnAnim.Stop();
        btnAnim.clip = btnAnim.GetClip("GameUI_PopUp_Button_In");
        btnAnim.Play();

        while (!popupComplete)
        {
            yield return null;
        }

        yield return PopUpEnd();

        gameObject.SetActive(false);
    }

    public void PlayerCompletePopup(int player)
    {
        popupComplete = true;
    }

    IEnumerator PopUpEnd()
    {
        InputController.Instance.ButtonAUpEvent -= PlayerCompletePopup;
        BattleManagerScript.Instance.CurrentBattleState = startingBattleState;

        if (btnAnim.isPlaying) btnAnim.Stop();
        btnAnim.clip = anim.GetClip("GameUI_PopUp_Button_Out");
        btnAnim.Play();

        if (anim.isPlaying) anim.Stop();
        anim.clip = anim.GetClip("GameUI_PopUp_Out");
        anim.Play();
        while (anim.isPlaying) yield return null;

    }
}
