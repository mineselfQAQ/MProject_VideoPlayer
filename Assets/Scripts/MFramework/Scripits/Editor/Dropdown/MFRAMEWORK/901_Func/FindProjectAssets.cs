using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MFramework
{
    public class FindProjectAssets : EditorWindow
    {
        public Object obj = null;

        private List<OutputInfo> outputList = new List<OutputInfo>();
        private ObjectMode objectMode = ObjectMode.None;
        private bool isStart;
        private bool isCurrentScene;
        private bool isAllScenes;

        private System.Type scriptType = null;
        private Shader shader;
        private Material material;
        private Texture texture;
        private Sprite sprite;

        private Scene defaultScene;
        private SceneAsset s = null;

        private Vector2 scrollPos;

        private List<string> scenePaths = new List<string>();
        private List<string> sceneNames = new List<string>();

        [MenuItem("MFramework/Find Project Assets", false, 903)]
        public static void Init()
        {
            EditorWindow window = GetWindow<FindProjectAssets>(true, "FindProjectAssets", false);
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("Ѱ��Project�����ڳ����еĹ���", GetStyle(20, TextAnchor.MiddleCenter, EditorGUIUtility.isProSkin ? Color.white : Color.black));
                EditorGUILayout.Space(5);
                obj = EditorGUILayout.ObjectField("������Project�е�Object��", obj, typeof(Object), false);
                if (!isStart)
                {
                    //��ȡ��ǰ�Ѽ��س���(��ԭ��)
                    for (int n = 0; n < SceneManager.sceneCount; n++)
                    {
                        Scene scene = SceneManager.GetSceneAt(n);
                        scenePaths.Add(scene.path);
                        sceneNames.Add(scene.name);
                    }

                    //���ܣ�ѡ������ʱֱ��"����ObjectField"
                    Object activeObj = Selection.activeObject;
                    if (activeObj && (AssetDatabase.IsForeignAsset(activeObj) || AssetDatabase.IsNativeAsset(activeObj)))
                    {
                        obj = activeObj;
                    }

                    isStart = true;
                }
            }
            EditorGUILayout.EndVertical();



            //������ť
            EditorGUILayout.BeginHorizontal();
            //��ť1���ڵ�ǰ������Ѱ��
            if (GUILayout.Button("�ڵ�ǰ������Ѱ��"))
            {
                EditorGUILayout.BeginVertical();
                {
                    //��ʼ������
                    isCurrentScene = true;
                    isAllScenes = false;
                    outputList.Clear();

                    //Ѱ�Ҳ���
                    if (!obj) return;

                    GetFieldAndMode();

                    //���Ϊ������ģʽ��defaultScene��ȷ����
                    if (SceneManager.sceneCount == 1)
                    {
                        defaultScene = SceneManager.GetActiveScene();
                        FindAssets(false);
                    }
                    //���Ϊ�ೡ��ģʽ����Ҫ���´�ÿһ��������Ѱ��
                    //(��ΪѰ��ʹ�õ�FindObjectsOfType()��֧�ֶೡ���ģ�����gameObjectIndex��ͻ)
                    else
                    {
                        foreach (string scenePath in scenePaths)
                        {
                            defaultScene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                            FindAssets(true);
                        }
                    }

                    //��������---��ԭ��ʼ����
                    RestoreScene();
                }
                EditorGUILayout.EndVertical();
            }
            //��ť2�������г�����Ѱ��
            if (GUILayout.Button("�����г�����Ѱ��"))
            {
                EditorGUILayout.BeginVertical();
                {
                    //��ʼ������
                    isAllScenes = true;
                    isCurrentScene = false;
                    outputList.Clear();

                    //Ѱ�Ҳ���
                    GetFieldAndMode();

                    string[] guids = AssetDatabase.FindAssets("t:Scene");
                    //���δ�����Project�еĳ�������
                    foreach (string guid in guids)
                    {
                        string path = AssetDatabase.GUIDToAssetPath(guid);
                        s = (SceneAsset)AssetDatabase.LoadAssetAtPath(path, typeof(SceneAsset));
                        Scene scene = EditorSceneManager.OpenScene(path);
                        defaultScene = scene;

                        FindAssets(true);
                    }

                    //��������---��ԭ��ʼ����
                    RestoreScene();
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();



            //������
            EditorGUILayout.BeginVertical();
            {
                if (isCurrentScene)
                {
                    DrawTitle();
                    DrawContent_CurrentScene();
                }
                else if (isAllScenes)
                {
                    DrawTitle();
                    DrawContent_AllScenes();
                }
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// ��ȡһЩ��Ҫ����Ϣ��Ҳ�����ֶ��Լ�ObjectModeö��
        /// </summary>
        private void GetFieldAndMode()
        {
            //�ű����
            MonoScript ms = obj as MonoScript;
            if (ms)
            {
                objectMode = ObjectMode.Script;
                scriptType = ms.GetClass();
            }
            //Prefab���
            if (PrefabUtility.IsPartOfAnyPrefab(obj))
            {
                objectMode = ObjectMode.Prefab;
            }
            //Shader���
            shader = obj as Shader;
            if (shader)
            {
                objectMode = ObjectMode.Shader;
            }
            //Material���
            material = obj as Material;
            if (material)
            {
                objectMode = ObjectMode.Material;
            }
            //Texture���
            texture = obj as Texture;
            if (texture)
            {
                objectMode = ObjectMode.Texture;
            }
            //Sprite���
            sprite = obj as Sprite;
            if (sprite)
            {
                objectMode = ObjectMode.Sprite;
            }
        }
        /// <summary>
        /// ��ȡ��Դ��ÿ�ҵ�һ����ȷ����Դ�ͻὫ�����outputList��
        /// </summary>
        private void FindAssets(bool isOutputScene)
        {
            switch (objectMode)
            {
                case ObjectMode.Prefab:
                    foreach (GameObject gameObject in FindObjectsOfType<GameObject>(true))
                    {
                        if (PrefabUtility.IsPartOfAnyPrefab(gameObject))
                        {
                            GetOutputInfo("��������" + gameObject.name, gameObject.name,
                                GetHierarchyIndex(gameObject), defaultScene.name, s, isOutputScene);
                        }
                    }
                    break;
                case ObjectMode.Script:
                    foreach (GameObject gameObject in FindObjectsOfType<GameObject>(true))
                    {
                        Component[] components = gameObject.GetComponents(typeof(Component));
                        foreach (Component component in components)
                        {
                            if (component == null) return;//�ű�Missing���

                            if (component.GetType() == scriptType)
                            {
                                GetOutputInfo("�������壺" + gameObject.name, gameObject.name,
                                    GetHierarchyIndex(gameObject), defaultScene.name, s, isOutputScene);
                            }
                        }
                    }
                    break;
                case ObjectMode.Shader:
                    foreach (GameObject gameObject in FindObjectsOfType<GameObject>(true))
                    {
                        MeshRenderer mr = gameObject.GetComponent<MeshRenderer>();
                        if (!mr) break;
                        Material[] mats = mr.sharedMaterials;
                        foreach (var mat in mats)
                        {
                            if (mat.shader == shader)
                            {
                                GetOutputInfo("�������壺" + gameObject.name + " ��������" + mat.name,
                                    gameObject.name, GetHierarchyIndex(gameObject), defaultScene.name, s, isOutputScene);
                            }
                        }
                    }
                    break;
                case ObjectMode.Material:
                    foreach (GameObject gameObject in FindObjectsOfType<GameObject>(true))
                    {
                        MeshRenderer mr = gameObject.GetComponent<MeshRenderer>();
                        if (!mr) break;
                        Material[] mats = mr.sharedMaterials;
                        foreach (var mat in mats)
                        {
                            if (mat == material)
                            {
                                GetOutputInfo("�������壺" + gameObject.name, gameObject.name,
                                    GetHierarchyIndex(gameObject), defaultScene.name, s, isOutputScene);
                            }
                        }
                    }
                    break;
                case ObjectMode.Texture://����Texture��˵��̫�����ˣ�ֻд��ĳЩ���
                    foreach (GameObject gameObject in FindObjectsOfType<GameObject>(true))
                    {
                        Component[] components = gameObject.GetComponents(typeof(Component));
                        foreach (Component component in components)
                        {
                            RawImage rawImage;
                            Image image;
                            SpriteRenderer spriteRenderer = null;
                            if (rawImage = component as RawImage)
                            {
                                if (rawImage.texture == texture)
                                {
                                    GetOutputInfo("�������壺" + gameObject.name, gameObject.name,
                                        GetHierarchyIndex(gameObject), defaultScene.name, s, isOutputScene);
                                }
                            }
                            else if (image = component as Image)
                            {
                                if (image.sprite.texture == texture)
                                {
                                    GetOutputInfo("�������壺" + gameObject.name, gameObject.name,
                                        GetHierarchyIndex(gameObject), defaultScene.name, s, isOutputScene);
                                }
                            }
                            else if (spriteRenderer = component as SpriteRenderer)
                            {
                                if (spriteRenderer.sprite.texture == texture)
                                {
                                    GetOutputInfo("�������壺" + gameObject.name, gameObject.name,
                                        GetHierarchyIndex(gameObject), defaultScene.name, s, isOutputScene);
                                }
                            }
                        }
                    }
                    break;
                case ObjectMode.Sprite://����Sprite��˵��̫�����ˣ�ֻд��ĳЩ���
                    foreach (GameObject gameObject in FindObjectsOfType<GameObject>(true))
                    {
                        Component[] components = gameObject.GetComponents(typeof(Component));
                        foreach (Component component in components)
                        {
                            Image image;
                            SpriteRenderer spriteRenderer = null;
                            if (image = component as Image)
                            {
                                if (image.sprite == sprite)
                                {
                                    GetOutputInfo("�������壺" + gameObject.name, gameObject.name,
                                        GetHierarchyIndex(gameObject), defaultScene.name, s, isOutputScene);
                                }
                            }
                            else if (spriteRenderer = component as SpriteRenderer)
                            {
                                if (spriteRenderer.sprite == sprite)
                                {
                                    GetOutputInfo("�������壺" + gameObject.name, gameObject.name,
                                        GetHierarchyIndex(gameObject), defaultScene.name, s, isOutputScene);
                                }
                            }
                        }
                    }
                    break;
            }
        }

        private void DrawTitle()
        {
            if (outputList.Count == 0)
            {
                EditorGUILayout.Space(3);
                EditorGUILayout.LabelField("δ�ҵ��κ����ݣ�����");
                return;
            }
            else
            {
                EditorGUILayout.Space(3);
                EditorGUILayout.LabelField("��Ѱ�����");
                EditorGUILayout.LabelField($"���ҵ�{outputList.Count}��");
                EditorGUILayout.Space(3);
            }
        }
        private void DrawContent_CurrentScene()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            foreach (OutputInfo output in outputList)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField(output.info);
                    if (GUILayout.Button("��λ"))
                    {
                        Ping(output);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }
        private void DrawContent_AllScenes()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            foreach (OutputInfo output in outputList)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField(output.info);
                    if (GUILayout.Button("��λ"))
                    {
                        if (!Ping(output))
                        {
                            EditorGUIUtility.PingObject(output.sceneAsset);
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        /// �������
        /// </summary>
        private bool Ping(OutputInfo output)
        {
            int x = (int)output.gameObjectIndex.x;
            int y = (int)output.gameObjectIndex.y;
            int z = (int)output.gameObjectIndex.z;

            //����ÿһ��Hierarchy�³������ҵ���Ӧ�������ڳ������ҵ���Ӧ����
            for (int n = 0; n < SceneManager.sceneCount; n++)
            {
                Scene scene = SceneManager.GetSceneAt(n);
                if (scene.name == output.sceneName)
                {
                    GameObject[] gos = scene.GetRootGameObjects();
                    foreach (GameObject go in gos)
                    {
                        Transform pingObject = go.transform;
                        if (x == go.transform.GetSiblingIndex())
                        {
                            for (int i = 0; i < y - 1; i++)
                            {
                                pingObject = pingObject.GetChild(0);
                            }
                            if (y != 0) pingObject = pingObject.GetChild(z);

                            EditorGUIUtility.PingObject(pingObject);//����Transformһ����ָ��Hierarchy����
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// ��ԭ����
        /// </summary>
        private void RestoreScene()
        {
            bool isLoadFirstScene = true;
            foreach (string scenePath in scenePaths)
            {
                if (isLoadFirstScene)
                {
                    EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                    isLoadFirstScene = false;
                }
                else
                {
                    EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
                }
            }
        }

        /// <summary>
        /// ��ȡ��������Ӧ������
        /// </summary>
        //֮������ҪVector3�������ģ�
        //�����ȵ�ȷ���ڼ�������---x
        //Ȼ����Ҫ֪�����ڸ������µڼ���---y
        //�����Ҫ֪��������һ���µڼ���---z
        private Vector3 GetHierarchyIndex(GameObject go)
        {
            Transform t = go.transform;

            //��ȡy
            Transform child = t;
            Transform parent = t.parent;
            int indexX = 0;
            while (parent != null)
            {
                child = parent;
                parent = child.parent;
                indexX++;
            }
            //���yΪ0������ζ������root�ڵ㣬zһ��Ϊ0(����z��õ�x�Ľ��)
            if (indexX == 0)
            {
                return new Vector3(child.GetSiblingIndex(), indexX, 0);
            }
            return new Vector3(child.GetSiblingIndex(), indexX, t.GetSiblingIndex());
        }
        private void GetOutputInfo(string info, string name, Vector2 index, string sceneName, SceneAsset sceneAsset, bool isOutputScene)
        {
            OutputInfo output = new OutputInfo();

            output.info = info;
            output.gameObjectName = name;
            output.gameObjectIndex = index;
            output.sceneName = sceneName;
            if (sceneAsset) output.sceneAsset = sceneAsset;

            if (sceneName != null && sceneName != "SampleScene" && isOutputScene)
            {
                output.info += "\t����������" + sceneName;
            }

            outputList.Add(output);
        }

        private GUIStyle GetStyle(int size, TextAnchor mode, Color color)
        {
            GUIStyle style = new GUIStyle();

            style.fontSize = size;
            style.alignment = mode;
            style.normal.textColor = color;

            return style;
        }
        private GUIStyle GetStyle(int size)
        {
            GUIStyle style = new GUIStyle();

            style.fontSize = size;

            return style;
        }
    }


    /// <summary>
    /// ����ṹ�����а�������Ҫ����Ϣ
    /// </summary>
    //info---������ݣ�������Ϊ����ʾ��Ϣ
    //sceneAsset---��������Ӧ��Project����Դ
    //֮���Ի�ȡ���¼���������GameObject��Scene����Ϊ�����л��ᵼ�����ö�ʧ
    //gameObjectName---����Ҫ�����������
    //gameObjectIndex---����Ҫ�����������
    //sceneName---�������ڳ�����
    public class OutputInfo
    {
        public string info;
        public string gameObjectName;
        public Vector3 gameObjectIndex;
        public string sceneName;
        public SceneAsset sceneAsset;
    }

    //Ҫ֪������Щ���ݿ��Ա����룬ֻҪ��Project�е����ݻ����϶��ǿ��Է���ģ��������෱�࣬��Ҫ��ѡ����Ҫ��һЩ��
    //1.Script
    //2.Shader
    //3.Material
    //4.Prefab
    //5.Texture
    //6.Sprite
    //7.Model(û���������е��鷳)
    public enum ObjectMode
    {
        Script,
        Prefab,
        Shader,
        Material,
        Texture,
        Sprite,
        None
    }
}