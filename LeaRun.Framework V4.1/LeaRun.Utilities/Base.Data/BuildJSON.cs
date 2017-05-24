using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;


namespace LeaRun.Utilities
{
    /// <summary>
    ///功能描述    :    用于表格的Json处理类
    ///开发者      :    ZhuXiangfei
    ///建立时间    :    2009-08-18 17:26:30
    ///修订描述    :    
    ///进度描述    :    
    ///版本号      :    1.0
    ///最后修改时间:    2011-10-17 20:26:30
    /// </summary>
    public class BuildGridJson<TEntity, IdType>
    {
        ///// <summary>
        ///// MVC 无分页 Json数据构造
        ///// </summary>
        ///// <param name="entities">对象实体集合</param>
        ///// <param name="idFunc">泛型委托，传入匿名函数lambda 传入TEntity 返回 IdType</param>
        ///// <param name="propertyFuncs">泛型委托集合 用法同上</param>
        //public BuildJson(
        //    List<TEntity> entities,
        //    Func<TEntity, IdType> idFunc,
        //    params Func<TEntity, string>[] propertyFuncs
        //    )
        //{
        //    //无分页，实际分页为1
        //    this.ActualPageIndex = 1;
        //    //总页数为1
        //    this.TotalPageCount = 1;
        //    //总行数则为对象实体集合的实体个数
        //    this.TotalRowCount = entities.Count;
        //    //声明一个List来存入构造出的对象(每一行代表一个实体)
        //    this.Rows = new List<object>();

        //    //循环传入的实体集合
        //    foreach (TEntity entity in entities)
        //    {
        //        //声明一个obj的list来存放泛型委托集合中每个泛型委托返回的值
        //        List<string> obj = new List<string>();
        //        //循环泛型委托集合
        //        foreach (Func<TEntity, string> propertyFunc in propertyFuncs)
        //        {
        //            //将每一个泛型委托 的返回值传入obj
        //            obj.Add(propertyFunc(entity));
        //        }
        //        //将每个实体的ID 以及其他属性存入一个row当中
        //        this.Rows.Add(new
        //        {
        //            id = idFunc(entity),
        //            cell = obj.ToArray()
        //        });
        //    }

        //}

        //实际页数
        public string ActualPageIndex { get; set; }
        //总页数
        public string TotalPageCount { get; set; }
        //总行数
        public string TotalRowCount { get; set; }
        //接收每个实体的集合
        public List<object> Rows { get; private set; }


        //数据组装
        public object Build()
        {
            return new
            {
                total = this.TotalRowCount,
                page = this.ActualPageIndex,
                //pages = this.TotalPageCount,
                rows = this.Rows
            };
        }
        public object BuildAll()
        {
            return new { Rows = this.Rows };
        }


        ///// <summary>
        ///// MVC 分页 Json数据构造，用于JqGrid插件
        ///// </summary>
        ///// <param name="pagerResponse"></param>
        ///// <param name="idFunc"></param>
        ///// <param name="propertyFuncs"></param>
        //public BuildJsonForJqGrid(
        //    PagerResponse<TEntity> pagerResponse,
        //    Func<TEntity, IdType> idFunc,
        //    params Func<TEntity, string>[] propertyFuncs)
        //{
        //    this.TotalRowCount = pagerResponse.TotalRowCount;
        //    this.ActualPageIndex = pagerResponse.ActualPageIndex;
        //    this.TotalPageCount = pagerResponse.PageCount;

        //    this.Rows = new List<object>();

        //    foreach (TEntity entity in pagerResponse.Entities)
        //    {
        //        List<string> obj = new List<string>();
        //        foreach (Func<TEntity, string> propertyFunc in propertyFuncs)
        //        {
        //            obj.Add(propertyFunc(entity));
        //        }

        //        this.Rows.Add(
        //            new
        //            {
        //                id = idFunc(entity),
        //                cell = obj.ToArray()
        //            });
        //    }
        //}

