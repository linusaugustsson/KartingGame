using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewCharacter : MonoBehaviour
{

    public List<GameObject> previewObjects = new List<GameObject>();

    public void ChangeCharacterPreview(int _characterValue) {
        for(int i = 0; i < previewObjects.Count; i++) {
            previewObjects[i].SetActive(false);
        }

        previewObjects[_characterValue].SetActive(true);
    }


}
