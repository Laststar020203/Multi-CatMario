using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartSceneManager : MonoBehaviour, IEventListener
{
    // Start is called before the first frame update

    public AudioSource bgm;
    public Image characterImage;
    public InputField nameInputField;
    public Slider sound;

    public GameObject settingPanel;

    private int characterCode;
    void Start()
    {
        GameManager.instance.StartScene(0);
        bgm.volume = GameManager.instance.Setting.SoundValue;
        settingPanel.SetActive(false);
        AddListener();

    }

    private void InitSetting()
    {
        setCharacterColor(GameManager.instance.Setting.CharacterCode);
        nameInputField.text = GameManager.instance.Setting.Name;
        sound.value = GameManager.instance.Setting.SoundValue;
    }

    public void OptionButton()
    {
        settingPanel.SetActive(true);
        InitSetting();
    }

    public void StartGameButton()
    {
        LoadSceneManager.instance.NextSceneLoad();
    }

    private void Escape(EscapeEvent e)
    {
        if (settingPanel != null && settingPanel.activeSelf)
        {
            SettingUpdate();
            settingPanel.SetActive(false);
            return;

        }
        Application.Quit();
    }


    public void setSoundPower(Slider slider)
    {
        bgm.volume = slider.value;
    }

    public void setCharacterColor(int index)
    {
        switch (index)
        {
            case 0:
                characterImage.color = new Color(255, 255, 255);
                break;
            case 1:
                characterImage.color = new Color(255, 0, 0);
                break;
            case 2:
                characterImage.color = new Color(0, 180, 255);
                break;
            case 3:
                characterImage.color = new Color(255, 255, 0);
                break;

        }
        this.characterCode = index;
    }

    private void SettingUpdate()
    {
        string name = nameInputField.text != null ? nameInputField.text : "Player";
        PlayerPrefs.SetString("Name", name);
        PlayerPrefs.SetInt("CharacterCode", characterCode);
        PlayerPrefs.SetFloat("Sound", sound.value);

        GameManager.instance.Setting.SoundValue = sound.value;
        GameManager.instance.Me.CharacteID = (byte)characterCode;
        GameManager.instance.Me.Name = name;


    }


    public void AddListener()
    {
        EventManager.AddListener<EscapeEvent>(Escape);
    }

    public void RemoveListener()
    {
        EventManager.RemoveListener<EscapeEvent>(Escape);

    }
}
