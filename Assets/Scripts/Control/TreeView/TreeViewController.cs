////////////////////////////////////////////////////////////////////
//                          _ooOoo_                               //
//                         o8888888o                              //
//                         88" . "88                              //
//                         (| ^_^ |)                              //
//                         O\  =  /O                              //
//                      ____/`---'\____                           //
//                    .'  \\|     |//  `.                         //
//                   /  \\|||  :  |||//  \                        //
//                  /  _||||| -:- |||||-  \                       //
//                  |   | \\\  -  /// |   |                       //
//                  | \_|  ''\---/''  |   |                       //
//                  \  .-\__  `-`  ___/-. /                       //
//                ___`. .'  /--.--\  `. . ___                     //
//              ."" '<  `.___\_<|>_/___.'  >'"".                  //
//            | | :  `- \`.;`\ _ /`;.`/ - ` : | |                 //
//            \  \ `-.   \_ __\ /__ _/   .-` /  /                 //
//      ========`-.____`-.___\_____/___.-`____.-'========         //
//                           `=---='                              //
//      ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^        //
//         佛祖保佑       永无BUG     永不修改                    //
////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Common;
using Assets.Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Control.TreeView
{
    public class TreeViewController : MonoBehaviour
    {
        private List<SpatialStructure> _spatialList;

        //树节点模板
        public GameObject TreeViewTemplatePrefabs;

        public List<TreeViewItem> TreeViewList;

        public Sprite DefaultShowChildSprite;
        public Sprite DefaultSelectedSprite;

        public List<TreeViewItem> TreeViewRootItemsList;

        void Awake()
        {

            _spatialList = new List<SpatialStructure>()
            {
                new SpatialStructure(){ID = 1000,Name = "根节点",ParentID = -1},
                new SpatialStructure(){ID = 1001,Name = "节点1",ParentID = 1000},
                new SpatialStructure(){ID = 1002,Name = "节点2",ParentID = 1000},
                new SpatialStructure(){ID = 1011,Name = "节点1-1",ParentID = 1001},
                new SpatialStructure(){ID = 1013,Name = "节点1-1-1",ParentID = 1011},
                new SpatialStructure(){ID = 1055,Name = "节点3",ParentID = 1000},
                new SpatialStructure(){ID = 1065,Name = "节点3-1",ParentID = 1055},
                new SpatialStructure(){ID = 1075,Name = "节点3-1",ParentID = 1055},
            };

        }
        // Use this for initialization
        void Start()
        {
            TreeViewList = new List<TreeViewItem>();
            TreeViewRootItemsList = new List<TreeViewItem>();
            DefaultShowChildSprite = TreeViewTemplatePrefabs.transform.Find("ShowButton/Background").GetComponent<Image>().sprite;
            DefaultSelectedSprite = TreeViewTemplatePrefabs.transform.Find("Toggle/Background").GetComponent<Image>().sprite;
            //todo: 绑定数据
            _spatialList.OrderBy(p => p.ParentID);
            TreeViewInit();
        }

        /// <summary>
        /// 初始化树结构
        /// </summary>
        public void TreeViewInit()
        {
            if (_spatialList.Count <= 0)
                return;
            for (int i = 0; i < _spatialList.Count; i++)
            {
                if (_spatialList[i].ParentID == 0)
                {
                    continue;
                }
                var template = Instantiate(TreeViewTemplatePrefabs, transform).GetComponent<TreeViewItem>();
                template.Spatial = _spatialList[i];
                if (_spatialList[i].ParentID != -1)
                {
                    //设置父物体
                    try
                    {
                        var parentTree = TreeViewList.FirstOrDefault(p => p.Spatial.ID == template.Spatial.ParentID);
                        if (parentTree != null) parentTree.ChildList.Add(template);
                        template.transform.SetParent(parentTree.transform);
                        //设置位置间隔
                        template.GetComponent<Transform>().localPosition = new Vector3(50, -50 * parentTree.ChildList.Count, 0);
                        template.gameObject.SetActive(false);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }
                else
                {
                    template.transform.SetParent(CommonManager.FindObjWithName(gameObject, "Tree View Content").transform);
                    TreeViewRootItemsList.Add(template);

                    template.transform.localPosition = new Vector3(0, -50 * (TreeViewRootItemsList.Count - 1), 0);
                    template.tag = "RootTag";
                }
                template.name = "template";
                template.GetComponentInChildren<Text>().text = _spatialList[i].Name;
                Debug.Log(_spatialList[i].Name);
                template.Hierarchy = GetTreeViewitemHierarchy(template);
                TreeViewList.Add(template);
            }
        }

        /// <summary>
        /// 刷新
        /// </summary>
        public void Refresh(TreeViewItem item)
        {
            var parentItem = TreeViewList.FirstOrDefault(p => p.Spatial.ID == item.Spatial.ParentID && item.Spatial.ParentID != -1);
            var currentRootItem = item.FindGameObjectByTagInParent<TreeViewItem>("RootTag");
            //获取当前节点之后的节点
            var rightBrotherItem = new List<TreeViewItem>();
            var rightRootBrotherItem = new List<TreeViewItem>();

            for (var i = TreeViewRootItemsList.IndexOf(currentRootItem) + 1; i < TreeViewRootItemsList.Count; i++)
            {
                rightRootBrotherItem.Add(TreeViewRootItemsList[i]);
            }
            var width = item.IsShowChild ? TreeViewItem.TreeViewItemWidth : -TreeViewItem.TreeViewItemWidth;

            var count = item.ChildList.Count;
            while (parentItem)
            {
                rightBrotherItem =
                  parentItem.ChildList.Where(p => parentItem.ChildList.IndexOf(p) > parentItem.ChildList.IndexOf(item)).ToList();
                foreach (var temp in rightBrotherItem)
                {
                    temp.GetComponent<Transform>().localPosition = new Vector3(TreeViewItem.TreeVIewLeftPadding, temp.GetComponent<Transform>().localPosition.y - width * count, 0);
                }
                item = parentItem;
                parentItem = TreeViewList.FirstOrDefault(p => p.Spatial.ID == item.Spatial.ParentID);
            }

            foreach (var temp in rightRootBrotherItem)
            {
                temp.GetComponent<Transform>().localPosition = new Vector3(0, temp.GetComponent<Transform>().localPosition.y - width * count, 0);
            }
            SetScrollView(item);
        }

        /// <summary>
        /// 设置ScrollView滚动条
        /// </summary>
        public void SetScrollView(TreeViewItem item)
        {
            var maxHierarchy = TreeViewList.Where(p => p.gameObject.activeSelf).OrderByDescending(p => p.Hierarchy).FirstOrDefault().Hierarchy;
            var rect = new Vector2(item.GetComponent<RectTransform>().sizeDelta.x + maxHierarchy * TreeViewItem.TreeVIewLeftPadding, (TreeViewList.Where(p => p.gameObject.activeSelf).ToList().Count + 1) * TreeViewItem.TreeViewItemWidth);
            GetComponentInChildren<ScrollRect>().content.sizeDelta = rect;
        }

        private int GetTreeViewitemHierarchy(TreeViewItem item)
        {
            int hierarchy = 0;
            if (item.Spatial.ParentID == -1) return hierarchy;
            var parent = TreeViewList.FirstOrDefault(p => p.Spatial.ID == item.Spatial.ParentID);
            do
            {
                parent = TreeViewList.FirstOrDefault(p => p.Spatial.ID == parent.Spatial.ParentID);
                ++hierarchy;
            } while (parent != null);

            return hierarchy;
        }

    }
    public class SpatialStructure
    {
        public int ID { get; set; }
        public int ParentID { get; set; }
        public string Name { get; set; }
    }
}
