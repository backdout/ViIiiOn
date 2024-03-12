using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;
public class UnitAniManager : MonoBehaviour
{
 
    public float _version;
    public SPUM_SpriteList _spriteOBj;
    public bool EditChk;
    public string _code;
    public Animator _anim;
    public bool _horse;
    public bool isRideHorse{
        get => _horse;
        set {
            _horse = value;
            UnitTypeChanged?.Invoke();
        }
    }
    public string _horseString;

    public UnityEvent UnitTypeChanged = new UnityEvent();
    private AnimationClip[] _animationClips;
    public AnimationClip[] AnimationClips => _animationClips;
    private Dictionary<string, int> _nameToHashPair = new Dictionary<string, int>();
    private void InitAnimPair(){
        _nameToHashPair.Clear();
        _animationClips = _anim.runtimeAnimatorController.animationClips;
        foreach (var clip in _animationClips)
        {
            int hash = Animator.StringToHash(clip.name);
            _nameToHashPair.Add(clip.name, hash);
        }
    }
    private void Awake() {
        InitAnimPair();
    }
    private void Start() {
        UnitTypeChanged.AddListener(InitAnimPair);
    }
    // 이름으로 애니메이션 실행
    public void PlayAnimation(UnitHandler.STATE state, UnitHandler.ATK_TYPE atkType = UnitHandler.ATK_TYPE.Sword)
    {

        //Debug.Log(name);
        string name = GetAniName(state, atkType);


        foreach (var animationName in _nameToHashPair)
        {
            if(animationName.Key.ToLower().Contains(name.ToLower()) ){
                _anim.Play(animationName.Value, 0);
                break;
            }
        }
    }



    private string GetAniName(UnitHandler.STATE state, UnitHandler.ATK_TYPE atkType)
    {
        switch(state)
        {
            case UnitHandler.STATE.IDLE:
                return "0_idle";
            case UnitHandler.STATE.ATTACK:
                {
                    return atkType == UnitHandler.ATK_TYPE.Sword ? "2_Attack_Normal" : atkType == UnitHandler.ATK_TYPE.Magic ? "2_Attack_Magic" : "2_Attack_Bow";
                }
            case UnitHandler.STATE.RUN :
                return "1_Run";

            case UnitHandler.STATE.DEATH:
                return "4_Death";

            default:
                return "0_idle";

        }
    }


}