        /// <summary>
        /// MVC 分页 Json数据构造，用于FlexiGrid插件
        /// </summary>
        /// <param name="pagerResponse"></param>
        /// <param name="idFunc"></param>
        /// <param name="propertyFuncs"></param>
        public BuildGridJson(PagerResponse<TEntity> pagerResponse, Func<TEntity, IdType> idFunc, params Func<TEntity, string>[] propertyFuncs)
        {
            this.TotalRowCount = pagerResponse.TotalRowCount.ToString();
            this.ActualPageIndex = pagerResponse.ActualPageIndex.ToString();
            this.TotalPageCount = pagerResponse.PageCount.ToString();

            this.Rows = new List<object>();

            foreach (TEntity entity in pagerResponse.Entities)
            {
                List<string> obj = new List<string>();
                foreach (Func<TEntity, string> propertyFunc in propertyFuncs)
                {
                    obj.Add(propertyFunc(entity));
                }

                this.Rows.Add(
                    new
                    {
                        id = idFunc(entity),
                        cell = obj.ToArray()
                    });
            }
        }

        public BuildGridJson(DataTable dt, int page, int rp, int totalCount)
        {
            this.TotalRowCount = totalCount.ToString();
            this.ActualPageIndex = page.ToString();
            this.TotalPageCount = rp.ToString();
            int num = 0;
            this.Rows = new List<object>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                List<string> lis = new List<string>();
                num = 0;
                while (num < (dt.Columns.Count))
                {
                    lis.Add(dt.Rows[i][num].ToString());
                    num++;
                }
                this.Rows.Add(new { id = lis[0].ToString(), cell = lis.ToArray() });
            }
        }
        public BuildGridJson(DataTable dt, int page, int rp, int totalCount, params Func<DataRow, string>[] propertyFuncs)
        {
            this.TotalRowCount = totalCount.ToString();
            this.ActualPageIndex = page.ToString();
            this.TotalPageCount = rp.ToString();
            this.Rows = new List<object>();
            for (int i = 0; i < dt.Rows.Count; i++)
            //foreach(TEntity entity in dt)
            {
                List<string> lis = new List<string>();
                foreach (Func<DataRow, string> propertyFunc in propertyFuncs)
                {
                    lis.Add(propertyFunc(dt.Rows[i]).ToString());
                }

                this.Rows.Add(new { id = lis[0].ToString(), cell = lis.ToArray() });
            }
        }

