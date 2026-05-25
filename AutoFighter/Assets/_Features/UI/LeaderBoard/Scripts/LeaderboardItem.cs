using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderboardItem : MonoBehaviour {

    public TextMeshProUGUI NameLabelRef;
    public TextMeshProUGUI PointsLabelRef;
    public TextMeshProUGUI OrderLabelRef;

    public void SetName(string text) {
        NameLabelRef.text = text;
    }

    public void SetPoints(string text) {
        PointsLabelRef.text = text;
    }

    public void SetOrder(string text) {
        OrderLabelRef.text = text;
    }


}
