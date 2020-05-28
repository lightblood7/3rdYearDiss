﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lean.Gui;
using UnityEngine.EventSystems;
using Utilities;
using System;

public class CircuitValuesPanel : MonoBehaviour
{


    private CircuitComponent currentCircuit;
    private InputField voltage, current, resistance, selectedText;
    private Toggle voltHidden, currentHidden, resistanceHidden;
    private Dropdown componentType;
    private LeanSwitch direction;

    private CanvasGroup canvasGroup;
    private Image upArrow, downArrow;
    private Color onColor, offColor;
    private GameObject autoV, autoC;
    // Start is called before the first frame update

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        voltage = transform.Find("Values/Voltage").GetComponent<InputField>();
        current = transform.Find("Values/Current").GetComponent<InputField>();
        resistance = transform.Find("Values/Resistance").GetComponent<InputField>();
        selectedText = transform.Find("Values/SelectedName").GetComponent<InputField>();

        voltHidden = transform.Find("Values/Voltage/Hidden").GetComponent<Toggle>();
        currentHidden = transform.Find("Values/Current/Hidden").GetComponent<Toggle>();
        resistanceHidden = transform.Find("Values/Resistance/Hidden").GetComponent<Toggle>();

        componentType = transform.Find("Values/Dropdown").GetComponent<Dropdown>();

        direction = transform.Find("Values/DirectionSelector").GetComponent<LeanSwitch>();
        upArrow = direction.transform.Find("BtoA (R)").GetComponent<Image>();
        downArrow = direction.transform.Find("AtoB (L)").GetComponent<Image>();
        onColor = Color.white;
        offColor = upArrow.color;

