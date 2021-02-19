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
        private readonly string classPrefix = "QueryClass";
        private readonly Dictionary<string, object> selectClassStorage = new Dictionary<string, object>();
        private readonly Queue<string> fifoLinqQueries = new Queue<string>();
        private readonly IfcQueryClassFactory ifcQueryFactory = new IfcQueryClassFactory();
        private int maximumStoredQueries = 500;
        private readonly QueryParser internalParser = new QueryParser();

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
            ISelectEntity queryClass;

            if (this.selectClassStorage.ContainsKey(linqQuery))
            {
                //select statement already known
                queryClass = this.selectClassStorage[linqQuery] as ISelectEntity;
            }
            else
            {
                string className = this.GetNewClassName();
                string assemblyName = this.GetNewAssemblyName(); 
                Assembly queryAssembly = this.ifcQueryFactory.GetSelectQueryAssembly(className, internalNameSpace, assemblyName, linqQuery);
                queryClass = this.SimpleQueryClassExtraction<ISelectEntity>(queryAssembly);
                
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
        public IEnumerable<IPersistEntity> Update(string linqQueryToDefineEntitiesToModify, Type inputParameterType, string inputParameterName, string methodBody)
        {
            string className = this.GetNewClassName();
            string assemblyName = this.GetNewAssemblyName();
            Assembly updateAssembly = this.ifcQueryFactory.GetUpdateQueryAssembly(className, inputParameterType, inputParameterName, 
                internalNameSpace, assemblyName, methodBody);
            IUpdateEntity UpdateClass = this.SimpleQueryClassExtraction<IUpdateEntity>(updateAssembly);
            IEnumerable<IPersistEntity> updateEntities = this.Select(linqQueryToDefineEntitiesToModify);
            using ITransaction updateTrans = this.internalStore.BeginTransaction("update with user query");
            UpdateClass.UpdateEntities(updateEntities, this.internalStore);
            updateTrans.Commit();
            return updateEntities;
        }

        /// <summary>
        /// Update the entities from the select query with the method body
        /// </summary>
        /// <param name="linqQueryToDefineEntitiesToModify"></param>
        /// <param name="methodBody"></param>
        /// <param name="inputParameterName">Name of the input parameter in methodBody</param>
        /// <param name="inputParameterType">Assembly qualified name of the parameter type</param>
        public IEnumerable<IPersistEntity> Update(string linqQueryToDefineEntitiesToModify, string inputParameterType, string inputParameterName, string methodBody)
        {
            Type inputType = Type.GetType(inputParameterType);
            if (inputType == null) 
            {
                throw new ArgumentException("No such type found " + inputParameterType);
            }
            return this.Update(linqQueryToDefineEntitiesToModify, inputType, inputParameterName, methodBody);
        }

        /// <summary>
        /// Adds a new entity and performs the given action on it
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="nameOfTypeToInsert">assembly qualified name of the type to insert</param>
        /// <returns></returns>
        public IInstantiableEntity Insert(string nameOfTypeToInsert)
        {
            using ITransaction addTransaction = this.internalStore.BeginTransaction("addTransaction");
            Type typeToInsert = Type.GetType(nameOfTypeToInsert);
            IInstantiableEntity result = (IInstantiableEntity)this.internalStore.Model.Instances.New(typeToInsert);
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
            string assemblyQualifiedTypeName;
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
                        assemblyQualifiedTypeName = this.GetAssemblyQualifiedTypeName(updateInformation.ObjectType);
                        result.ReturnedObject = this.Update(updateInformation.SelectQuery, assemblyQualifiedTypeName, updateInformation.EntityName, updateInformation.MethodBody);
                        break;

                    case QueryTypeEnum.INSERT:
                        UpdateQueryInformation insertInformation = this.internalParser.GetInsertInformation(linqQuery);
                        assemblyQualifiedTypeName = this.GetAssemblyQualifiedTypeName(insertInformation.ObjectType);
                        IInstantiableEntity newEntity = this.Insert(assemblyQualifiedTypeName);
                        if (!string.IsNullOrEmpty(insertInformation.MethodBody))
                        {
                            string selectStatement = "from " + " ifcEntity in model.Instances where ifcEntity.EntityLabel == "
                                + newEntity.EntityLabel.ToString() + " select (" + insertInformation.ObjectType + ") ifcEntity";
                            this.Update(selectStatement, assemblyQualifiedTypeName, insertInformation.EntityName, insertInformation.MethodBody);
                        }
                        result.ReturnedObject = newEntity;
                        break;

                    default:
                        throw new InvalidOperationException("Unknown query Type");
                }
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Error = ex;
            }
            return result;
        }

        /// <summary>
        /// Returns the assembly qualified type name of the ifc classes.
        /// </summary>
        /// <param name="typeName">Full name of the type</param>
        /// <returns>Assembly qualified name of the type</returns>
        /// <remarks>The first two parts of the namespace are most times the assembly name within xBIM</remarks>
        public string GetAssemblyQualifiedTypeName (string typeName)
        {
            string[] parts = typeName.Split('.');
            return typeName + ", " + parts[0] + "." + parts[1];
        }

        //private T ExtractQueryByInterfaceType<T>(Assembly parentAssembly)
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
