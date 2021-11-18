using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    /* SECTION GAMEOBJECT */
    public GameObject cheeseImage;
    public GameObject algaeImage;
    public GameObject meatImage;
    public GameObject swordImage;
    public GameObject bowImage;
    public GameObject magicImage;
    public GameObject wonImage;
    public GameObject lostImage;
    public GameObject deathCount;
    public GameObject menuPause;
    public GameObject bgMusic;
    public GameObject enemiesObject;

    /* SECTION TEXTMESHPRO */
    private TextMeshProUGUI _deathCounter;
    public TextMeshProUGUI scoreDisplay;
    public TextMeshProUGUI lifeDisplay;
    public TextMeshProUGUI ammoCount;

    private void ToggleActive(GameObject[] elem)
    {
        foreach (GameObject gO in elem)
        {
            gO.SetActive(!gO.activeSelf);
        }
    }

    private void ToggleActive(GameObject elem)
    {
        elem.SetActive(!elem.activeSelf);
    }

    private void Awake()
    {
        _deathCounter = deathCount.GetComponent<TextMeshProUGUI>();

        ToggleActive(
            new[] { menuPause, cheeseImage, algaeImage, meatImage, bowImage, magicImage, wonImage, lostImage }
        );
    }

    public void ToggleMenuPause()
    {
        ToggleActive(menuPause);
    }

    public void DisplayWon()
    {
        ToggleActive(new [] { wonImage, enemiesObject });
    }

    public void DisplayLost()
    {
        ToggleActive(new [] { lostImage, enemiesObject });
    }

    // Désactive l'icône d'arc et active le bâton
    public void UiMagic()
    {
        bowImage.SetActive(false);
        magicImage.SetActive(true);
    }

    // Désactive l'icône d'épée et active l'arc
    public void UiBow()
    {
        swordImage.SetActive(false);
        bowImage.SetActive(true);
    }

    public void ToggleCheeseImage()
    {
        ToggleActive(cheeseImage);
    }

    public void ToggleAlgaeImage()
    {
        ToggleActive(algaeImage);
    }

    public void ToggleMeatImage()
    {
        ToggleActive(meatImage);
    }

    public void UpdateDeathScore(int currentDeathScore)
    {
        _deathCounter.SetText($"Death : {currentDeathScore}");
    }

    public void UpdateLivesDisplay(int livesNumber)
    {
        lifeDisplay.SetText($"Lives : {livesNumber}");
    }

    public void UpdateScoreDisplay(int currentScore)
    {
        scoreDisplay.SetText($"Score : {currentScore}");
    }

    public void UpdateAmmoCount(int currentAmmos)
    {
        ammoCount.SetText($"{currentAmmos}");
        if (currentAmmos == 0) ammoCount.color = Color.red;
    }
}
