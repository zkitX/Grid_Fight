using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharTesterScript : MonoBehaviour
{
    public List<VFXTesterCharClass> Characters = new List<VFXTesterCharClass>();
    public GameObject CharacterBasePrefab;
    private GameObject charOnScene;
    private BaseCharacter currentCharacter;


    [Header("Char creation")]
    public TMP_Dropdown CharToUse;
    public TMP_Dropdown CharacterClass;
    public TMP_Dropdown CharacterAttackType;

    [Header("Particles")]
    public TMP_Dropdown ParticleType;
    public TMP_Dropdown CharacterLevel;

    public Slider AttackSpeed;
    public Slider SpeedOfBullets;
    public Slider MountainDelay;
    public TextMeshProUGUI AttackSpeedText;
    public TextMeshProUGUI SpeedOfBulletsText;
    public TextMeshProUGUI MountainDelayText;

    [Header("Animation")]
    public TMP_Dropdown AnimToUse;
    public Toggle Loop;
    public Slider TransitionTime;
    public TextMeshProUGUI TransitionTimeText;
    public Slider AnimationSpeed;
    public TextMeshProUGUI AnimationSpeedText;


    private IEnumerator MoveCo;
    private float CurrentSpeed = 1;
    public List<ScriptableObjectAttackBase> AttacksTypeInfo = new List<ScriptableObjectAttackBase>();


    private void Start()
    {

        for (int i = 0; i <= Enum.GetValues(typeof(CharacterNameType)).Cast<int>().Last(); i++)
        {
            CharToUse.options.Add(new TMP_Dropdown.OptionData(((CharacterNameType)i).ToString()));

        }

        for (int i = 0; i <= Enum.GetValues(typeof(CharacterLevelType)).Cast<int>().Last(); i++)
        {
            CharacterLevel.options.Add(new TMP_Dropdown.OptionData(((CharacterLevelType)i).ToString()));

        }

        for (int i = 0; i <= Enum.GetValues(typeof(AttackParticleType)).Cast<int>().Last(); i++)
        {
            ParticleType.options.Add(new TMP_Dropdown.OptionData(((AttackParticleType)i).ToString()));

        }

        for (int i = 0; i <= Enum.GetValues(typeof(CharacterClassType)).Cast<int>().Last(); i++)
        {
            CharacterClass.options.Add(new TMP_Dropdown.OptionData(((CharacterClassType)i).ToString()));

        }

        for (int i = 0; i <= Enum.GetValues(typeof(AttackType)).Cast<int>().Last(); i++)
        {
            CharacterAttackType.options.Add(new TMP_Dropdown.OptionData(((AttackType)i).ToString()));

        }

        for (int i = 0; i <= Enum.GetValues(typeof(CharacterAnimationStateType)).Cast<int>().Last(); i++)
        {
            AnimToUse.options.Add(new TMP_Dropdown.OptionData(((CharacterAnimationStateType)i).ToString()));

        }

    }

    private void Update()
    {
        AttackSpeedText.text = AttackSpeed.value.ToString("F2");
        SpeedOfBulletsText.text = SpeedOfBullets.value.ToString("F2");
        MountainDelayText.text = MountainDelay.value.ToString("F2");
        TransitionTimeText.text = TransitionTime.value.ToString("F2");
        AnimationSpeedText.text = AnimationSpeed.value.ToString("F2");
        if (charOnScene != null && Input.GetKeyUp(KeyCode.O))
        {
            StartCoroutine(charOnScene.GetComponent<CharacterType_Script>().StartChargingAttack( AttackAnimType.Powerful_Atk));
        }

        if (charOnScene != null && Input.GetKeyUp(KeyCode.B))
        {
            StartCoroutine(charOnScene.GetComponent<CharacterType_Script>().StartChargingAttack(AttackAnimType.Skill1));
        }

        if (charOnScene != null && Input.GetKeyUp(KeyCode.V))
        {
            StartCoroutine(charOnScene.GetComponent<CharacterType_Script>().StartChargingAttack(AttackAnimType.Skill2));
        }
    }

    // Start is called before the first frame update
    public void CreateChar()
    {
        Destroy(charOnScene);
        BattleTileScript bts = GridManagerScript.Instance.GetBattleTile(new Vector2Int(2, 3));
        charOnScene = Instantiate(CharacterBasePrefab, bts.transform.position, Quaternion.identity);
        GameObject child = Instantiate(Characters.Where(r => r.CharName.ToString() == CharToUse.options[CharToUse.value].text).First().Char, charOnScene.transform.position, Quaternion.identity, charOnScene.transform);
        currentCharacter = (BaseCharacter)charOnScene.AddComponent(Type.GetType(child.GetComponentInChildren<CharacterInfoScript>().BaseCharacterType.ToString()));
        currentCharacter.UMS = currentCharacter.GetComponent<UnitManagementScript>();
        currentCharacter.UMS.CurrentAttackType = (AttackType)Enum.Parse(typeof(AttackType), CharacterAttackType.options[CharacterAttackType.value].text);
        currentCharacter.UMS.CharOwner = currentCharacter;
        currentCharacter.UMS.Facing = FacingType.Right;
        currentCharacter.UMS.WalkingSide = WalkingSideType.LeftSide;
        currentCharacter.UMS.Side = SideType.LeftSide;
        currentCharacter.UMS.CurrentTilePos = bts.Pos;
        currentCharacter.CharInfo.ClassType = (CharacterClassType)Enum.Parse(typeof(CharacterClassType), CharacterClass.options[CharacterClass.value].text);
        currentCharacter.VFXTestMode = true;
        currentCharacter.UMS.Pos.Add(bts.Pos);
        currentCharacter.SetupCharacterSide();
        currentCharacter.SpineAnimatorsetup();
        
    }


    public void ParticlesSetup()
    {
        currentCharacter.CharInfo.CharacterLevel = (CharacterLevelType)Enum.Parse(typeof(CharacterLevelType), CharacterLevel.options[CharacterLevel.value].text);
        currentCharacter.CharInfo.ParticleID = (AttackParticleType)Enum.Parse(typeof(AttackParticleType), ParticleType.options[ParticleType.value].text);
        currentCharacter.CharInfo.SpeedStats.AttackSpeedRatio = AttackSpeed.value;
        currentCharacter.CharInfo.SpeedStats.BulletSpeed = SpeedOfBullets.value;
        currentCharacter.CharInfo.CurrentAttackTypeInfo = AttacksTypeInfo.Where(r => r.ParticlesAtk.CharacterClass == currentCharacter.CharInfo.ClassType).ToList();
        currentCharacter.CharInfo.DamageStats.ChildrenBulletDelay = MountainDelay.value;
    }

    public void AnimationSetup()
    {
        if (MoveCo != null)
        {
            StopCoroutine(MoveCo);
        }
        ResetPos();
        CharacterAnimationStateType nextAnim = (CharacterAnimationStateType)Enum.Parse(typeof(CharacterAnimationStateType), AnimToUse.options[AnimToUse.value].text);
        CurrentSpeed = 1 * AnimationSpeed.value;
        currentCharacter.SpineAnim.SetAnimationSpeed(CurrentSpeed);
        currentCharacter.CharInfo.BaseSpeed = CurrentSpeed;
        if (nextAnim.ToString().Contains("Atk"))
        {
            currentCharacter.GetAttack();
        }

        if (nextAnim.ToString().Contains("Dash"))
        {
            MoveCo = MoveChar((InputDirection)Enum.Parse(typeof(InputDirection), nextAnim.ToString().Substring(4)));
            StartCoroutine(MoveCo);
            return;
        }
        currentCharacter.SpineAnim.SetAnim(nextAnim, Loop.isOn, TransitionTime.value);
    }


    private IEnumerator MoveChar(InputDirection dir)
    {
        bool isMoving = true;
        int i = 0;
        while(isMoving)
        {
            if(i == 2)
            {

                ResetPos();
                /* dir = dir == InputDirection.Up ? InputDirection.Down :
                     dir == InputDirection.Down ? InputDirection.Up :
                     dir == InputDirection.Left ? InputDirection.Right : InputDirection.Left;*/
                i = 0;
            }
            currentCharacter.MoveCharOnDirection(dir);
            yield return null;
            currentCharacter.SpineAnim.SetAnimationSpeed(CurrentSpeed);
            while (currentCharacter.isMoving)
            {
                yield return null;
            }

            yield return new WaitForSecondsRealtime(1);
            i++;
        }
    }


    private void ResetPos()
    {
        currentCharacter.transform.position = GridManagerScript.Instance.GetBattleTile(new Vector2Int(2, 3)).transform.position;
        currentCharacter.UMS.Pos[0] = new Vector2Int(2, 3);
        currentCharacter.UMS.CurrentTilePos = new Vector2Int(2, 3);
    }
}
