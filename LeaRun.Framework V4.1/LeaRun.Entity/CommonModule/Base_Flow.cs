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
    [PrimaryKey("FlowID")]
    public class Base_Flow : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// 流程ID
        /// </summary>
        /// <returns></returns>
        [DisplayName("主键")]
        public string FlowID { get; set; }
        /// <summary>
        /// 流程名称
        /// </summary>
        /// <returns></returns>
        [DisplayName("流程名称")]
        public string FlowName { get; set; }
       
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.FlowID = CommonHelper.GetGuid;
            

        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="KeyValue"></param>
        public override void Modify(string KeyValue)
        {
            this.FlowID = KeyValue;

        }
        #endregion
    }
}