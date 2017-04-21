using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kit
{
    public class KitMaterialWindow : MonoBehaviour
    {
        Vector2 scrolling;
        KitCore core;
        KitPiece piece;
        KitLevelWindow levelEditor;
        bool firstLoad;

        public void Start()
        {
            piece = transform.root.gameObject.GetComponent<KitPiece>();
            core = FindObjectOfType(typeof(KitCore)) as KitCore;
            levelEditor = core.GetComponent<KitLevelWindow>();
            if (levelEditor == null)
            {
                enabled = false;
            }
            StartCoroutine(StartFirstLoad());
        }

        IEnumerator StartFirstLoad()
        {
            if (piece.isMaterialCustomizable)
            {
                var myTiling = piece.Material.mainTextureScale.x * piece.tiling;
                piece.Material.mainTextureScale = new Vector2(myTiling, myTiling);
                if (piece.preset != "" || piece.preset != null)
                {
                    piece.Material.preset = piece.preset;
                }
                while (!firstLoad)
                {
                    yield return new WaitForEndOfFrame();
                    if (!piece.Material.isProcessing)
                    {
                        firstLoad = true;
                        if (levelEditor != null)
                        {
                            core.LoadedMaterial();
                            levelEditor.movement.Repick(this);
                        }
                    }
                }
            }
            else
            {
                yield return new WaitForEndOfFrame();
                firstLoad = true;
                if (levelEditor != null)
                {
                    core.LoadedMaterial();
                    levelEditor.movement.Repick(this);
                }
            }
            yield return null;
        }

        void Reload()
        {
            if (piece.isMaterialCustomizable)
            {
                if (piece.preset != "" || piece.preset != null)
                {
                    piece.Material.preset = piece.preset;
                }
            }
        }

        void OnGUI()
        {
            if (firstLoad)
            {
                Rect windowRect = new Rect(Screen.width - 250, 30, 220, Screen.height - 60);
                if (piece.isMaterialCustomizable)
                {
                    if (piece.rend.sharedMaterial as ProceduralMaterial)
                    {
                        GUI.Window(0, windowRect, ProceduralPropertiesGUI, piece.rend.sharedMaterial.name + gameObject.GetInstanceID().ToString());
                    }
                }
                else
                {
                    GUI.Window(3, windowRect, MaterialPropertiesGUI, piece.rend.sharedMaterial.name + gameObject.GetInstanceID().ToString());
                }
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                core.RemovePiece(piece);
            }
        }

        void MaterialPropertiesGUI(int windowId)
        {
            scrolling = GUILayout.BeginScrollView(scrolling);
            if (GUILayout.Button("Remove Piece"))
            {
                core.RemovePiece(piece);
            }
            GUILayout.EndScrollView();
        }

        void ProceduralPropertiesGUI(int windowId)
        {
            scrolling = GUILayout.BeginScrollView(scrolling);
            if (GUILayout.Button("Copy Preset"))
            {
                levelEditor.currentPreset = piece.Material.preset;
            }
            if (GUILayout.Button("Paste Preset"))
            {
                piece.Material.preset = levelEditor.currentPreset;
            }
            GUILayout.Space(10);
            if (GUILayout.Button("Save Changes"))
            {
                piece.preset = piece.Material.preset;
            }
            if (GUILayout.Button("Revert Changes"))
            {
                Reload();
            }
            GUILayout.Space(10);
            if (GUILayout.Button("Remove Piece"))
            {
                core.RemovePiece(piece);
            }
            GUILayout.Space(10);
            ProceduralPropertyDescription[] inputs = piece.Material.GetProceduralPropertyDescriptions();
            int i = 0;
            while (i < inputs.Length)
            {
                ProceduralPropertyDescription input = inputs[i];
                ProceduralPropertyType type = input.type;
                if (type == ProceduralPropertyType.Boolean)
                {
                    bool inputBool = piece.Material.GetProceduralBoolean(input.name);
                    bool oldInputBool = inputBool;
                    inputBool = GUILayout.Toggle(inputBool, input.name);
                    if (inputBool != oldInputBool)
                        piece.Material.SetProceduralBoolean(input.name, inputBool);

                }
                else if (type == ProceduralPropertyType.Float)
                {
                    if (input.hasRange)
                    {
                        float inputFloat = piece.Material.GetProceduralFloat(input.name);
                        float oldInputFloat = inputFloat;
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(input.name);
                        GUILayout.FlexibleSpace();
                        GUILayout.Label(inputFloat.ToString());
                        GUILayout.EndHorizontal();
                        inputFloat = GUILayout.HorizontalSlider(inputFloat, input.minimum, input.maximum);
                        if (inputFloat != oldInputFloat)
                            piece.Material.SetProceduralFloat(input.name, inputFloat);

                    }
                }
                else if (type == ProceduralPropertyType.Vector2 || type == ProceduralPropertyType.Vector3 || type == ProceduralPropertyType.Vector4)
                {
                    if (input.hasRange)
                    {
                        GUILayout.Label(input.name);
                        int vectorComponentAmount = 4;
                        if (type == ProceduralPropertyType.Vector2)
                            vectorComponentAmount = 2;

                        if (type == ProceduralPropertyType.Vector3)
                            vectorComponentAmount = 3;

                        Vector4 inputVector = piece.Material.GetProceduralVector(input.name);
                        Vector4 oldInputVector = inputVector;
                        int c = 0;
                        while (c < vectorComponentAmount)
                        {
                            inputVector[c] = GUILayout.HorizontalSlider(inputVector[c], input.minimum, input.maximum);
                            c++;
                        }
                        if (inputVector != oldInputVector)
                            piece.Material.SetProceduralVector(input.name, inputVector);

                    }
                }
                else if (type == ProceduralPropertyType.Color3 || type == ProceduralPropertyType.Color4)
                {
                    GUILayout.Label(input.name);
                    int colorComponentAmount = ((type == ProceduralPropertyType.Color3) ? 3 : 4);
                    Color colorInput = piece.Material.GetProceduralColor(input.name);
                    Color oldColorInput = colorInput;
                    int d = 0;
                    while (d < colorComponentAmount)
                    {
                        GUILayout.BeginHorizontal();
                        var dd = "R";//ed Channel";
                        if (d == 1)
                        {
                            dd = "G";//reen Channel";
                        }
                        else if (d == 2)
                        {
                            dd = "B";//lue Channel";
                        }
                        else if (d == 3)
                        {
                            dd = "A";//lpha Channel";
                        }
                        GUILayout.Label(dd);
                        GUILayout.FlexibleSpace();
                        var s = Mathf.Round(float.Parse(colorInput[d].ToString()) * 255);
                        GUILayout.Label(s.ToString());
                        GUILayout.EndHorizontal();
                        colorInput[d] = GUILayout.HorizontalSlider(colorInput[d], 0, 1);
                        d++;
                    }
                    if (colorInput != oldColorInput)
                    {
                        piece.Material.SetProceduralColor(input.name, colorInput);
                    }
                }
                else if (type == ProceduralPropertyType.Enum)
                {
                    GUILayout.Label(input.name);
                    int enumInput = piece.Material.GetProceduralEnum(input.name);
                    int oldEnumInput = enumInput;
                    string[] enumOptions = input.enumOptions;
                    enumInput = GUILayout.SelectionGrid(enumInput, enumOptions, 1);
                    if (enumInput != oldEnumInput)
                    {
                        piece.Material.SetProceduralEnum(input.name, enumInput);
                    }
                }
                i++;
            }
            piece.Material.RebuildTextures();
            GUILayout.EndScrollView();
        }
    }
}
