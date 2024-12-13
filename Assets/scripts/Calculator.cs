using UnityEngine;
using TMPro;

public class Calculator : MonoBehaviour {

    public TextMeshProUGUI input;
    public TextMeshProUGUI target;

    public TextMeshProUGUI calculated;

    public TextMeshProUGUI timeToNext;

    private string currentInput = "";

    private double targetValue = 0;

    public void Start() {
        startANewDay();
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
        else if (buttonText == "=") {
            CalculateResult();
            // TODO indicate end of round
            return;
        }
        else {
            currentInput += buttonText;
        }
        input.text = currentInput;
    }

    public void CalculateResult() {
        // If the last character will make the equation invalid, remove it.
        if (currentInput.EndsWith("+") || currentInput.EndsWith("-") || 
            currentInput.EndsWith("*") || currentInput.EndsWith("/")) {
            currentInput = currentInput.Substring(0, currentInput.Length - 1);
        }
        // If the brackets don't match, add extra brackets.
        int left = 0;
        int right = 0;
        foreach (char c in currentInput) {
            if (c == '(') {
                left++;
            }
            else if (c == ')') {
                right++;
            }
        }
        for (uint i = 0; i < (left - right); i++) {
            currentInput += ')';
        }

        string resultText;
        try {
            double result = System.Convert.ToDouble(new System.Data.DataTable().Compute(currentInput, ""));
            resultText =  result.ToString();
        }
        catch (System.Exception ) {
            resultText = "Error";
        }
        calculated.text = resultText;
        input.text = currentInput;
    }   

    public void Update() {
        timeToNext.text = Timeline.TimeToNextDayStr();
    }

    private void startANewDay() {
        byte[] seed = SeedGen.GenerateSeed(0, 0);
        uint val = SeedGen.GetNextValue(seed, 0, 1000);
        targetValue = val;
        target.text = val.ToString();
        calculated.text = "";
    }

}


