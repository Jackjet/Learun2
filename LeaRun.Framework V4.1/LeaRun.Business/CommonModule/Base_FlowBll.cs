using LeaRun.DataAccess;
using LeaRun.Entity;
using LeaRun.Repository;
using LeaRun.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace LeaRun.Business
{
    public class Base_FlowBll : RepositoryFactory<Base_Flow>
    {
        public DataTable GetPageList(string keyword, ref JqGridParam jqgridparam)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append(@"select * from base_flow where 1=1 ");
//            if (!string.IsNullOrEmpty(keyword))
//            {
//                strSql.Append(@" AND (code LIKE @keyword
//                                    OR FullName LIKE @keyword
//                                    OR CreateUserName LIKE @keyword
//                                    )");
//                parameter.Add(DbFactory.CreateDbParameter("@keyword", '%' + keyword + '%'));
//            }

            return Repository().FindTablePageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);
        }



        public DataTable NodeList(string KeyValue)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append(@"select PostId,Code,FullName,SortCode from Base_Post where 1=1");
            //if (!ManageProvider.Provider.Current().IsSystem)
            //{
            //    strSql.Append(" AND ( RoleId IN ( SELECT ResourceId FROM Base_DataScopePermission WHERE");
            //    strSql.Append(" ObjectId IN ('" + ManageProvider.Provider.Current().ObjectId.Replace(",", "','") + "') ");
            //    strSql.Append(" ) )");
            //}
            //strSql.Append(" AND r.CompanyId = @CompanyId");
            //parameter.Add(DbFactory.CreateDbParameter("@UserId", UserId));
            //parameter.Add(DbFactory.CreateDbParameter("@CompanyId", CompanyId));
            return Repository().FindTableBySql(strSql.ToString(), parameter.ToArray());
        }

        //当前流程图
        public DataTable CurrentFlow(string KeyValue)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append(@"select b.FullName from Base_FlowStep a left join Base_Post b on a.CurrentPostID=b.PostId where a.FlowID=@FlowID ");
            parameter.Add(DbFactory.CreateDbParameter("@FlowID", KeyValue));
            return Repository().FindTableBySql(strSql.ToString(), parameter.ToArray());
        }



        public int BatchAddObject(string FlowID, string[] arrayObjectId, string Category)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                StringBuilder sbDelete = new StringBuilder("DELETE FROM base_flowstep WHERE FlowID = @FlowID ");
                List<DbParameter> parameter = new List<DbParameter>();
                parameter.Add(DbFactory.CreateDbParameter("@FlowID", FlowID));
                
                database.ExecuteBySql(sbDelete, parameter.ToArray(), isOpenTrans);
                int index = 1;
                foreach (string item in arrayObjectId)
                {
                    if (item.Length > 0)
                    {
                        Base_FlowStep entity = new Base_FlowStep();
                        entity.FlowStepID = CommonHelper.GetGuid;
                        entity.FlowID = FlowID;
                        entity.CurrentPostID = item;
                        
                        entity.Create();

                        //流程节点顺序
                        entity.StepNO = index.ToString();
                        index++;
                        database.Insert(entity, isOpenTrans);
                    }
                }
                DataFactory.Database().Commit();
                return 1;
            }
            catch
            {
                database.Rollback();
                database.Close();
                throw;
            }
        }
    }
}