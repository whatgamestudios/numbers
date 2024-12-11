using UnityEngine;
using TMPro;

public class Calculator : MonoBehaviour {

    public TextMeshProUGUI input;
    public TextMeshProUGUI target;

    public TextMeshProUGUI calculated;

    private string currentInput = "";

    private string resultText = "";

    private double result = 0.0;

    private double targetValue = 765;

    public void Start() {
        byte[] seed = SeedGen.GenerateSeed(0, 0);
        uint val = SeedGen.GetNextValue(seed, 0, 1000);
        targetValue = val;
    }


    public void OnButtonClick(string buttonText) {
        if (buttonText == "C") {
            currentInput = "";
        }
        else if (buttonText == "B") {
            if (currentInput.Length != 0) {
                currentInput = currentInput.Substring(0, currentInput.Length - 1);
            }
        }
        else {
            currentInput += buttonText;
        }
        CalculateResult();

    }

    public void CalculateResult() {
        try {
            result = System.Convert.ToDouble(new System.Data.DataTable().Compute(currentInput, ""));
            resultText =  result.ToString();
            UpdateDisplay();
        }
        catch (System.Exception ) {
            resultText = "Error";
            UpdateDisplay();
        }
    }

    public void UpdateDisplay() {
        input.text = currentInput;
        target.text = targetValue.ToString();
        calculated.text = resultText;
    }

}


