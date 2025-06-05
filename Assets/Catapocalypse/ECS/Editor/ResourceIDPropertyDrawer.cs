using System.Linq;
using Catapocalypse.Core.Resources;
using Catapocalypse.ECS.GathererSystem;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Catapocalypse.ECS.Editor
{
    [CustomPropertyDrawer(typeof(ResourceID))]
    public class ResourceIDPropertyDrawer : PropertyDrawer
    {

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            if (property.propertyPath == "m_ResourceID")
            {
                return base.CreatePropertyGUI(property);
            }
            
            VisualElement container = new VisualElement();
            
            if (!TryGetResourceDatabase(out var database))
                return new Label("Error");
            
            var choices = database.resourcesById.Select(kvp => $"{kvp.Key.InternalID:00} - {kvp.Value.Name}").ToList();
            var dropdown = new DropdownField(nameof(ResourceID))
            {
                choices = choices,
            };
            dropdown.RegisterValueChangedCallback(evt =>
            {
                var id = evt.newValue[..2];
                var resourceID = new ResourceID(uint.Parse(id));

                property.boxedValue = resourceID;
            });
            container.Add(dropdown);
            
            return container;
        }
        
        private static bool TryGetResourceDatabase(out ResourceDatabase result)
        {
            result = null;
            
            var guids = AssetDatabase.FindAssets("t:ResourceDatabase");
            if (guids.Length == 0)
            {
                Debug.LogWarning("No ResourceDatabase asset found.");
                return false;
            }

            var path = AssetDatabase.GUIDToAssetPath(guids[0]);
            result = AssetDatabase.LoadAssetAtPath<ResourceDatabase>(path);

            if (result == null)
            {
                Debug.LogError($"Failed to load ResourceDatabase at path: {path}");
                return false;
            }

            return true;
        }
        
    }
}