using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;




public class SimpleCustomEditor : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    [MenuItem("Window/UI Toolkit/SimpleCustomEditor")]
    public static void ShowExample()
    {
        SimpleCustomEditor wnd = GetWindow<SimpleCustomEditor>();
        wnd.titleContent = new GUIContent("SimpleCustomEditor");
    }

    [SerializeField]
//    private VisualTreeAsset m_UXMLTree = default;

    private int m_ClickCount = 0;

    private const string m_ButtonPrefix = "button";


    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // VisualElements objects can contain other VisualElement following a tree hierarchy.
        VisualElement label = new Label("These controls were created using C# code.");
        root.Add(label);

        Button button = new Button();
        button.name = "button3";
        button.text = "This is button3.";
        root.Add(button);

        Toggle toggle = new Toggle();
        toggle.name = "toggle3";
        toggle.label = "Number?";
        root.Add(toggle);        

        // Instantiate UXML
        VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
        root.Add(labelFromUXML);

        // Import UXML created manually.
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/SimpleCustomEditor_uxml.uxml");
        VisualElement labelFromUXML_uxml = visualTree.Instantiate();
        root.Add(labelFromUXML_uxml);

        //Call the event handler
        SetupButtonHandler();
    }

    //Functions as the event handlers for your button click and number counts
    private void SetupButtonHandler()
    {
        VisualElement root = rootVisualElement;

        var buttons = root.Query<Button>();
        buttons.ForEach(RegisterHandler);
    }

    private void RegisterHandler(Button button)
    {
        button.RegisterCallback<ClickEvent>(HandleClick);
    }

    private void PrintClickMessage(ClickEvent evt)
    {
        VisualElement root = rootVisualElement;

        ++m_ClickCount;

        //Because of the names we gave the buttons and toggles, we can use the
        //button name to find the toggle name.
        Button button = evt.currentTarget as Button;
        string buttonNumber = button.name.Substring(m_ButtonPrefix.Length);
        string toggleName = "toggle" + buttonNumber;
        Toggle toggle = root.Q<Toggle>(toggleName);

        Debug.Log("Button was clicked!" +
            (toggle.value ? " Count: " + m_ClickCount : ""));
    }


    private void HandleClick(ClickEvent evt) {
        VisualElement root = rootVisualElement;
        System.Collections.Generic.IEnumerable<VisualElement> children = root.Children();
        Debug.Log("Children");
        // foreach (var element in children) {
        //     Debug.Log("_" + element.name);
        // }

        // ++m_ClickCount;

        // //Because of the names we gave the buttons and toggles, we can use the
        // //button name to find the toggle name.
        // Button button = evt.currentTarget as Button;
        // string buttonNumber = button.name.Substring(m_ButtonPrefix.Length);
        // string toggleName = "toggle" + buttonNumber;
        // Toggle toggle = root.Q<Toggle>(toggleName);

        // Debug.Log("Button was clicked!" +
        //     (toggle.value ? " Count: " + m_ClickCount : ""));
    }

}


