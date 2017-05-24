using LeaRun.Business;
using LeaRun.DataAccess;
using LeaRun.Entity;
using LeaRun.Repository;
using LeaRun.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using System.Net;

namespace LeaRun.WebApp.Areas.CommonModule.Controllers
{
    public class RapidController : Controller
    {
        RepositoryFactory<FY_Rapid> repositoryfactory = new RepositoryFactory<FY_Rapid>();
        FY_RapidBll rapidbll = new FY_RapidBll();
        //
        // GET: /ExampleModule/Test/

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult GridPageListJson(string keywords, string CompanyId, string DepartmentId, JqGridParam jqgridparam, string ParameterJson,string MyTask)
        {
            try
            {
                Stopwatch watch = CommonHelper.TimerStart();
                DataTable ListData = rapidbll.GetPageList(keywords, ref jqgridparam,ParameterJson,MyTask);
                var JsonData = new
                {
                    total = jqgridparam.total,
                    page = jqgridparam.page,
                    records = jqgridparam.records,
                    costtime = CommonHelper.TimerEnd(watch),
                    rows = ListData,
                };
                return Content(JsonData.ToJson());
            }
            catch (Exception ex)
            {
                Base_SysLogBll.Instance.WriteLog("", OperationType.Query, "-1", "异常错误：" + ex.Message);
                return null;
            }
        }

        public ActionResult Form()
        {
            string sql = " select FullName from Base_User a left join  [Base_ObjectUserRelation] b on a.UserId=b.UserId left join Base_Roles c on b.ObjectId=c.RoleId where a.UserId='" + ManageProvider.Provider.Current().UserId+"'";
            DataTable dt = rapidbll.GetDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                ViewData["dt"] = dt.Rows[0][0].ToString();
            }
            string UserName = ManageProvider.Provider.Current().Code;
            ViewData["UserName"] = UserName;
            return View();
        }


