using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using System.Linq;
namespace Gridly.Internal
{
    public class TermEditor : GridlyEditor
    {
        Vector3 m_Scroll = new Vector3();
        public static TermEditor window;

        static int gridSelectIndex;
        static int pathSelectedIndex;

        static Grid grid
        {
            get
            {
                try
                {
                    return Project.singleton.grids[gridSelectIndex];
                }
                catch
                {
                    gridSelectIndex = 0;
                    Refesh();
                    return Project.singleton.grids[gridSelectIndex];
                }
            }
        }


        [MenuItem("Tools/Gridly/String Editor", false, 0)]
        private static void Init()
        {
            if (Project.singleton.grids.Count == 0)
            {
                "you need to setup data before using this feature".Print();
                return;
            }

            window = (TermEditor)GetWindow(typeof(TermEditor), false, "String Editor - " + GridlyInfo.ver);
            Vector2 vector2 = new Vector2(400, 400);
            window.minSize = vector2;
            PopulateDataSendList();
            Refesh();
            window.Show();
        }

        public static void RepaintThis()
        {
            if (window != null)
            {
                window.Repaint();

            }
        }

        public static void PopulateDataSendList()
        {

            Project.singleton.DataToSend.Clear();
            Project.singleton.DataToSend.Add("Source text");
            Project.singleton.DataToSend.Add("Source text and screenshots");

            foreach (LangSupport lang in Project.singleton.langSupports)
            {
                Project.singleton.DataToSend.Add(lang.name);
            }
        }


        static string gridSelect = "";
        static string pathSelect = "";
        static List<Record> records;
        static GridlyArrData popupData = new GridlyArrData();
        static string[] paths;
        public static void Refesh()
        {

            OnEnableEditor();
            init = true;
            popupData.RefeshAll(gridSelect, null);
            RefeshList();
            paths = getSceneList().ToArray();
            Debug.Log("レコード数 " + records.Count);
        }
        static void RefeshList()
        {
            if (popupData.indexGrid == -1)
                return;
            records = getRecord(grid);
            TermListLegth = records.Count;

        }

        private static List<string> getSceneList()
        {
            List<string> list = new List<string>();
            list.Add("");
            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                //Debug.Log(scene.path);
                //Debug.Log("loading level");
                list.Add(Path.GetFileNameWithoutExtension(scene.path));

            }
            return list;
        }

        static float mRowSize = 40;
        static int TermListLegth = 0;
        float scrollHeight;
        string selectRecordID;
        float YPos;
        float ScrolSizeY => 280 + position.height - 415;
        static string search;
        float res = 0;
        int countReset = 0;
        static List<Record> getRecord(Grid grid)
        {
            if (!string.IsNullOrEmpty(search))
            {
                return grid.records.FindAll(x => x.recordID.ToLower().Contains(search.ToLower()) |
                    x.columns.Find(x => x.columnID == UserData.singleton.mainLangEditor.ToString() & x.text.ToLower().Contains(search.ToLower())) != null
                );
            }
            return grid.records;
        }
        static bool init;

