using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YG;

public class Manager : MonoBehaviour
{
    [SerializeField] private YandexGame sdk;


    [SerializeField] float dmg;
    [SerializeField] float passiveDmg;
    [SerializeField] float passiveCooldown;
    [SerializeField] float money;

    [SerializeField] GameObject mainObject;
    [SerializeField] Animator mainObjectAnim;
    [SerializeField] float animationsClickVariants;
    [SerializeField] bool needToPlayAnim; //False ������ � Unity editor

    [SerializeField] Text moneyText;
    [SerializeField] GameObject WarningText;

    [SerializeField] Image background;
    [SerializeField] List<Sprite> backgrounds;
    private int curBackgroundIndex = -1;

    [SerializeField] List<GameObject> allMainObjects;
    [SerializeField] int objectIndex = 0;

    public int isBonusActive = 0;
    bool isGameWasCompleted;

    public int adWatchEndBonus;
    [SerializeField] Text endButtonText;
    [SerializeField] GameObject endButton;

    private IEnumerator coroutine;

    private float lastPlayTime;
    private float minTimeBetweenPlays = 0.5f;

    private void Start()
    {
        
        YandexGame.RewardVideoEvent += Reward;
        PlayerPrefsLoad();
        StartCoroutine(passiveDMGUse());
    }

    public void characterBuyChange()
    {
        if (checkMoney(599, $"�������� ������� �������"))
        {
            characterChange();
        }
    }
    
    public void Reward(int id)
    {
        if (id == 1)
            StartCoroutine(adBonusCoroutine());
        if (id == 2)
            if (needToPlayAnim)
            {
                mainObjectAnim.Play("SecretAnim");
            }
        if (id == 3)
            backgroundChange();
        if (id == 4)
            addMoney(1280);
        if (id==5)
        {
            adWatchEndBonus++;
            endButtonText.text = $"���������� 3 ������� �������� ����������� ������ {adWatchEndBonus} / 3";
            if (adWatchEndBonus==3)
            {
                endButton.SetActive(false);
                ShowWarningText($"�� �������� ����������� ���������� �����");
                addMoney(Mathf.Infinity);
            }
        }
        PlayerPrefsSave();

    }

    public void showAdd(int id0)
    {
        sdk._RewardedShow(id0);
    }

    IEnumerator adBonusCoroutine()
    {
        isBonusActive += 1;
        dmg *= 3;
        PlayerPrefsSave();
        yield return new WaitForSeconds(60);
        isBonusActive -= 1;
        dmg /= 3;
        PlayerPrefsSave();

    }

    IEnumerator passiveDMGUse()
    {
        while (true)
        {
            yield return new WaitForSeconds(passiveCooldown);
            PassiveClickObject();
        }
    }    

    public void ClickObject()
    {
        PlayRandomCharacterSound();
        addMoney(dmg);
        PlayerPrefsSave();
        if (needToPlayAnim) {   mainObjectAnim.Play("Click" + UnityEngine.Random.Range(0, animationsClickVariants + 1));   }
       
    }
    public void PassiveClickObject()
    {
        PlayRandomCharacterSound();
        addMoney(passiveDmg);
        PlayerPrefsSave();
        if (needToPlayAnim) { mainObjectAnim.Play("Click" + UnityEngine.Random.Range(0, animationsClickVariants + 1)); }

    }
    private void addMoney(float arg) { money += arg; moneyText.text = Math.Round(money, 1) + ""; }

    public void dmgUpgrade()
    {
        if (checkMoney(39,$"���� ������� ���� �������� �� {dmg * 1.1f}"))
        {
            dmg *= 1.1f;
        }
       
    }
    public void passiveCooldownUpgrade()
    {
        if (checkMoney(99, $"����� ���������� ������� ���� ��������� �� {passiveCooldown * 0.95f}"))
        {
            if (passiveCooldown > 0.3f)
            {
                passiveCooldown *= 0.95f;

            }
            else
            {
                addMoney(99);
                PlayerPrefsSave();
                ShowWarningText($"�� �������� �������. �� �� ������ ������ �������� �������� ���������� �������");
            }
        }
    }
    public void passiveDmgUpgrade()
    {
        if (checkMoney(69, $"��������� ���� ������� ���� �������� �� {passiveDmg * 1.1f}"))
        {
            passiveDmg *= 1.1f;
        }
    }

    public bool checkMoney(float arg0, string arg1)
    {
        if (money>= arg0)
        {
            ShowWarningText($"�� ������ ������ ���������. {arg1}");
            money -= arg0;
            moneyText.text = Math.Round(money, 1) + "";
            PlayerPrefsSave();
            return true;
        }
        else
        {
            ShowWarningText($"��� ����� ��� {Math.Round((arg0 - money), 1)}");
            return false;
        }

    }


    public void ShowWarningText(string arg0)
    {
        if (coroutine!=null)
        {
            StopCoroutine(coroutine);
        }
       
        coroutine = showTextCoroutine(arg0);
        StartCoroutine(coroutine);
    }

    IEnumerator showTextCoroutine(string arg0)
    {
        WarningText.gameObject.SetActive(true);
        WarningText.GetComponent<Text>().text = arg0;
        yield return new WaitForSeconds(1);
        WarningText.GetComponent<Text>().text = "";
    }

    public void backgroundChange()
    {
       
        var LocalcurBackgroundIndex = UnityEngine.Random.Range(1, backgrounds.Count);
        background.sprite = backgrounds[LocalcurBackgroundIndex];


        var elementToMove = backgrounds[LocalcurBackgroundIndex];


        backgrounds.RemoveAt(LocalcurBackgroundIndex);


        backgrounds.Insert(0, elementToMove);


        curBackgroundIndex = LocalcurBackgroundIndex;
        PlayerPrefsSave();
    }

