using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using Newtonsoft.Json;
using MultiRepositories;

namespace MultiRepositories.Repositories
{
    public class InMemoryRepository<T> : IRepository<T> where T : BaseEntity, new()
    {
        private static ConcurrentDictionary<String, ConcurrentDictionary<Guid, BaseEntity>> _data =
            new ConcurrentDictionary<string, ConcurrentDictionary<Guid, BaseEntity>>(StringComparer.OrdinalIgnoreCase);
        protected String _name;
        protected AppProperties _properties;

        public InMemoryRepository(AppProperties properties)
        {
            _properties = properties;

            if (!Directory.Exists(_properties.DbDir)){
                Directory.CreateDirectory(_properties.DbDir);
            }
        }

        public static T Clone(T source, T dest)
        {
            var sourceProps = typeof(T).GetRuntimeProperties().Where(x => x.CanRead).ToList();


            foreach (var sourceProp in sourceProps)
            {
                if (sourceProp.CanWrite)
                { // check if the property can be set or no.
                    sourceProp.SetValue(dest, sourceProp.GetValue(source, null), null);
                }
            }
            return dest;
        }

        public virtual void Clean(ITransaction transaction = null)
        {
            var data = GetData();
            data.Clear();
            Save();
        }


        public virtual void Initialize()
        {
            _name = typeof(T).Name;
            if (!_data.ContainsKey(_name))
            {
                _data[_name] = new ConcurrentDictionary<Guid, BaseEntity>();
            }
            Load();
        }

        public virtual int Save(T be, ITransaction transaction = null)
        {
            var data = GetData();
            var result = data.TryAdd(be.Id,Clone(be,new T()))?1:0;
            Save();
            return result;
        }

        public virtual int Update(T be, ITransaction transaction = null)
        {
            var data = GetData();
            if (!data.ContainsKey(be.Id))
            {
                return 0;
            }
            data[be.Id]= Clone(be, new T());
            Save();
            return 1;
        }

        public virtual T GetById(Guid id, ITransaction transaction = null)
        {
            var data = GetData();
            if (data.ContainsKey(id))
            {
                return Clone((T)data[id], new T());
            }
            return default(T);
        }

        public long Count
        {
            get
            {
                return GetData().Count;
            }
        }


        public void Delete(Guid id, ITransaction transaction = null)
        {
            var data = GetData();
            if (data.ContainsKey(id))
            {
                data.TryRemove(id, out BaseEntity removed);
                Save();
            }
        }

        public virtual IEnumerable<T> GetAll(ITransaction transaction = null)
        {
            var data = GetData();
            foreach(var item in data.Values)
            {
                yield return Clone((T)item, new T());
            }
        }

        protected ConcurrentDictionary<Guid, BaseEntity> GetData()
        {
            Initialize();
            return _data[_name];
        }


        private void Save()
        {
            var allData = JsonConvert.SerializeObject(_data[_name]);
            File.WriteAllText(Path.Combine(_properties.DbDir,_name + ".dat"), allData);
        }

        private void Load()
        {
            if (File.Exists(Path.Combine(_properties.DbDir, _name + ".dat")))
            {
                var str = File.ReadAllText(Path.Combine(_properties.DbDir, _name + ".dat"));
                var data= JsonConvert.DeserializeObject<Dictionary<Guid, T>>(str);
                foreach(var item in data)
                {
                    _data[_name][item.Key] = item.Value;
                }
                
            }
        }
    }
}