        public void OnGUI()
        {

            if (!init)
            {
                Refesh();
            }

            GUILayout.Space(20);

            #region select data



            EditorGUI.BeginChangeCheck();
            gridSelectIndex = EditorGUILayout.Popup("Grid", popupData.indexGrid, popupData.gridArr);
            gridSelect = popupData.gridArr[gridSelectIndex];
            if (EditorGUI.EndChangeCheck())
            {
                m_Scroll = Vector3.zero;
                UserData.singleton.Save();
                Refesh();
                return;
            }


            #endregion
            GUILayout.Space(10);

            #region Search
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();

            EditorGUI.BeginChangeCheck();
            search = GUILayout.TextField(search, GUI.skin.GetStyle("ToolbarSeachTextField"));
            if (EditorGUI.EndChangeCheck())
            {
                RefeshList();
                //selectRecordID = "";
                m_Scroll = Vector3.zero;
                //showCreateKey = false;
            }
            GUILayout.Label("Search", GUILayout.Width(60));

            if (GUILayout.Button(new GUIContent() { text = "▲", tooltip = "Scroll to the top" }, GUILayout.Width(35)))
            {
                m_Scroll.y = 0;
            }

            if (GUILayout.Button(new GUIContent() { text = "▼", tooltip = "Scroll to the bottom" }, GUILayout.Width(35)))
            {
                m_Scroll.y = TermListLegth * mRowSize;
                showCreateKey = true;
                OnGUI();
            }

            GUILayout.EndHorizontal();

            #endregion

            GUILayout.Space(5);


            m_Scroll = GUILayout.BeginScrollView(m_Scroll, TextStyle, GUILayout.MinHeight(ScrolSizeY), GUILayout.ExpandHeight(true));


            if (Event.current != null && Event.current.type == EventType.Layout)
                scrollHeight = m_Scroll.y;

            #region draw record

            YPos = 0;

            int nDraw = 0;
            float nSkipStart = 0;
            bool spaceStart = false;

            for (int i = 0; i < TermListLegth; i++)
            {


                if (YPos < scrollHeight - mRowSize && !spaceStart)
                {
                    spaceStart = true;
                    nSkipStart = ((scrollHeight - mRowSize) / mRowSize);
                    YPos = scrollHeight - mRowSize;
                    i = (int)nSkipStart;
                    GUILayout.Space(nSkipStart * mRowSize);
                }

                GUILayout.Space(mRowSize);
                bool selected = false;
                float befforY = YPos;
                try
                {
                    DrawRecord(i + 1, records[i], ref selected);
                }
                catch
                {
                    break;
                }
                YPos += mRowSize;


                if (selected)
                {
                    if (countReset > 0)
                    {
                        res = GUILayoutUtility.GetLastRect().yMin - befforY;
                        countReset -= 1;
                    }

                    YPos += res;


                }

                nDraw++;

                //cut
                if (YPos > scrollHeight + ScrolSizeY)
                {
                    break;
                }


            }
            GUILayout.Space((TermListLegth + 1 - (nDraw + nSkipStart)) * (mRowSize));
            DrawAddRecord();

            #endregion
            GUILayout.EndScrollView();


            #region Button

            GUILayout.Space(5);
            GUILayout.BeginHorizontal(GUILayout.ExpandHeight(true));

            if (GUILayout.Button(new GUIContent() { text = "選択したグリッドをプル", tooltip = "Gridlyから選択したグリッドのデータをプル" }))
            {
                if (EditorUtility.DisplayDialog("グリッドのインポート確認", "Are you sure you want to import Grid from Gridly?. It will overwrite the old data including translations.", "Yes", "Cancel"))
                {
                    popupData.grid.records.Clear();
                    Refesh();
                    RepaintThis();

                    GridlyFunctionEditor.editor.doneOneProcess += Refesh;
                    GridlyFunctionEditor.editor.doneOneProcess += RepaintThis;

                    GridlyFunctionEditor.editor.RefeshDowloadTotal();
                    GridlyFunctionEditor.editor.SetupRecords(popupData.grid, 0);
                }
            }

            if (GUILayout.Button(new GUIContent() { text = "すべてのグリッドをプル", tooltip = "Gridlyのすべてのデータをし、Unityのグリッドを更新" }))
            {
                if (EditorUtility.DisplayDialog("エクスポート確認", "本当に Gridly からすべてのデータをインポートしますか？ この操作は、翻訳を含む古いデータを上書きします。", "はい", "キャンセル"))
                {
                    GridlyFunctionEditor.editor.doneOneProcess += Refesh;
                    GridlyFunctionEditor.editor.doneOneProcess += RepaintThis;
                    GridlyFunctionEditor.editor.SetupDatabases();
                }
            }


            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("言語スクリーンショット / 全ターゲット言語スクリーンショット"), Project.singleton.DataToSendSelectedItems.Contains("全ターゲット言語スクリーンショット"), OnColorSelected, "全ターゲット言語スクリーンショット");

