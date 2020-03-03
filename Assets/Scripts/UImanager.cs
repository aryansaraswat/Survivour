using UnityEngine;

public class UImanager : MonoBehaviour
{
    public GameObject Inventorytab;
    public GameObject[] obj;
    int i;
    void Start()
    {
        Inventorytab.SetActive(false);
    }
    public void DisplayFoodmenu(GameObject foodmenu)
    {
        bool active = foodmenu.activeSelf;
        foodmenu.SetActive(!active);
    }
    public void DisplayInfo(int i)
    {
      if(i==0)
        {
            bool active = obj[i].activeSelf;
            obj[i].SetActive(!active);
        }
      else if(i==1)
        {
            bool active = obj[i].activeSelf;
            obj[i].SetActive(!active);
        }
    }
    void Update()
    {
      
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Inventorytab.SetActive(true);
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            Inventorytab.SetActive(false);
        }
    }
    public void PopupDisplay(GameObject pop)
    {
        pop.SetActive(true);
    }
    public void PopupInactive(GameObject pop)
    {
        pop.SetActive(false);
    }
}
