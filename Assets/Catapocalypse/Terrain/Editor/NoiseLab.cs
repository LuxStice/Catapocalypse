using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Catapocalypse.Terrain
{
    public class NoiseLab : EditorWindow
    {
        public ComputeShader shader;
        
        public VisualTreeAsset Template;
        public VisualTreeAsset TreeItemTemplate;

        private ObjectField objectField;
        private VisualElement imageContainer;
        private ListView listView;

        private VisualElement layerSettingsContainer;
        private TextField nameField;
        private Vector2Field scaleField;
        private Vector2Field offsetField;
        private VisualElement imagePreviewField;
        
        private NoiseCollection currentNoiseCollection;

        [MenuItem("Catapocalypse/Noise Lab")]
        private static void ShowWindow()
        {
            var window = GetWindow<NoiseLab>();
            window.titleContent = new GUIContent("Noise Lab");
            window.Show();
        }

        private void CreateGUI()
        {
            var labContainer = Template.Instantiate();
            rootVisualElement.Add(labContainer);

            labContainer.style.flexGrow = 1;

            listView = labContainer.Q<ListView>();
            
            listView.itemsSourceChanged += SourceChanged;

            listView.selectedIndicesChanged += OnSelectedChanged;

            objectField = labContainer.Q<ObjectField>();
            listView.style.display = objectField.value is null ? DisplayStyle.None : DisplayStyle.Flex;
            objectField.RegisterValueChangedCallback(OnObjectSelected);

            Action<VisualElement, int> bindItem = (ve, index) =>
            {
                var layer = currentNoiseCollection.Layers[index];
                ve.Q<Label>("layerName").text = layer.name;
            };
            listView.bindItem = bindItem;
            
            layerSettingsContainer = rootVisualElement.Q<VisualElement>("layerSettingsContainer");
            layerSettingsContainer.style.display = objectField.value is null ? DisplayStyle.None : DisplayStyle.Flex;

            nameField = layerSettingsContainer.Q<TextField>("nameField");
            scaleField = layerSettingsContainer.Q<Vector2Field>("scaleField");
            offsetField = layerSettingsContainer.Q<Vector2Field>("offsetField");
            imagePreviewField = rootVisualElement.Q<VisualElement>("imageField");
        }

        void SourceChanged()
        {
            listView.RefreshItems();
        }

        private int selectedIndex = -1;
        
        void OnSelectedChanged(IEnumerable<int> obj)
        {
            if(obj.Count() > 1) throw new OverflowException("Only one selected object can be selected.");

            if (!obj.Any())
            {
                layerSettingsContainer.style.display = DisplayStyle.None;
                return;
            }
            else layerSettingsContainer.style.display = DisplayStyle.Flex;
            selectedIndex = obj.First();
            var layer = currentNoiseCollection.Layers[selectedIndex];
            
            nameField.SetValueWithoutNotify(layer.name);
            nameField.RegisterValueChangedCallback((evt) =>
            {
                layer.name = evt.newValue;
                currentNoiseCollection.Layers[selectedIndex] = layer;
            });
            
            scaleField.SetValueWithoutNotify(layer.data.scale);
            scaleField.RegisterValueChangedCallback((evt) =>
            {
                layer.data.scale = evt.newValue;
                currentNoiseCollection.Layers[selectedIndex] = layer;
                GeneratePreview();
            });
            
            offsetField.SetValueWithoutNotify(layer.data.offset);
            offsetField.RegisterValueChangedCallback((evt) =>
            {
                layer.data.offset = evt.newValue;
                currentNoiseCollection.Layers[selectedIndex] = layer;
                GeneratePreview();
            });
            
            GeneratePreview();
        }
        
        void OnObjectSelected(ChangeEvent<Object> evt)
        {
            var newSettings = evt.newValue as NoiseCollection;
            
            if(newSettings is null) return;
            
            currentNoiseCollection = newSettings;

            listView.style.display = newSettings is null ? DisplayStyle.None : DisplayStyle.Flex;
            layerSettingsContainer.style.display = objectField.value is null ? DisplayStyle.None : DisplayStyle.Flex;

            listView.itemsSource = newSettings.Layers;
            if (newSettings.Layers.Count > 0)
                listView.selectedIndex = 0;
        }

        private void GeneratePreview()
        {
            var generator = new NoiseGenerator(shader, currentNoiseCollection);

            imagePreviewField.style.backgroundImage = new StyleBackground(generator.GenerateNoise());
        }
    }
}