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
    public class Base_FlowStepBll : RepositoryFactory<Base_FlowStep>
    {
        public DataTable GetPageList(string keyword, ref JqGridParam jqgridparam)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append(@"select * from testtable where 1=1 ");
            if (!string.IsNullOrEmpty(keyword))
            {
                strSql.Append(@" AND (code LIKE @keyword
                                    OR FullName LIKE @keyword
                                    OR CreateUserName LIKE @keyword
                                    )");
                parameter.Add(DbFactory.CreateDbParameter("@keyword", '%' + keyword + '%'));
            }

            return Repository().FindTablePageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);
        }
    }
}