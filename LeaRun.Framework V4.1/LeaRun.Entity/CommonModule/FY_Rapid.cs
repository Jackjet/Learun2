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
    [Description("快速反应")]
    [PrimaryKey("res_id")]
    public class FY_Rapid : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// 流程ID
        /// </summary>
        /// <returns></returns>
        [DisplayName("主键")]
        public string res_id { get; set; }
        /// <summary>
        /// 流程名称
        /// </summary>
        /// <returns></returns>
        [DisplayName("状态")]
        public string res_zt { get; set; }

        public string cf { get; set; }

        public string res_kf { get; set; }

        public string res_dd { get; set; }

        public string res_mc { get; set; }

        public string res_ms { get; set; }

        public DateTime? res_cdate { get; set; }

        //添加一个虚拟字段，用来格式化日期；数据库中没有对应的字段
        //orm框架有缺陷，数据库没有对应字段会报错，汗！！
        //public virtual string res_cdate_v { get { string d1 = ""; if (res_cdate != null) { d1 = Convert.ToDateTime(res_cdate).ToString("d"); } return d1; } }

        public string res_cpeo { get; set; }

        public DateTime? res_ndate { get; set; }

        public string res_yzb { get; set; }

        public string res_fx { get; set; }

        public string res_cs { get; set; }

        public string res_fcf { get; set; }

        public string res_fcsh { get; set; }

        public string res_csgz { get; set; }

        public string res_fmea { get; set; }

        public string res_bzgx { get; set; }

        public string res_jyjx { get; set; }

        public string res_yy { get; set; }

        public string res_docs { get; set; }

       

        public string yy_lj { get; set; }

        public string cs_lj { get; set; }

        public string res_ok { get; set; }

       

        public string res_jb { get; set; }

        public string res_cd { get; set; }

        public string mes_lj { get; set; }

        public string res_enew { get; set; }

        public string res_eback { get; set; }
        public int IsEmail { get; set; }

        public string Rate { get; set; }

        public string RapidState { get; set; }

#region 问题描述附件相对地址
        public string res_msfj { get; set; }
        public string res_yzbfj { get; set; }
        public DateTime? res_yzbdt { get; set; }
        public string res_yzbnode { get; set; }

        public DateTime? res_fxdt { get; set; }
        public string res_fxnode { get; set; }
        public string res_fxfj { get; set; }

        public DateTime? res_csdt { get; set; }
        public string res_csnode { get; set; }
        public string res_csfj { get; set; }

        public DateTime? res_fcfdt { get; set; }
        public string res_fcfnode { get; set; }
        public string res_fcffj { get; set; }
        public DateTime? res_fcshdt { get; set; }
        public string res_fcshnode { get; set; }
        public string res_fcshfj { get; set; }

        public DateTime? res_csgzdt { get; set; }
        public string res_csgznode { get; set; }
        public string res_csgzfj { get; set; }

        public DateTime? res_fmeadt { get; set; }
        public string res_fmeanode { get; set; }
        public string res_fmeafj { get; set; }

        public DateTime? res_bzgxdt { get; set; }
        public string res_bzgxnode { get; set; }
        public string res_bzgxfj { get; set; }

        public DateTime? res_jyjxdt { get; set; }
        public string res_jyjxnode { get; set; }
        public string res_jyjxfj { get; set; }

        //未按进度完成原因
        public string NotCompleteReason { get; set; }
        public string NotCompleteReasonfj { get; set; }
        public string Action { get; set; }
        public string Actionfj { get; set; }

        public DateTime? PlanTime { get; set; }
        public DateTime? RealTime { get; set; }
        public DateTime? CustomerTime { get; set; }
#endregion
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.res_id = CommonHelper.GetGuid;
            this.res_yzb="未提交";
            this.res_fx = "未提交";
            this.res_cs = "未提交";
            this.res_fcf = "未提交";
            this.res_fcsh = "未提交";
            this.res_csgz = "未提交";
            this.res_fmea = "未提交";
            this.res_bzgx = "未提交";
            this.res_jyjx = "未提交";

            this.RapidState = "进行中";

            //this.res_yzbdt = DateTime.Now;
            //this.res_fxdt = DateTime.Now;
            //this.res_csdt = DateTime.Now;
            //this.res_fcfdt = DateTime.Now;
            //this.res_fcshdt = DateTime.Now;
            //this.res_csgzdt = DateTime.Now;
            //this.res_fmeadt = DateTime.Now;
            //this.res_bzgxdt = DateTime.Now;
            //this.res_jyjxdt = DateTime.Now;



        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="KeyValue"></param>
        public override void Modify(string KeyValue)
        {
            this.res_id = KeyValue;

        }
        #endregion
    }
}