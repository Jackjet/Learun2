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
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace LeaRun.WebApp.Areas.CommonModule.Controllers
{
    public class FlowController : Controller
    {
        //
        // GET: /CommonModule/Flow/
        Base_FlowBll flowbll = new Base_FlowBll();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GridPageListJson(string keywords, string CompanyId, string DepartmentId, JqGridParam jqgridparam)
        {
            try
            {
                Stopwatch watch = CommonHelper.TimerStart();
                DataTable ListData = flowbll.GetPageList(keywords, ref jqgridparam);
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
            return View();
        }

        [HttpPost]
        public ActionResult SubmitFlowForm(string KeyValue, Base_Flow flow, string BuildFormJson)
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
                    flow.Modify(KeyValue);
                    database.Update(flow, isOpenTrans);

                }
                else
                {
                    flow.Create();

                    database.Insert(flow, isOpenTrans);
                    //database.Insert(base_employee, isOpenTrans);
                    Base_DataScopePermissionBll.Instance.AddScopeDefault(ModuleId, ManageProvider.Provider.Current().UserId, flow.FlowID, isOpenTrans);
                }
                Base_FormAttributeBll.Instance.SaveBuildForm(BuildFormJson, flow.FlowID, ModuleId, isOpenTrans);
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


        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SetFlowForm(string KeyValue)
        {
            Base_Flow base_user = DataFactory.Database().FindEntity<Base_Flow>(KeyValue);
            if (base_user == null)
            {
                return Content("");
            }
            //Base_Employee base_employee = DataFactory.Database().FindEntity<Base_Employee>(KeyValue);
            //Base_Company base_company = DataFactory.Database().FindEntity<Base_Company>(base_user.CompanyId);
            string strJson = base_user.ToJson();
            //公司
            //strJson = strJson.Insert(1, "\"CompanyName\":\"" + base_company.FullName + "\",");
            //员工信息
            //strJson = strJson.Insert(1, base_employee.ToJson().Replace("{", "").Replace("}", "") + ",");
            //自定义
            strJson = strJson.Insert(1, Base_FormAttributeBll.Instance.GetBuildForm(KeyValue));
            return Content(strJson);
        }


        public ActionResult Node()
        {
            return View();
        }

        public ActionResult NodeList(string KeyValue)
        {
            StringBuilder sb = new StringBuilder();
            DataTable dt = flowbll.NodeList(KeyValue);
            foreach (DataRow dr in dt.Rows)
            {
                string strchecked = "";
                //if (!string.IsNullOrEmpty(dr["objectid"].ToString()))//判断是否选中
                //{
                //    strchecked = "selected";
                //}
                sb.Append("<li title=\"" + dr["PostId"] +";"+ dr["FullName"] + "\" class=\"\">");
                sb.Append("<a id=\"" + dr["PostId"] + "\"><img src=\"../../Content/Images/Icon16/role.png \">" + dr["fullname"] + "</a><i></i>");
                sb.Append("</li>");
            }
            return Content(sb.ToString());
        }




        public ActionResult FlowNodeSubmit(string UserId, string ObjectId)
        {
            try
            {
                string[] array = ObjectId.Split(',');
                int IsOk = flowbll.BatchAddObject(UserId, array, "2");
                return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = "操作成功。" }.ToString());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败，错误：" + ex.Message }.ToString());
            }
        }

        public ActionResult CurrentFlow(string KeyValue)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("当前流程:");
            DataTable dt = flowbll.CurrentFlow(KeyValue);
            foreach (DataRow dr in dt.Rows)
            {
                sb.Append(dr["FullName"] + "->");
            }
            //sb.Remove(sb.Length - 3, sb.Length-1);
            return Content(sb.ToString());
           
        }

    }
}