    public void characterChange()
    {
        objectIndex++;
        if (objectIndex >= allMainObjects.Count)
        {
            objectIndex = 0;
        }
        for (int i = 0; i < allMainObjects.Count; i++)
        {
            allMainObjects[i].SetActive(false);
        }

        if (objectIndex==allMainObjects.Count-1 && !isGameWasCompleted)
        {
            EndGame();
        }
       

        allMainObjects[objectIndex].SetActive(true);
        PlayerPrefsSave();

        mainObject = allMainObjects[objectIndex];
    }

    public void PlayRandomCharacterSound()
    {
        var characterSounds = Resources.LoadAll<AudioClip>("Audio/CharacterSound");

        if (characterSounds.Length == 0)
        {
            Debug.LogWarning("����������� ����� ���������. ���������, ��� ��� ��������� � Resources/Audio/CharacterSound.");
            return;
        }

        // ��������, ������ �� ���������� ������� � ������� ���������� ���������������
        if (Time.time - lastPlayTime < minTimeBetweenPlays)
        {
            return; // ������ ������������ �������
        }

        // ����� ���������� �����
        AudioClip randomSound = characterSounds[UnityEngine.Random.Range(0, characterSounds.Length)];

        // �������� ������ ������� � ����������� AudioSource
        GameObject audioObject = new GameObject("CharacterSoundObject");
        AudioSource audioSource = audioObject.AddComponent<AudioSource>();

        // ������������ �����
        audioSource.clip = randomSound;
        audioSource.Play();

        // ��������� ������� ���������� ���������������
        lastPlayTime = Time.time;

        // ��������� ������� ��� �������� ������� ����� ���������� ��������������� �����
        Destroy(audioObject, randomSound.length);
    }
    private void OnApplicationQuit()
    {
        PlayerPrefsSave();
    }

    void PlayerPrefsSave()
    {
        PlayerPrefs.SetFloat("money", money);
        PlayerPrefs.SetFloat("dmg", dmg);
        PlayerPrefs.SetInt("isBonusActive", isBonusActive);
        PlayerPrefs.SetFloat("passiveDmg", passiveDmg);
        PlayerPrefs.SetFloat("passiveCooldown", passiveCooldown);
        PlayerPrefs.SetInt("objectIndex", objectIndex);
        PlayerPrefs.SetInt("curBackgroundIndex", curBackgroundIndex);
        PlayerPrefs.SetInt("isGameWasCompleted", boolToInt(isGameWasCompleted));
        PlayerPrefs.SetInt("adWatchEndBonus", adWatchEndBonus);

        Debug.Log($"���������� ������ �������. Money: {money}, Damage: {dmg}, isBonusActive {isBonusActive}, Passive Damage: {passiveDmg}, Passive Cooldown: {passiveCooldown}, Object Index: {objectIndex}, Current Background Index: {curBackgroundIndex}");
    }
    void PlayerPrefsLoad()
    {
        money = PlayerPrefs.GetFloat("money");
        dmg = PlayerPrefs.GetFloat("dmg", 1);
        isBonusActive = PlayerPrefs.GetInt("isBonusActive", 0);
        passiveDmg = PlayerPrefs.GetInt("passiveDmg", 1);
        if (isBonusActive>0)
        {
            dmg /= (float)Math.Pow(3,isBonusActive);
        }
        isBonusActive = 0;

        passiveCooldown = PlayerPrefs.GetFloat("passiveCooldown", 5);
        objectIndex = PlayerPrefs.GetInt("objectIndex");
        curBackgroundIndex = PlayerPrefs.GetInt("curBackgroundIndex");

        var LocalcurBackgroundIndex = curBackgroundIndex;
        background.sprite = backgrounds[LocalcurBackgroundIndex];

        mainObject.SetActive(true);

        isGameWasCompleted = intToBool(PlayerPrefs.GetInt("isGameWasCompleted"));
        adWatchEndBonus = PlayerPrefs.GetInt("adWatchEndBonus");

        if (adWatchEndBonus!=3 && isGameWasCompleted)
        {
            endButton.SetActive(true);
            endButtonText.text = $"���������� 3 �������� �������� ����������� ������ {adWatchEndBonus} / 3";
        }

        if (objectIndex >= allMainObjects.Count)
        {
            objectIndex = 0;
        }
        for (int i = 0; i < allMainObjects.Count; i++)
        {
            allMainObjects[i].SetActive(false);
        }

        allMainObjects[objectIndex].SetActive(true);
        mainObject = allMainObjects[objectIndex];

        moneyText.text = Math.Round(money, 1) + "";

        // ������������� ����������� ��������
        Debug.Log($"�������� ������ �������. Money: {money}, Damage: {dmg}, isBonusActive {isBonusActive}, Passive Damage: {passiveDmg}, Passive Cooldown: {passiveCooldown}, Object Index: {objectIndex}, Current Background Index: {curBackgroundIndex}");
    }

    void EndGame()
    {
        ShowWarningText("�����������! �� ������ ����. ������ �� ������ ���������� 3 ������� � � ��� ����� ����������� ������");
        endButton.SetActive(true);
        isGameWasCompleted = true;
        PlayerPrefsSave();
    }

    int boolToInt(bool val)
    {
        if (val)
            return 1;
        else
            return 0;
    }

    bool intToBool(int val)
    {
        if (val != 0)
            return true;
        else
            return false;
    }
}

