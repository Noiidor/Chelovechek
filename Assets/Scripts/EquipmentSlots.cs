using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentSlots : MonoBehaviour
{
    [SerializeField] private Slot[] slots;
    private KeyCode savedKey;
    private MonoBehaviour savedScript = null;

    private void Awake()
    {
        foreach (Slot slot in slots)
        {
            slot.script.enabled = false;
        }
    }

    void Update()
    {
        foreach (Slot slot in slots)
        {
            if (Input.GetKeyDown(slot.key))
            {
                if (savedKey != slot.key)
                {
                    if (savedScript != null)
                    {
                        savedScript.enabled = false;
                    }
                }
                savedKey = slot.key;
                savedScript = slot.script;
                slot.script.enabled = true;
            }
        }
    }
}
