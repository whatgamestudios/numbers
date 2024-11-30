using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class MainScene : MonoBehaviour {


    //Add logic that interacts with the UI controls in the `OnEnable` methods
    // private void OnEnable()
    // {
    //     // The UXML is already instantiated by the UIDocument component
    //     var uiDocument = GetComponent<UIDocument>();

    //     _button = uiDocument.rootVisualElement.Q("button") as Button;
    //     _toggle = uiDocument.rootVisualElement.Q("toggle") as Toggle;

    //     _button.RegisterCallback<ClickEvent>(PrintClickMessage);

    //     var _inputFields = uiDocument.rootVisualElement.Q("input-message");
    //     _inputFields.RegisterCallback<ChangeEvent<string>>(InputMessage);
    // }

    // private void OnDisable()
    // {
    //     _button.UnregisterCallback<ClickEvent>(PrintClickMessage);
    // }

    // private void PrintClickMessage(ClickEvent evt)
    // {
    //     ++_clickCount;

    //     Debug.Log($"{"button"} was clicked**" +
    //             (_toggle.value ? " Count: " + _clickCount : ""));
    // }

    // public static void InputMessage(ChangeEvent<string> evt)
    // {
    //     Debug.Log($"{evt.newValue} -> {evt.target}");
    // }



    //     // Start is called once before the first execution of Update after the MonoBehaviour is created
    // void Start()
    // {
        
    // }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }


    public void Start()
    {
        // The UXML is already instantiated by the UIDocument component
        var uiDocument = GetComponent<UIDocument>();
        VisualElement root = uiDocument.rootVisualElement;

        // Import UXML created manually.
        // var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/MainUI.uxml");
        // VisualElement labelFromUXML_uxml = visualTree.Instantiate();
        // root.Add(labelFromUXML_uxml);

        //Call the event handler
        SetupButtonHandler();
    }

    //Functions as the event handlers for your button click and number counts
    private void SetupButtonHandler()
    {
        //VisualElement root = rootVisualElement;
        var uiDocument = GetComponent<UIDocument>();
        VisualElement root = uiDocument.rootVisualElement;


        var buttons = root.Query<Button>();
        buttons.ForEach(RegisterHandler);
    }

    private void RegisterHandler(Button button)
    {
        button.RegisterCallback<ClickEvent>(PrintClickMessage);
    }

    private void PrintClickMessage(ClickEvent evt)
    {
        //VisualElement root = rootVisualElement;
        var uiDocument = GetComponent<UIDocument>();
        VisualElement root = uiDocument.rootVisualElement;

        Button button = evt.currentTarget as Button;
        Debug.Log("Button was clicked!" + button.name);
    }



}