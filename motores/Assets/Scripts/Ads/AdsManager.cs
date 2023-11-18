using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using TMPro;

public class AdsManager : MonoBehaviour, IUnityAdsListener
{
    [SerializeField] string gameID = "5472025";
    [SerializeField] string rewardedAdID = "Rewarded_Android";
    [SerializeField] TextMeshProUGUI rewardText;
    [SerializeField] GameObject rewardPanel;
    [SerializeField] private int rewardFull = 20;
    [SerializeField] private int rewardHalf = 10;
    [SerializeField] private MenuUI menuUI;

    private void Start()
    {
        Advertisement.AddListener(this);
        Advertisement.Initialize(gameID);
        rewardPanel.SetActive(false);
    }

    public void ShowAd()
    {
        if (!Advertisement.IsReady()) return;
        Advertisement.Show(rewardedAdID);
    }

    public void OnUnityAdsDidError(string message)
    {
        //Cuando el anuncio lanza un error, se ejecutará este método
    }
     

    public void OnUnityAdsDidStart(string placementId)
    {
        //Cuando el anuncio comienza se ejecutará este método
    }

    public void OnUnityAdsReady(string placementId)
    {
        //Cuando el anuncia se pre carga, se ejecutará este método.
        //Debug.Log("Is ready ti show");

    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        //Cuando el anuncio terminó de verse completo o la mitad, se ejecutará este método
        if(placementId==rewardedAdID)
        {
            rewardPanel.SetActive(true);

            if (showResult == ShowResult.Finished)
            {
                rewardText.text = "You earn " + rewardFull + " coins!";
                GameManager.Instance.sumarcoinst(rewardFull);
            }

            else if (showResult == ShowResult.Skipped)
            {
                rewardText.text = "You earn " + rewardHalf + " coins!";
                GameManager.Instance.sumarcoinst(rewardHalf);
            }

            PlayerData.Get().SaveGame();
            menuUI.RefreshData();
        }
    }
}
