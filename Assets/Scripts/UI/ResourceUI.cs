using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ResourceUI : MonoBehaviour
{
    Image image;
    [SerializeField] Sprite sprite;
    [SerializeField] Camera camera;
    TextMeshProUGUI TMP;
    ResourceBuilding resourceBuilding;
    TowerBuilding towerBuilding;
    GameObject root;
    bool onece = false;

    // Start is called before the first frame update
    void Start()
    {
     root=transform.root.gameObject;

        image = GetComponentInChildren<Image>();
        TMP = GetComponentInChildren<TextMeshProUGUI>();
        root.TryGetComponent<ResourceBuilding>(out resourceBuilding);
        root.TryGetComponent<TowerBuilding>(out towerBuilding);
        camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        //transform.LookAt(camera.transform, Vector3.up);
        transform.rotation = Quaternion.LookRotation(-camera.transform.position+transform.position,Vector3.up);

        if (GameManager.Instance.GameState == GameState.WaitingForRound)
        {
            if (resourceBuilding != null)
            {
                image.sprite = sprite;
                TMP.text = "+" + resourceBuilding.GetBuildingIncome();
                TMP.fontSize = 3;
            }
            else
            {
                if(!onece)
                {
                    Destroy(image);
                    TMP.rectTransform.localPosition = new Vector3(0,TMP.rectTransform.localPosition.y +TMP.fontSize/2, 0);
                    onece = true;
                    TMP.fontSize = 3;
                    TMP.alignment = TextAlignmentOptions.BottomGeoAligned;
                }
                TMP.text = "Damage: " + towerBuilding.GetTowerDamage();
            }
        }
    }
}
