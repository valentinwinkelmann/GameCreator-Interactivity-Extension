using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using GameCreator.Runtime.Common;
using System.IO;

namespace vwgamedev.GameCreator.Editor
{
    [CustomEditor(typeof(InteractiveMonoBehaviour), true)]
    public class InteractiveMonoBehaviourEditor : UnityEditor.Editor
    {
        private Foldout basicSettingsFoldout;
        private Foldout componentSettingsFoldout;
        private Foldout basicEventsFoldout;
        private VisualElement basicSettingsContainer;
        private VisualElement componentSettingsContainer;
        private VisualElement basicEventsContainer;

        public override VisualElement CreateInspectorGUI()
        {
            // Load the UXML
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(GetUXMLPath());
            var root = visualTree.CloneTree();

            // Reference Foldouts and Containers
            basicSettingsFoldout = root.Q<Foldout>("BasicSettingsFoldout");
            componentSettingsFoldout = root.Q<Foldout>("ComponentSettingsFoldout");
            basicEventsFoldout = root.Q<Foldout>("BasicEventsFoldout");

            basicSettingsContainer = root.Q<VisualElement>("BasicSettings");
            componentSettingsContainer = root.Q<VisualElement>("ComponentSettings");
            basicEventsContainer = root.Q<VisualElement>("BasicEvents");

            // Set the text and icon of the ComponentSettingsFoldout
            AddCustomIconToFoldout(componentSettingsFoldout, "Assets/Plugins/GameCreator/Packages/Core/Editor/Gizmos/GizmoState.png"); // Specify your icon path here
            AddCustomIconToFoldout(basicEventsFoldout, "Assets/Plugins/GameCreator/Packages/Core/Editor/Gizmos/GizmoTrigger.png"); // Specify your icon path here
            AddCustomIconToFoldout(basicSettingsFoldout, "Assets/Plugins/GameCreator/Packages/Core/Editor/Gizmos/GizmoInstaller.png"); // Specify your icon path here

            // Set Basic Settings and Basic Events to be collapsed by default
            basicSettingsFoldout.value = false;
            basicEventsFoldout.value = false;

            // Fill in the Basic Settings container
            AddPropertyToContainer(basicSettingsContainer, "m_CharacterBusy");
            AddPropertyToContainer(basicSettingsContainer, "m_CharacterControllable");
            AddPropertyToContainer(basicSettingsContainer, "m_CharacterMount");
            AddPropertyToContainer(basicSettingsContainer, "m_MountObject");
            AddPropertyToContainer(basicSettingsContainer, "m_CharacterLocation");
            AddPropertyToContainer(basicSettingsContainer, "m_CharacterState");
            AddPropertyToContainer(basicSettingsContainer, "m_CharacterIKPoints");

            // Fill in the Component Settings container if there are derived properties
            bool hasDerivedProperties = DrawDerivedClassFields(componentSettingsContainer);
            componentSettingsFoldout.visible = hasDerivedProperties;

            // Fill in the Basic Events container
            basicEventsContainer.Add(new Label("Before Interact:"));
            AddPropertyToContainer(basicEventsContainer, "m_OnBeforeInteract");
            basicEventsContainer.Add(new Label("On Interact:"));
            AddPropertyToContainer(basicEventsContainer, "m_OnInteract");
            basicEventsContainer.Add(new Label("Before Stop:"));
            AddPropertyToContainer(basicEventsContainer, "m_OnBeforeStop");
            basicEventsContainer.Add(new Label("On Stop:"));
            AddPropertyToContainer(basicEventsContainer, "m_OnStop");

            return root;
        }

        private void AddCustomIconToFoldout(Foldout foldout, string iconPath)
        {
            // Access the Toggle element within the Foldout
            var toggle = foldout.Q<Toggle>();

            // Find the VisualElement inside the Toggle
            var visualElement = toggle.Q<VisualElement>(className: "unity-base-field__input");

            // Create a VisualElement for the custom icon
            var iconElement = new VisualElement();
            iconElement.AddToClassList("Foldout-Icon");

            // Load the custom image as a texture
            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);
            if (texture != null)
            {
                iconElement.style.backgroundImage = new StyleBackground(texture);
            }

            // Insert the icon between the VisualElement and the Label
            visualElement.Insert(1, iconElement);
        }

        private void AddPropertyToContainer(VisualElement container, string propertyName)
        {
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            if (property != null)
            {
                PropertyField field = new PropertyField(property);
                container.Add(field);  // Add the field to the specific container
            }
        }

        private bool DrawDerivedClassFields(VisualElement container)
        {
            SerializedProperty iterator = serializedObject.GetIterator();
            bool enterChildren = true;
            bool hasDerivedProperties = false;

            while (iterator.NextVisible(enterChildren))
            {
                // Skip the "m_Script" property (which is the script reference)
                if (iterator.propertyPath == "m_Script") continue;

                if (iterator.propertyPath != "m_CharacterBusy" &&
                    iterator.propertyPath != "m_CharacterControllable" &&
                    iterator.propertyPath != "m_CharacterMount" &&
                    iterator.propertyPath != "m_MountObject" &&
                    iterator.propertyPath != "m_CharacterLocation" &&
                    iterator.propertyPath != "m_CharacterState" &&
                    iterator.propertyPath != "m_CharacterIKPoints" &&
                    iterator.propertyPath != "m_OnBeforeInteract" &&
                    iterator.propertyPath != "m_OnInteract" &&
                    iterator.propertyPath != "m_OnBeforeStop" &&
                    iterator.propertyPath != "m_OnStop")
                {
                    PropertyField propertyField = new PropertyField(iterator);
                    container.Add(propertyField);  // Add the field to the specific container
                    hasDerivedProperties = true;
                }

                enterChildren = false;
            }

            return hasDerivedProperties;
        }

        private string GetUXMLPath()
        {
            // Assumes UXML is in the same folder as the script
            var scriptPath = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this));
            return Path.Combine(Path.GetDirectoryName(scriptPath), "InteractiveMonoBehaviourPanel.uxml");
        }
    }
}