        [HttpPost]
        public ActionResult SubmitTestTableForm(string KeyValue, FY_Rapid rapid, string BuildFormJson, HttpPostedFileBase Filedata)
        {
            string ModuleId = DESEncrypt.Decrypt(CookieHelper.GetCookie("ModuleId"));
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                string Message = KeyValue == "" ? "新增成功。" : "编辑成功。";
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    if (KeyValue == ManageProvider.Provider.Current().UserId)
                    {
                        throw new Exception("无权限编辑信息");
                    }

                    //base_user.Modify(KeyValue);
                    rapid.Modify(KeyValue);
                    if (rapid.IsEmail != 1)
                    {
                        int IsEmail = SendEmail(rapid.res_cpeo, rapid.res_ms);
                        rapid.IsEmail = IsEmail;
                    }
                    if (rapid.RealTime != null && rapid.res_cdate != null && rapid.PlanTime != null)
                    {
                        //int planNum = new TimeSpan(rapid.PlanTime.Ticks - rapid.res_cdate.Ticks).Days;
                        //int realNum = (rapid.RealTime - rapid.res_cdate).Days;
                        //TimeSpan d3 = rapid.RealTime.Subtract(rapid.res_cdate);
                        int realNum = Math.Abs(((TimeSpan)(rapid.RealTime - rapid.res_cdate)).Days);
                        int planNum = Math.Abs(((TimeSpan)(rapid.PlanTime - rapid.res_cdate)).Days);
                        rapid.Rate = Math.Round(((2.0 - (realNum / (planNum * 1.0))) * 100),2).ToString() + "%";
                        rapid.RapidState = "已完成";
                    }
                    database.Update(rapid, isOpenTrans);

                }
                else
                {
                    rapid.Create();
                    int IsEmail = SendEmail(rapid.res_cpeo, rapid.res_ms);
                    rapid.IsEmail = IsEmail;
                    database.Insert(rapid, isOpenTrans);
                    //database.Insert(base_employee, isOpenTrans);
                    Base_DataScopePermissionBll.Instance.AddScopeDefault(ModuleId, ManageProvider.Provider.Current().UserId, rapid.res_id, isOpenTrans);
                }
                Base_FormAttributeBll.Instance.SaveBuildForm(BuildFormJson, rapid.res_id, ModuleId, isOpenTrans);
                database.Commit();
                return Content(new JsonMessage { Success = true, Code = "1", Message = Message }.ToString());
            }
            catch (Exception ex)
            {
                database.Rollback();
                database.Close();
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="code">用户编码</param>
        /// <returns>1成功，0失败</returns>
        public int SendEmail(string code,string Content)
        {
            string sql = " select Email from base_user where code='"+code+"'";
            DataTable dt = rapidbll.GetDataTable(sql);
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

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SetTestForm(string KeyValue)
        {
            FY_Rapid rapid = DataFactory.Database().FindEntity<FY_Rapid>(KeyValue);
            if (rapid == null)
            {
                return Content("");
            }
            //Base_Employee base_employee = DataFactory.Database().FindEntity<Base_Employee>(KeyValue);
            //Base_Company base_company = DataFactory.Database().FindEntity<Base_Company>(base_user.CompanyId);
            string strJson = rapid.ToJson();
            //公司
            //strJson = strJson.Insert(1, "\"CompanyName\":\"" + base_company.FullName + "\",");
            //员工信息
            //strJson = strJson.Insert(1, base_employee.ToJson().Replace("{", "").Replace("}", "") + ",");
            //自定义
            strJson = strJson.Insert(1, Base_FormAttributeBll.Instance.GetBuildForm(KeyValue));
            return Content(strJson);
        }






        public ActionResult SubmitUploadify(string FolderId, HttpPostedFileBase Filedata, string type)
        {
            try
            {
                
                    Thread.Sleep(1000);////延迟500毫秒
                    Base_NetworkFile entity = new Base_NetworkFile();
                    FY_Rapid rapidentity = DataFactory.Database().FindEntity<FY_Rapid>(FolderId);

                    string IsOk = "";
                    //没有文件上传，直接返回
                    if (Filedata == null || string.IsNullOrEmpty(Filedata.FileName) || Filedata.ContentLength == 0)
                    {
                        return HttpNotFound();
                    }
                    //获取文件完整文件名(包含绝对路径)
                    //文件存放路径格式：/Resource/Document/NetworkDisk/{日期}/{guid}.{后缀名}
                    //例如：/Resource/Document/Email/20130913/43CA215D947F8C1F1DDFCED383C4D706.jpg
                    string fileGuid = CommonHelper.GetGuid;
                    long filesize = Filedata.ContentLength;
                    string FileEextension = Path.GetExtension(Filedata.FileName);
                    string uploadDate = DateTime.Now.ToString("yyyyMMdd");
                    string UserId = ManageProvider.Provider.Current().UserId;

                    string virtualPath = string.Format("~/Resource/Document/NetworkDisk/{0}/{1}/{2}{3}", UserId, uploadDate, fileGuid, FileEextension);
                    //rapidentity.res_msfj = virtualPath;

                    string fullFileName = this.Server.MapPath(virtualPath);
                    //创建文件夹，保存文件
                    string path = Path.GetDirectoryName(fullFileName);
                    Directory.CreateDirectory(path);
                    if (!System.IO.File.Exists(fullFileName))
                    {
                        Filedata.SaveAs(fullFileName);
                        try
                        {
                            //文件信息写入数据库
                            //entity.Create();
                            //entity.NetworkFileId = fileGuid;
                            //entity.FolderId = FolderId;
                            //entity.FileName = Filedata.FileName;
                            //entity.FilePath = virtualPath;
                            //entity.FileSize = filesize.ToString();
                            //entity.FileExtensions = FileEextension;
                            //string _FileType = "";
                            //string _Icon = "";
                            //this.DocumentType(FileEextension, ref _FileType, ref _Icon);
                            //entity.Icon = _Icon;
                            //entity.FileType = _FileType;
                            //IsOk = DataFactory.Database().Insert<Base_NetworkFile>(entity).ToString();
                            
                            if (type == "res_msfj")
                            {
                                rapidentity.res_msfj = virtualPath;
                            }
                            if (type == "res_yzb")
                            {
                                rapidentity.res_yzbfj = virtualPath;
                            }
                            if (type == "res_fx")
                            {
                                rapidentity.res_fxfj = virtualPath;
                            }
                            if (type == "res_cs")
                            {
                                rapidentity.res_csfj = virtualPath;
                            }
                            if (type == "res_fcf")
                            {
                                rapidentity.res_fcffj = virtualPath;
                            }
                            if (type == "res_fcsh")
                            {
                                rapidentity.res_fcshfj = virtualPath;
                            }
                            if (type == "res_csgz")
                            {
                                rapidentity.res_csgzfj = virtualPath;
                            }
                            if (type == "res_fmea")
                            {
                                rapidentity.res_fmeafj = virtualPath;
                            }
                            if (type == "res_jyjx")
                            {
                                rapidentity.res_jyjxfj = virtualPath;
                            }
                            if (type == "uploadifyNotCompleteReason")
                            {
                                rapidentity.NotCompleteReasonfj = virtualPath;
                            }
                            if (type == "Action")
                            {
                                rapidentity.Actionfj = virtualPath;
                            }
                            if (type == "res_bzgx")
                            {
                                rapidentity.res_bzgxfj = virtualPath;
                            }
                            DataFactory.Database().Update<FY_Rapid>(rapidentity);
                        }
                        catch (Exception ex)
                        {
                            //IsOk = ex.Message;
                            //System.IO.File.Delete(virtualPath);
                        }
                    }
                    var JsonData = new
                    {
                        Status = IsOk,
                        NetworkFile = rapidentity,
                    };
                    return Content(JsonData.ToJson());
                
                
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        public void DocumentType(string Eextension, ref string FileType, ref string Icon)
        {
            string _FileType = "";
            string _Icon = "";
            switch (Eextension)
            {
                case ".docx":
                    _FileType = "word文件";
                    _Icon = "doc";
                    break;
                case ".doc":
                    _FileType = "word文件";
                    _Icon = "doc";
                    break;
                case ".xlsx":
                    _FileType = "excel文件";
                    _Icon = "xls";
                    break;
                case ".xls":
                    _FileType = "excel文件";
                    _Icon = "xls";
                    break;
                case ".pptx":
                    _FileType = "ppt文件";
                    _Icon = "ppt";
                    break;
                case ".ppt":
                    _FileType = "ppt文件";
                    _Icon = "ppt";
                    break;
                case ".txt":
                    _FileType = "记事本文件";
                    _Icon = "txt";
                    break;
                case ".pdf":
                    _FileType = "pdf文件";
                    _Icon = "pdf";
                    break;
                case ".zip":
                    _FileType = "压缩文件";
                    _Icon = "zip";
                    break;
                case ".rar":
                    _FileType = "压缩文件";
                    _Icon = "rar";
                    break;
                case ".png":
                    _FileType = "png图片";
                    _Icon = "png";
                    break;
                case ".gif":
                    _FileType = "gif图片";
                    _Icon = "gif";
                    break;
                case ".jpg":
                    _FileType = "jpg图片";
                    _Icon = "jpeg";
                    break;
                case ".mp3":
                    _FileType = "mp3文件";
                    _Icon = "mp3";
                    break;
                case ".html":
                    _FileType = "html文件";
                    _Icon = "html";
                    break;
                case ".css":
                    _FileType = "css文件";
                    _Icon = "css";
                    break;
                case ".mpeg":
                    _FileType = "mpeg文件";
                    _Icon = "mpeg";
                    break;
                case ".pds":
                    _FileType = "pds文件";
                    _Icon = "pds";
                    break;
                case ".ttf":
                    _FileType = "ttf文件";
                    _Icon = "ttf";
                    break;
                case ".swf":
                    _FileType = "swf文件";
                    _Icon = "swf";
                    break;
                default:
                    _FileType = "其他文件";
                    _Icon = "new";
                    //return "else.png";
                    break;
            }
            FileType = _FileType;
            Icon = _Icon;
        }



        /// <summary>
        /// 审批
        /// </summary>
        /// <param name="KeyValue">rapid主键</param>
        /// <param name="field">字段名称</param>
        /// <param name="state">当前状态</param>
        /// <param name="isok">选择操作</param>
        /// <returns></returns>
        public int  Approve(string KeyValue,string field,string state,string isok,string dt,string node)
        {
            try
            {
                
                int result = rapidbll.Approve(KeyValue, field, state, isok,dt,node);
                return result;
            }
            catch
            {
                return 0;
            }
        }

        //人员下拉列表

        public ActionResult ListJson(string CompanyId)
        {
            DataTable ListData = rapidbll.GetList();
            return Content(ListData.ToJson());
        }

        //客户下拉列表
        public ActionResult CustomerJson()
        {
            DataTable ListData = rapidbll.GetCustomerList();
            return Content(ListData.ToJson());
        }

       

        //月度报表
        public ActionResult RapidMonthlyReport()
        {
            return View();
        }

        public ActionResult GetReportJson(string keywords, string CompanyId, string DepartmentId, JqGridParam jqgridparam)
        {
            try
            {
                Stopwatch watch = CommonHelper.TimerStart();
                DataTable ListData = rapidbll.GetReportJson(keywords, ref jqgridparam);
                var JsonData = new
                {
                    total = jqgridparam.total,
                    page = jqgridparam.page,
                    records = jqgridparam.records,
                    costtime = CommonHelper.TimerEnd(watch),
                    rows = ListData,
                };
                return Content(JsonData.ToJson());
            }
            catch (Exception ex)
            {
                Base_SysLogBll.Instance.WriteLog("", OperationType.Query, "-1", "异常错误：" + ex.Message);
                return null;
            }
        }

        public ActionResult PictureReport()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Delete(string KeyValue)
        {
            try
            {
                var Message = "删除失败。";
                int IsOk = 0;
                
                IsOk = repositoryfactory.Repository().Delete(KeyValue);
                if (IsOk > 0)
                {
                    Message = "删除成功。";
                }
                
                WriteLog(IsOk, KeyValue, Message);
                return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = Message }.ToString());
            }
            catch (Exception ex)
            {
                WriteLog(-1, KeyValue, "操作失败：" + ex.Message);
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }

        public void WriteLog(int IsOk, string KeyValue, string Message = "")
        {
            string[] array = KeyValue.Split(',');
            Base_SysLogBll.Instance.WriteLog<FY_Rapid>(array, IsOk.ToString(), Message);
        }



        public ActionResult PictureIndex()
        {
            return View();
        }


        public string GetPictueData()
        {
            string result = "";
            string temp1 = "";
            string temp2 = "";
            string sql = " select count(*) as rapidcount,MONTH(res_cdate) as month from FY_Rapid where YEAR(res_cdate)=YEAR(GETDATE()) group by MONTH(res_cdate),YEAR(res_cdate)  ";
            DataTable dt = rapidbll.GetDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    temp1 = temp1 + dt.Rows[i][0]+",";
                    temp2 = temp2 + dt.Rows[i][1]+",";
                }
                temp1=temp1.Substring(0,temp1.Length-1);
                temp2=temp2.Substring(0,temp2.Length-1);
            }
            result = temp1 + "|" + temp2;
            
            
            return result;
        }

        public string GetPieData()
        {
            string result = "";
            string temp1 = "";
            string temp2 = "";
            string sql = "select count(*) as cishu,fullname from RapidList_New group by fullname ";
            DataTable dt = rapidbll.GetDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    temp1 = temp1 + dt.Rows[i][0] + ",";
                    temp2 = temp2 + dt.Rows[i][1] + ",";
                }
                temp1 = temp1.Substring(0, temp1.Length - 1);
                temp2 = temp2.Substring(0, temp2.Length - 1);
            }
            result = temp1 + "|" + temp2;
            return result;
        }







       
        #region highcharts需要的json数据格式
        public string DataTableToJson(DataTable dt)
        {
            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append("{\"");
            jsonBuilder.Append("list");
            jsonBuilder.Append("\":[");

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                jsonBuilder.Append("{");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    jsonBuilder.Append("\"");
                    jsonBuilder.Append(dt.Columns[j].ColumnName);
                    jsonBuilder.Append("\":");
                    //jsonBuilder.Append("\":\"");
                    //判断下是否纯数字，highcharts插件不是纯数字的值要加双引号
                    if (IsNumber(dt.Rows[i][j].ToString()))
                    {
                        jsonBuilder.Append(dt.Rows[i][j].ToString());
                    }
                    else
                    {
                        jsonBuilder.Append("\"");
                        jsonBuilder.Append(dt.Rows[i][j].ToString());
                        jsonBuilder.Append("\"");
                    }
                    jsonBuilder.Append(",");
                    //jsonBuilder.Append("\",");
                }
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
                jsonBuilder.Append("},");
            }
            jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
            jsonBuilder.Append("]");
            jsonBuilder.Append("}");
            return jsonBuilder.ToString();
        }

        //json转换是判断是否纯数字
        public bool IsNumber(string str)
        {
            if (str == null || str.Length == 0)    //验证这个参数是否为空  
                return false;                           //是，就返回False  
            ASCIIEncoding ascii = new ASCIIEncoding();//new ASCIIEncoding 的实例  
            byte[] bytestr = ascii.GetBytes(str);         //把string类型的参数保存到数组里  

            foreach (byte c in bytestr)                   //遍历这个数组里的内容  
            {
                if (c < 48 || c > 57)                          //判断是否为数字  
                {
                    return false;                              //不是，就返回False  
                }
            }
            return true;                                        //是，就返回True  
        }
        #endregion


        public ActionResult FormNew()
        {
            return View();
        }

    }
}
