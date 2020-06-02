using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


public class InteractionManager : MonoBehaviour
{
    public GameObject LoginScreen;
    public Button SubmitButton;

    #region privateMembers

    private string age = "Select Age Group";
    private string initial = "AI";
    private string chosen = "VR";


    private string[] genders = new string[] { "Female", "Male", "Other" };
    private string selectedGender;
    private string[] ageGroups = new string[] { "Less than 25", "26 to 35", "36 to 45", "46 to 55", "56 and older" };
    private string selectedAgeGroup;

    private string[] capabilities = new string[] { "AI", "AG", "BC", "CL", "CY", "DS", "OP", "VR" };

    #endregion

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

    // Launch the fetch to the API and grab the data
    void Start()
    {
        StartCoroutine(GetRequest("https://us-central1-octo-ar-demo.cloudfunctions.net/getUsers"));
        SubmitButton.interactable = false;
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

    #region Setters
    public void SetGender(int genderInput)
    {
        selectedGender = genders[genderInput];
    }

    public void SetAge(int ageInput)
    {
        selectedAgeGroup = ageGroups[ageInput];
    }

    public void SetInitial(string initialInput)
    {
        initial = initialInput;
    }

    #region capabilitySetters
    public void Set_AI()
    {
        initial = capabilities[0];
        print(initial);
    }

    public void Set_AG()
    {
        initial = capabilities[1];
        print(initial);
    }


    public void Set_BC()
    {
        initial = capabilities[2];
        print(initial);
    }

    public void Set_CL()
    {
        initial = capabilities[3];
        print(initial);
    }

    public void Set_CY()
    {
        initial = capabilities[4];
        print(initial);
    }

    public void Set_DS()
    {
        initial = capabilities[5];
        print(initial);
    }

    public void Set_OP()
    {
        initial = capabilities[6];
        print(initial);
    }

    public void Set_VR()
    {
        initial = capabilities[7];
        print(initial);
    }


    #endregion
    public void SetChosen(string chosenInput)
    {
        chosen = chosenInput;
    }

    #endregion

    // Submit form will store age and gender, and hide the login screen
    public void SubmitForm()
    {
        LoginScreen.SetActive(false);

    }


    // StartCoroutine(PostData());
    IEnumerator PostData()
    {
        string data = "data";
        string url = $"https://us-central1-octo-ar-demo.cloudfunctions.net/addUser?age={selectedAgeGroup}&gender={selectedGender}&initial={initial}&chosen={chosen}";
        using (UnityWebRequest webRequest = UnityWebRequest.Post(url, data))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.Log("Error Posting Data: " + webRequest.error);
            }
            else
            {
                Debug.Log($"Successful Post from a {selectedAgeGroup} year old {selectedGender} with inital({initial}) and chosen({chosen})");
            }
        }
    }



    // Update is called once per frame
    void Update()
    {
        bool validGender = ((selectedGender != "Select Gender") && (selectedGender != null));
        bool validAge = ((selectedAgeGroup != "Select Age Group") && (selectedAgeGroup != null));

        if (validAge && validGender)
        {
            SubmitButton.interactable = true;
        }





    }

}
