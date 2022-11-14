using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScript : MonoBehaviour
{
    public Transform ProgressBar;
    public Transform Farms;
    public Transform Zone;
    public Transform IDs;
    public Transform Logs;
    public Transform NextDay;

    public float AngularVelocity = 12.0f;
    public float Min = 0.1f;
    public float Max = 0.9f;
    
    private bool mExpanding = false;
    private bool mHasInitialize;
    private float mAccumulatror = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        mHasInitialize = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(!ModelHandler.IsModelRunning())
        {
            Hide();
            return;
        }

        Show();

        mAccumulatror += Time.deltaTime;
        Image img = ProgressBar.GetComponent<Image>();
        if(mAccumulatror >= 1.0f)
        {
            mExpanding = !mExpanding;
            mAccumulatror -= 1.0f;
        }

        float min = mExpanding ? Min : Max;
        float max = mExpanding ? Max : Min;
        img.fillAmount = Mathf.Lerp(min, max, mAccumulatror);

        float z = ProgressBar.eulerAngles.z - Time.deltaTime * AngularVelocity;
        ProgressBar.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, z));
    }

    private void Hide()
    {
        transform.GetComponent<SpriteRenderer>().enabled = false;
        ProgressBar.GetComponent<Image>().enabled = false;

        if(!mHasInitialize)
        {
            List<ushort> infected = ModelHandler.GetInfected();
            if (infected.Count > 0)
            {
                int index = UnityEngine.Random.Range(0, infected.Count);
                Farms.GetChild(infected[index] - 1).GetChild(1).gameObject.SetActive(true);
            }
            else
                UnityEngine.Debug.Log("No infected farm");
            mHasInitialize = true;
        }

        mExpanding = true;
        mAccumulatror = 0.0f;
        ProgressBar.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f));

        foreach(Transform farm in Farms)
        {
            Button btn = farm.GetComponentInChildren<Button>();
            btn.interactable = true;
        }

        Zone.GetComponentInChildren<Button>().interactable = true;
        IDs.GetComponentInChildren<Button>().interactable = true;
        Logs.GetComponentInChildren<Button>().interactable = true;
        NextDay.GetComponentInChildren<Button>().interactable = true;
    }

    private void Show()
    {
        transform.GetComponent<SpriteRenderer>().enabled = true;
        ProgressBar.GetComponent<Image>().enabled = true;

        foreach (Transform farm in Farms)
        {
            Button btn = farm.GetComponentInChildren<Button>();
            btn.interactable = false;
        }

        Zone.GetComponentInChildren<Button>().interactable = false;
        IDs.GetComponentInChildren<Button>().interactable = false;
        Logs.GetComponentInChildren<Button>().interactable = false;
        NextDay.GetComponentInChildren<Button>().interactable = false;
    }
}
