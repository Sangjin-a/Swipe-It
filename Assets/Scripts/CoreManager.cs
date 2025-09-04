using Assets.Scripts;
using SwipeSort;
using TMPro;
using UnityEngine;

public class CoreManager : MonoBehaviour
{
    public static CoreManager instance;
    [SerializeField] public TextMeshProUGUI scoreTMP;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    void Start()
    {

    }
   
    // Update is called once per frame
    void Update()
    {

    }


}
