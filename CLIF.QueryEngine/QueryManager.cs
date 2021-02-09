using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xbim.Ifc;
using Xbim.Common;

using CLIF.Common;
using CLIF.LibraryFactory;
using CLIF.Codegenerator;
using Xbim.Common.Metadata;

namespace CLIF.QueryEngine
{
    /// <summary>
    /// Manages incoming queries
    /// </summary>
    public class QueryManager
    {
        private IfcStore internalStore;
        private int classCounter = 1;
        private readonly string internalNameSpace = typeof(QueryManager).Namespace;
        private readonly string classPrefix = "SelectQuery";
        private readonly Dictionary<string, object> selectClassStorage = new Dictionary<string, object>();
        private readonly Queue<string> fifoLinqQueries = new Queue<string>();
        private readonly IfcQueryClassFactory ifcQueryFactory = new IfcQueryClassFactory();
        private int maximumStoredQueries = 500;
        private QueryParser internalParser = new QueryParser();

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
        public IEnumerable<IPersistEntity> Select (string linqQuery)
        {
            IIfcSelectQueryClassCreator queryClass;

            if (this.selectClassStorage.ContainsKey(linqQuery))
            {
                //select statement already known
                queryClass = this.selectClassStorage[linqQuery] as IIfcSelectQueryClassCreator;
            }
            else
            {
                string className = this.GetNewClassName();
                string assemblyName = this.GetNewAssemblyName(); 
                Assembly queryAssembly = this.ifcQueryFactory.GetSelectQueryAssembly(className, internalNameSpace, assemblyName, linqQuery);
                queryClass = this.SimpleQueryClassExtraction<IIfcSelectQueryClassCreator>(queryAssembly);
                
                this.StoreNewClass(linqQuery, queryClass);
            }

            return queryClass.Select(this.internalStore.Model);
        }

        /// <summary>
        /// Deletes all entites, which are filtered by the given Linq query
        /// </summary>
        /// <param name="linqQueryToDefineEntitiesToDelete">select query to define the entities to delete, e.g. 
        /// from ifcEntity in model.Instances where ifcEntity.Label == 2 select ifcEntity</param>
        public void Delete(string linqQueryToDefineEntitiesToDelete)
        {
            IEnumerable<IPersistEntity> entitiesToDelete = this.Select(linqQueryToDefineEntitiesToDelete);
            using ITransaction transaction = this.internalStore.Model.BeginTransaction("delete entities");
            foreach (IPersistEntity entityToDelete in entitiesToDelete)
            {
                this.internalStore.Model.Delete(entityToDelete);
            }
            transaction.Commit();
        }

        /// <summary>
        /// Update the entities from the select query with the method body
        /// </summary>
        /// <param name="linqQueryToDefineEntitiesToModify"></param>
        /// <param name="methodBody"></param>
        public void Update(string linqQueryToDefineEntitiesToModify, Type inputParameterType, string inputParameterName, string methodBody)
        {
            string className = this.GetNewClassName();
            string assemblyName = this.GetNewAssemblyName();
            Assembly updateAssembly = this.ifcQueryFactory.GetUpdateQueryAssembly(className, inputParameterType, inputParameterName, 
                internalNameSpace, assemblyName, methodBody);
            IUpdateEntity UpdateClass = this.SimpleQueryClassExtraction<IUpdateEntity>(updateAssembly);
            IEnumerable<IPersistEntity> updateEntities = this.Select(linqQueryToDefineEntitiesToModify);
            UpdateClass.UpdateEntities(updateEntities, this.internalStore);

        }

        /// <summary>
        /// Update the entities from the select query with the method body
        /// </summary>
        /// <param name="linqQueryToDefineEntitiesToModify"></param>
        /// <param name="methodBody"></param>
        public void Update(string linqQueryToDefineEntitiesToModify, string inputParameterType, string inputParameterName, string methodBody)
        {
            Type inputType = Type.GetType(inputParameterType); Problem: Assembly qualified name...
            this.Update(linqQueryToDefineEntitiesToModify, inputType, inputParameterName, methodBody);
        }

