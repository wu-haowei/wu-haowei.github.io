using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using PWA_TEST.Repository;
using System.Data.Entity;
using PWA_TEST.Models;
using System.Data;

namespace PWA_TEST.Repository
{
    public class DBRepository
    {
        private readonly DbContext _context;

        public DBRepository() : this(new ModelDbContext())
        { }

        public DBRepository(DbContext context)
        {
            if (context == null) { throw new ArgumentNullException(); }
            _context = context;
        }
        /// <summary>
        /// 取得DbContext的執行個體
        /// </summary>
        public DbContext Context { get { return _context; } }

        /// <summary>
        /// 標註Entity的狀態為新增
        /// </summary>
        /// <typeparam name="T">Entity 型別</typeparam>
        /// <param name="entity">Entity 執行個體</param>
        public virtual void Create<T>(T entity) where T : class
        {
            _context.Entry(entity).State = EntityState.Added;
        }

        /// <summary>
        /// 標註Entity的狀態為修改
        /// </summary>
        /// <typeparam name="T">Entity 型別</typeparam>
        /// <param name="entity">Entity 執行個體</param>
        public virtual void Update<T>(T entity) where T : class
        {
            _context.Entry(entity).State = EntityState.Modified;
        }

        /// <summary>
        /// 標註Entity的狀態為刪除
        /// </summary>
        /// <typeparam name="T">Entity 型別</typeparam>
        /// <param name="entity">Entity 執行個體</param>
        public virtual void Delete<T>(T entity) where T : class
        {
            _context.Entry(entity).State = EntityState.Deleted;
        }

        /// <summary>
        /// 取得該實體類型的單一筆資料，依據傳入的委派方法
        /// </summary>
        /// <typeparam name="T">Entity 型別</typeparam>
        /// <param name="predicate">委派方法</param>
        /// <returns>回傳符合委派方法的第一筆資料，不符合會回傳預設值</returns>
        public virtual T Get<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return _context.Set<T>().FirstOrDefault(predicate);
        }

        /// <summary>
        /// 取得該實體類型的全部資料
        /// </summary>
        /// <typeparam name="T">Entity 型別</typeparam>
        /// <returns>回傳Entity實體類型的集合(評估查詢)</returns>
        public virtual IQueryable<T> GetAll<T>() where T : class
        {
            return _context.Set<T>();
        }


        /// <summary>
        /// 儲存EF的異動
        /// </summary>
        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }

}