        autoV = voltage.transform.Find("AutoV").gameObject;
        autoC = current.transform.Find("AutoC").gameObject;
    }

    private void Update()
    {
        if (!currentCircuit)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;


        }
        else
        {
            if (direction.State == 0)
            {
                upArrow.color = offColor;
                downArrow.color = onColor;
            }
            else
            {
                upArrow.color = onColor;
                downArrow.color = offColor;
            }

            // currentCircuit.ColorToSelected();


            if (Input.GetMouseButtonDown(1))
            {
                updatecomponentValues();
                //   currentCircuit.ColorToMain();
                currentCircuit.hideHighlight();
                currentCircuit = null;

            }
            if (componentType.value == 0)
            {
                currentCircuit.component.Values[ComponentParameter.RESISTANCE].value = 0;
                resistance.gameObject.SetActive(false);
                direction.gameObject.SetActive(false);
                autoC.SetActive(false);
                autoV.SetActive(false);
                selectedText.text = "CELL";
                selectedText.interactable = false;
            }
            else
            {
                resistance.gameObject.SetActive(true);
                direction.gameObject.SetActive(true);
                autoC.SetActive(true);
                autoV.SetActive(true);
                selectedText.interactable = true;

            }
        }




    }



    public void newSelected(CircuitComponent selectedComponent)
    {
        if (currentCircuit && currentCircuit != selectedComponent)
        {
            updatecomponentValues();
            currentCircuit.hideHighlight();
        }

        //if(currentCircuit && currentCircuit != selectedCoponent) currentCircuit.ColorToMain();
        this.currentCircuit = null;



        // canvasGroup.blocksRaycasts = true;


        this.currentCircuit = selectedComponent;
        if (currentCircuit)
        {
            if (currentCircuit.isBuilder)
            {
                currentHidden.gameObject.SetActive(true);
                voltHidden.gameObject.SetActive(true);
                resistanceHidden.gameObject.SetActive(true);
            }
            else
            {
                currentHidden.gameObject.SetActive(false);
                voltHidden.gameObject.SetActive(false);
                resistanceHidden.gameObject.SetActive(false);

            }
        }




        currentCircuit.ShowHighlight();
        // currentCircuit.ColorToSelected();
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        selectedText.text = currentCircuit.gameObject.name;
        voltage.text = currentCircuit.component.Values[ComponentParameter.VOLTAGE].value.ToString();
        current.text = currentCircuit.component.Values[ComponentParameter.CURRENT].value.ToString();
        resistance.text = currentCircuit.component.Values[ComponentParameter.RESISTANCE].value.ToString();
        componentType.value = (int)currentCircuit.component.type - 1;
        voltHidden.isOn = currentCircuit.component.Values[ComponentParameter.VOLTAGE].hidden;
        currentHidden.isOn = currentCircuit.component.Values[ComponentParameter.CURRENT].hidden;
        resistanceHidden.isOn = currentCircuit.component.Values[ComponentParameter.RESISTANCE].hidden;





    }



    public void updatecomponentValues()
    {
        if (decimal.Parse(voltage.text
        , System.Globalization.CultureInfo.InvariantCulture.NumberFormat) > (decimal)0.001f)
            currentCircuit.component.Values[ComponentParameter.VOLTAGE].value = (float)Math.Round(decimal.Parse(voltage.text
            , System.Globalization.CultureInfo.InvariantCulture.NumberFormat), 2);

        if (decimal.Parse(current.text
        , System.Globalization.CultureInfo.InvariantCulture.NumberFormat) > (decimal)0.001f)
            currentCircuit.component.Values[ComponentParameter.CURRENT].value = (float)Math.Round(decimal.Parse(current.text
            , System.Globalization.CultureInfo.InvariantCulture.NumberFormat), 2);

        if (decimal.Parse(resistance.text
                 , System.Globalization.CultureInfo.InvariantCulture.NumberFormat) > 0 && componentType.value != 0)
            currentCircuit.component.Values[ComponentParameter.RESISTANCE].value = (float)Math.Round(decimal.Parse(resistance.text
            , System.Globalization.CultureInfo.InvariantCulture.NumberFormat), 3);




        if (direction.State == 0)
        {
            currentCircuit.component.direction = Direction.A_to_B;
        }
        else
        {
            currentCircuit.component.direction = Direction.B_to_A;
        }



        if (selectedText.text != "")
        {
            currentCircuit.name = selectedText.text;
            currentCircuit.gameObject.name = selectedText.text;
        }

        currentCircuit.component.type = (ComponentType)componentType.value + 1;
        componentType.value = (int)currentCircuit.component.type - 1;

        currentCircuit.component.Values[ComponentParameter.VOLTAGE].hidden = voltHidden.isOn;
        currentCircuit.component.Values[ComponentParameter.CURRENT].hidden = currentHidden.isOn;
        currentCircuit.component.Values[ComponentParameter.RESISTANCE].hidden = resistanceHidden.isOn;

        //         Debug.Log(currentAvow.component.Values[ComponentParameter.VOLTAGE].hidden+" "+
        // currentAvow.component.Values[ComponentParameter.CURRENT].hidden +" "+currentAvow.component.Values[ComponentParameter.RESISTANCE ].hidden);
        newSelected(currentCircuit);
    }

    public void DestoryCurrentSelected()
    {
        if (currentCircuit)
        {
            Destroy(currentCircuit.gameObject);
            currentCircuit = null;
            Cursor.visible = true;
        }
    }

    public void btnAutoV()
    {
        autoPick(ComponentParameter.VOLTAGE);
    }

    public void btnAutoC()
    {
        autoPick(ComponentParameter.CURRENT);
    }

    public void btnAutoR()
    {
        autoPick(ComponentParameter.RESISTANCE);
    }

    public void autoPick(ComponentParameter c)
    {
        float voltageT = 0f;
        float currentT = 0f;
        float resistanceT = 0f;
        if (decimal.Parse(voltage.text
        , System.Globalization.CultureInfo.InvariantCulture.NumberFormat) > (decimal)0.0001f)
            voltageT = (float)Math.Round(decimal.Parse(voltage.text
            , System.Globalization.CultureInfo.InvariantCulture.NumberFormat), 2);

        if (decimal.Parse(current.text
        , System.Globalization.CultureInfo.InvariantCulture.NumberFormat) > (decimal)0.0001f)
            currentT = (float)Math.Round(decimal.Parse(current.text
            , System.Globalization.CultureInfo.InvariantCulture.NumberFormat), 2);

        if (decimal.Parse(resistance.text
                 , System.Globalization.CultureInfo.InvariantCulture.NumberFormat) > (decimal)0.0001f && componentType.value != 0)
            resistanceT = (float)Math.Round(decimal.Parse(resistance.text
            , System.Globalization.CultureInfo.InvariantCulture.NumberFormat), 2);
        if (voltageT > 0f && currentT > 0f && resistanceT > 0)
        {

            switch (c)
            {
                case ComponentParameter.VOLTAGE:
                    voltage.text = (resistanceT * currentT).ToString();
                    break;
                case ComponentParameter.CURRENT:
                    current.text = (voltageT / resistanceT).ToString();
                    break;
                case ComponentParameter.RESISTANCE:
                    resistance.text = Math.Round((voltageT / currentT), 2).ToString();
                    break;
                default:
                    Debug.LogError("invalide type");
                    break;
            }
            updatecomponentValues();
        }
        else
        {
        }




    }

    public void close()
    {
        currentCircuit.hideHighlight();
        currentCircuit = null;
    }

    public void delete()
    {
        currentCircuit.hideHighlight();
        currentCircuit.removeWireConnections();
        GameObject.Destroy(currentCircuit.gameObject);
        close();
    }

    public void Move()
    {
        currentCircuit.hideHighlight();
        currentCircuit.removeWireConnections();
        currentCircuit.GetComponent<CircuitClickAndDrag>().MoveStart();
        close();
    }


}


