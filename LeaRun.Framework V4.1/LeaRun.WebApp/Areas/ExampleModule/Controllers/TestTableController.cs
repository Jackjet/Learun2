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

namespace LeaRun.WebApp.Areas.ExampleModule.Controllers
{
    public class TestTableController : Controller
    {

        TestTableBll testtablebll = new TestTableBll();
        //
        // GET: /ExampleModule/Test/

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult GridPageListJson(string keywords, string CompanyId, string DepartmentId, JqGridParam jqgridparam)
        {
            try
            {
                Stopwatch watch = CommonHelper.TimerStart();
                DataTable ListData = testtablebll.GetPageList(keywords,ref jqgridparam);
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
        public ActionResult SubmitTestTableForm(string KeyValue, TestTable testtable, string BuildFormJson)
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
                    testtable.Modify(KeyValue);
                    database.Update(testtable, isOpenTrans);
                    
                }
                else
                {
                    testtable.Create();

                    database.Insert(testtable, isOpenTrans);
                    //database.Insert(base_employee, isOpenTrans);
                    Base_DataScopePermissionBll.Instance.AddScopeDefault(ModuleId, ManageProvider.Provider.Current().UserId, testtable.TestId, isOpenTrans);
                }
                Base_FormAttributeBll.Instance.SaveBuildForm(BuildFormJson, testtable.TestId, ModuleId, isOpenTrans);
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
        public ActionResult SetTestForm(string KeyValue)
        {
            TestTable base_user = DataFactory.Database().FindEntity<TestTable>(KeyValue);
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


        public ActionResult Flexgrid()
        {
            return View();
        }

        public JsonResult GetGridList3(string query, string qtype, string sortname, string sortorder, string page, string rp, string condition, string infoPatch)
        {
            //ARTEA.BAM.BLL.IUserAgent userAgent = ARTEA.BAM.BLL.TopMaster.Instance.GetUserAgent();
            //ARTEA.BAM.DTO.UserDTO currentUser = userAgent.CurrentUser;
            //string sql;
            //sql = "exec Report_ManufactureList " + currentUser.Comp.ToString();

            //_sqlManager.GetDataSetBySQL(sql);
            //string[] sqlPrd = new string[2];
            //if (condition == "undefined")
            //{
            //    sqlPrd[0] = " select distinct * from ##ManufactureList where 1=1";
            //    sqlPrd[1] = " select Count(*) from ##ManufactureList where 1=1";
            //}
            //else
            //{
            //    sqlPrd[0] = " select distinct * from ##ManufactureList where 1=1" + condition;
            //    sqlPrd[1] = " select count(*) from ##ManufactureList where 1=1" + condition;


            //}
            
            //var dt = _sqlManager.GetDataSetBySQL(sql).Tables[0];
            page = "1";
            rp = "15";
            sortname = "ID";
            sortorder = "desc";

            
            DataTable dt=new DataTable();
            int recordCount = 0;
            recordCount = 1;

            var retVal = new BuildGridJson<string, int>(
                dt,
                Convert.ToInt32(page),
                Convert.ToInt32(rp),
                recordCount,
                entity => "11",
                entity => "22",
                entity => "33",
                entity => "44",
                entity => "55"
                
                ).Build();
            return Json(retVal, JsonRequestBehavior.AllowGet);
        }

        

        
    }
}
