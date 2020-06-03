using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


public class InteractionManager : MonoBehaviour
{
    public GameObject LoginScreen;
    public Button SubmitButton;
    public GameObject SummaryButton;
    public GameObject SummaryScreen;
    public Text totalDataText;
    public Text initialText;
    public Text chosenText;



    #region privateMembers

    private int totalDataSize;
    private Color visitedTextColor = new Color32(243, 136, 50, 255);
    private Color solidPanelColor = new Color32(255, 142, 0, 255);
    private string age = "Select Age Group";
    private string initial;
    private string chosen = "AG";


    private string[] genders = new string[] { "Female", "Male", "Other" };
    private string selectedGender;
    private string[] ageGroups = new string[] { "Less than 25", "26 to 35", "36 to 45", "46 to 55", "56 and older" };
    private string selectedAgeGroup;

    private string[] capabilities = new string[] { "AI", "AG", "BC", "CL", "CY", "DS", "OP", "VR" };
    private string[] capabilityStrings = new string[] { "Artificial Intelligence", "Agile DevOps", "Blockchain", "Cloud & Infrastructure", "Cyber", "Data Services", "Open Source", "Virtual Reality" };

    private int visitedCapabilities = 0;
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
        SummaryButton.SetActive(false);
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
                print(octoData.all);
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


    #region capabilitySetters

    private void handleTextColorChange(string textObj)
    {
        GameObject text = GameObject.Find(textObj);
        if (text.GetComponent<Text>().color != visitedTextColor)
        {
            text.GetComponent<Text>().color = visitedTextColor;
            visitedCapabilities++;

        }

    }
    public void Set_AI()
    {
        if (initial == null)
        {
            initial = capabilities[0];
        }

        handleTextColorChange("Text_AI");
    }

    public void Set_AG()
    {
        if (initial == null)
        {
            initial = capabilities[1];
        }
        handleTextColorChange("Text_AG");
    }

    public void Set_BC()
    {
        if (initial == null)
        {
            initial = capabilities[2];
        }
        handleTextColorChange("Text_BC");

    }

    public void Set_CL()
    {
        if (initial == null)
        {
            initial = capabilities[3];
        }
        handleTextColorChange("Text_CL");

    }

    public void Set_CY()
    {
        if (initial == null)
        {
            initial = capabilities[4];
        }
        handleTextColorChange("Text_CY");

    }

    public void Set_DS()
    {
        if (initial == null)
        {
            initial = capabilities[5];
        }
        handleTextColorChange("Text_DS");

    }

    public void Set_OP()
    {
        if (initial == null)
        {
            initial = capabilities[6];
        }
        handleTextColorChange("Text_OP");

    }

    public void Set_VR()
    {
        if (initial == null)
        {
            initial = capabilities[7];
        }
        handleTextColorChange("Text_VR");

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


    public void ShowSummary()
    {
        SummaryScreen.SetActive(true);
        FilterInitialData();
    }

    private string HandleTextDisplay(string choice)
    {
        // first grab the index within the shortened array
        int index = System.Array.IndexOf(capabilities, choice);
        // now return the full string from the other array
        return capabilityStrings[index];
    }
    private void FilterInitialData()
    {
        // inital settings
        totalDataSize = octoData.all.Count;
        totalDataText.text = totalDataSize.ToString();
        initialText.text = HandleTextDisplay(initial);
        chosenText.text = HandleTextDisplay(chosen);

        // now lets handle some filtering
        // first setup all the capability categories

        List<Users> femaleList = octoData.all.Where(x => x.gender == "Female").ToList();
        List<Users> maleList = octoData.all.Where(x => x.gender == "Male").ToList();
        List<Users> otherList = octoData.all.Where(x => x.gender == "Other").ToList();

        print("Female: " + femaleList.Count);
        print("Male: " + maleList.Count);
        print("Other: " + otherList.Count);

    }


    // Update is called once per frame
    void Update()
    {
        bool validGender = ((selectedGender != "Select Gender") && (selectedGender != null));
        bool validAge = ((selectedAgeGroup != "Select Age Group") && (selectedAgeGroup != null));

        // make sure they choose valid things before we let them submit
        if (validAge && validGender)
        {
            SubmitButton.interactable = true;
        }

        // once they have visited all capabilities
        if (visitedCapabilities == 8)
        {
            // unlock view summary button
            SummaryButton.SetActive(true);
        }



    }

}
