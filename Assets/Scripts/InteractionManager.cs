using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class InteractionManager : MonoBehaviour
{


    /*  =============== JSON related =====================  */
    #region JSONrelated
    [System.Serializable]
    private class OctoDemoData
    {
        public List<Users> all;
    }


    [System.Serializable]
    private class Users
    {
        public int age;
        public string gender;
        public string initial;
        public string chosen;
    }

    private OctoDemoData octoData;
    #endregion

    // Start is called before the first frame update
    void Start()
    {

        StartCoroutine(GetRequest("https://us-central1-octo-ar-demo.cloudfunctions.net/getUsers"));
    }

    IEnumerator GetRequest(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = url.Split('/');
            int page = pages.Length - 1;

            if (webRequest.isNetworkError)
            {
                Debug.Log(pages[page] + ": Error: " + webRequest.error);
            }
            else
            {
                var data = webRequest.downloadHandler.text;
                octoData = JsonUtility.FromJson<OctoDemoData>(data);
            }
        }
    }

    public void GetData()
    {
        print(octoData.all[0].gender);
    }




    // Update is called once per frame
    void Update()
    {





    }

}
