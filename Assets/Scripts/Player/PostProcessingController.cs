using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using static UnityEngine.Rendering.DebugUI;

public class PostProcessingController : MonoBehaviour
{
    [SerializeField] private Volume _staminaVolume;
    [SerializeField] private Volume _recoilVolume;
    [SerializeField] private Volume _adsVolume;
    delegate void onChangeStaminaValue(float weight);
    delegate void onChangeRecoilValue(float weight);
    delegate void onChangeADSValue(float weight);
    static event onChangeStaminaValue OnChangeStaminaValue;
    static event onChangeRecoilValue OnChangeRecoilValue;
    static event onChangeADSValue OnChangeADSValue;
    public static float RecoilVignette { get; private set; }

    private void Start()
    {
        OnChangeStaminaValue += PP_OnChangeStaminaValue;
        OnChangeRecoilValue += PP_OnChangeRecoilValue;
        OnChangeADSValue += PP_OnChangeADSValue;
    }

    public static void ChangeStaminaVolumeValue(float value) => OnChangeStaminaValue.Invoke(value);
    public static void ChangeRecoilVolumeValue(float value) => OnChangeRecoilValue.Invoke(value);
    public static void ChangeADSVolumeValue(float value) => OnChangeADSValue.Invoke(value);
    void PP_OnChangeStaminaValue(float weight) => _staminaVolume.weight = weight;
    void PP_OnChangeRecoilValue(float weight) => _recoilVolume.weight = weight;
    void PP_OnChangeADSValue(float weight) => _adsVolume.weight = weight;
}
