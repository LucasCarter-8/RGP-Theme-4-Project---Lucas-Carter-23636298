using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class ButtonSFX : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private SFXEmitter emitter;

    public void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => ClickSound());
        emitter = GetComponent<SFXEmitter>();
    }

    public void OnPointerEnter(PointerEventData _)
    {
        emitter.Play(SoundEffectType.UIHover);
    }

    private void ClickSound()
    {
        emitter.Play(SoundEffectType.UIClick);
    }
}