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
    [Description("用户管理")]
    [PrimaryKey("TestId")]
    public class TestTable : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// 用户主键
        /// </summary>
        /// <returns></returns>
        [DisplayName("主键")]
        public string TestId { get; set; }
        /// <summary>
        /// 公司主键
        /// </summary>
        /// <returns></returns>
        [DisplayName("代号")]
        public int? Code { get; set; }
        /// <summary>
        /// 部门主键
        /// </summary>
        /// <returns></returns>
        [DisplayName("名称")]
        public string FullName { get; set; }
        /// <summary>
        /// 内部用户
        /// </summary>
        /// <returns></returns>
        [DisplayName("创建日期")]
        public DateTime? CreateDate { get; set; }
        /// <summary>
        /// 用户编码
        /// </summary>
        /// <returns></returns>
        [DisplayName("创建人")]
        public string CreateUserName { get; set; }

        [DisplayName("备注")]
        public string Remark { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.TestId = CommonHelper.GetGuid;
            this.CreateDate = DateTime.Now;
            
            this.CreateUserName = ManageProvider.Provider.Current().UserName;
            
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="KeyValue"></param>
        public override void Modify(string KeyValue)
        {
            this.TestId = KeyValue;
           
        }
        #endregion
    }
}