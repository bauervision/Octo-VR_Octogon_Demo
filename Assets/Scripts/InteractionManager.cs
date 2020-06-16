using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Playables;
using System.Runtime.InteropServices;


public class InteractionManager : MonoBehaviour
{
    public GameObject LoadingScreen;
    public GameObject LoginScreen;
    public GameObject InitialDialog;
    public GameObject InitialPanel;
    public GameObject NewLoader;
    public PlayableDirector timeline;
    public GameObject MainButtons;
    public GameObject RoomButtons;
    public GameObject RoomSprite;
    public Text RoomTitle;
    public GameObject RoomPOC;

    public Sprite AI_sprite;
    public Sprite AG_sprite;
    public Sprite BC_sprite;
    public Sprite CL_sprite;
    public Sprite CY_sprite;
    public Sprite DS_sprite;
    public Sprite OP_sprite;
    public Sprite VR_sprite;

    public GameObject UI_initial_sprite;
    public GameObject UI_chosen_sprite;



    public Button SubmitButton;
    public GameObject SummaryButton;
    public GameObject SummaryScreen;
    public GameObject SummaryPanel;
    public Button OverallSummary;
    public Button ParticipantSummary;

    public Text totalDataText;
    public Text initialText;
    public Text chosenText;

    public Text panel25Text;
    public Text panel26Text;
    public Text panel36Text;
    public Text panel46Text;
    public Text panel56Text;

    public Color defaultColor = new Color32(255, 142, 0, 9);
    public Color summaryColor = new Color32(255, 142, 0, 255);
    public Color tieColor = new Color32(255, 142, 0, 150);


    #region privateMembers

    [DllImport("__Internal")]
    private static extern void Select(int side);

    [DllImport("__Internal")]
    private static extern void SendData(int side);

    private int totalDataSize;
    private Color visitedTextColor = new Color32(243, 136, 50, 255);
    private string age = "Select Age Group";
    private string initial;
    private string chosen = "AG";
    private List<Users> femaleList;
    private List<Users> maleList;
    private List<Users> otherList;


    private string[] genders = new string[] { "Female", "Male", "Other" };
    private string selectedGender;
    private string[] ageGroups = new string[] { "Less than 25", "26 to 35", "36 to 45", "46 to 55", "56 and older" };
    private string selectedAgeGroup;

    private string[] capabilities = new string[] { "AI", "AG", "BC", "CL", "CY", "DS", "OP", "VR" };
    private string[] capabilityStrings = new string[] { "Artificial Intelligence", "Agile DevOps", "Blockchain", "Cloud & Infrastructure", "Cyber", "Data Services", "Open Source", "Virtual Reality" };

    private int visitedCapabilities = 0;
    private string viewAgeGroup = "Less than 25"; // default to the youngest age group


    private GameObject panel25;
    private GameObject panel26;
    private GameObject panel36;
    private GameObject panel46;
    private GameObject panel56;

    private Text dataSetText;

    private string currentTitle;

    private Sprite chosenSprite;
    private Sprite initialSprite;


    private bool showInitialData = true;
    private bool showParticipantDataTotal = false;
    private bool showParticipantDataSummary = true;

