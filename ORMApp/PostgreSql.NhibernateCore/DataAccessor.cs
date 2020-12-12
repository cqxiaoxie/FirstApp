using NHibernate;
using NHibernate.Tool.hbm2ddl;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace PostgreSql.NhibernateCore
{
    public class DataAccessor : IDataAccessor
    {
        private static readonly object lockObj = new object();
        private static ISessionFactory _sessionFactory;

        private static ISessionFactory _sessionFactoryUpdateSchema { get; set; }
        /// <summary>
        /// 是否更新表结构
        /// </summary>
        private bool _isUpdateSchema = false;

        public DataAccessor()
        {
            _isUpdateSchema = false;
            if (_sessionFactory == null)
            {
                lock (lockObj)
                {
                    if (_sessionFactory == null)
                    {
                        _sessionFactory = BuildSessionFactory();
                    }
                }
            }
        }

        /// <summary>
        /// 更新表结构
        /// </summary>
        /// <param name="isUpdateSchema">是否更新表结构</param>
        public DataAccessor(bool isUpdateSchema)
        {
            _isUpdateSchema = true;
            if (_sessionFactoryUpdateSchema == null)
            {
                lock (lockObj)
                {
                    if (_sessionFactoryUpdateSchema == null)
                    {
                        _sessionFactoryUpdateSchema = BuildSessionFactory(true);
                    }
                }
            }
        }

        private static void BindSession()
        {
            //if (!CurrentSessionContext.HasBind(_sessionFactory))
            //{
            //    //CurrentSessionContext.Bind(_sessionFactory.OpenSession(new SQLWatcher())); // 调试sql,放开这句，注释下面一条
            //    CurrentSessionContext.Bind(_sessionFactory.OpenSession());
            //}
        }

        public ISession OpenSession()
        {
            return _isUpdateSchema ? _sessionFactoryUpdateSchema.OpenSession() : _sessionFactory.OpenSession();
        }

        private static void Flush(ISession session)
        {
            if (session != null)
                session.Flush();
        }

        private static ISessionFactory BuildSessionFactory(bool isUpdateSchema = false)
        {
            if (isUpdateSchema)
            {
                var update = new SchemaUpdate(Configuration);
                update.Execute(false, true);
                return Configuration.BuildSessionFactory();
            }
            else
            {
                return Configuration.BuildSessionFactory();
            }
        }

        private static Configuration _configuration;

        public static Configuration Configuration
        {
            get
            {
                if (_configuration == null)
                    InitConfiguration();

                return _configuration;
            }
            set
            {
                _configuration = value;
                _sessionFactory = null;
            }
        }

        private static void InitConfiguration()
        {
            var fileName = "hibernate.config";
            EDoc2DbType dbType = (EDoc2DbType)EDoc2Utility.DatabaseType;
            switch (dbType)
            {
                case EDoc2DbType.SqlServer:
                    fileName = "hibernate.sqlserver.config";
                    break;
                case EDoc2DbType.Oracle:
                    fileName = "hibernate.oracle.config";
                    break;
                case EDoc2DbType.MySql:
                    fileName = "hibernate.mysql.config";
                    break;
                case EDoc2DbType.Dm:
                    fileName = "hibernate.dm.config";
                    break;
                default:
                    break;
            }
            _configuration = new Configuration().Configure(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName));
            string connectionString = EDoc2Utility.GenerateConnectionString();
            if (_configuration.Properties.ContainsKey(NHibernate.Cfg.Environment.ConnectionProvider))
            {
                _configuration.Properties.Remove(NHibernate.Cfg.Environment.ConnectionProvider);
            }
            if (!string.IsNullOrEmpty(connectionString))
            {
                _configuration.SetProperty(NHibernate.Cfg.Environment.ConnectionString, connectionString);
                int commandTimeout = PageHelper.GetConfigurationSettingsValue("CommandTimeout", 1800);
                _configuration.SetProperty(NHibernate.Cfg.Environment.CommandTimeout, commandTimeout.ToString());
            }
            using (var stream = new MemoryStream())
            {
                HbmSerializer.Default.Serialize(stream, System.Reflection.Assembly.Load("Macrowing.Portal.Platform.Core"));

#if DEBUG // 生成实体映射文件
                try
                {
                    using (var fileStream = File.Create(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Macrowing.Portal.Platform.Core.hbm.xml")))
                    {
                        stream.Seek(0, SeekOrigin.Begin);
                        stream.CopyTo(fileStream);
                    }
                }
                catch (Exception ex)
                {
                    EDoc2.Package.Log.LogManager.Error(ex.Message, ex.ToString());
                }
#endif

                stream.Position = 0;
                _configuration.AddInputStream(stream);
            }
        }

        public void SetDefaultConnectionStr()
        {
            if (_configuration != null)
            {
                string connectionString = EDoc2Utility.GenerateConnectionString();
                if (!string.IsNullOrEmpty(connectionString))
                {
                    _configuration.SetProperty(NHibernate.Cfg.Environment.ConnectionString, connectionString);
                }
            }
        }

        public object Create<T>(T instance, ISession session = null) where T : class
        {
            if (instance == null) throw new ArgumentNullException(string.Format("参数为空"));
            DealSession(ref session);
            try
            {
                object id = session.Save(instance);
                Flush(session);
                return id;
            }
            catch (Exception exception)
            {
                throw new DataAccessException(
                    string.Format("添加失败"), exception);
            }
        }

        public object Save<T>(T instance, ISession session = null) where T : class
        {
            if (instance == null) throw new ArgumentNullException(string.Format("参数为空", "instance"));
            DealSession(ref session);
            try
            {
                object id = session.Save(session.Merge(instance));
                Flush(session);
                return id;
            }
            catch (Exception exception)
            {
                throw new DataAccessException(
                    string.Format("保存失败"), exception);
            }
        }

        public void Saves<T>(IEnumerable<T> instances, ISession session = null) where T : class
        {
            if (instances == null) throw new ArgumentNullException(string.Format("参数为空"));
            DealSession(ref session);
            try
            {
                var tempArray = instances.ToArray();
                foreach (var intance in tempArray)
                {
                    session.SaveOrUpdate(session.Merge<T>(intance));
                }
                Flush(session);
            }
            catch (Exception exception)
            {
                throw new DataAccessException(
                    string.Format("保存失败"), exception);
            }
        }

        public void Update<T>(T instance, ISession session = null) where T : class
        {
            if (instance == null) throw new ArgumentNullException(string.Format("参数为空", "instance"));
            DealSession(ref session);
            try
            {
                session.SaveOrUpdate(session.Merge(instance));
                Flush(session);
            }
            catch (Exception exception)
            {
                throw new DataAccessException(
                    string.Format("更新失败"), exception);
            }
        }

        public void Delete<T>(T instance, ISession session = null) where T : class
        {
            if (instance == null) throw new ArgumentNullException(string.Format("参数为空", "instance"));
            DealSession(ref session);
            try
            {
                session.Delete(session.Merge(instance));
                Flush(session);
            }
            catch (Exception exception)
            {
                throw new DataAccessException(
                    string.Format("删除失败"), exception);
            }
        }

        private void DealSession(ref ISession session)
        {
            if (session == null)
            {
                session = OpenSession();
            }
        }

        public int DeleteByHQL(string hql, IDictionary parameters, ISession session = null)
        {
            try
            {
                DealSession(ref session);

                IQuery query = session.CreateQuery(hql);
                SetQueryParameters(query, parameters);
                return query.ExecuteUpdate();
            }
            catch (Exception exception)
            {
                throw new DataAccessException(
                    string.Format("删除失败"), exception);
            }
        }

        private static void SetQueryParameters(IQuery query, IDictionary parameters)
        {
            if (parameters != null)
            {
                foreach (var key in parameters.Keys)
                {
                    var value = parameters[key];
                    if (value is object[])
                    {
                        query.SetParameterList(key.ToString(), (object[])value);
                    }
                    else if (value is ICollection)
                    {
                        query.SetParameterList(key.ToString(), value as ICollection);
                    }
                    else
                    {
                        query.SetParameter(key.ToString(), parameters[key]);
                    }
                }
            }
        }

        public void DeleteNotMerge<T>(T instance, ISession session = null) where T : class
        {
            if (instance == null) throw new ArgumentNullException(string.Format("参数为空", "instance"));
            DealSession(ref session);
            try
            {
                session.Delete(instance);
                Flush(session);
            }
            catch (Exception exception)
            {
                throw new DataAccessException(
                    string.Format("删除失败"), exception);
            }
        }

        public void DeleteByPrimaryKey<T>(object id, ISession session = null)
        {
            if (id == null) throw new ArgumentNullException(string.Format("参数为空", "id"));
            DealSession(ref session);
            try
            {
                var entity = session.Get<T>(id);
                if (entity != null)
                    session.Delete(entity);
                Flush(session);
            }
            catch (Exception exception)
            {
                throw new DataAccessException(
                    string.Format("删除失败"), exception);
            }
        }

        public void DeleteAll<T>(ISession session = null) where T : class
        {
            DealSession(ref session);
            try
            {
                session.Delete(string.Format("from {0}", typeof(T).Name));
                Flush(session);
            }
            catch (Exception exception)
            {
                throw new DataAccessException(
                    string.Format("删除失败"), exception);
            }
        }

        public T FindByPrimaryKey<T>(object id)
        {
            T result;
            var session = OpenSession();
            try
            {
                result = session.Get<T>(id);
            }
            catch (ObjectNotFoundException ex)
            {
                throw new DataAccessException(
                    string.Format("查询失败"), ex);
            }
            catch (Exception exception)
            {
                throw new DataAccessException(
                    string.Format("查询失败"), exception);
            }
            return result;
        }

        private static void AddOrdersToCriteria(ICriteria criteria, IEnumerable<Order> orders)
        {
            if (orders == null) return;

            var tempArray = orders.ToArray();
            foreach (var order in tempArray)
                criteria.AddOrder(order);
        }

        public IList<T> FindAll<T>(DetachedCriteria detachedCriteria, params Order[] orders)
        {
            if (detachedCriteria == null) throw new ArgumentNullException(string.Format("参数为空"));
            var session = OpenSession();
            try
            {
                var criteria = detachedCriteria.GetExecutableCriteria(session);
                AddOrdersToCriteria(criteria, orders);
                return criteria.List<T>();
            }
            catch (Exception exception)
            {
                throw new DataAccessException(
                    string.Format("查询失败"), exception);
            }
        }

        public IList<T> FindAll<T>(string hql, IDictionary parameters, Action<IQuery> action)
        {
            if (string.IsNullOrEmpty(hql)) throw new ArgumentNullException(string.Format("参数不能为空"));
            var session = OpenSession();
            try
            {
                var query = session.CreateQuery(hql);
                if (parameters != null)
                    foreach (var key in parameters.Keys)
                    {
                        var value = parameters[key];
                        if (value is object[])
                            query.SetParameterList(key.ToString(), (object[])value);
                        else if (value is ICollection)
                            query.SetParameterList(key.ToString(), value as ICollection);
                        else
                            query.SetParameter(key.ToString(), parameters[key]);
                    }
                if (action != null)
                    action(query);
                return query.List<T>();
            }
            catch (Exception exception)
            {
                throw new DataAccessException(
                    string.Format("查询失败"), exception);
            }
        }

        public IList<T> FindAll<T>(string hql, IDictionary parameters)
        {
            return FindAll<T>(hql, parameters, null);
        }

        public IList<T> SlicedFindAll<T>(int firstResult, int maxResults, IEnumerable<Order> orders,
                                      DetachedCriteria detachedCriteria)
        {
            if (detachedCriteria == null) throw new ArgumentNullException(string.Format("参数不能为空"));
            var session = OpenSession();
            try
            {
                var executableCriteria = detachedCriteria.GetExecutableCriteria(session);
                AddOrdersToCriteria(executableCriteria, orders);
                executableCriteria.SetFirstResult(firstResult);
                executableCriteria.SetMaxResults(maxResults);
                return executableCriteria.List<T>();
            }
            catch (Exception exception)
            {
                throw new DataAccessException(
                    string.Format("查询失败"), exception);
            }
        }

        public IList<T> SlicedFindAll<T>(int firstResult, int maxResults, params ICriterion[] criteria) where T : class
        {
            //            return SlicedFindAll<T>(firstResult, maxResults, null, criteria);
            var session = OpenSession();
            try
            {
                var executableCriteria = session.CreateCriteria<T>();
                if (criteria != null)
                {
                    foreach (var criterion in criteria)
                    {
                        executableCriteria.Add(criterion);
                    }
                }
                executableCriteria.SetFirstResult(firstResult);
                executableCriteria.SetMaxResults(maxResults);
                return executableCriteria.List<T>();
            }
            catch (Exception exception)
            {
                throw new DataAccessException(
                    string.Format("查询失败"), exception);
            }
        }

        public IList<T> SlicedFindAll<T>(int firstResult, int maxResults, DetachedCriteria criteria)
        {
            return SlicedFindAll<T>(firstResult, maxResults, null, criteria);
        }

        public void Refresh<T>(T instance)
        {
            if (instance == null) throw new ArgumentNullException(string.Format("参数为空"));
            var session = OpenSession();
            try
            {
                session.Refresh(instance);
            }
            catch (Exception exception)
            {
                throw new DataAccessException(
                    string.Format("刷新失败"), exception);
            }
        }

        public void AllInOneTransaction(Action<IDataAccessor, ISession> action, Action<IDataAccessor, ISession> rollbackAction = null)
        {
            var session = OpenSession();

            ITransaction tran = null;
            try
            {
                tran = session.BeginTransaction();
                action(this, session);
                Flush(session);
                tran.Commit();
            }
            catch (Exception exception)
            {
                tran?.Rollback();
                rollbackAction?.Invoke(this, session);
                throw new DataAccessException(string.Format("事务回滚"), exception);
            }
        }

        public T AllInOneTransaction<T>(Func<IDataAccessor, ISession, T> action, Action<IDataAccessor, ISession> rollbackAction = null)
        {
            var session = OpenSession();

            ITransaction tran = null;
            try
            {
                tran = session.BeginTransaction();
                var result = action(this, session);
                Flush(session);
                tran.Commit();
                return result;
            }
            catch (Exception exception)
            {
                tran?.Rollback();
                rollbackAction?.Invoke(this, session);
                throw new DataAccessException(string.Format("事务回滚"), exception);
            }
        }

        public void AllInOneSession(Action<ISession> action)
        {
            var session = OpenSession();
            action(session);
        }

        public int UpdateBYHQL(string hql, IDictionary parameters, ISession session)
        {
            DealSession(ref session);

            var query = session.CreateQuery(hql);
            if (parameters != null)
            {
                foreach (var key in parameters.Keys)
                {
                    var value = parameters[key];
                    if (value is object[])
                        query.SetParameterList(key.ToString(), (object[])value);
                    else if (value is ICollection)
                        query.SetParameterList(key.ToString(), value as ICollection);
                    else
                        query.SetParameter(key.ToString(), parameters[key]);
                }
            }

            return query.ExecuteUpdate();
        }
    }
}
