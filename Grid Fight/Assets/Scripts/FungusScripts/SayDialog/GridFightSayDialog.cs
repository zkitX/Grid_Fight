using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;
using System;

public class GridFightSayDialog : SayDialog
{
    public Character LastCharacter = null;
    [SerializeField]
    public Animator SayDialogAnimatorController;
    private bool isAnimCompleted = false;
    private bool IsAlreadySubscribed = false;

    private Character currentChar;
    protected override void Awake()
    {
        closeOtherDialogs = false;
        base.Awake();
    }

    protected override void Start()
    {
        InputController.Instance.ButtonAUpEvent += Instance_ButtonAUpEvent;
        base.Start();
    }

    private void Instance_ButtonAUpEvent(int player)
    {
        AnimSpeedChanger(5);
    }

    private void Item_AnimationInfoScriptAnimationCompletedEvent()
    {
        isAnimCompleted = true;
    }

    public override IEnumerator DoSay(string text, bool clearPrevious, bool waitForInput, bool fadeWhenDone, bool stopVoiceover, bool waitForVO, AudioClip voiceOverClip, Character nextChar)
    {

        Debug.Log(text);
        AnimSpeedChanger(1);
        BattleManagerScript.Instance.FungusState = FungusDialogType.Dialog;

        if (!IsAlreadySubscribed)
        {
            IsAlreadySubscribed = true;
            foreach (AnimationInfoScript item in SayDialogAnimatorController.GetBehaviours<AnimationInfoScript>())
            {
                item.AnimationInfoScriptAnimationCompletedEvent += Item_AnimationInfoScriptAnimationCompletedEvent;
            }
        }
        GetCanvasGroup().alpha = 1f;

        while (BattleManagerScript.Instance == null)
        {
            yield return null;
        }

        if (LastCharacter != null && LastCharacter.name != nextChar.name)
        {
            if (SayDialogAnimatorController.GetBool("InOut"))
            {
                SayDialogAnimatorController.SetBool("IsSelected", false);
                while (!isAnimCompleted)
                {
                    yield return null;
                }
                isAnimCompleted = false;

                yield return base.DoSay("", clearPrevious, false, false, stopVoiceover, waitForVO, voiceOverClip, delegate { });
                SayDialogAnimatorController.SetBool("InOut", false);
                while (!isAnimCompleted)
                {
                    yield return null;
                }
                isAnimCompleted = false;

                SayDialogAnimatorController.SetBool("InOut", true);
                SetChar();
                SetCharacterImage(currentChar.Portraits[0]);


                while (!isAnimCompleted)
                {
                    yield return null;
                }
                isAnimCompleted = false;

                SayDialogAnimatorController.SetBool("IsSelected", true);
                LastCharacter = nextChar;

                while (!isAnimCompleted)
                {
                    yield return null;
                }
                isAnimCompleted = false;
            }
        }
        else if (LastCharacter != null && LastCharacter.name == nextChar.name)
        {
            isAnimCompleted = true;
        }
        else if (LastCharacter == null)
        {
            SayDialogAnimatorController.SetBool("InOut", true);
            SetChar();
            SetCharacterImage(currentChar.Portraits[0]);


            while (!isAnimCompleted)
            {
                yield return null;
            }
            isAnimCompleted = false;

            SayDialogAnimatorController.SetBool("IsSelected", true);
            LastCharacter = nextChar;

            while (!isAnimCompleted)
            {
                yield return null;
            }
            isAnimCompleted = false;
        }

        yield return base.DoSay(text, clearPrevious, waitForInput, fadeWhenDone, stopVoiceover, waitForVO, voiceOverClip, delegate {});

       
    }


    private void AnimSpeedChanger(float speed)
    {
        SayDialogAnimatorController.speed = speed;
    }


    public override void SetCharacter(Character character)
    {
        currentChar = character;
    }


    private void SetChar()
    {
        if (currentChar == null)
        {
            if (characterImage != null)
            {
                characterImage.gameObject.SetActive(false);
            }
            if (NameText != null)
            {
                NameText = "";
            }
            speakingCharacter = null;
        }
        else
        {
            var prevSpeakingCharacter = speakingCharacter;
            speakingCharacter = currentChar;

            // Dim portraits of non-speaking characters
            var activeStages = Stage.ActiveStages;
            for (int i = 0; i < activeStages.Count; i++)
            {
                var stage = activeStages[i];
                if (stage.DimPortraits)
                {
                    var charactersOnStage = stage.CharactersOnStage;
                    for (int j = 0; j < charactersOnStage.Count; j++)
                    {
                        var c = charactersOnStage[j];
                        if (prevSpeakingCharacter != speakingCharacter)
                        {
                            if (c != null && !c.Equals(speakingCharacter))
                            {
                                stage.SetDimmed(c, true);
                            }
                            else
                            {
                                stage.SetDimmed(c, false);
                            }
                        }
                    }
                }
            }

            string characterName = currentChar.NameText;

            if (characterName == "")
            {
                // Use game object name as default
                characterName = currentChar.GetObjectName();
            }

            SetCharacterName(characterName, currentChar.NameColor);
        }
    }

    protected override void UpdateAlpha()
    {
        if (GetWriter().IsWriting)
        {
            targetAlpha = 1f;
            fadeCoolDownTimer = 0.1f;
        }
        else if (fadeWhenDone && Mathf.Approximately(fadeCoolDownTimer, 0f))
        {
            targetAlpha = 0f;
        }
        else
        {
            // Add a short delay before we start fading in case there's another Say command in the next frame or two.
            // This avoids a noticeable flicker between consecutive Say commands.
            fadeCoolDownTimer = Mathf.Max(0f, fadeCoolDownTimer - Time.deltaTime);
        }
        /*CanvasGroup canvasGroup = GetCanvasGroup();
        if (fadeDuration <= 0f)
        {
            canvasGroup.alpha = targetAlpha;
        }
        else
        {
            float delta = (1f / fadeDuration) * Time.deltaTime;
            float alpha = Mathf.MoveTowards(canvasGroup.alpha, targetAlpha, delta);
            canvasGroup.alpha = alpha;

            if (alpha <= 0f)
            {
                // Deactivate dialog object once invisible
                //gameObject.SetActive(false);
            }
        }*/
    }
}