    private bool TimeLineIsPlaying = false;

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
        public string age;
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
        InitializeUI();

    }

    private void InitializeUI()
    {
        // set the panels
        panel25 = GameObject.Find("Panel_25");
        panel25.SetActive(true);
        panel26 = GameObject.Find("Panel_26");
        panel26.SetActive(false);
        panel36 = GameObject.Find("Panel_36");
        panel36.SetActive(false);
        panel46 = GameObject.Find("Panel_46");
        panel46.SetActive(false);
        panel56 = GameObject.Find("Panel_56");
        panel56.SetActive(false);

        dataSetText = GameObject.Find("DataSetText").GetComponent<Text>();
        // now hide the summary
        SummaryScreen.SetActive(false);
        SummaryButton.SetActive(false);
        SummaryPanel.SetActive(false);
        NewLoader.SetActive(false);
        RoomButtons.SetActive(false);


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

                femaleList = octoData.all.Where(x => x.gender == "Female").ToList();
                maleList = octoData.all.Where(x => x.gender == "Male").ToList();
                otherList = octoData.all.Where(x => x.gender == "Other").ToList();
                print(octoData.all.Count + " points of Data Received");

                // when we run this fetch the second time...
                if (visitedCapabilities == 8)
                {
                    NewLoader.SetActive(false);
                    // set general summary
                    totalDataSize = octoData.all.Count;
                    totalDataText.text = totalDataSize.ToString();
                    initialText.text = HandleTextDisplay(initial);
                    chosenText.text = HandleTextDisplay(chosen);
                    // display the panel
                    SummaryScreen.SetActive(true);
                    OverallSummary.Select();
                    FilterData();
                    RunSummaries();

                    UI_chosen_sprite.GetComponent<Image>().sprite = chosenSprite;
                    UI_initial_sprite.GetComponent<Image>().sprite = initialSprite;
                }
                LoadingScreen.SetActive(false);
            }
        }
    }

    #region Setters

    public void SetGender(int genderInput)
    {
        selectedGender = genders[genderInput - 1];
    }

    public void SetAge(int ageInput)
    {
        // account for the "Select..."
        selectedAgeGroup = ageGroups[ageInput - 1];
    }




    private void handleTextColorChange(string textObj)
    {
        Text text = GameObject.Find(textObj).GetComponent<Text>();
        if (text != null)
        {
            if (text.color != visitedTextColor)
            {
                text.color = visitedTextColor;
                visitedCapabilities++;

            }
        }


    }

    #region IconSetters

    private void handleIconSwap(Sprite sprite)
    {
        RoomSprite.GetComponent<SpriteRenderer>().sprite = sprite;
    }


    private void handleTitleChange(string newTitle)
    {
        currentTitle = newTitle;
    }


    public void Set_AI()
    {

        if (initial == null)
        {
            initial = capabilities[0];
            initialSprite = AI_sprite;
            InitialPanel.SetActive(false);
        }

        if (visitedCapabilities == 8)
        {
            chosen = capabilities[0];
            chosenSprite = AI_sprite;
            // unlock view summary button
            SummaryButton.SetActive(true);
            SummaryPanel.SetActive(false);
            HideMainButtons();
        }
        else
        {
            handleTextColorChange("Text_AI");
            handleIconSwap(AI_sprite);
            handleTitleChange(capabilityStrings[0]);
            PlayTimeline();
        }
    }

    public void Set_AG()
    {

        if (initial == null)
        {
            initial = capabilities[1];
            initialSprite = AG_sprite;
            InitialPanel.SetActive(false);
        }

        if (visitedCapabilities == 8)
        {
            chosen = capabilities[1];
            chosenSprite = AG_sprite;
            // unlock view summary button
            SummaryButton.SetActive(true);
            SummaryPanel.SetActive(false);
            HideMainButtons();
        }
        else
        {
            handleTextColorChange("Text_AG");
            handleIconSwap(AG_sprite);
            handleTitleChange(capabilityStrings[1]);
            PlayTimeline();
        }
    }

    public void Set_BC()
    {

        if (initial == null)
        {
            initial = capabilities[2];
            initialSprite = BC_sprite;
            InitialPanel.SetActive(false);
        }

        if (visitedCapabilities == 8)
        {
            chosen = capabilities[2];
            chosenSprite = BC_sprite;
            SummaryButton.SetActive(true);
            SummaryPanel.SetActive(false);
            HideMainButtons();
        }
        else
        {
            handleTextColorChange("Text_BC");
            handleIconSwap(BC_sprite);
            handleTitleChange(capabilityStrings[2]);
            PlayTimeline();
        }
    }

    public void Set_CL()
    {

        if (initial == null)
        {
            initial = capabilities[3];
            initialSprite = CL_sprite;
            InitialPanel.SetActive(false);
        }

        if (visitedCapabilities == 8)
        {
            chosen = capabilities[3];
            chosenSprite = CL_sprite;
            SummaryButton.SetActive(true);
            SummaryPanel.SetActive(false);
            HideMainButtons();
        }
        else
        {
            handleTextColorChange("Text_CL");
            handleIconSwap(CL_sprite);
            handleTitleChange(capabilityStrings[3]);
            PlayTimeline();
        }
    }

    public void Set_CY()
    {

        if (initial == null)
        {
            initial = capabilities[4];
            initialSprite = CY_sprite;
            InitialPanel.SetActive(false);
        }

        if (visitedCapabilities == 8)
        {
            chosen = capabilities[4];
            chosenSprite = CY_sprite;
            SummaryButton.SetActive(true);
            SummaryPanel.SetActive(false);
            HideMainButtons();
        }
        else
        {
            handleTextColorChange("Text_CY");
            handleIconSwap(CY_sprite);
            handleTitleChange(capabilityStrings[4]);
            PlayTimeline();
        }
    }

    public void Set_DS()
    {

        if (initial == null)
        {
            initial = capabilities[5];
            initialSprite = DS_sprite;
            InitialPanel.SetActive(false);
        }

        if (visitedCapabilities == 8)
        {
            chosen = capabilities[5];
            chosenSprite = DS_sprite;
            SummaryButton.SetActive(true);
            SummaryPanel.SetActive(false);
            HideMainButtons();
        }
        else
        {
            handleTextColorChange("Text_DS");
            handleIconSwap(DS_sprite);
            handleTitleChange(capabilityStrings[5]);
            PlayTimeline();
        }
    }

    public void Set_OP()
    {

        if (initial == null)
        {
            initial = capabilities[6];
            initialSprite = OP_sprite;
            InitialPanel.SetActive(false);
        }

        if (visitedCapabilities == 8)
        {
            chosen = capabilities[6];
            chosenSprite = OP_sprite;
            SummaryButton.SetActive(true);
            SummaryPanel.SetActive(false);
            HideMainButtons();
        }
        else
        {
            handleTextColorChange("Text_OP");
            handleIconSwap(OP_sprite);
            handleTitleChange(capabilityStrings[6]);
            PlayTimeline();
        }
    }

    public void Set_VR()
    {

        if (initial == null)
        {
            initial = capabilities[7];
            initialSprite = VR_sprite;
            InitialPanel.SetActive(false);
        }

        if (visitedCapabilities == 8)
        {
            chosen = capabilities[7];
            chosenSprite = VR_sprite;
            SummaryButton.SetActive(true);
            SummaryPanel.SetActive(false);
            HideMainButtons();
        }
        else
        {
            handleTextColorChange("Text_VR");
            handleIconSwap(VR_sprite);
            handleTitleChange(capabilityStrings[7]);
            PlayTimeline();
        }
    }

    #endregion



    public void SetChosen(string chosenInput)
    {
        chosen = chosenInput;
    }

    #endregion

    public void HideInitialDialog()
    {
        InitialDialog.SetActive(false);
    }

    // Submit form will store age and gender, and hide the login screen
    public void SubmitForm()
    {
        LoginScreen.SetActive(false);
    }

    //
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
                StartCoroutine(GetRequest("https://us-central1-octo-ar-demo.cloudfunctions.net/getUsers"));
            }
        }
    }


    public void ShowSummary()
    {
        SummaryButton.SetActive(false);
        NewLoader.SetActive(true);
        StartCoroutine(PostData());
    }

    public void ToggleDataDisplay()
    {
        showInitialData = !showInitialData;
        FilterData();
        if (showParticipantDataSummary)
        {
            RunSummaries();
        }
        dataSetText.text = showInitialData ? "Initial Interest Data Displayed" : "Chosen Interest Data Displayed";
    }


    private string HandleTextDisplay(string choice)
    {
        // first grab the index within the shortened array
        int index = System.Array.IndexOf(capabilities, choice);
        // now return the full string from the other array
        return capabilityStrings[index];
    }
    private void FilterData()
    {
        SetStrings("AI");
        SetStrings("AG");
        SetStrings("BC");
        SetStrings("CL");
        SetStrings("CY");
        SetStrings("DS");
        SetStrings("OP");
        SetStrings("VR");
    }

    private void SetStrings(string capability)
    {
        // reset to default colors before doing anything
        GameObject.Find($"Female-{capability}-Panel").GetComponent<Image>().color = defaultColor;
        GameObject.Find($"Male-{capability}-Panel").GetComponent<Image>().color = defaultColor;
        GameObject.Find($"Other-{capability}-Panel").GetComponent<Image>().color = defaultColor;

        GameObject.Find($"Female-{capability}").GetComponent<Text>().text = HandleData(capability, femaleList);
        GameObject.Find($"Male-{capability}").GetComponent<Text>().text = HandleData(capability, maleList);
        GameObject.Find($"Other-{capability}").GetComponent<Text>().text = HandleData(capability, otherList);
    }
    private string HandleData(string parameter, List<Users> list)
    {
        // if we dont want to show  data total and data summary, then run filters
        if (!showParticipantDataTotal && !showParticipantDataSummary)
        {
            // run filter first on parameter
            IEnumerable parameterQuery = list.Where(x => (showInitialData ? x.initial : x.chosen) == parameter);

            if (parameterQuery != null)
            {
                List<Users> parameterList = list.Where(x => (showInitialData ? x.initial : x.chosen) == parameter).ToList();
                // now run filter on age group
                IEnumerable ageQuery = parameterList.Where(y => y.age == viewAgeGroup);

                if (ageQuery != null)
                {
                    string newString = parameterList.Where(y => y.age == viewAgeGroup).ToList().Count.ToString();
                    if (newString == "0")
                    {
                        return "";
                    }
                    return newString;
                }
                return "";
            }
            return "";
        }
        else
        {
            // run filter first on parameter
            IEnumerable parameterQuery = list.Where(x => (showInitialData ? x.initial : x.chosen) == parameter);

            if (parameterQuery != null)
            {
                string newString = list.Where(x => (showInitialData ? x.initial : x.chosen) == parameter).ToList().Count.ToString();

                if (newString == "0")
                {
                    return "";
                }
                return newString;
            }
            return "";
        }
    }


    #region Set_DataFilters_Summary
    public void SetAge25()
    {
        viewAgeGroup = ageGroups[0];
        showParticipantDataTotal = false;
        showParticipantDataSummary = false;
        FilterData();
        // display proper panel, and turn off the others
        panel25.SetActive(true);
        panel26.SetActive(false);
        panel36.SetActive(false);
        panel46.SetActive(false);
        panel56.SetActive(false);
    }

    public void SetAge26()
    {
        viewAgeGroup = ageGroups[1];
        showParticipantDataTotal = false;
        showParticipantDataSummary = false;
        FilterData();
        // display proper panel, and turn off the others
        panel25.SetActive(false);
        panel26.SetActive(true);
        panel36.SetActive(false);
        panel46.SetActive(false);
        panel56.SetActive(false);
    }

    public void SetAge36()
    {
        viewAgeGroup = ageGroups[2];
        showParticipantDataTotal = false;
        showParticipantDataSummary = false;
        FilterData();
        // display proper panel, and turn off the others
        panel25.SetActive(false);
        panel26.SetActive(false);
        panel36.SetActive(true);
        panel46.SetActive(false);
        panel56.SetActive(false);
    }

    public void SetAge46()
    {
        viewAgeGroup = ageGroups[3];
        showParticipantDataTotal = false;
        showParticipantDataSummary = false;
        FilterData();
        // display proper panel, and turn off the others
        panel25.SetActive(false);
        panel26.SetActive(false);
        panel36.SetActive(false);
        panel46.SetActive(true);
        panel56.SetActive(false);
    }

    public void SetAge56()
    {
        viewAgeGroup = ageGroups[4];
        showParticipantDataTotal = false;
        showParticipantDataSummary = false;
        FilterData();
        // display proper panel, and turn off the others
        panel25.SetActive(false);
        panel26.SetActive(false);
        panel36.SetActive(false);
        panel46.SetActive(false);
        panel56.SetActive(true);
    }


    public void ShowParticipantData()
    {
        showParticipantDataTotal = true;
        showParticipantDataSummary = false;
        FilterData();
    }

    public void ShowParticipantSummary()
    {
        showParticipantDataTotal = false;
        showParticipantDataSummary = true;
        FilterData();
        RunSummaries();


    }

    #endregion

    private void RunSummaries()
    {
        RunSummaryColor("AI");
        RunSummaryColor("AG");
        RunSummaryColor("BC");
        RunSummaryColor("CL");
        RunSummaryColor("CY");
        RunSummaryColor("DS");
        RunSummaryColor("OP");
        RunSummaryColor("VR");
    }
    private void RunSummaryColor(string capability)
    {
        // first we need to store and covert the value of each gender
        int femaleValue, maleValue, otherValue;
        int.TryParse(GameObject.Find($"Female-{capability}").GetComponent<Text>().text, out femaleValue);
        int.TryParse(GameObject.Find($"Male-{capability}").GetComponent<Text>().text, out maleValue);
        int.TryParse(GameObject.Find($"Other-{capability}").GetComponent<Text>().text, out otherValue);

        HandleComparisons(femaleValue, maleValue, otherValue, capability);
    }

    private void HandleComparisons(int femaleValue, int maleValue, int otherValue, string capability)
    {
        bool femaleWon = (femaleValue > maleValue) && (femaleValue > otherValue);
        bool maleWon = (maleValue > femaleValue) && (maleValue > otherValue);
        bool otherWon = (otherValue > maleValue) && (otherValue > femaleValue);

        // now compare them, greatest value wins
        if (femaleWon)
        {
            // change to solid color for the winner
            GameObject.Find($"Female-{capability}-Panel").GetComponent<Image>().color = summaryColor;
            // and erase the text for the losers
            GameObject.Find($"Male-{capability}").GetComponent<Text>().text = "";
            GameObject.Find($"Other-{capability}").GetComponent<Text>().text = "";
        }
        else if (maleWon)
        {
            GameObject.Find($"Male-{capability}-Panel").GetComponent<Image>().color = summaryColor;
            GameObject.Find($"Female-{capability}").GetComponent<Text>().text = "";
            GameObject.Find($"Other-{capability}").GetComponent<Text>().text = "";
        }
        else if (otherWon)
        {
            GameObject.Find($"Other-{capability}-Panel").GetComponent<Image>().color = summaryColor;
            GameObject.Find($"Male-{capability}").GetComponent<Text>().text = "";
            GameObject.Find($"Female-{capability}").GetComponent<Text>().text = "";
        }
        else
        {
            // at this point there wasn't a clear winner, which means there was a tie somewhere
            if (femaleValue == maleValue)
            {
                GameObject.Find($"Female-{capability}-Panel").GetComponent<Image>().color = tieColor;
                GameObject.Find($"Male-{capability}-Panel").GetComponent<Image>().color = tieColor;
                GameObject.Find($"Female-{capability}").GetComponent<Text>().text = "";
                GameObject.Find($"Male-{capability}").GetComponent<Text>().text = "";

                //handle the other value
                if (otherValue < femaleValue)
                {
                    GameObject.Find($"Other-{capability}").GetComponent<Text>().text = "";
                }
            }

            if (femaleValue == otherValue)
            {
                GameObject.Find($"Female-{capability}-Panel").GetComponent<Image>().color = tieColor;
                GameObject.Find($"Other-{capability}-Panel").GetComponent<Image>().color = tieColor;
                GameObject.Find($"Female-{capability}").GetComponent<Text>().text = "";
                GameObject.Find($"Other-{capability}").GetComponent<Text>().text = "";

                //handle the other value
                if (maleValue < femaleValue)
                {
                    GameObject.Find($"Male-{capability}").GetComponent<Text>().text = "";
                }
            }

            if (otherValue == maleValue)
            {
                GameObject.Find($"Other-{capability}-Panel").GetComponent<Image>().color = tieColor;
                GameObject.Find($"Male-{capability}-Panel").GetComponent<Image>().color = tieColor;
                GameObject.Find($"Other-{capability}").GetComponent<Text>().text = "";
                GameObject.Find($"Male-{capability}").GetComponent<Text>().text = "";

                //handle the other value
                if (femaleValue < otherValue)
                {
                    GameObject.Find($"Female-{capability}").GetComponent<Text>().text = "";
                }
            }
        }

    }


    public void Exit()
    {
        Application.Quit();
    }


    private void PlayTimeline()
    {
        timeline.Play();
        TimeLineIsPlaying = true;
        HideMainButtons();
    }


    private void HideMainButtons()
    {
        MainButtons.SetActive(false);
    }



    public void TimelineComplete()
    {
        TimeLineIsPlaying = false;
        RoomButtons.SetActive(true);
        RoomTitle.text = currentTitle;
    }

    public void ResetCamera()
    {
        timeline.time = 0;
        timeline.RebuildGraph();
        timeline.Evaluate();
        RoomButtons.SetActive(false);
        MainButtons.SetActive(true);

        if (visitedCapabilities == 8)
        {
            SummaryPanel.SetActive(true);
        }


    }

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


        // handle button switcher text based on showInitialData
        panel25Text.text = showInitialData ? "Initial Interest" : "Chosen Interest";
        panel26Text.text = showInitialData ? "Initial Interest" : "Chosen Interest";
        panel36Text.text = showInitialData ? "Initial Interest" : "Chosen Interest";
        panel46Text.text = showInitialData ? "Initial Interest" : "Chosen Interest";
        panel56Text.text = showInitialData ? "Initial Interest" : "Chosen Interest";

        if (showParticipantDataSummary && SummaryScreen.activeInHierarchy)
        {
            OverallSummary.Select();
        }

        if (showParticipantDataTotal && SummaryScreen.activeInHierarchy)
        {
            ParticipantSummary.Select();
        }




    }

}
