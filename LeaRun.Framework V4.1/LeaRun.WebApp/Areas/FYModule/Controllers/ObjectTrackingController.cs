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

namespace LeaRun.WebApp.Areas.FYModule.Controllers
{
    public class ObjectTrackingController : Controller
    {
        FY_ObjectTrackingBll ObjectTrackBll = new FY_ObjectTrackingBll();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GridPageListJson(string keywords, string CompanyId, string DepartmentId, JqGridParam jqgridparam, string ParameterJson)
        {
            try
            {
                Stopwatch watch = CommonHelper.TimerStart();
                DataTable ListData = ObjectTrackBll.GetPageList(keywords, ref jqgridparam, ParameterJson);
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
            ViewData["UserName"] = @ManageProvider.Provider.Current().UserName;
            return View();
        }

        [HttpPost]
        public ActionResult SubmitForm(string KeyValue, FY_ObjectTracking entity, string BuildFormJson, HttpPostedFileBase Filedata)
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


                    entity.Modify(KeyValue);

                    
                    database.Update(entity, isOpenTrans);

                }
                else
                {
                    entity.Create();

                   
                    database.Insert(entity, isOpenTrans);

                    Base_DataScopePermissionBll.Instance.AddScopeDefault(ModuleId, ManageProvider.Provider.Current().UserId, entity.TrackingID, isOpenTrans);
                }
                Base_FormAttributeBll.Instance.SaveBuildForm(BuildFormJson, entity.TrackingID, ModuleId, isOpenTrans);
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

        public ActionResult FormEdit()
        {
            string sql = " select FullName from Base_User a left join  [Base_ObjectUserRelation] b on a.UserId=b.UserId left join Base_Roles c on b.ObjectId=c.RoleId where a.UserId='" + ManageProvider.Provider.Current().UserId + "'";
            DataTable dt = ObjectTrackBll.GetDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                ViewData["dt"] = dt.Rows[0][0].ToString();
            }
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SetForm(string KeyValue)
        {
            FY_ObjectTracking entity = DataFactory.Database().FindEntity<FY_ObjectTracking>(KeyValue);
            if (entity == null)
            {
                return Content("");
            }
            
            string strJson = entity.ToJson();
            
            strJson = strJson.Insert(1, Base_FormAttributeBll.Instance.GetBuildForm(KeyValue));
            return Content(strJson);
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="code">用户编码</param>
        /// <returns>1成功，0失败</returns>
        public int SendEmail(string code, string Content)
        {
            string sql = " select Email from base_user where code='" + code + "'";
            DataTable dt = ObjectTrackBll.GetDataTable(sql);
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

        public ActionResult SubmitUploadify(string FolderId, HttpPostedFileBase Filedata, string tag)
        {
            try
            {

                Thread.Sleep(1000);////延迟500毫秒
                Base_NetworkFile entity = new Base_NetworkFile();
                FY_ObjectTracking FyEntity = DataFactory.Database().FindEntity<FY_ObjectTracking>(FolderId);

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

                        if (tag == "Attach")
                        {
                            FyEntity.Attach = virtualPath;
                        }
                        if (tag == "DescripeAttach")
                        {
                            FyEntity.DescripeAttach = virtualPath;
                        }

                       
                        DataFactory.Database().Update<FY_ObjectTracking>(FyEntity);
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
                    NetworkFile = FyEntity,
                };
                return Content(JsonData.ToJson());


            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        public int delay(string KeyValue, string date)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendFormat(" update FY_ObjectTracking set delayCount=delayCount+1,delayDate='{0}' where TrackingID='{1}'", date, KeyValue);
                int result = ObjectTrackBll.ExecuteSql(strSql);
                return result;
            }
            catch
            {
                return 0;
            }
        }


        public ActionResult SubmitUploadifyInsert(string FolderId, HttpPostedFileBase Filedata, string type)
        {
            try
            {

                Thread.Sleep(1000);////延迟500毫秒
                Base_NetworkFile entity = new Base_NetworkFile();
                FY_ObjectTracking FyEntity = new FY_ObjectTracking();

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

                        FyEntity.DescripeAttach = virtualPath;
                        //DataFactory.Database().Update<FY_ObjectTracking>(FyEntity);
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
                    NetworkFile = FyEntity,
                };
                return Content(JsonData.ToJson());


            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        public int deleteAttach(string KeyValue)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendFormat(" update FY_ObjectTracking set DescripeAttach='' where TrackingID='{0}'", KeyValue);
                int result = ObjectTrackBll.ExecuteSql(strSql);
                return result;
            }
            catch
            {
                return 0;
            }
        }

        //谨慎调用，删除所有数据
        public int DevilSummon(string KeyValue)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendFormat("delete from FY_ObjectTracking");
                int result = ObjectTrackBll.ExecuteSql(strSql);
                return result;
            }
            catch
            {
                return 0;
            }
        }

    }
}
