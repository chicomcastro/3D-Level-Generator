using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace LevelGenerator
{

    public enum Dimension
    {
        TwoD,
        ThreeD,
    };

    [ExecuteInEditMode]
    public class LevelGenerator : MonoBehaviour
    {

        public Dimension dimension = Dimension.ThreeD;

        public Vector3 correctionFactor = Vector3.one;
        public bool generateAtStart = false;

        [Space]

        public Texture2D map;

        public ColorToPrefab[] colorMappings;
        private List<GameObject> instantiatedLevel = new List<GameObject>();
        public List<GameObject> instantiatedFloor = new List<GameObject>();

        public GameObject[] trees;
        public string treeNameForResearch = "tree";
        public float treeProb = 0.2f;

        public GameObject[] flower;
        public string flowerNameForResearch = "flower";
        public float flowerProb = 0.2f;

        public GameObject[] rock;
        public string rockNameForResearch = "rock";
        public float rockProb = 0.2f;

        public GameObject[] stone;
        public string stoneNameForResearch = "stone";
        public float stoneProb = 0.2f;

        public GameObject[] otherScenariosObject;
        public float otherProb = 0.2f;

        public float dispersionRadius = 5.0f;

        // 3D setting
        public Vector2 boardSize = new Vector2(10, 10);
        public Vector3 floorOffset = new Vector3(0, 0, 0);
        public GameObject floorPrefab;
        public float multFactor = 1;

        private void Start()
        {
            if (generateAtStart)
            {
                GenerateLevel();
            }
        }

        public void GenerateLevel()
        {
            print("Generating new level");

            GameObject level = new GameObject();
            level.name = "Level";

            for (int x = 0; x < map.width; x++)
            {
                for (int y = 0; y < map.height; y++)
                {
                    GenerateTile(x, y, level);
                }
            }
        }

        void GenerateTile(int x, int y, GameObject parent)
        {
            Color pixelColor = map.GetPixel(x, y);

            if (pixelColor.a == 0)
                return;

            foreach (ColorToPrefab colorMapping in colorMappings)
            {
                if (colorMapping.color.Equals(pixelColor))
                {
                    Vector3 position = new Vector3(x * correctionFactor.x, 0, y * correctionFactor.y);
                    GameObject gamo = Instantiate(colorMapping.prefab, position, Quaternion.identity, parent.transform);
                    if (gamo.GetComponent<Collider>() == null)
                        gamo.AddComponent<BoxCollider>();
                    instantiatedLevel.Add(gamo);
                }
            }
        }

        public void DeleteLevel()
        {
            foreach (GameObject gamo in instantiatedLevel)
            {
                DestroyImmediate(gamo);
            }

            instantiatedLevel.Clear();

            GameObject level = GameObject.Find("Level");
            if (level == null)
                return;

            SpriteRenderer[] gamos = level.GetComponentsInChildren<SpriteRenderer>(); ;

            foreach (SpriteRenderer gamo in gamos)
            {
                DestroyImmediate(gamo.gameObject);
            }

            DestroyImmediate(level);
        }

        public void GenerateFloor()
        {
            GameObject floor = new GameObject();
            floor.name = "Floor";

            for (int i = 0; i < boardSize.x; i++)
            {
                for (int j = 0; j < boardSize.y; j++)
                {
                    GameObject gamo = Instantiate(floorPrefab, (new Vector3(i, 0, j)) * multFactor + floorOffset, Quaternion.identity, floor.transform);
                    if (gamo.GetComponent<Collider>() == null)
                        gamo.AddComponent<BoxCollider>();
                    instantiatedLevel.Add(gamo);
                }
            }
        }
        public void DeleteFloor()
        {
            GameObject floor = GameObject.Find("Floor");

            if (floor == null)
                return;

            DestroyImmediate(floor);

            instantiatedFloor.Clear();
        }

        public void ResetGeneration()
        {

            foreach (GameObject gamo in instantiatedLevel)
            {
                DestroyImmediate(gamo);
            }

            instantiatedLevel.Clear();

            foreach (GameObject gamo in instantiatedFloor)
            {
                DestroyImmediate(gamo);
            }

            instantiatedFloor.Clear();
        }

        public void LookForGameObjects()
        {
            List<GameObject> found = new List<GameObject>();
            foreach (GameObject gamo in GetComponent<AssetExposer>().objectsToExpose)
            {
                if (gamo.name.ToLower().Contains(treeNameForResearch))
                {
                    found.Add(gamo);
                }
            }

            trees = ToArray(found);

            found = new List<GameObject>();
            foreach (GameObject gamo in GetComponent<AssetExposer>().objectsToExpose)
            {
                if (gamo.name.ToLower().Contains(flowerNameForResearch))
                {
                    found.Add(gamo);
                }
            }

            flower = ToArray(found);

            found = new List<GameObject>();
            foreach (GameObject gamo in GetComponent<AssetExposer>().objectsToExpose)
            {
                if (gamo.name.ToLower().Contains(rockNameForResearch))
                {
                    found.Add(gamo);
                }
            }

            rock = ToArray(found);

            found = new List<GameObject>();
            foreach (GameObject gamo in GetComponent<AssetExposer>().objectsToExpose)
            {
                if (gamo.name.ToLower().Contains(stoneNameForResearch))
                {
                    found.Add(gamo);
                }
            }

            stone = ToArray(found);
        }

        public void SearchForFloor()
        {
            GameObject floor = GameObject.Find("Floor");
            MeshRenderer[] floors = floor.GetComponentsInChildren<MeshRenderer>();

            foreach (MeshRenderer _m in floors)
            {
                instantiatedFloor.Add(_m.gameObject);
            }
        }

        public void GenerateVegetation()
        {
            foreach (GameObject gamo in instantiatedFloor)
            {
                Vector3 pos = UnityEngine.Random.insideUnitSphere * dispersionRadius;
                pos.y = 0;
                pos += gamo.transform.position - floorOffset + new Vector3(2.5f, 0f, 2.5f);

                Quaternion rot = Quaternion.Euler(0, UnityEngine.Random.Range(0f, 360f), 0);

                GameObject g;

                if (UnityEngine.Random.Range(0f, 1f) <= treeProb)
                {
                    g = Instantiate(trees[UnityEngine.Random.Range(0, trees.Length)], pos, rot, gamo.transform);
                }
                else if (UnityEngine.Random.Range(0f, 1f) <= flowerProb)
                {
                    g = Instantiate(flower[UnityEngine.Random.Range(0, flower.Length)], pos, rot, gamo.transform);
                }
                else if (UnityEngine.Random.Range(0f, 1f) <= rockProb)
                {
                    g = Instantiate(rock[UnityEngine.Random.Range(0, rock.Length)], pos, rot, gamo.transform);
                }
                else if (UnityEngine.Random.Range(0f, 1f) <= stoneProb)
                {
                    g = Instantiate(stone[UnityEngine.Random.Range(0, stone.Length)], pos, rot, gamo.transform);
                }
                else if (UnityEngine.Random.Range(0f, 1f) <= otherProb)
                {
                    g = Instantiate(otherScenariosObject[UnityEngine.Random.Range(0, otherScenariosObject.Length)], pos, rot, gamo.transform);
                }
                else
                    g = null;
                    
                if (g != null)
                    if (g.GetComponent<Collider>() == null)
                        g.AddComponent<BoxCollider>();
            }
        }

        private List<GameObject> ToList(GameObject[] _array)
        {
            List<GameObject> list = new List<GameObject>();

            foreach (GameObject gamo in _array)
            {
                list.Add(gamo);
            }

            return list;
        }
        private GameObject[] ToArray(List<GameObject> _list)
        {
            GameObject[] _array = new GameObject[_list.Count];

            for (int i = 0; i < _list.Count; i++)
            {
                _array[i] = _list[i];
            }

            return _array;
        }

        void OnValidate()
        {
            if (treeProb > 1)
            {
                treeProb = 1;
            }
            if (flowerProb > 1)
            {
                flowerProb = 1;
            }
            if (rockProb > 1)
            {
                rockProb = 1;
            }
            if (stoneProb > 1)
            {
                stoneProb = 1;
            }
            if (otherProb > 1)
            {
                otherProb = 1;
            }
        }
    }

    /* Requisites for sprites
	* 
	* Compression = none
	* Filter mode = point (no filter)
	* Advanced > Can read/write
	*
    * Remember to set alpha to 1 at color mapping's elements
    */

}

/* To do
- Place an object on terrain and normally to it
- Easy editying (click, double, positioning with mouse, undo) */