        /// <summary>
        /// Adds a new entity and performs the given action on it
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="actionToPerformAfterInsert"></param>
        /// <returns></returns>
        public T Insert<T>(Action<T> actionToPerformAfterInsert = null) where T : IInstantiableEntity
        {
            using ITransaction addTransaction = this.internalStore.BeginTransaction("addTransaction");
            T result = this.internalStore.Model.Instances.New<T>();
            actionToPerformAfterInsert?.Invoke(result);
            addTransaction.Commit();
            return result;
        }

        /// <summary>
        /// performs a general query
        /// </summary>
        /// <param name="linqQuery"></param>
        /// <returns></returns>
        public QueryResult PerformQuery (string linqQuery)
        {
            //define query type
            QueryResult result = new QueryResult();
            try
            {
                result.QueryType = this.internalParser.GetQueryType(linqQuery);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Error = ex;
                return result;
            }
            
            //perform different queries
            try
            {
                switch (result.QueryType)
                {
                    case QueryTypeEnum.SELECT:
                        result.ReturnedObject = this.Select(linqQuery);
                        break;

                    case QueryTypeEnum.DELETE:
                        string selectForDelete = this.internalParser.GetDeleteIInformation(linqQuery).SelectStatement;
                        this.Delete(selectForDelete);
                        break;

                    case QueryTypeEnum.UPDATE:
                        UpdateQueryInformation updateInformation = this.internalParser.GetUpdateInformation(linqQuery);
                        this.Update(updateInformation.SelectQuery, updateInformation.ObjectType, updateInformation.EntityName, updateInformation.MethodBody);
                        break;
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Error = ex;
            }
            return result;
        }

        //private T ExtractQueryClassFromAssembly<T>(Assembly parentAssembly)
        //{
        //    Type interfaceType = typeof(T);
        //    Type queryClass = parentAssembly.ExportedTypes.FirstOrDefault(x => x.GetInterface(interfaceType.Name) != null);
        //    if (queryClass == null)
        //    {
        //        throw new InvalidOperationException("The assembly " + parentAssembly.FullName + " does not contain a class, which implements the interface " + interfaceType.FullName);
        //    }

        //    ConstructorInfo conInfo = queryClass.GetConstructor(Type.EmptyTypes);
        //    if (conInfo == null)
        //    {
        //        throw new InvalidOperationException("The class" + queryClass.FullName + " does not have a parameterless constructor");
        //    }

        //    T instance = (T)conInfo.Invoke(null);
        //    return instance;
        //}

        private T SimpleQueryClassExtraction<T> (Assembly parentAssembly)
        {
            Type queryClass = parentAssembly.ExportedTypes.First();
            ConstructorInfo conInfo = queryClass.GetConstructor(Type.EmptyTypes);
            if (conInfo == null)
            {
                throw new InvalidOperationException("The class" + queryClass.FullName + " does not have a parameterless constructor");
            }
            return (T)conInfo.Invoke(null);
        }

        private void TrimInternalStorage()
        {
            if (this.selectClassStorage.Count > this.maximumStoredQueries)
            {
                string linqToDelete = this.fifoLinqQueries.Dequeue();
                this.selectClassStorage.Remove(linqToDelete);
            }
        }

        private void StoreNewClass(string linqQuery, object queryClass)
        {
            this.fifoLinqQueries.Enqueue(linqQuery);
            this.selectClassStorage.Add(linqQuery, queryClass);
        }

        /// <summary>
        /// generate a new class name and increase the counter
        /// </summary>
        /// <returns></returns>
        private string GetNewClassName()
        {
            string result = this.classPrefix + this.classCounter.ToString();
            this.classCounter++;
            return result;
        }

        /// <summary>
        /// Generate a new name for an assembly
        /// </summary>
        /// <returns></returns>
        private string GetNewAssemblyName()
        {
            Guid assemblyGuid = Guid.NewGuid();
            return assemblyGuid.ToString().Replace("-", string.Empty);
        }

        /// <summary>
        /// saves the current model
        /// </summary>
        /// <param name="pathToStore">location to save the model</param>
        public void SaveModel (string pathToStore)
        {
            this.internalStore.SaveAs(pathToStore);
        }
    }
}
