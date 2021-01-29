using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using Xbim.Ifc;
using Xbim.Common;
using CLIF.Codegenerator;
using CLIF.LibraryFactory;

namespace CLIF.QueryEngine
{
    public class QueryManager
    {
        private IfcStore internalStore;
        private int classCounter = 1;
        private readonly string internalNameSpace = typeof(QueryManager).Namespace;
        private readonly string classPrefix = "SelectQuery";
        private readonly Dictionary<string, IIfcSelectQueryClassCreator> selectClassStorage = new Dictionary<string, IIfcSelectQueryClassCreator>();
        private readonly Queue<string> fifoLinqQueries = new Queue<string>();
        private readonly IfcQueryClassFactory selectQueryFactory = new IfcQueryClassFactory();
        private int maximumStoredQueries = 500;

        /// <summary>
        /// maximum number of queries stored in memory
        /// </summary>
        public int MaximumStoredQueries
        {
            get { return maximumStoredQueries; }
            set 
            { 
                maximumStoredQueries = value;
                this.TrimInternalStorage();
            }
        }

        /// <summary>
        /// default constructor
        /// </summary>
        /// <param name="filePath">path to the related IfcFile</param>
        public QueryManager(string filePath)
        {
            this.SetIfcFile(filePath);
        }

        /// <summary>
        /// set the ifc file to operate on
        /// </summary>
        /// <param name="newIfcPath"></param>
        public void SetIfcFile (string newIfcPath)
        {
            if (this.internalStore != null)
            {
                this.internalStore.Dispose();
            }

            this.internalStore = IfcStore.Open(newIfcPath);
        }

        /// <summary>
        /// Perform the given select query on the model internal model, given through the constructor or via SetIfcFile
        /// </summary>
        /// <param name="linqQuery">Query to perform</param>
        /// <returns></returns>
        public IEnumerable<IPersistEntity> SelectEntities (string linqQuery)
        {
            IIfcSelectQueryClassCreator queryClass;

            if (this.selectClassStorage.ContainsKey(linqQuery))
            {
                //select statement already known
                queryClass = this.selectClassStorage[linqQuery];
            }
            else
            {
                string className = this.classPrefix + this.classCounter.ToString();
                Guid assemblyGuid = Guid.NewGuid();
                string assemblyName = assemblyGuid.ToString().Replace("-", string.Empty);
                Assembly queryAssembly = this.selectQueryFactory.GetQueryAssembly(className, internalNameSpace, assemblyName, linqQuery);
                queryClass = this.ExtractSelectQueryClassFromAssembly(queryAssembly);
                this.StoreNewSelectClass(linqQuery, queryClass);
            }

            //var trans = this.internalStore.BeginTransaction();
            //trans.

            return queryClass.Select(this.internalStore.Model);
        }        

        private IIfcSelectQueryClassCreator ExtractSelectQueryClassFromAssembly(Assembly parentAssembly)
        {
            Type interfaceType = typeof(IIfcSelectQueryClassCreator);
            Type queryClass = parentAssembly.ExportedTypes.FirstOrDefault(x => x.GetInterface(interfaceType.Name) != null);
            if (queryClass == null)
            {
                throw new InvalidOperationException("The assembly " + parentAssembly.FullName + " does not contain a class, which implements the interface " + interfaceType.FullName);
            }

            ConstructorInfo conInfo = queryClass.GetConstructor(Type.EmptyTypes);
            if (conInfo == null)
            {
                throw new InvalidOperationException("The class" + queryClass.FullName + " does not have a parameterless constructor");
            }

            IIfcSelectQueryClassCreator instance = (IIfcSelectQueryClassCreator)conInfo.Invoke(null);
            return instance;
        }

        private void TrimInternalStorage()
        {
            if (this.selectClassStorage.Count > this.maximumStoredQueries)
            {
                string linqToDelete = this.fifoLinqQueries.Dequeue();
                this.selectClassStorage.Remove(linqToDelete);
            }
        }

        private void StoreNewSelectClass(string linqQuery, IIfcSelectQueryClassCreator queryClass)
        {
            this.fifoLinqQueries.Enqueue(linqQuery);
            this.selectClassStorage.Add(linqQuery, queryClass);
            this.classCounter++;
        }
    }
}
