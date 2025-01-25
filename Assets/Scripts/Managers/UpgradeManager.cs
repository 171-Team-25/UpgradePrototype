using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System;
using Unity.Collections.LowLevel.Unsafe;

public class UpgradeManager : MonoBehaviour
{
    private Button button1, button2, button0;
    private List<UpgradeCard> upgradeCards = new List<UpgradeCard>();
    private Type currType;
    private Canvas canvas;

    private void OnEnable()
    {
        GameManager.UpgradeEvent += ChooseAndDisplayUpgrades;
        GameManager.CombatPhaseStart += MakeSureUIIsTurnedOff;
    }

    private void OnDisable()
    {
        GameManager.UpgradeEvent -= ChooseAndDisplayUpgrades;
        GameManager.CombatPhaseStart -= MakeSureUIIsTurnedOff;
    }

    private void MakeSureUIIsTurnedOff()
    {
        button0.gameObject.SetActive(false);
        button1.gameObject.SetActive(false);
        button2.gameObject.SetActive(false);
    }

    private void ChooseAndDisplayUpgrades()
    {
        Debug.Log("DISPLAYING");
        upgradeCards = GameManager.Instance.GetRandomUpgradeCards(3);
        button0.GetComponent<Image>().sprite = upgradeCards[0].upgradeVisual;
        button0.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = upgradeCards[0].upgradeText;
        button1.GetComponent<Image>().sprite = upgradeCards[1].upgradeVisual;
        button1.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = upgradeCards[1].upgradeText;
        button2.GetComponent<Image>().sprite = upgradeCards[2].upgradeVisual;
        button2.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = upgradeCards[2].upgradeText;
        button0.gameObject.SetActive(true);
        button1.gameObject.SetActive(true);
        button2.gameObject.SetActive(true);
    }

    private void Start()
    {
        button0 = transform.GetChild(0).GetComponent<Button>();
        button0.onClick.AddListener(UpgradeButton0);
        button1 = transform.GetChild(1).GetComponent<Button>();
        button1.onClick.AddListener(UpgradeButton1);
        button2 = transform.GetChild(2).GetComponent<Button>();
        button2.onClick.AddListener(UpgradeButton2);
        canvas = GetComponent<Canvas>();
        MakeSureUIIsTurnedOff();
    }

    private void UpgradeButton0()
    {
        currType = upgradeCards[0].upgradeType;
        transform.parent.gameObject.AddComponent(currType);
        button0.gameObject.SetActive(false);
        button1.gameObject.SetActive(false);
        button2.gameObject.SetActive(false);
        GameManager.Instance.PlayerReadyUp();
    }

    private void UpgradeButton1()
    {
        currType = upgradeCards[1].upgradeType;
        transform.parent.gameObject.AddComponent(currType);
        button0.gameObject.SetActive(false);
        button1.gameObject.SetActive(false);
        button2.gameObject.SetActive(false);
        GameManager.Instance.PlayerReadyUp();
    }

    private void UpgradeButton2()
    {
        currType = upgradeCards[2].upgradeType;
        transform.parent.gameObject.AddComponent(currType);
        button0.gameObject.SetActive(false);
        button1.gameObject.SetActive(false);
        button2.gameObject.SetActive(false);
        GameManager.Instance.PlayerReadyUp();
    }
}