            foreach (string data in Project.singleton.DataToSendSelectedItems.ToList())
            {
                if (!Project.singleton.DataToSend.Contains(data))
                {
                    Project.singleton.DataToSendSelectedItems.Remove(data);
                }
            }

            foreach (string item in Project.singleton.DataToSend)
            {
                if (item.Length > 4)
                {
                    menu.AddItem(new GUIContent(item), Project.singleton.DataToSendSelectedItems.Contains(item), OnColorSelected, item);
                }
                else
                {
                    if (item != UserData.singleton.mainLangEditor.ToString())
                    {
                        menu.AddItem(new GUIContent("言語スクリーンショット /" + item), Project.singleton.DataToSendSelectedItems.Contains(item), OnColorSelected, item);
                    }
                }
            }




            if (EditorGUILayout.DropdownButton(new GUIContent() { text = "プッシュオプション", tooltip = "Gridly に送信するスクリーンショットの言語を選択" }, 0, EditorStyles.miniButton))
            {
                menu.ShowAsContext();
            }


            if (GUILayout.Button(new GUIContent() { text = "データをプッシュ", tooltip = "ソース文字列と選択された言語のスクリーンショットを Gridly にプッシュ" }))
            {
                string msg = "以下の設定でデータをプッシュしようとしています。\n";
                foreach (string data in Project.singleton.DataToSendSelectedItems)
                {
                    if (data.Length == 4)
                    {
                        if (!Project.singleton.DataToSendSelectedItems.Contains("全ターゲット言語スクリーンショット"))
                            msg += "- " + data + " screenshots" + "\n";
                    }
                    else
                    {
                        msg += "- " + data + "\n";
                    }
                }
                if (EditorUtility.DisplayDialog("Gridly にデータをプッシュ", msg, "はい", "キャンセル"))
                {
                    GridlyFunctionEditor.editor.AddUpdateRecordAll(popupData.grid.records, popupData.grid.choesenViewID, false, true);
                }
            }

            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            GUILayout.FlexibleSpace();
            Project.singleton.SendIfChanged = GUILayout.Toggle(Project.singleton.SendIfChanged, new GUIContent { text = "変更されたレコードのみプッシュ", tooltip = "Unity で変更されたレコードのみを、Gridly に送信" });
            if (EditorGUI.EndChangeCheck())
                Project.singleton.setDirty();
            GUILayout.EndHorizontal();


