using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour {

    public Slider HP1;
    public Slider HP2;
    public Slider Mana1;
    public Slider Mana2;
    public TextMeshProUGUI Status1;
    public TextMeshProUGUI Status2;

    public void SetStatus1(string text) {
        Status1.text = text;
    }

}
