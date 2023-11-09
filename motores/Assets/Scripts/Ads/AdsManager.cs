using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdsManager : MonoBehaviour, IUnityAdsListener
{
    [SerializeField] string gameID = "5472025";
    [SerializeField] string rewardedAdID = "Rewarded_Android";


    private void Start()
    {
        Advertisement.AddListener(this);
        Advertisement.Initialize(gameID);

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
        //Cuando el anuncio comienza se escutará este método
    }

    public void OnUnityAdsReady(string placementId)
    {
        //Cuando el anuncia se pre carga, se ejecutará este método.
        Debug.Log("Is ready ti show");

    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        //Cuando el anuncio terminó de verse completo o la mitad, se ejecutará este método
        if(placementId==rewardedAdID)
        {
            if (showResult == ShowResult.Finished) Debug.Log("Full rewards");
            else if (showResult == ShowResult.Skipped) Debug.Log("Half rewards");
            else if (showResult == ShowResult.Failed) Debug.Log("No rewards");


        }
    }
}
