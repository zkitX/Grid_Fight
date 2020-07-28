using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Grid_UICursor : MonoBehaviour
{
    public bool state = true;
    public bool startAtCentre = true;
    public float cursorMoveSpeed = 1f;
    public float cursorSnapTime = 0.3f;
    List<Color> startingColors = new List<Color>();
    List<Vector2> screenBounds = new List<Vector2>();

    public AnimationClip press;
    public AnimationClip rejectedPress;

    public bool IsOverButton
    {
        get
        {
            Collider2D cursorCollider = GetComponent<Collider2D>();
            foreach (Grid_UIButton btn in Grid_UINavigator.Instance.ActiveButtons)
            {
                if (cursorCollider.IsTouching(btn.GetComponent<Collider2D>())) return true;
            }
            return false;
        }
    }
    public Vector2 CursorScreenNormalised
    {
        get
        {
            return new Vector2((transform.position.x - (Screen.width / 2f)) / (Screen.width / 2f), (transform.position.y - (Screen.height / 2f)) / (Screen.height / 2f));
        }
    }

    private void Awake()
    {
        screenBounds.Add(new Vector3(0, 0));
        screenBounds.Add(new Vector3(Screen.width, Screen.height));
        Debug.Log(screenBounds[0].ToString() + "   " + screenBounds[1].ToString());

        foreach (Image img in GetComponentsInChildren<Image>()) startingColors.Add(img.color);
        SetCursorVisable(false);
        if (startAtCentre) transform.position = new Vector3(Screen.width / 2f, Screen.height / 2f);
    }

    public void SetCursorVisable(bool _state)
    {
        Image[] imgs = GetComponentsInChildren<Image>();
        for(int i = 0; i < imgs.Length; i++)
        {
            imgs[i].color = state ? startingColors[i] : new Color(0,0,0,0);
        }
    }

    public void EnableCursor(bool _state = true, Grid_UIButton target = null)
    {
        if (state == _state) return;
        state = _state;
        SetCursorVisable(state);

        if (target != null) transform.position = target.transform.position;

        if (state)
        {
            InputController.Instance.LeftJoystickUsedEvent += MoveCursor;
        }
        else
        {
            InputController.Instance.LeftJoystickUsedEvent -= MoveCursor;
        }
    }


    bool cursorMoving = false;
    void MoveCursor(int player, InputDirection direction, float value)
    {
        if (MoveEnder != null) StopCoroutine(MoveEnder);
        MoveEnder = EndMove();
        StartCoroutine(MoveEnder);

        Vector2 currentPos = transform.position;

        Vector2 move = InputController.Instance.Joystic;
        float angle = Vector2.SignedAngle(Vector2.up, move); 
        move = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
        move = new Vector2(move.y * Time.deltaTime * cursorMoveSpeed * -500f, move.x * Time.deltaTime * cursorMoveSpeed * 500f);
        if (currentPos.x + move.x < screenBounds[0].x || currentPos.x + move.x > screenBounds[1].x) move.x = 0f;
        if (currentPos.y + move.y < screenBounds[0].y || currentPos.y + move.y > screenBounds[1].y) move.y = 0f;
        transform.position += new Vector3(move.x * (Screen.width / 1920f), move.y * (Screen.height / 1080f));
    }

    IEnumerator MoveEnder = null;
    IEnumerator EndMove()
    {
        cursorMoving = true;
        yield return new WaitForSeconds(0.05f);
        cursorMoving = false;
        SnapToClosestActiveButton();
    }

    public void SnapToClosestActiveButton()
    {
        if (Grid_UINavigator.Instance.ActiveButtons.Length == 0) return;

        StartCoroutine(SnapCoroutine(
            Grid_UINavigator.Instance.GetClosestButtonFromArray(transform.position, Grid_UINavigator.Instance.ActiveButtons.ToArray()).transform.position)
            );
    }

    public void SnapToButton(Grid_UIButton btn)
    {
        if (Grid_UINavigator.Instance.ActiveButtons.Where(r => r.ID == btn.ID).FirstOrDefault() == null || !Grid_UINavigator.Instance.CanNavigate(MenuNavigationType.Cursor)) return;

        StartCoroutine(SnapCoroutine(
            Grid_UINavigator.Instance.ActiveButtons.Where(r => r.ID == btn.ID).First().transform.position)
            );
    }

    IEnumerator SnapCoroutine(Vector3 posToMoveTo) //TODO: THE SNAP ISN'T PERFECT, NEED TO CONSTANTLY UPDATE THE POSTOMOVE TO
    {
        float timeLeft = cursorSnapTime;
        while(transform.position != posToMoveTo && !cursorMoving)
        {
            timeLeft = Mathf.Clamp(timeLeft - Time.deltaTime, 0f, 1000f);
            transform.position = Vector3.Lerp(transform.position, posToMoveTo, 1f - (timeLeft / cursorSnapTime));
            yield return null;
        }
    }

    public void PlayPressAnimation()
    {
        Animation anim = GetComponent<Animation>();
        anim.Stop();
        anim.clip = Grid_UINavigator.Instance.selectedButton == null ? rejectedPress : press;
        anim.Play();
    }

}