            #endregion
        }
        bool showCreateKey = true;
        string nameNewKey;
        void OnColorSelected(object selectedDataItem)
        {
            string selectedText = selectedDataItem.ToString();

            if (selectedText.Length == 4 && Project.singleton.DataToSendSelectedItems.Contains("全ターゲット言語スクリーンショット"))
            {
                Project.singleton.DataToSendSelectedItems.Remove("全ターゲット言語スクリーンショット");
            }

            if (selectedText == "全ターゲット言語スクリーンショット")
            {
                bool allLangAdded = true;
                foreach (LangSupport lang in Project.singleton.langSupports)
                {

                    if (lang.name != UserData.singleton.mainLangEditor.ToString())
                    {
                        if (!Project.singleton.DataToSendSelectedItems.Contains(lang.name))
                        {
                            allLangAdded = false;
                        }
                    }
                }

                if (allLangAdded)
                {
                    foreach (LangSupport lang in Project.singleton.langSupports)
                    {
                        if (Project.singleton.DataToSendSelectedItems.Contains(lang.name))
                        {
                            Project.singleton.DataToSendSelectedItems.Remove(lang.name);
                        }
                    }

                }
                else
                {
                    foreach (LangSupport lang in Project.singleton.langSupports)
                    {
                        if (lang.name != UserData.singleton.mainLangEditor.ToString())
                        {
                            if (!Project.singleton.DataToSendSelectedItems.Contains(lang.name))
                            {
                                Project.singleton.DataToSendSelectedItems.Add(lang.name);
                            }
                        }
                    }
                }

            }

            Debug.Log(selectedText);

            if (Project.singleton.DataToSendSelectedItems.Contains(selectedText))
            {
                Project.singleton.DataToSendSelectedItems.Remove(selectedText);
            }
            else
            {
                Project.singleton.DataToSendSelectedItems.Add(selectedText);
            }

            if (!Project.singleton.DataToSendSelectedItems.Where(d => d.Contains("Source")).Any())
            {
                Project.singleton.DataToSendSelectedItems.Remove(UserData.singleton.mainLangEditor.ToString());
            }

            Project.singleton.setDirty();

        }
        void DrawAddRecord()
        {

            GUI.backgroundColor = new Color(1, 1, 1, 0.4f);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("+", EditorStyles.toolbarButton, GUILayout.Width(30)))
            {
                showCreateKey = !showCreateKey;
            }
            GUI.color = Color.white;
            if (showCreateKey) ShowCreateKey();
            GUILayout.EndHorizontal();

            void ShowCreateKey()
            {
                GUILayout.BeginHorizontal(EditorStyles.toolbar);
                nameNewKey = EditorGUILayout.TextField(nameNewKey, EditorStyles.toolbarTextField, GUILayout.ExpandWidth(true));
                GUILayout.EndHorizontal();

                if (GUILayout.Button("レコードを作成", "toolbarbutton", GUILayout.ExpandWidth(false)))
                {


                    Record record = new Record();
                    record.recordID = nameNewKey;
                    GridlyFunctionEditor.editor.AddRecord(record, grid.choesenViewID);

                    foreach (var i in Project.singleton.langSupports)
                    {
                        record.columns.Add(new Column(i.languagesSuport.ToString(), ""));
                    }

                    grid.records.Add(record);
                    Project.singleton.setDirty();

                    showCreateKey = false;
                    selectRecordID = nameNewKey;
                    nameNewKey = "";
                    RefeshList();
                }
            }

        }

        void DrawRecord(int i, Record record, ref bool selected)
        {
            selected = selectRecordID == record.recordID;
            if (selected)
            {
                Color color = Color.Lerp(Color.cyan, Color.white, 0.3f);
                color.a = 0.3f;
                GUI.backgroundColor = color;
                SelectStyle.normal.textColor = Color.white;
            }
            else
            {
                GUI.backgroundColor = darkLightColor;
                SelectStyle.normal.textColor = new Color(1, 1, 1, 0.2f);
            }


            Rect rect = new Rect(2, YPos, position.width, mRowSize);
            //draw record
            if (GUI.Button(rect, new GUIContent(i + ". " + record.recordID + " |  " + GetShowText(ref record).Replace('\n', ' ')), SelectStyle))
            //if(GUILayout.Button(new GUIContent(i + ". " + record.recordID + " |  " + GetShowText(ref record).Replace('\n', ' ')), GUILayout.Width(position.width-15), GUILayout.Height(mRowSize)))
            {
                countReset = 2;
                if (selectRecordID == record.recordID)
                    selectRecordID = "";
                else selectRecordID = record.recordID;

                //-turn off rename feature
                isRename = false;
                theNameToRename = "";

                //GUI.FocusControl(null);
            }


            if (selected)
            {
                DrawDetailRecord(record);
            }

        }

        string GetShowText(ref Record record)
        {

            foreach (var i in record.columns)
            {
                if (i.columnID == UserData.singleton.mainLangEditor.ToString())
                    return LimitText(ref i.text);
            }
            return string.Empty;
        }

        string LimitText(ref string input)
        {
            string final = "";
            const int length = 50;
            for (int i = 0; i < input.Length; i++)
            {
                if (i >= length)
                {
                    final += "...";
                    break;
                }
                final += input[i];
            }
            return final;
        }

        bool isRename;
        string theNameToRename = "";
        void DrawDetailRecord(Record record)
        {
            //return;
            EditorGUI.BeginChangeCheck();
            int selectedIndex = 0;
            if (ArrayUtility.IndexOf(paths, record.pathTag) >= 0)
            {
                selectedIndex = ArrayUtility.IndexOf(paths, record.pathTag);
            }
            pathSelectedIndex = EditorGUILayout.Popup("パス", selectedIndex, paths);
            pathSelect = paths[pathSelectedIndex];
            if (EditorGUI.EndChangeCheck())
            {
                record.pathTag = pathSelect;
                //Refesh();
                return;
            }
            //Debug.Log(record.pathTag);
            foreach (var i in Project.singleton.langSupports)
            {

                //IDE

                GUILayout.BeginHorizontal(TextStyle);
                string name = i.name;

                GUIContent contenLabel;
                if (i.languagesSuport == UserData.singleton.mainLangEditor)
                {
                    name = "*" + name;
                    contenLabel = new GUIContent() { text = name, tooltip = "これはソース言語です" };
                }
                else contenLabel = new GUIContent() { text = name };





                float widthNameLang = 70 + (position.width - 400) * 0.1f;
                GUILayout.Label(contenLabel, EditorStyles.boldLabel, GUILayout.Width(widthNameLang));

                Column col = record.columns.Find(x => x.columnID == i.languagesSuport.ToString());
                string text = "";
                if (col != null)
                    text = col.text;


                EditorGUI.BeginChangeCheck();
                //text = GUILayout.TextField(text, GUILayout.MaxWidth(position.width-80 - widthNameLang));
                text = GUILayout.TextArea(text, GUILayout.MaxWidth(position.width - 80 - widthNameLang));

                if (EditorGUI.EndChangeCheck())
                {
                    if (col != null)
                    {
                        col.text = text;
                        Project.singleton.setDirty();
                    }
                    else
                    {
                        Debug.Log(i.languagesSuport + "カラムが Gridly に存在しないため、このフィールドは編集できません。修正するためには、Gridly にログインしてカラムIDを\"" + i.languagesSuport + "\" で追加してください。");
                    }
                }

                if (GUILayout.Button(new GUIContent() { text = "Export", tooltip = "Export this field to Gridly" }, GUILayout.Width(60)))
                {
                    GridlyFunctionEditor.editor.UpdateRecordLang(record, grid.choesenViewID, i.languagesSuport);
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.Space(2);

            if (isRename)
            {
                ShowRename();
                void ShowRename()
                {
                    GUILayout.BeginHorizontal(TextStyle);
                    GUILayout.Label(record.recordID);


                    theNameToRename = GUILayout.TextField(theNameToRename, GUILayout.Width(position.width * 0.7f));
                    //theNameToRename = GUILayout.TextField(theNameToRename, GUILayout.ExpandWidth(true));
                    if (GUILayout.Button("Save", GUILayout.Width(50)))
                    {
                        selectRecordID = theNameToRename;
                        GridlyFunctionEditor.editor.DeleteRecord(record.recordID, grid.choesenViewID, null); //delete old record before rename
                        record.recordID = theNameToRename;
                        GridlyFunctionEditor.editor.AddRecord(record, grid.choesenViewID);
                        Project.singleton.setDirty();
                    }

                    GUILayout.EndHorizontal();
                }
            }
            #region delete, rename
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("削除"))
            {
                if (EditorUtility.DisplayDialog("削除確認", "本当にこのレコードを削除しますか？", "はい", "キャンセル"))
                {
                    EditorUtility.SetDirty(UserData.singleton);
                    GridlyFunctionEditor.editor.DeleteRecord(record.recordID, grid.choesenViewID, null);
                    grid.records.Remove(record);
                    Refesh();
                    Project.singleton.setDirty();
                }

            }


            if (GUILayout.Button("改名"))
            {
                isRename = !isRename;
            }

            /*
            if (GUILayout.Button("Update"))
            {
                GridlyFunctionEditor.editor.AddRecord(record, grid.choesenViewID);
            }
            */

            GUILayout.EndHorizontal();
            #endregion

            //YPos = GUILayoutUtility.GetLastRect().y;
            //Debug.Log(GUILayoutUtility.GetLastRect().y);
            GUILayout.Space(mRowSize);
        }
    }


}
