using LeaRun.DataAccess;
using LeaRun.Entity;
using LeaRun.Repository;
using LeaRun.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace LeaRun.Business
{
    public class FY_RapidBll : RepositoryFactory<FY_Rapid>
    {
        public DataTable GetPageList(string keyword, ref JqGridParam jqgridparam, string ParameterJson, string MyTask)
        {
            
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append(@"select * from RapidList_New where 1=1 ");
            if (!string.IsNullOrEmpty(keyword))
            {
                strSql.Append(@" AND (realname LIKE @keyword
                                    OR FullName LIKE @keyword
                                    OR res_ms LIKE @keyword
                                    )");
                parameter.Add(DbFactory.CreateDbParameter("@keyword", '%' + keyword + '%'));
            }
            if (!string.IsNullOrEmpty(ParameterJson) && ParameterJson.Length > 2)
            {
                strSql.Append(ConditionBuilder.GetWhereSql(ParameterJson.JonsToList<Condition>(), out parameter));
            }
            if (MyTask == "yes")
            {
                strSql.AppendFormat(" and RapidState!='已完成' and RealName like '{0}' ", ManageProvider.Provider.Current().UserName);
            }
            return Repository().FindTablePageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);
        }


        public DataTable GetDataTable(string sql)
        {
            return Repository().FindDataSetBySql(sql).Tables[0];
        }


        public int Approve(string KeyValue, string field, string state, string isok,string dt,string node)
        {
            StringBuilder strSql = new StringBuilder();
            if (isok == "y")
            {
                if (state == "未提交"||state=="回退")
                {
                    strSql.AppendFormat(" update fy_rapid set {0}='待审',RapidState='进行中',{0}dt='{2}',{0}node='{3}' where res_id='{1}'", field, KeyValue,dt,node);
                }
                if (state == "待审")
                {
                    strSql.AppendFormat(" update fy_rapid set {0}='通过',{0}dt='{2}',{0}node='{3}' where res_id='{1}'", field, KeyValue, dt, node);
                }


            }
            if (isok == "n")
            {
                if (state == "待审")
                {
                    strSql.AppendFormat(" update fy_rapid set {0}='回退',RapidState='回退',{0}dt='{2}',{0}node='{3}' where res_id='{1}'", field, KeyValue, dt, node);
                    SendEmail(KeyValue, "你提交的QSB方案被退回");
                }
                if (state == "通过")
                {
                    strSql.AppendFormat(" update fy_rapid set {0}='回退',RapidState='回退',{0}dt='{2}',{0}node='{3}' where res_id='{1}'", field, KeyValue, dt, node);
                    SendEmail(KeyValue, "你提交的QSB方案被退回");
                }
            }
            int result = Repository().ExecuteBySql(strSql);
            return result;
        }

        public DataTable GetList()
        {
            string sql = " select code,RealName from Base_User ";
            return Repository().FindDataSetBySql(sql).Tables[0];
        }

        public DataTable GetCustomerList()
        {
            string sql = " select fy_cus_name from FY_CUS ";
            return Repository().FindDataSetBySql(sql).Tables[0];
        }

        public DataTable GetReportJson(string keyword, ref JqGridParam jqgridparam)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append(@"select * from ##list ");
            StringBuilder proc = new StringBuilder();
            proc.AppendFormat(@"RapidMonthlyReport 2017 ");
            
            //Repository().ExecuteBySql(proc);
            //if (!string.IsNullOrEmpty(keyword))
            //{
                
                //parameter.Add(DbFactory.CreateDbParameter("@keyword", 2017));
            //}
            parameter.Add(DbFactory.CreateDbParameter("@Year", 2017));

                return Repository().FindDataSetByProc("RapidMonthlyReport", parameter.ToArray()).Tables[0];
        }




        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="code">用户编码</param>
        /// <returns>1成功，0失败</returns>
        public int SendEmail(string code, string Content)
        {
            string sql = " select Email from base_user where code in (select res_cpeo from FY_Rapid where res_id='"+code+"' ) ";
            DataTable dt = GetDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                try
                {
                    var emailAcount = "991011509@qq.com";
                    var emailPassword = "sh514229";
                    var reciver = dt.Rows[0][0].ToString();
                    var content = Content;
                    MailMessage message = new MailMessage();
                    //设置发件人,发件人需要与设置的邮件发送服务器的邮箱一致
                    MailAddress fromAddr = new MailAddress("991011509@qq.com");
                    message.From = fromAddr;
                    //设置收件人,可添加多个,添加方法与下面的一样
                    message.To.Add(reciver);
                    //设置抄送人
                    message.CC.Add("jun.shen@fuyaogroup.com");
                    //设置邮件标题
                    message.Subject = "QSB快速反应";
                    //设置邮件内容
                    message.Body = content;
                    //设置邮件发送服务器,服务器根据你使用的邮箱而不同,可以到相应的 邮箱管理后台查看,下面是QQ的
                    SmtpClient client = new SmtpClient("smtp.qq.com", 25);
                    //设置发送人的邮箱账号和密码
                    client.Credentials = new NetworkCredential(emailAcount, emailPassword);
                    //启用ssl,也就是安全发送
                    client.EnableSsl = true;
                    //发送邮件
                    client.Send(message);
                    return 1;
                }
                catch
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }
    }
}
