using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.WebCam;

public class CutsceneManager : MonoBehaviour {

    public GameObject IntroCam1;
    public GameObject IntroCam2;
    public GameObject WinCam;

    private void Awake() {
        IntroCam1.SetActive(true);
        IntroCam2.SetActive(false);
        WinCam.SetActive(false);
    }

    public void IntroductionScene(Transform lookAt1, Transform lookAt2, Action callback) {
        StartCoroutine(IntroCoroutine(lookAt1, lookAt2, callback));
    }

    IEnumerator IntroCoroutine(Transform lookAt1, Transform lookAt2, Action callback) {
        IntroCam1.GetComponent<CinemachineVirtualCamera>().LookAt = lookAt1.transform;
        IntroCam2.GetComponent<CinemachineVirtualCamera>().LookAt = lookAt2.transform;
        IntroCam1.SetActive(true);
        yield return new WaitForSeconds(2f);
        IntroCam1.SetActive(false);
        IntroCam2.SetActive(true);
        yield return new WaitForSeconds(4f);
        IntroCam2.SetActive(false);
        callback?.Invoke();
    }

    public void WinScene(GameObject winnerGO, Action callback) {
        StartCoroutine(WinCoroutine(winnerGO, callback));
    }

    IEnumerator WinCoroutine(GameObject winnerGO, Action callback) {
        print("win coroutine");
        winnerGO.GetComponent<AnimationManager>().ShutOffAllStates();
        winnerGO.GetComponent<AnimationManager>().FireTrigger("dance");
        WinCam.SetActive(true);
        WinCam.GetComponent<CinemachineVirtualCamera>().LookAt = winnerGO.transform;
        WinCam.GetComponent<CinemachineVirtualCamera>().Follow = winnerGO.transform;
        yield return new WaitForSeconds(5f);
        callback();
    //    WinCam.SetActive(false);
    }


}
