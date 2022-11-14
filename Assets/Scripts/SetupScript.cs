using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SetupScript : MonoBehaviour
{
    public Transform ProgressBar;
    public Transform Tip;
    public List<string> Tips;

    public uint Days = 7;
    public float Beta = 0.1f;
    public float Gamma = 0.077f;

    public float AngularVelocity = 360.0f;
    public float Min = 0.05f;
    public float Max = 0.9f;

    private bool mExpanding = true;
    private float mAccumulatror = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        GameContext.sCurrentDay = 1;
        GameContext.sBeta = Beta;
        GameContext.sGamma = Gamma;
        ModelHandler.Init(Days);
    }

    // Update is called once per frame
    void Update()
    {
        if(!ModelHandler.IsModelRunning())
        {
            SceneManager.LoadScene("WorldMap");
            return;
        }

        Image img = ProgressBar.GetComponent<Image>();
        if (mAccumulatror >= 1.0f)
        {
            mExpanding = !mExpanding;
            mAccumulatror -= 1.0f;
        }

        float min = mExpanding ? Min : Max;
        float max = mExpanding ? Max : Min;
        img.fillAmount = Mathf.Lerp(min, max, mAccumulatror);
        mAccumulatror += Time.deltaTime;

        float z = ProgressBar.eulerAngles.z - Time.deltaTime * AngularVelocity;
        ProgressBar.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, z));
    }
}
