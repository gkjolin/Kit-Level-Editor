using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kit
{
    public class KitLevelWindow : MonoBehaviour
    {
        public Light editorLight, cameraLight;
        public GameObject axisIndicator;
        Vector2 scrolling;
        KitCore core;
        public KitMovement movement;
        string searchPiece = "", searchMaterial = "";
        string[] foundPieces, foundMaterials;
        public Vector3 spawnPosition;
        public Quaternion spawnRotation;
        bool axisEnabled = false;
        public string currentPreset;

        public string SearchPiece
        {
            get
            {
                return searchPiece;
            }
            set
            {
                searchPiece = value;
                foundPieces = FindResults(searchPiece, core.kit.pieces);
            }
        }

        public string SearchMaterial
        {
            get
            {
                return searchMaterial;
            }
            set
            {
                searchMaterial = value;
                foundMaterials = FindResults(searchMaterial, core.kit.materials);
            }
        }

        public string[] FindResults(string keyword, string[] inside)
        {
            List<string> l = new List<string>();
            foreach (var p in inside)
            {
                if (p.Contains(keyword))
                {
                    l.Add(p);
                }
            }
            return l.ToArray();
        }

        void Start()
        {
            core = GetComponent<KitCore>();
        }

        private void OnGUI()
        {
            Rect windowRect = new Rect(30, 30, 220, Screen.height - 60);
            Rect boxRect = new Rect(280, 30, Screen.width - 560, 45);
            GUI.Window(1, windowRect, LevelEditorGUI, "Level Editor");
            GUI.Window(2, boxRect, LevelPropertiesGUI, "Level Properties");
        }

        void LevelEditorGUI(int windowId)
        {
            scrolling = GUILayout.BeginScrollView(scrolling);
            GUILayout.Label("Position");
            GUILayout.BeginHorizontal();
            GUILayout.Label(spawnPosition.x.ToString());
            GUILayout.Label(spawnPosition.y.ToString());
            GUILayout.Label(spawnPosition.z.ToString());
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Editor Light"))
            {
                editorLight.enabled = !editorLight.enabled;
            }
            if (GUILayout.Button("Camera Light"))
            {
                cameraLight.enabled = !cameraLight.enabled;
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Cursor"))
            {
                movement.cursor.gameObject.SetActive(!movement.cursor.gameObject.activeSelf);
            }
            if (GUILayout.Button("Axis Indicator"))
            {
                axisIndicator.SetActive(!axisIndicator.activeSelf);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Pieces");
            if (GUILayout.Button("Clear"))
            {
                SearchPiece = "";
            }
            GUILayout.EndHorizontal();
            SearchPiece = GUILayout.TextField(SearchPiece, 30);
            foreach (string foundPiece in foundPieces)
            {
                if (GUILayout.Button(foundPiece))
                {
                    SearchPiece = foundPiece;
                }
            }
            GUILayout.BeginHorizontal();
            GUILayout.Label("Materials");
            if (GUILayout.Button("Clear"))
            {
                SearchMaterial = "";
            }
            GUILayout.EndHorizontal();
            SearchMaterial = GUILayout.TextField(SearchMaterial, 30);
            foreach (string foundMaterial in foundMaterials)
            {
                if (GUILayout.Button(foundMaterial))
                {
                    SearchMaterial = foundMaterial;
                }
            }
            GUILayout.Space(10);
            if (GUILayout.Button("Add Piece"))
            {
                AddPiece();
            }
            GUILayout.Space(10);
            GUILayout.Label("Required Pieces");
            for (int i = 0; i < core.requiredPieces.Length; i++)
            {
                GUILayout.Label(core.requiredPieces[i].piece.PieceName + " " + core.requiredPieces[i].Amount + " of " + core.requiredPieces[i].maxAmount);
                //if (GUILayout.Button(core.requiredPieces[i].piece.PieceName + " " + core.requiredPieces[i].amount + " of " + core.requiredPieces[i].maxAmount))
                //{
                //    if (core.requiredPieces[i].amount < core.requiredPieces[i].maxAmount)
                //    {
                //        KitPiece instance = Instantiate(core.requiredPieces[i].piece, spawnPosition, spawnRotation) as KitPiece;
                //        instance.PieceName = core.requiredPieces[i].piece.PieceName;
                //        core.requiredPieces[i].amount++;
                //        core.pieces.Add(instance);
                //    }
                //}
            }
            GUILayout.EndScrollView();
        }

        void AddPiece()
        {
            if (isPlacementAllowed())
            {
                core.AddPiece(SearchPiece, spawnPosition, spawnRotation, SearchMaterial, "");
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                AddPiece();
            }
            if (Input.GetMouseButtonDown(2) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                RaycastHit hit;
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.transform.root.gameObject.GetComponent<KitPiece>() != null)
                    {
                        SearchPiece = hit.collider.transform.root.gameObject.GetComponent<KitPiece>().PieceName;
                        if (hit.collider.transform.root.gameObject.GetComponent<KitPiece>().Material != null)
                        {
                            SearchMaterial = hit.collider.transform.root.gameObject.GetComponent<KitPiece>().Material.name; 
                        }
                    }
                }
            }
        }

        public bool isPlacementAllowed()
        {
            if (Array.Exists(core.kit.pieces, s =>
            {
                if (s == SearchPiece)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }) && (Array.Exists(core.kit.materials, s =>
            {
                if (s == SearchMaterial)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            })))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        void LevelPropertiesGUI(int windowId)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Level Name");
            core.levelName = GUILayout.TextField(core.levelName, 30);
            if (GUILayout.Button("Save Level"))
            {
                core.GenerateLevelFile();
            }
            if (GUILayout.Button("Load Level"))
            {
                core.ReadLevelFile();
            }
            if (GUILayout.Button("Reset Level"))
            {
                core.RemoveAllPieces();
            }
            if (GUILayout.Button("Exit"))
            {
                Application.Quit();
            }
            GUILayout.EndHorizontal();
            //if (GUILayout.Button("Test"))
            //{
            //    core.AddPiece("f_1", Vector3.zero, Quaternion.identity, true, "dc");
            //}
            //GUILayout.Label("Pieces");
            //GUILayout.TextField("", 30);
            //GUILayout.Label("Materials");
            //GUILayout.TextField("", 30);
        }
    }
}
