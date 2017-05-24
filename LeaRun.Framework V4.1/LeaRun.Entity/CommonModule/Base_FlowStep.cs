//=====================================================================================
// All Rights Reserved , Copyright @ Learun 2014
// Software Developers @ Learun 2014
//=====================================================================================

using LeaRun.DataAccess.Attributes;
using LeaRun.Utilities;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LeaRun.Entity
{
    /// <summary>
    /// 用户管理
    /// <author>
    ///		<name>she</name>
    ///		<date>2014.08.11 15:45</date>
    /// </author>
    /// </summary>
    [Description("流程管理")]
    [PrimaryKey("FlowStepID")]
    public class Base_FlowStep : BaseEntity
    {
       
        [DisplayName("主键")]
        public string FlowStepID { get; set; }
        
        [DisplayName("流程名称")]
        public string FlowName { get; set; }

        [DisplayName("主表ID")]
        public string FlowID { get; set; }

        [DisplayName("当前岗位名称")]
        public string CurrentPostName { get; set; }

        [DisplayName("当前岗位ID")]
        public string CurrentPostID { get; set; }

        [DisplayName("节点顺序")]
        public string StepNO { get; set; }

        //#region 扩展操作
        ///// <summary>
        ///// 新增调用
        ///// </summary>
        //public override void Create()
        //{
        //    this.FlowStepID = CommonHelper.GetGuid;


        //}
        ///// <summary>
        ///// 编辑调用
        ///// </summary>
        ///// <param name="KeyValue"></param>
        //public override void Modify(string KeyValue)
        //{
        //    this.FlowID = KeyValue;

        //}
        //#endregion
    }
}