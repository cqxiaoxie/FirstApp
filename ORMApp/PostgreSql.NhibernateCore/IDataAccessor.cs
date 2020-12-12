using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace PostgreSql.NhibernateCore
{
    public interface IDataAccessor
    {
        /// <summary>
        /// 获取一个session
        /// </summary>
        /// <returns></returns>
        ISession OpenSession();

        /// <summary>
        /// 向数据库添加一行记录
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="instance">将要添加的实体实例</param>
        /// <returns>返回已与数据库同步的实体实例</returns>
        object Create<T>(T instance, ISession session = null) where T : class;

        /// <summary>
        /// 向数据库保存（添加或更新）一行记录
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="instance">将要保存的实体实例</param>
        /// <returns>本数据存取接口</returns>
        object Save<T>(T instance, ISession session = null) where T : class;

        /// <summary>
        /// 保存多条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instances"></param>
        /// <returns></returns>
        void Saves<T>(IEnumerable<T> instances, ISession session = null) where T : class;

        /// <summary>
        /// 向数据库更新一行记录
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="instance">将要更新的实体实例</param>
        /// <returns>本数据操作实例</returns>
        void Update<T>(T instance, ISession session = null) where T : class;

        /// <summary>
        /// 从数据库删除一行记录
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="instance">将要删除的实体实例</param>
        /// <returns>本数据操作实例</returns>
        void Delete<T>(T instance, ISession session = null) where T : class;

        /// <summary>
        /// 根据HQL条件删除
        /// </summary>
        /// <param name="hql"></param>
        /// <param name="parameters"></param>
        int DeleteByHQL(string hql, IDictionary parameters, ISession session = null);

        void DeleteNotMerge<T>(T instance, ISession session = null) where T : class;

        /// <summary>
        /// 根据主键删除
        /// </summary>
        /// <param name="hql"></param>
        /// <param name="parameters"></param>
        void DeleteByPrimaryKey<T>(object id, ISession session = null);

        /// <summary>
        /// 从数据库删除类型为“T”的所有记录
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <returns>本数据操作实例</returns>
        void DeleteAll<T>(ISession session = null) where T : class;

        /// <summary>
        /// 从数据库根据“Id”值检索一行记录
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="id">标识</param>
        /// <returns>返回检索到的实体实例</returns>
        T FindByPrimaryKey<T>(object id);

        /// <summary>
        /// 从数据库检索类型为“T”所有记录
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="detachedCriteria">分离的检索表达式</param>
        /// <param name="orders">排序方式</param>
        /// <returns>返回检索到的实体实例集合</returns>
        IList<T> FindAll<T>(DetachedCriteria detachedCriteria, params Order[] orders);

        /// <summary>
        ///  从数据库检索类型为“T”所有记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hql">HQL语句</param>
        /// <param name="parameters">参数</param>
        /// <param name="action">对query执行额外的操作</param>
        /// <returns>返回检索到的实体实例集合</returns>
        IList<T> FindAll<T>(string hql, IDictionary parameters, Action<IQuery> action);

        /// <summary>
        /// 从数据库检索类型为“T”所有记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hql">HQL语句</param>
        /// <param name="parameters">参数</param>
        /// <returns>返回检索到的实体实例集合</returns>
        IList<T> FindAll<T>(string hql, IDictionary parameters);

        /// <summary>
        /// 从数据库以区间方式检索类型为“T”的所有记录
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="firstResult">从第几行起始</param>
        /// <param name="maxResults">从起始行算起共需要几行</param>
        /// <param name="orders">排序方式</param>
        /// <param name="detachedCriteria">分离的检索表达式</param>
        /// <returns>返回检索到的实体实例区间集合</returns>
        IList<T> SlicedFindAll<T>(int firstResult, int maxResults, IEnumerable<Order> orders,
                                  DetachedCriteria detachedCriteria);

        /// <summary>
        /// 从数据库以区间方式检索类型为“T”的所有记录
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="firstResult">从第几行起始</param>
        /// <param name="maxResults">从起始行算起共需要几行</param>
        /// <param name="criteria">条件集合</param>
        /// <returns>返回检索到的实体实例区间集合</returns>
        IList<T> SlicedFindAll<T>(int firstResult, int maxResults, params ICriterion[] criteria) where T : class;

        /// <summary>
        /// 从数据库以区间方式检索类型为“T”的所有记录
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="firstResult">从第几行起始</param>
        /// <param name="maxResults">从起始行算起共需要几行</param>
        /// <param name="criteria">条件集合</param>
        /// <returns>返回检索到的实体实例区间集合</returns>
        IList<T> SlicedFindAll<T>(int firstResult, int maxResults, DetachedCriteria criteria);

        /// <summary>
        /// 使所有的“action”在同一个事务中运行
        /// </summary>
        /// <param name="action">外部回调</param>
        /// <returns>本数据操作实例</returns>
        void AllInOneTransaction(Action<IDataAccessor, ISession> action, Action<IDataAccessor, ISession> rollbackAction = null);

        T AllInOneTransaction<T>(Func<IDataAccessor, ISession, T> action, Action<IDataAccessor, ISession> rollbackAction = null);

        /// <summary>
        /// 使所有的“action”在同一个Session中运行
        /// </summary>
        /// <param name="action"></param>
        void AllInOneSession(Action<ISession> action);

        int UpdateBYHQL(string hql, IDictionary parameters, ISession session);
    }
}