        public BuildGridJson(IList<TEntity> lis, Func<TEntity, IdType> idFunc, params Func<TEntity, string>[] propertyFuncs)
        {
            this.Rows = new List<object>();
            foreach (TEntity entity in lis)
            {
                List<object> obj = new List<object>();
                foreach (Func<TEntity, string> propertyFunc in propertyFuncs)
                {
                    obj.Add(propertyFunc(entity));
                }

                this.Rows.Add(new { Cells = obj.ToArray() });
            }
        }
    }

    /// <summary>
    ///功能描述    :    用户树形的Json处理类，老树JQuery.TreeView插件的方法
    ///开发者      :    ZhuXiangfei
    ///建立时间    :    2009-08-18 17:26:30
    ///修订描述    :    
    ///进度描述    :    
    ///版本号      :    1.0
    ///最后修改时间:    2011-10-17 20:26:30
    /// </summary>
    public class BuildTreeJson<TEntity, IdType>
    {
        //接收每个实体的集合
        public List<object> entities { set; get; }


        //数据组装
        public List<object> Build()
        {
            List<object> lis = this.entities;
            return lis;
        }


        /// <summary>
        /// MVC 分页 Json数据构造，用于FlexiGrid插件
        /// </summary>
        /// <param name="pagerResponse"></param>
        /// <param name="idFunc"></param>
        /// <param name="propertyFuncs"></param>
        public BuildTreeJson(IList<TEntity> entities, params Func<TEntity, string>[] propertyFuncs)
        {
            this.entities = new List<object>();
            foreach (TEntity entity in entities)
            {
                List<string> obj = new List<string>();
                foreach (Func<TEntity, string> propertyFunc in propertyFuncs)
                {
                    obj.Add(propertyFunc(entity));
                }
                if (obj[0].ToString() == obj[1].ToString())
                {//根节点
                    //List<object> cls = new Extension.BuildJsonTree<TEntity,IdType>(entities,propertyFuncs).Build();
                    this.entities.Add(new { text = obj[2].ToString(), id = obj[0].ToString(), expanded = "true", hasChildren = obj[3].ToString() });
                }
                else
                {//子节点列表
                    this.entities.Add(new
                    {
                        text = obj[2].ToString(),
                        id = obj[0].ToString(),
                        hasChildren = Convert.ToBoolean(obj[3].ToString())
                    });
                }
            }


        }
    }

    /// <summary>
    ///功能描述    :    用户树形的Json处理类，新树 JQuery.zTree 插件的方法
    ///开发者      :    ZhuXiangfei
    ///建立时间    :    2009-08-18 17:26:30
    ///修订描述    :    跟老树不一样，老树返回的是id和text，zTree要的是id和name。。。
    ///进度描述    :    
    ///版本号      :    1.0
    ///最后修改时间:    2011-10-17 20:26:30
    /// </summary>
    public class zTreeJSON<TEntity, IdType>
    {
        //接收每个实体的集合
        public List<object> entities { set; get; }
        public string entitiesStr { set; get; }

        //数据组装
        public List<object> Build()
        {
            List<object> lis = this.entities;
            return lis;
        }

        public string BuildString()
        {

            return this.entitiesStr;
        }

        public zTreeJSON(IList<TEntity> entities, params Func<TEntity, string>[] propertyFuncs)
        {
            this.entities = new List<object>();
            this.entitiesStr = "[";
            int i = 0;
            foreach (TEntity entity in entities)
            {
                List<string> obj = new List<string>();
                foreach (Func<TEntity, string> propertyFunc in propertyFuncs)
                {
                    obj.Add(propertyFunc(entity));
                }
                //if (obj[0].ToString() == obj[1].ToString())
                //{//根节点
                //    //List<object> cls = new Extension.BuildJsonTree<TEntity,IdType>(entities,propertyFuncs).Build();
                //    this.entities.Add(new { id = obj[0].ToString(), pId = obj[1].ToString(), name = obj[2].ToString(), isParent = Convert.ToBoolean(obj[3].ToString()) });
                //    this.entitiesStr += "{ id:" + obj[0].ToString() + " ,pId:" + obj[1].ToString() + ", name:'" + obj[2].ToString() + "',isParent:" + obj[3].ToString() + "}";// "', tabTag:'" + obj[3].ToString() + "',trueID:" + obj[4].ToString() + "}";
                //}
                //else
                //{//子节点列表
                //    this.entities.Add(new { id = obj[0].ToString(), pId = obj[1].ToString(), name = obj[2].ToString()/*, isParent = Convert.ToBoolean(obj[3].ToString())*/ });
                //    this.entitiesStr += "{ id:" + obj[0].ToString() + " ,pId:" + obj[1].ToString() + ", name:'" + obj[2].ToString() + "',isParent:" + obj[3].ToString() + "}";// "', tabTag:'" + obj[3].ToString() + "',trueID:" + obj[4].ToString() + "}";
                //}

                //为了角色模块里面的check tree，修改了根节点和子节点的判断原则，以及子节点的最后一个属性。
                //如果发现影响了其他的功能，就把上面的注释打开，替换下面的方法，再把角色模块的调用改到最下面那个去
                if (obj.Count > 3 &&  Convert.ToBoolean(obj[3].ToString()))
                {//根节点
                    //List<object> cls = new Extension.BuildJsonTree<TEntity,IdType>(entities,propertyFuncs).Build();
                    this.entities.Add(new { id = obj[0].ToString(), pId = obj[1].ToString(), name = obj[2].ToString(), isParent = Convert.ToBoolean(obj[3].ToString()) });
                    this.entitiesStr += "{ id:" + obj[0].ToString() + " ,pId:" + obj[1].ToString() + ", name:'" + obj[2].ToString() + "',isParent:" + obj[3].ToString() + "}";// "', tabTag:'" + obj[3].ToString() + "',trueID:" + obj[4].ToString() + "}";
                }
                else
                {//子节点列表
                    this.entities.Add(new { id = obj[0].ToString(), pId = obj[1].ToString(), name = obj[2].ToString()/*, isParent = Convert.ToBoolean(obj[3].ToString())*/ });
                    this.entitiesStr += "{ id:" + obj[0].ToString() + " ,pId:" + obj[1].ToString() + ", name:'" + obj[2].ToString() + "',isParent:" + obj[3].ToString() + "}";// "', tabTag:'" + obj[3].ToString() + "',trueID:" + obj[4].ToString() + "}";
                }
                if (i < entities.Count - 1) { this.entitiesStr += ","; }
                i++;
            }
            this.entitiesStr += "]";

        }

        public zTreeJSON(DataTable dt, params Func<DataRow, string>[] propertyFuncs)
        {
            this.entities = new List<object>();
            this.entitiesStr = "[";

            for (int i = 0; i < dt.Rows.Count; i++)
            //foreach(TEntity entity in dt)
            {
                List<string> obj = new List<string>();
                foreach (Func<DataRow, string> propertyFunc in propertyFuncs)
                {
                    obj.Add(propertyFunc(dt.Rows[i]).ToString());
                }
                if (obj[0].ToString() == obj[1].ToString())
                {//根节点
                    //List<object> cls = new Extension.BuildJsonTree<TEntity,IdType>(entities,propertyFuncs).Build();
                    this.entities.Add(new { id = obj[0].ToString(), pId = obj[1].ToString(), name = obj[2].ToString(), open = true });
                    this.entitiesStr += "{ id:" + obj[0].ToString() + " ,pId:" + obj[1].ToString() + ", name:'" + obj[2].ToString() + "',open:true}";
                }
                else
                {//子节点列表
                    this.entities.Add(new { id = obj[0].ToString(), pId = obj[1].ToString(), name = obj[2].ToString() });
                    this.entitiesStr += "{ id:" + obj[0].ToString() + " ,pId:" + obj[1].ToString() + ", name:'" + obj[2].ToString() + "'}";
                }
                if (i < dt.Rows.Count - 1) { this.entitiesStr += ","; }
            }
            this.entitiesStr += "]";

        }

        public zTreeJSON(IList<TEntity> entities, IList<TEntity> mylis, params Func<TEntity, string>[] propertyFuncs)
        {
            this.entities = new List<object>();
            this.entitiesStr = "[";
            int i = 0;
            foreach (TEntity entity in entities)
            {
                List<string> obj = new List<string>();
                foreach (Func<TEntity, string> propertyFunc in propertyFuncs)
                {
                    obj.Add(propertyFunc(entity));
                }

                if (obj.Count > 3 && Convert.ToBoolean(obj[3].ToString()))
                {//根节点
                    if (this._CheckMyList(obj[0].ToString(), mylis, propertyFuncs))
                    {
                        this.entities.Add(new { id = obj[0].ToString(), pId = obj[1].ToString(), name = obj[2].ToString(), isParent = Convert.ToBoolean(obj[3].ToString()), Checked = true});
                        this.entitiesStr += "{ id:" + obj[0].ToString() + " ,pId:" + obj[1].ToString() + ", name:'" + obj[2].ToString() + "',isParent:" + obj[3].ToString() + ",checked:true" +"}";// "', tabTag:'" + obj[3].ToString() + "',trueID:" + obj[4].ToString() + "}";
                    }
                    else {
                        this.entities.Add(new { id = obj[0].ToString(), pId = obj[1].ToString(), name = obj[2].ToString(), isParent = Convert.ToBoolean(obj[3].ToString()) });
                        this.entitiesStr += "{ id:" + obj[0].ToString() + " ,pId:" + obj[1].ToString() + ", name:'" + obj[2].ToString() + "',isParent:" + obj[3].ToString() + "}";// "', tabTag:'" + obj[3].ToString() + "',trueID:" + obj[4].ToString() + "}";
                    }
                }
                else
                {//子节点列表
                    if (this._CheckMyList(obj[0].ToString(), mylis, propertyFuncs))
                    {
                        this.entities.Add(new { id = obj[0].ToString(), pId = obj[1].ToString(), name = obj[2].ToString(), Checked = true/*, isParent = Convert.ToBoolean(obj[3].ToString())*/ });
                        this.entitiesStr += "{ id:" + obj[0].ToString() + " ,pId:" + obj[1].ToString() + ", name:'" + obj[2].ToString() + ",checked:true" + "}";// "', tabTag:'" + obj[3].ToString() + "',trueID:" + obj[4].ToString() + "}";
                    }
                    else
                    {
                        this.entities.Add(new { id = obj[0].ToString(), pId = obj[1].ToString(), name = obj[2].ToString()/*, isParent = Convert.ToBoolean(obj[3].ToString())*/ });
                        this.entitiesStr += "{ id:" + obj[0].ToString() + " ,pId:" + obj[1].ToString() + ", name:'" + obj[2].ToString() + "}";// "', tabTag:'" + obj[3].ToString() + "',trueID:" + obj[4].ToString() + "}";
                    }
                }
                
                if (i < entities.Count - 1) { this.entitiesStr += ","; }
                i++;
            }
            this.entitiesStr += "]";
        }

        private bool _CheckMyList(string id, IList<TEntity> mylis, params Func<TEntity, string>[] propertyFuncs)
        {
            bool val = false;
            foreach (TEntity entity in mylis)
            {
                List<string> obj = new List<string>();
                foreach (Func<TEntity, string> propertyFunc in propertyFuncs) { obj.Add(propertyFunc(entity)); }
                if (id == obj[0].ToString()) { val = true; break; }
            }
            return val;
        }
    }

    /// <summary>
    ///功能描述    :    用户信息显示的Json处理类
    ///开发者      :    ZhuXiangfei
    ///建立时间    :    2009-08-18 17:26:30
    ///修订描述    :    
    ///进度描述    :    
    ///版本号      :    1.0
    ///最后修改时间:    2011-10-17 20:26:30
    /// </summary>
    public class BuildInfoJson<TEntity, IdType>
    {
        //接收实体
        public object entity { set; get; }


        //数据组装
        public object Build()
        {
            return this.entity;
        }

        public BuildInfoJson() { }

        /// <summary>
        /// MVC 分页 Json数据构造，用于FlexiGrid插件
        /// </summary>
        /// <param name="pagerResponse"></param>
        /// <param name="idFunc"></param>
        /// <param name="propertyFuncs"></param>
        public BuildInfoJson( TEntity ent, params Func<TEntity, string>[] propertyFuncs)
        {

            List<string> obj = new List<string>();
            foreach (Func<TEntity, string> propertyFunc in propertyFuncs)
            {
                try
                {
                    obj.Add(propertyFunc(ent));
                }
                catch
                {
                    obj.Add("");
                }
            }

            this.entity = obj as object;
        }

        /// <summary>
        /// 提供给jquery.autocomplete插件，返回的是一串字符串。。。
        /// 如果单个字段，就用\n分割就好了，如果要返回值、名的，中间用"|"分割，然后在用"\n"分割...
        /// 狗屎的插件，直接接受json不就完了么，艹
        /// </summary>
        /// <param name="dt">DataTable数据源</param>
        /// <param name="propertyFuncs">抽取datatable中的列名规则</param>
        public BuildInfoJson(DataTable dt, params Func<DataRow, string>[] propertyFuncs)
        {
            //List<string> obj = new List<string>();
            string str = "";

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                List<string> lis = new List<string>();
                foreach (Func<DataRow, string> propertyFunc in propertyFuncs)
                {
                    lis.Add(propertyFunc(dt.Rows[i]).ToString());
                }
                //if (lis.Count > 1) { obj.Add(lis[0].ToString() + "|" + lis[1].ToString()); } else if (lis.Count > 0) { obj.Add(lis[0].ToString()); }
                if (lis.Count > 2) { str += (lis[1].ToString() + "|" + lis[0].ToString() + "|" + lis[2].ToString()); }
                else if (lis.Count > 1) { str += (lis[1].ToString() + "|" + lis[0].ToString()); }
                else if (lis.Count > 0) { str +=(lis[0].ToString()); }
                if (i < dt.Rows.Count - 1) { str += "\n"; }
            }
            //this.entity = obj as object;
            this.entity = str as object;
        }


        /// <summary>
        /// 暂用在角色某个模块的权限列表(Role Model Rights)，并返回选中状态。。。
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="propertyFuncs"></param>
        public object GetInfoJson(DataTable dt, params Func<DataRow, string>[] propertyFuncs)
        {
            //string str = "";
            List<object> obj = new List<object>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                List<string> lis = new List<string>();
                foreach (Func<DataRow, string> propertyFunc in propertyFuncs)
                {
                    lis.Add(propertyFunc(dt.Rows[i]).ToString());
                }
                obj.Add(new { id = lis[0].ToString(), name = lis[1].ToString(), Checked = lis[2].ToString() });
                //if (lis.Count > 1) { str += (lis[1].ToString() + "|" + lis[0].ToString()); } else if (lis.Count > 0) { str += (lis[0].ToString()); }
                //if (i < dt.Rows.Count - 1) { str += "\n"; }
            }
            return obj as object;
        }

    }

    //public class BaseInfoJson {
    //    //接收每个实体的集合
    //    public List<object> entities { set; get; }
    //    public string entitiesStr { set; get; }

    //    //数据组装
    //    public List<object> Build()
    //    {
    //        List<object> lis = this.entities;
    //        return lis;
    //    }

    //    public string BuildString()
    //    {

    //        return this.entitiesStr;
    //    }

    //    public BaseInfoJson(DataTable dt, params Func<DataRow, string>[] propertyFuncs)
    //    {
    //        this.entities = new List<object>();
    //        this.entitiesStr = "[";

    //        for (int i = 0; i < dt.Rows.Count; i++)
    //        //foreach(TEntity entity in dt)
    //        {
    //            List<string> obj = new List<string>();
    //            foreach (Func<DataRow, string> propertyFunc in propertyFuncs)
    //            {
    //                obj.Add(propertyFunc(dt.Rows[i]).ToString());
    //            }
    //            if (obj[0].ToString() == obj[1].ToString())
    //            {//根节点
    //                //List<object> cls = new Extension.BuildJsonTree<TEntity,IdType>(entities,propertyFuncs).Build();
    //                this.entities.Add(new { id = obj[0].ToString(), pId = obj[1].ToString(), name = obj[2].ToString(), open = true });
    //                this.entitiesStr += "{ id:" + obj[0].ToString() + " ,pId:" + obj[1].ToString() + ", name:'" + obj[2].ToString() + "',open:true}";
    //            }
    //            else
    //            {//子节点列表
    //                this.entities.Add(new { id = obj[0].ToString(), pId = obj[1].ToString(), name = obj[2].ToString() });
    //                this.entitiesStr += "{ id:" + obj[0].ToString() + " ,pId:" + obj[1].ToString() + ", name:'" + obj[2].ToString() + "'}";
    //            }
    //            if (i < dt.Rows.Count - 1) { this.entitiesStr += ","; }
    //        }
    //        this.entitiesStr += "]";

    //    }
    //}
}