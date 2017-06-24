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

using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Common;
using Assets.Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Control.TreeView
{
    public class TreeViewItem : MonoBehaviour
    {
        public TreeViewController TreeViewContrller;
        public SpatialStructure Spatial;
        public List<TreeViewItem> ChildList;
        //层级，用来计算横向滚动条
        public int Hierarchy;

        private CommonButtonUiScripts _showButton;
        private CommonButtonUiScripts _selectToggle;

        public bool SelectAll;
        private int _childSelectCount;
        public int ChildSelectCount
        {
            get
            {
                return _childSelectCount;
            }

            set
            {
                _childSelectCount = value;
                SelectAll = _childSelectCount == ChildList.Count ? true : false;
            }
        }

        private bool _isShowChild;
        //是否展开
        public bool IsShowChild
        {
            get { return _isShowChild; }
            set
            {
                _isShowChild = value;

                _showButton.transform.GetChild(0).GetComponent<Image>().sprite = _isShowChild
                    ? null
                    : TreeViewContrller.DefaultShowChildSprite;
            }
        }



        //是否选中
        public bool IsSelected;

        public static float TreeViewItemWidth;
        public static float TreeVIewLeftPadding;

        void Awake()
        {
            _showButton = CommonManager.FindComponentWithName<CommonButtonUiScripts>(gameObject, "ShowButton");
            _selectToggle = CommonManager.FindComponentWithName<CommonButtonUiScripts>(gameObject, "Toggle");
        }

        // Use this for initialization
        void Start()
        {
            TreeViewItemWidth = TreeVIewLeftPadding = 50f;
            TreeViewContrller = FindObjectOfType<TreeViewController>();


            _showButton.OnClick += go =>
            {
                ShowChildContext(gameObject.GetComponent<TreeViewItem>());
            };

            _selectToggle.OnClick += go =>
            {
                SelectItemAndChildren(gameObject.GetComponent<TreeViewItem>());
            };
        }

        /// <summary>
        /// 展示子节点
        /// </summary>
        public void ShowChildContext(TreeViewItem item)
        {
            Debug.Log("显示下层节点");
            item.IsShowChild = !item.IsShowChild;

            foreach (TreeViewItem t in item.ChildList)
            {
                t.gameObject.SetActive(IsShowChild);
                if (t.ChildList.Count <= 0)
                {
                    t._showButton.gameObject.SetActive(false);
                }
            }
            TreeViewContrller.Refresh(item);
        }

        /// <summary>
        /// 选中本身和子物体并设置选中图片
        /// </summary>
        /// <param name="item"></param>
        /// <param name="isSelect"></param>
        public void SelectItemAndChildren(TreeViewItem item)
        {
            item.IsSelected = !item.IsSelected;
            TreeViewItem parent;
            if (parent = TreeViewContrller.TreeViewList.FirstOrDefault(p => p.Spatial.ID == item.Spatial.ParentID))
            {
                parent.ChildSelectCount = item.IsSelected ? parent.ChildSelectCount + 1 : parent.ChildSelectCount - 1;
            }
            item._selectToggle.GetComponent<Toggle>().isOn = IsSelected;
            foreach (TreeViewItem t in item.ChildList)
                SelectItemAndChildren(t);

        }


    }
}
