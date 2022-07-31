using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class te : MonoBehaviour
{
    Transform tran;
    Button btn;

    // Start is called before the first frame update
    void Start()
    {
        tran = this.transform.Find("Buttons").transform;
        btn = tran.Find("ButtonMusicPlay").GetComponent<Button>();

        btn.onClick.AddListener(() =>
        {
            Debug.Log("this is a button test");
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
