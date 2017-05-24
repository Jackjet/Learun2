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
    [Description("事物管理")]
    [PrimaryKey("TrackingID")]
    public class FY_ObjectTracking : BaseEntity
    {
        
        [DisplayName("主键")]
        public string TrackingID { get; set; }

        public string ObjectState { get; set; }

        public string ObjectType { get; set; }

        public string TrackingBy { get; set; }

        public string CreateBy { get; set; }

        public string ResponseBy { get; set; }

        public DateTime? DecideDate { get; set; }

        public DateTime? PlanFinishDate { get; set; }

        public string Attach { get; set; }

        public string ObjectDescripe { get; set; }

        public string ObjectProgress { get; set; }

        public string TrackingStates { get; set; }

        public int delayCount { get; set; }

        public DateTime? delayDate { get; set; }

        public string DescripeAttach { get; set; }

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.TrackingID = CommonHelper.GetGuid;
            //this.CreateBy
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="KeyValue"></param>
        public override void Modify(string KeyValue)
        {
            this.TrackingID = KeyValue;

        }
        #endregion
    }
}